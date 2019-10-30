/*global define*/
define(['knockout', 'jquery-dateFormat'], function (ko, dateFormat) {

	"use strict";

	var evaluateNewValue,
		DEFAULT_FORMAT = 'd MMMM yyyy';

	evaluateNewValue = function (valueAccessor, allBindingsAccessor) {

		var value = valueAccessor(),
			valueAsDate,
			format = allBindingsAccessor().format || DEFAULT_FORMAT,
			formatted = value ? String(value) : "";

		if (isNaN(Date.parse(value)) === false) {
			valueAsDate = new Date(value);
			formatted = dateFormat.format.date(valueAsDate, format);
		}

		return formatted;
	};

	ko.bindingHandlers.formatDate = {

		init: function (element, valueAccessor, allBindingsAccessor, context) {
			var newValueAccessor = function () {
				return evaluateNewValue(valueAccessor, allBindingsAccessor);
			};

			ko.bindingHandlers.text.update(element, newValueAccessor, allBindingsAccessor, context);
		},

		update: function (element, valueAccessor, allBindingsAccessor, context) {
			var newValueAccessor = function () {
				return evaluateNewValue(valueAccessor, allBindingsAccessor);
			};

			ko.bindingHandlers.text.update(element, newValueAccessor, allBindingsAccessor, context);
		}
	};
	
});