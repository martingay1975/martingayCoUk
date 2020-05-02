/*global define*/
define(['text!./image-dialog.html'], function (componentTemplate) {

	"use strict";

	var ImageDialogComponentViewModel;

	ImageDialogComponentViewModel = function (params) {

		if (!(this instanceof ImageDialogComponentViewModel)) {
			throw "Must invoke the function ImageDialogComponentViewModel with the new operator";
        }
        
        
	};
	
	return { viewModel: ImageDialogComponentViewModel, template: componentTemplate };
});
