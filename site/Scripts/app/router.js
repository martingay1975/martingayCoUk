/*global define*/
define(["jquery", "knockout", "crossroads", "hasher", "routerConfig", "googleAnalytics"], function ($, ko, crossroads, hasher, routerConfig, googleAnalytics) {

	// This module configures crossroads.js, a routing library. 
	"use strict";

	var activateCrossroads, Router;

	activateCrossroads = function() {

		/* parameters are: newHash, old hash */
		var parseHash = function(newHash) {
			crossroads.parse(newHash);
		};

		crossroads.normalizeFn = crossroads.NORM_AS_OBJECT;
		hasher.initialized.add(parseHash);
		hasher.changed.add(parseHash);
		hasher.init();
	};

	Router = function (routerConfig) {

		var self = this,
			hashUpdated,
			init;

		this.currentRouteObservable = ko.observable({});

		// handler for when the hash changes value. 
		hashUpdated = function (route, requestParams) {

			var pageUrl, pageTitle;

			// merge in the parameter value in the query string along with the route information.
			$.extend(requestParams, route.params);

			// the key part. Update the current route observable value, the index.html page then reacts to this in terms of which 
			self.currentRouteObservable(requestParams);

			try {
				pageUrl = self.getDiaryUrl();
				pageTitle = route.id || requestParams.title || 'unknown';
				googleAnalytics.setPage(pageUrl, pageTitle);
				googleAnalytics.sendPageViewHit();
			} catch (e) {
				if (console && console.error) {
					console.error('Failed to send to google analytics');
				}
			}
		};

		this.getDiaryUrl = function(replacements) {
			var url, hash, res;

			hash = self.currentRouteObservable();
			res = $.extend(true, {}, hash, replacements);
			
			url = routerConfig.getDiaryUrlFromHash(res);

			return url;
		};

		this.updateCalendarHash = function (year) {
			self.setUrl("/calendar/" + year);
		};

		this.updateDiaryHash = function(replacements) {
			var url = self.getDiaryUrl(replacements);
			// the hash has all the info required to generate a new url.
			self.setUrl(url);
		};

		this.setUrl = function(url) {
			hasher.setHash(url);
		};

		init = function () {
			// register all the routes
			ko.utils.arrayForEach(routerConfig.routes, function (route) {
				var addedRoute = crossroads.addRoute(route.url, function (requestParams) {
					hashUpdated(route, requestParams);
				});

				// append specific rules
				if (route.rules) {
					addedRoute.rules = route.rules;
				}
			});

			// add a handler which captures a route that is invalid
			crossroads.bypassed.add(function (/*request*/) {
				window.alert("bad route");
			});

			activateCrossroads();
		};

		init();
	};
	
	return new Router(routerConfig);
});
