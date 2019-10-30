/*global define*/
define(['text!./strava.html'], function (componentTemplate) {

	"use strict";

	var ComponentViewModel;

	ComponentViewModel = function (params) {

		if (!(this instanceof ComponentViewModel)) {
			throw "Must invoke the function ComponentViewModel with the new operator";
		}
	};
	
	ComponentViewModel.prototype = {
		
	};

	return { viewModel: ComponentViewModel, template: componentTemplate };

});
