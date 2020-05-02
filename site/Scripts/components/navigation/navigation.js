/*global require*/
define(["moment", "monthTitle", "text!./navigation.html"], function(moment, MonthTitle, componentTemplate) {

	"use strict";

	var NavigationComponentViewModel, DateEntry;

	DateEntry = function (origDate) {
		let monthTitle = new MonthTitle();
		const month = monthTitle.getMonth({moment: origDate});
		
		this.stringDate = month.title;
		this.href = month.hash;
		this.imgSrc = "/Scripts/components/navigation/images/calendar/" + month.moment.format("MM") + ".png";
	};

	NavigationComponentViewModel = function () {

		var init, getArchiveSubMenu, self = this;

		this.dateEntries = [];

		getArchiveSubMenu = function() {
			
			for (let monthCountBack = 0; monthCountBack < 3; monthCountBack ++) {
				let origDate = moment().subtract(monthCountBack, 'months');
				let dateEntry = new DateEntry(origDate);
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
