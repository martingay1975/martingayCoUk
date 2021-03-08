/*global define*/
define(['text!./spotify.html'], function (componentTemplate) {

	"use strict";

	var ComponentViewModel;

	ComponentViewModel = function (params) {

		if (!(this instanceof ComponentViewModel)) {
			throw "Must invoke the function ComponentViewModel with the new operator";
		}

		// backdoor to clearing the local cache.
		if (params.clearcache) {
			window.localStorage.clear();
		}
	};

	ComponentViewModel.prototype = {

	};

	return { viewModel: ComponentViewModel, template: componentTemplate };

});
