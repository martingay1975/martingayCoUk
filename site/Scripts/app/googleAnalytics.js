/*global window, document */
define(['jquery'], function ($) {

	"use strict";

	// See documentation:
	// https://developers.google.com/analytics/devguides/collection/analyticsjs/single-page-applications#handling_multiple_urls_for_the_same_resource
	// https://developers.google.com/analytics/devguides/collection/analyticsjs/command-queue-reference#ready-callback

	// Generally goes, that we create a tracker when this (googleAnalytics) is created. This is in the call to gaTracker('create' ... .
	// 'UA-50811124-1' is the tracking Id for this website.

	// We track 'pageView' hits.
	// We must 'set' the state.
	// We commit the pageView hit in the call to 'send' which generates a Url based on the state (as defined in the 'set' call).

	// The url sent to analytics can be decoded:
	// See for documentation on the values sent in the call to google analytics 
	// https://developers.google.com/analytics/devguides/collection/analyticsjs/field-reference#nonInteraction

	// Here are the bits we are interested in:
	// Location is 
	// * dl=http%3A%2F%2Fwww.martingay.co.uk%2F

	// Document Path is
	// * dp=

	// Document Title is
	// * dt= title

	// The tracking id.
	// * cid=1691452445.1455207689

	// This is sample code that injects the call to analytics
	(function (i, s, o, g, r, a, m) {
		i.GoogleAnalyticsObject = r;
		i[r] = i[r] || function () {
			(i[r].q = i[r].q || []).push(arguments);
		};
		i[r].l = new Date();
		a = s.createElement(o);
		m = s.getElementsByTagName(o)[0];
		a.async = 1;
		a.src = g;
		m.parentNode.insertBefore(a, m);
	})(window, document, 'script', '//www.google-analytics.com/analytics.js', 'gaTracker');

	// create a default tracker object. 
	gaTracker('create', {
	    trackingId: 'UA-50811124-1',
	    cookieDomain: 'auto',
	    version: '2.0'
	});

	gaTracker(function (tracker) {
		if (console && tracker) {
			console.log("Tracker Ready. " + tracker.get('clientId'));
		}
	});

	var googleAnalytics = {

		/// sends the pageView's hit.
		sendPageViewHit: function (j) {

			// hittype can be one of: 'pageview', 'screenview', 'event', 'transaction', 'item', 'social', 'exception', 'timing'.
			//var mergedPageViewObj = $.extend({ hitType: "pageview" }, pageViewObj);
			gaTracker('send', 'pageview');
			console.log("Sent to google analytics");
		},

		/// set the tracker's page - which will get sent on every subsequent hit.... until setPage is called again.
		setPage: function (page, title) {
			gaTracker("set",
			{
				"page": page,
				"title": title
			});
		}
	};

	return googleAnalytics;
});