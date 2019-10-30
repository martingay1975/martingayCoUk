/*global require*/
define(['moment', 'text!./navigation.html'], function(moment, componentTemplate) {

	"use strict";

	var NavigationComponentViewModel, DateEntry;

	DateEntry = function (origDate) {

		var startOfMonth, endOfMonth;

		startOfMonth = origDate.startOf("month");
		endOfMonth = startOfMonth.clone().endOf("month");

		this.stringDate = startOfMonth.format("MMM YYYY");
		this.href = "#/entries/false/" + encodeURIComponent(this.stringDate) + "/startdatev=" + startOfMonth.format("YYYYMMDD") + "&enddatev=" + endOfMonth.format("YYYYMMDD");
		this.imgSrc = "/Scripts/components/navigation/images/calendar/" + startOfMonth.format("DD") + ".png";
	};

	NavigationComponentViewModel = function () {

		var init, getArchiveSubMenu, self = this;

		this.dateEntries = [];

		getArchiveSubMenu = function() {

			var monthCountBack,
				origDate,
				dateEntry;

			for (monthCountBack = 0; monthCountBack < 3; monthCountBack ++) {
				origDate = moment().subtract(monthCountBack, 'months');
				dateEntry = new DateEntry(origDate);
				self.dateEntries.push(dateEntry);
			}
		};

		init = function() {
			getArchiveSubMenu();
		};

		init();
	};

	return { viewModel: NavigationComponentViewModel, template: componentTemplate };
});
