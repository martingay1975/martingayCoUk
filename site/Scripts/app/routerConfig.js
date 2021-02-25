/*global require*/
define(function() {

	"use strict";

	var setDiaryParams, setCalendarParams, routerConfig, addSeparator;

	
	setDiaryParams = function (request, vals) {
		return {
			filter: vals.filter,
			title: decodeURIComponent(vals.title || ""),
			expanded: vals.expanded !== undefined ? (vals.expanded === "true") : true	// if no 'expanded' value defined then assume expanded = true. Convert the string to a boolean
		};
	};

	setCalendarParams = function(request, vals) {
		return {
			year: vals.year,
			month: vals.month
		};
	};

	addSeparator = function (value) {
		return value + "/";
	};

	// A hard coded sets of routes have been defined which is in the return statement
	// { } = non-optional variable
	// : : = optional variable
	routerConfig = {

		// Map from a Hash object to a url
		getDiaryUrlFromHash : function(hash) {
			var url = "";

			if (hash.diaryProvider === 'whoops') {
				url = "whoops";
			} else {
				url = "entries";
			}

			url = addSeparator(url);
			url += hash.expanded ? "true" : "false";

			if (hash.title && hash.diaryProvider === 'default') {
				url = addSeparator(url);
				url += hash.title;
			}

			if (hash.filter) {
				url = addSeparator(url);
				url += hash.filter;
			}

			return url;
		},

		routes: [
			// default empty hash
			{
				id: "default",
				url: '',
				params: {
					componentName: 'entries',
					diaryProvider: 'default'
				},
				rules: {
					normalize_: setDiaryParams
				}
			}

			// entries 
			,{
				id: "entries",
				url: '/entries/:expanded:/:title:/:filter:',
				params: {
					componentName: 'entries',
					diaryProvider: 'default'
				},
				rules: {
					normalize_: setDiaryParams
				}
			}

			// whoops
			,{
				id: "whoops",
				url: '/whoops/:expanded:/:filter:',
				params: {
					componentName: 'entries',
					diaryProvider: 'whoops'
				},
				rules: {
					normalize_: setDiaryParams
				}
			}

			// calendar
			,{
				id: "calendar",
				url: '/calendar/:year:/:month:',
				params: {
					componentName: 'calendar'
				},
				rules: {
					normalize_: setCalendarParams
				}
			}

			// special-days
			,{
				id: "specialdays",
				url: "/specialdays",
				params: {
					componentName: 'special-days'
				}
			}

			// strava
			,{
				id: "strava",
				url: "/strava",
				params: {
					componentName: "strava"
				}
			}

			// spotify
			,{
				id: "spotify",
				url: "/spotify",
				params: {
					componentName: "spotify"
				}
			}

			// diabetes
			, {
				id: "diabetes",
				url: "/diabetes",
				params: {
					componentName: "diabetes"
				}
			}

			// stravaData
			, {
				id: "stravaData",
				url: "/stravaData",
				params: {
					componentName: "stravaData"
				}
			}
		]
	};

	return routerConfig;

});