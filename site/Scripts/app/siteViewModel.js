/*global define*/
define(['knockout', 'router'], function (ko, router) {

	"use strict";

	var siteViewModel = {
		version: "v1.0",
		pageTitle: ko.observable("Latest Entries"),
		pageName: ko.observable("latest-entries"),
		currentRouteObservable: router.currentRouteObservable
	};

	return siteViewModel;
});