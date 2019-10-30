/*global define*/
define(['jquery', 'polyfillArrayFind'], function ($) {

	"use strict";

	var SiteOptions = function() {

		var self = this,
			initAsync;

		this.dateLastUpdated = null;
		this.hashes = [];

		initAsync = function() {

			var siteOptionsPromise = $.ajax({
				url: "/res/json/siteOptions.json",
				dataType: "json",
				cache: false
			});

			return siteOptionsPromise.then(function(siteOptionsServerData) {
				self.dateLastUpdated = siteOptionsServerData.dateLastUpdated;
				self.hashes = siteOptionsServerData.hashes;
			});
		};
		
		// Description: gets the hash value, given a key.
		this.getHashValueAsync = function(key) {

			var deferred = new $.Deferred();
			self.initAsyncPromise.done(function() {
				var hash = self.hashes.find(function (current) {
					return current.Key === key;
				});

				if (typeof hash === "undefined")
				{
					deferred.reject();
				}

				deferred.resolve(hash.Value);
			});

			self.initAsyncPromise.fail(function () {
				deferred.reject();
			});

			return deferred.promise();
		};

		this.initAsyncPromise = initAsync();
	};

	return new SiteOptions();
});

