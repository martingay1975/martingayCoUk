/*global require*/
define(['knockout', 'router', 'provider', 'stringUtil', 'text!./search.html'], function (ko, router, provider, stringUtil, componentTemplate) {

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

		// for the filter this will eventually end up in diaryFilter
		let filter = "";
		if (this.searchTerm().length === 4 && !isNaN(this.searchTerm())) {

			// search on the year
			const filterStartYear = this.searchTerm().substring(0, 4);
			const filterStartMonth = "01";
			const filterStartDay = "01";

			const filterEndYear = this.searchTerm().substring(0, 4);
			const filterEndMonth = "12";
			const filterEndDay = "31";

			filter = stringUtil.format("startdatev={0}{1}{2}&enddatev={3}{4}{5}",
				filterStartYear,
				filterStartMonth,
				filterStartDay,
				filterEndYear,
				filterEndMonth,
				filterEndDay);
		} else {
			// search on the title
			filter = "title=" + encodeURIComponent(this.searchTerm())
		}

		newHash = {
			expanded: false,
			filter: filter,
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
