/*global define*/
define(['jquery', 'diaryJsonReader'], function ($, diaryJsonReader) {

	"use strict";

	var NoInfoRepository = function() {

		var self = this,
			jsonFilePath = "/res/json/noInfo-all.json",
		    cache = null;
		    
		/*
		 * entries = [
		 *				{date : { d:10, m:7, y:1975 },
		 *				 datev : 19750710,
		 *				 title : "Martin Born" 
		 *				},
		 * ]\
		 */

		this.readAsync = function(filter) {
			return diaryJsonReader.readAsync(jsonFilePath, filter, cache);
		};

		this.getYearsAndMonthsAsync = function() {
			var readPromise,
				year,
				month,
				monthIndex,
				deferred = new $.Deferred(),
				ret = {};

			readPromise = self.readAsync();

			readPromise.done(function(noInfoResults) {

				try {
					noInfoResults.entries.forEach(function (entry) {

						year = entry.date.y.toString();
						month = entry.date.m || 0;

						if (!ret.hasOwnProperty(year)) {
							ret[year] = [];
						}

						monthIndex = ret[year].indexOf(month);
						if (monthIndex === -1) {
							ret[year].push(month);
						}
					});
				} catch (e) {
					deferred.reject(e);
				}

				deferred.resolve(ret);
			});

			return deferred;
		};
	};

	return new NoInfoRepository();
});