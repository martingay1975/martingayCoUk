define(['stringUtil'], function(stringUtil) {

	"use strict";

	var UrlBuilder = function() {

		var addSeparator;

		this.url = "";

		addSeparator = function() {
			if (this.url.length > 0) {
				this.url += "&";
			}
		};

		this.addFragment = function(token, field) {
			if (field) {
				var frag;
				addSeparator.call(this);
				frag = stringUtil.format("{0}={1}", token, field);
				this.url += frag;
			}
		};
	};

	return UrlBuilder;
});