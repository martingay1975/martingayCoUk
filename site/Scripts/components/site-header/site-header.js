/*global require*/
define(['knockout', 'siteOptions', 'text!./site-header.html'], function (ko, siteOptions, componentTemplate) {

	"use strict";

	var initAsync, HeaderComponentViewModel;

	initAsync = function () {
		var headerComponentViewModel = this;
		siteOptions.initAsyncPromise.done(function() {
			headerComponentViewModel.dateLastUpdated(siteOptions.dateLastUpdated);
		});
	};

	HeaderComponentViewModel = function () {
		this.dateLastUpdated = ko.observable();
		initAsync.call(this);
	};

    return { viewModel: HeaderComponentViewModel, template: componentTemplate };
});
