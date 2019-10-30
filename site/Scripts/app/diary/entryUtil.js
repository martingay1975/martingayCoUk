/*global define*/
define(["knockout"], function (ko) {

	"use strict";

	// private
	var months = ["", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],

	//public
		entryUtil = {
			getDateValue : function (entryDate, monthValue, separator) {

				var ret = "",
					plainEntry;

				if (separator === undefined) {
					separator = " ";
				}

				// handle if we pass in the entryDate as a ko object.
				plainEntry = ko.toJS(entryDate);

				if (monthValue === undefined) {
					monthValue = months[plainEntry.m];
				}

				if (plainEntry.d) {
					ret += plainEntry.d;
				}

				if (plainEntry.m) {
					ret += separator + monthValue;
				}

				ret += separator + plainEntry.y;

				return ret;
			},

			getId: function (entryDate) {
				// handle if we pass in the entryDate as a ko object.
				var plainEntry = ko.toJS(entryDate),
					id = this.getDateValue(plainEntry, plainEntry.m, "-");

				return id;
			},

			hasImages: function () {
				return this && this.images > 0;
			},

			hasYouTube: function () {
				if (this && this.info && this.info.content) {
					return this.info.content.indexOf("youtube.com ") > -1;
				}

				return false;
			}
		};

	return entryUtil;
});