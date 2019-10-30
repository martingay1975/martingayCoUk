/*global require*/
define(['knockout', 'router', 'provider', 'text!./search.html'], function (ko, router, provider, componentTemplate) {

	"use strict";

	var SearchComponentViewModel,
		init,
		executeSearch,
		searchClickHandler,
		checkThis,
		KEYCODE_ENTER = 13;

	checkThis = function() {
		if (!(this instanceof SearchComponentViewModel)) {
			throw "Must invoke the function SearchComponentViewModel with the new operator";
		}
	};

	executeSearch = function () {
		checkThis.call(this);

		var newHash;

		newHash = {
			expanded: false,
			filter: "title=" + encodeURIComponent(this.searchTerm()),
			title: this.searchTerm(),
			componentName: "entries"
		};

		if (!router.currentRouteObservable().diaryProvider) {
			newHash.diaryProvider = "default";
		}

		router.updateDiaryHash(newHash);
	};

	searchClickHandler = function (event) {
		checkThis.call(this);

		if (event.keyCode === KEYCODE_ENTER) {
			this.executeSearch();
		}

		return true;
	};

	init = function() {
		checkThis.call(this);

	};

	SearchComponentViewModel = function () {
		checkThis.call(this);

		var self = this;

		this.searchTerm = ko.observable("");
		this.executeSearch = executeSearch.bind(self);
		this.placeholder = ko.computed(function () {
			var diaryProvider = provider.getFromRoute();
			return "Search " + diaryProvider.defaultName;
		});
		
		// Description: When 'enter' key is pressed, run the search.
		this.keyPressEvent = function (ignore, event) {
			return searchClickHandler.call(self, event);
		};
		
		init.call(this);
	};

	return { viewModel: SearchComponentViewModel, template: componentTemplate };
});
