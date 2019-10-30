/*global define*/
define(['text!./componentTemplate.html'], function (componentTemplate) {

	"use strict";

	// TODO: - register component in app\knockout-startup.js
	// TODO: - run JSLint against file
	// TODO: - update the define statement with the correct filename for the componentTemplate

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
