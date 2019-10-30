/*global define*/
define(['diaryJsonReader'], function (diaryJsonReader) {

	"use strict";

	var WhoopsRepository = function () {

		var jsonFilePath = "/res/json/whoops.json",
			cache = null;

		this.readAsync = function (filter) {
			return diaryJsonReader.readAsync(jsonFilePath, filter, cache);
		};
	};

	return new WhoopsRepository();
});
