/*global define*/
define(function () {

	"use strict";

	var stringUtil = {

		// equivalent to the string.format function in .Net
		format: function (formatString) {
			var finalString = formatString, i, reg;
			for (i = 0; i < arguments.length - 1; i += 1) {
				reg = new RegExp("\\{" + i + "\\}", "gm");
				finalString = finalString.replace(reg, arguments[i + 1]);
			}
			return finalString;
		},

		formatDataSize: function (bytes) {
			var value, sizeSuffix, itr = 0, divisor = 1024; // Make 1000 to avoid; 1023.000 KB.

			sizeSuffix = ['B ', 'KB', 'MB', 'GB', 'TB', 'PB'];

			if (!isNaN(bytes)) {
				value = bytes;
				while ((value > divisor) && (itr < sizeSuffix.length - 1)) {
					itr += 1;
					value /= divisor;
				}

				// if "bytes" (itr = 0) then don't force 3 decimal places, just round to whole bytes.
				return (itr > 0 ? value.toFixed(3) : Math.round(value)) + " " + sizeSuffix[itr];
			}

			return bytes;
		},

		formatPercentage: function (val) {
			var ret = "",
				percentageNumber;

			if (typeof val === 'string') {
				percentageNumber = parseFloat(val, 10);
			} else {
				percentageNumber = val;
			}

			if (percentageNumber < 0.01) {
				percentageNumber = 0.009;
			}

			if (percentageNumber < 0.01) {
				ret = '<';
			}

			ret += percentageNumber.toFixed(2).toString() + "%";
			return ret;
		},

		// Description: example- pad(5, 'hi', '0')  => "000hi"
		pad: function (width, str, padding) {
			str = str.toString();
			return (width <= str.length) ? str : stringUtil.pad(width, padding + str, padding);
		},

		capitalizeFirstLetter: function (value) {
			return value.charAt(0).toUpperCase() + value.slice(1);
		},

		isNullEmptyOrWhitespace: function (str) {
			return str === undefined || str === null || str.trim().length === 0;
		},

		isNotNullEmptyOrWhitespace: function (str) {
			return !this.isNullEmptyOrWhitespace(str);
		}
	};

	return stringUtil;
});