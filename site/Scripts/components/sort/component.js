/*global require*/
define(['text!./component.html'], function (componentTemplate) {

	"use strict";

	var ComponentViewModel;

    ComponentViewModel = function () {

    };

    return { viewModel: ComponentViewModel, template: componentTemplate };
});
