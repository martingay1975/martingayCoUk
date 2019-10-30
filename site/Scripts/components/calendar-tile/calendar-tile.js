/*global define*/
define(['text!./calendar-tile.html'], function (componentTemplate) {

	"use strict";

	var CalendarTileComponentViewModel;

	CalendarTileComponentViewModel = function (params) {

		if (!(this instanceof CalendarTileComponentViewModel)) {
			throw "Must invoke the function CalendarTileComponentViewModel with the new operator";
		}

		this.menuOption = params.menuOption;
	};
	
	return { viewModel: CalendarTileComponentViewModel, template: componentTemplate };

});
