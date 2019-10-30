/*global define*/
define(['jquery', 'diaryFilter', 'whoopsRepository', 'entryExtension', 'stringUtil'], function ($, diaryFilter, whoopsRepository, entryExtension, stringUtil) {

	"use strict";

	var WhoopsProvider = function() {

		this.getEntriesAsync = function (params) {

			var whoopsRepositoryPromise,
				retDeferred = new $.Deferred(),
				filter;

			// create a filter object from the query string
			filter = diaryFilter.createFromQueryString(params.filter);

			whoopsRepositoryPromise = whoopsRepository.readAsync(filter);
			whoopsRepositoryPromise.done(function(whoopsEntries) {

				whoopsEntries.forEach(function(entry) {
					entryExtension.extendEntry(entry, true);
					entry.woopsRating = parseInt(entry.title.name, 10) * 10;
				});

				retDeferred.resolve(whoopsEntries);
			});

			whoopsRepositoryPromise.fail(function(err) {
				retDeferred.reject(err);
			});

			return retDeferred;
		};

		this.getDefaultFilter = function() {
			// if there are no filters, rather than return everything, just return the last 10 entries
			return "$orderBy=woopsRating desc";
		};

		this.orderByDateDescending = function() {
			return stringUtil.format('$orderBy=date desc', new Date().getFullYear() - 1, 11, 1);
		};

		this.buttons = [
						{label: "By Rating", hash: this.getDefaultFilter()}, 
						{label: "By Date", hash: this.orderByDateDescending()}
					];

		this.defaultName = "Whoops";

		this.getIconPath = function(filter) {
			return "/Scripts/components/navigation/images/calendar/whoops.png";
		};

	};

	return new WhoopsProvider();
});