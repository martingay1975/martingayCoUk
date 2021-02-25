/*global define*/
define(['text!./diabetes.html'], function (componentTemplate) {

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
