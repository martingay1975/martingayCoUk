/*global require*/
define(['knockout', 'menuOption', 'noInfoRepository', 'text!./calendar.html'], function (ko, MenuOption, noInfoRepository, componentTemplate) {

	"use strict";

	var CalendarComponentViewModel,
		ArchiveMenu;
	
	ArchiveMenu = function (selectedYearObservable) {

		var initAsync,
			self = this;

		this.years = [];

		initAsync = function () {
			var yearOption, yearProperty, months, getYearsPromise;

			getYearsPromise = noInfoRepository.getYearsAndMonthsAsync();

			return getYearsPromise.then(function (menuYears) {

				for (yearProperty in menuYears) {
					months = menuYears[yearProperty];
					yearOption = MenuOption.createYear(yearProperty, months, selectedYearObservable);
					self.years.unshift(yearOption);
				}
			});
		};

		this.getYearOption = function(year) {
			var yearIndex, yearOption;

			if (typeof year !== "string") {
				year = year.toString();
			}

			for (yearIndex = 0; yearIndex < self.years.length; yearIndex += 1) {
				yearOption = self.years[yearIndex];
				if (yearOption.title === year) {
					return yearOption;
				}
			}

			throw "Unable to find the year " + year;
		};

		this.initPromise = initAsync();
	};

	CalendarComponentViewModel = function (params) {

		var init,
			self = this,
			selectYear,
			archiveMenu,
			selectedYearSubscription;

		this.menuYears = ko.observableArray();
		this.menuMonths = ko.observableArray();
		this.selectedYearObservable = ko.observable();

		selectYear = function (selectedYear) {
			var yearOption = archiveMenu.getYearOption(selectedYear);
			yearOption.selected = true;
			self.menuMonths(yearOption.children);
		};

		selectedYearSubscription = this.selectedYearObservable.subscribe(selectYear);

		init = function () {
			archiveMenu = new ArchiveMenu(self.selectedYearObservable);

			archiveMenu.initPromise.done(function() {
				self.menuYears(archiveMenu.years);

				if (params.year) {
					self.selectedYearObservable(params.year);
				}
			});
		};

		this.dispose = function () {
			if (selectedYearSubscription) {
				selectedYearSubscription.dispose();
			}
		};

		init();
	};

	return { viewModel: CalendarComponentViewModel, template: componentTemplate };
});
