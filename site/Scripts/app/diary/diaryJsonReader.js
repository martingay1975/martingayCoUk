/*global define*/
define(['jquery'], function($) {

	"use strict";

	var lazyLoad, DiaryJSon;

	// Helper to lazily get JSON entries,
	lazyLoad = {
		getJSONAsync: function(fileUrl, cache) {

			var promise, deferred;

			if (!cache || cache.length === 0) {
				promise = $.getJSON(fileUrl);
				promise.done(function(response) {
					cache = response;
				});
				return promise;
			}

			deferred = new $.Deferred();
			deferred.resolve(cache);
			return deferred.promise();
		}
	};

	// Get diary entries from a file and apply filter.
	DiaryJSon = function () {

		// read in a whole file - remember the file could be whoops, noInfo or a specific year.
		this.readAsync = function(jsonFilePath, filter, cache) {

			var getJsonPromise, deferred, filtered;

			getJsonPromise = lazyLoad.getJSONAsync(jsonFilePath, cache);
			deferred = new $.Deferred();
			getJsonPromise.done(function(entriesArray) {
				try {
					// we have all the entries in entriesArray. Now filter them.
					if (filter) {
						filtered = filter.execute(entriesArray.entries);
					} else {
						// no filtering required
						filtered = entriesArray;
					}

					deferred.resolve(filtered);
				} catch (e) {
					console.log("DiaryJSon error. " + JSON.stringify(e));
					deferred.reject(e);
				}
			});

			// unable to get the Json file successfully.
			getJsonPromise.fail(function(e) {
				deferred.reject(e);
			});

			return deferred;
		};

		// read in a specific date.
	};

	return new DiaryJSon();
});