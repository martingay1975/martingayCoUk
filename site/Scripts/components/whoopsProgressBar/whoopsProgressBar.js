/*global define*/
define(['text!./whoopsProgressBar.html'], function (componentTemplate) {

	"use strict";

	var WoopsProgressBarComponentViewModel;

	WoopsProgressBarComponentViewModel = function (params) {

		if (!(this instanceof WoopsProgressBarComponentViewModel)) {
			throw "Must invoke the function WoopsProgressBarComponentViewModel with the new operator";
		}

		this.woopsRating = params.woopsRating;
	};
	
	return { viewModel: WoopsProgressBarComponentViewModel, template: componentTemplate };
});
