define(['stringUtil', 'router', 'diaryFilter', 'jquery-dateFormat'], function(stringUtil, router, DiaryFilter, dateFormat) {

	"use strict";

	var MenuOption, menuOptionYearClick, menuOptionMonthClick;

	menuOptionYearClick = function(menuOption) {
		//menuOption.selectedYearObservable(menuOption.year);

		// When we select a year we change the url.
		router.updateCalendarHash(menuOption.year);
	};

	menuOptionMonthClick = function(calendarTileComponentViewModel) {
		var startDateV,
			endDateV,
			diaryFilter,
			clickUrl,
			title,
			menuOption = calendarTileComponentViewModel.menuOption;

		startDateV = menuOption.year.toString() + menuOption.month.toString() + "00";
		endDateV = menuOption.year.toString() + menuOption.month.toString() + "32";
		diaryFilter = DiaryFilter.createWithDateRange(startDateV, endDateV);
		title = encodeURIComponent(menuOption.title + " " + menuOption.year.toString());
		clickUrl = "#/entries/false/" + title + "/" + diaryFilter.getUrl();
		router.setUrl(clickUrl);
	};

	MenuOption = function (id, title, clickCallback, imageUrl) {

		var self = this;

		this.hasChildren = function () {
			return self.children !== null;
		};

		this.id = id;
		this.title = title.toString();
		this.imageUrl = imageUrl;
		this.children = null;
		this.clickUrl = clickCallback;
	};

	MenuOption.createYear = function (year, months, selectedYearObservable) {
		var monthIndex, month, monthPadded, yearMenuOption, monthMenuOption, imageUrl, id;

		imageUrl = "images/tiles/" + year.toString() + ".jpg";
		id = "year" + year;
		yearMenuOption = new MenuOption(id, year, menuOptionYearClick, imageUrl);
		yearMenuOption.children = [];
		yearMenuOption.selectedYearObservable = selectedYearObservable;
		yearMenuOption.year = year;

		for (monthIndex = 0; monthIndex < months.length; monthIndex += 1) {
			month = months[monthIndex];
			monthPadded = stringUtil.pad(2, month.toString(), '0');
			monthMenuOption = MenuOption.createMonth(year, monthPadded);
			yearMenuOption.children.push(monthMenuOption);
		}

		return yearMenuOption;
	};

	MenuOption.createMonth = function (year, month) {
		var imageUrl, monthMenuOption, monthFormat, id;

		if (month === "00") {
			imageUrl = "images/tiles/" + year.toString() + ".jpg";
		} else {
			imageUrl = "images/tiles/" + year.toString() + "_" + month.toString() + ".jpg";
		}

		if (month === "00") {
			monthFormat = "-";
		} else {
			monthFormat = new Date(year, parseInt(month, 10) - 1, 1);
			monthFormat = dateFormat.format.date(monthFormat, "MMM");
		}

		id = "month" + monthFormat;
		monthMenuOption = new MenuOption(id, monthFormat, menuOptionMonthClick, imageUrl);
		monthMenuOption.year = year;
		monthMenuOption.month = month;

		return monthMenuOption;
	};

	return MenuOption;
});