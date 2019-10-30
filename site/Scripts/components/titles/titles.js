/*global require*/
define(['knockout', 'text!./titles.html'], function (ko, componentTemplate) {

	"use strict";

	var TitleComponentViewModel;

    TitleComponentViewModel = function (params) {
    	this.iconPath = params.iconPath;
	    this.title = params.title;
    };

    return { viewModel: TitleComponentViewModel, template: componentTemplate };
});
