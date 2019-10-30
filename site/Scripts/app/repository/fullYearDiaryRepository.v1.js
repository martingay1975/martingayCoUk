/*global define window*/
define(["jquery", "knockout", "stringUtil", "entryUtil", "siteOptions"], function ($, ko, stringUtil, entryUtil, siteOptions) {

	"use strict";

	/* private */
	var keyPrefix = "entry";
	var getEntryKey;
	var getEntryKeyFromDate;
	var diaryRepository;
	var CacheEntry;

	getEntryKeyFromDate = function (entryDate) {
		var id = entryUtil.getId(entryDate);
		return keyPrefix + id;
	};

	getEntryKey = function (entry) {
		var key = getEntryKeyFromDate(entry.date);
		return key;
	};

	var hasLocalStorage = function() {
		var uid = new Date;
		var storage;
		var result;
		try {
			(storage = window.localStorage).setItem(uid, uid);
			result = storage.getItem(uid) == uid;
			storage.removeItem(uid);
			return result;
		} catch (exception) {
			return false;
		}
	};


	CacheEntry = function(yearHashValue, entry) {
		this.yearHashValue = yearHashValue;
		this.entry = entry;
	};

	/* public */
	diaryRepository = {

		entries: [],
		
		localStorageSupport: function () {
			
			try {
				if (window.hasOwnProperty("localStorage")) {
					return window.localStorage !== null;
				}

				return false;
			} catch (ignore) {
				return false;
			}
			
		},

		clear : function () {
			window.localStorage.clear();
		},


		// Description: A request to get a diary entry, given a date
		getFullEntryAsync: function (entryDate) {

			// Need to know whether we have the latest entry in local storage.
			// Compare the hash value for the year from siteOptions (latest) with the one stored in localstorage.

			var key = getEntryKeyFromDate(entryDate);
			var getEntryPromise = new $.Deferred();
			var START_YEAR = 1995;
			var self = this;
			var cacheEntryJson = window.localStorage.getItem(key);
			var year = ko.toJS(entryDate).y;
			var jsonFilePath;
			var entry;
			var filename;

			if (year < START_YEAR) {
				filename = "old.json";
			} else {
				filename = stringUtil.format("{0}.json", year);
			}

			var getHashValuePromise = siteOptions.getHashValueAsync(filename);

			getHashValuePromise.done(function(hashValue) {
				
				if (cacheEntryJson !== null) {

					// the entry in local storage is the JSON version. Therefore need to turn back into a JS object.
					var cacheEntry = JSON.parse(cacheEntryJson);

					if (cacheEntry.yearHashValue === hashValue) {
						getEntryPromise.resolve(cacheEntry.entry);
					}
				}

				if (getEntryPromise.state() !== "resolved")
				{
					// We have not got a value or the up to date value. Therefore need to go and fetch the JSON
					jsonFilePath = "/res/json/" + filename;
					$.getJSON(jsonFilePath).then(function (rawDiary) {
						
						self.addEntries(hashValue, rawDiary.entries);

						// recursive call. We have filled local storage, so when we call now for a second time the correct result will be returned.
						cacheEntryJson = window.localStorage.getItem(key);
						entry = JSON.parse(cacheEntryJson).entry;
						getEntryPromise.resolve(entry);
					}, function (err) {
						var message = "Failed to get " + jsonFilePath + ". " + JSON.stringify(err);
						getEntryPromise.reject(message);
					});
				}
			});

			getHashValuePromise.fail(function () {
				getEntryPromise.reject();
			});

			return getEntryPromise.promise();
		},

		addEntries: function (yearHashValue, entries) {
			
			var self = this;
			$.each(entries, function (ignore, entry) {
				self.addEntry(yearHashValue, entry);
			});

			//this.console("after addEntries");
		},

		addEntry: function (yearHashValue, entry) {
			var key = getEntryKey(entry);

			// hmtl local storage can only store strings, so JSON-ify the object graph
			window.localStorage.setItem(key, ko.toJSON(new CacheEntry(yearHashValue, entry)));
		},

		console: function (title) {
			var i;
			var len = window.localStorage.length;
			var key;
			var value;

			window.console.log(title);
			for (i = 0; i < len; i += 1) {
				key = window.localStorage.key(i);
				value = window.localStorage[key];
				window.console.log(key + " => " + value);
			}
		}

	};
	
	return diaryRepository;
});
