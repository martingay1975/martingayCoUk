/*global define*/
define(['text!./calendar-year.html'], function (componentTemplate) {

	"use strict";

	var CalendarYearComponentViewModel;
	CalendarYearComponentViewModel = function (params) {

		if (!(this instanceof CalendarYearComponentViewModel)) {
			throw "Must invoke the function CalendarYearComponentViewModel with the new operator";
		}

		this.years = params.years;
	};
	
	return { viewModel: CalendarYearComponentViewModel, template: componentTemplate };

});
