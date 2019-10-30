/*global define*/
define(['stringUtil', 'entryUtil', 'urlBuilder'], function (stringUtil, entryUtil, UrlBuilder) {

	"use strict";

	var DiaryFilter, jsDateToDatev, TOKEN, FIELD, EXPRESSIONSPLITTER;

	TOKEN = {
		top: "$top",
		orderBy: "$orderby",
		startDate: "startdate",
		startDatev: "startdatev",
		endDate: "enddate",
		endDatev: "enddatev",
		id: "id",
		ids: "ids"
	};

	FIELD = {
		woopsRating: "woopsrating"
	};

	EXPRESSIONSPLITTER = "&";

	DiaryFilter = function () {

		var self = this;

		this.startDate = null;	// in datev format
		this.endDate = null;	// in datev format
		this.title = null;
		this.orderBy = null;
		this.orderByDescending = false;
		this.top = null;
		this.ids = null;

		this.execute = function (entriesArray) {

			var filtered, orderByFunc;

			// ===============
			// Apply the 'where' clause
			filtered = entriesArray.filter(function(entry) {

				var index = -1;

				// filter on the id (a composite of the date - be careful not all dates have a day or month value)
				if (self.ids) {
					var entryId = entryUtil.getId(entry.date);
					index = self.ids.indexOf(entryId);
					if (index === -1) {
						return false;
					}
				}

				// filter on the end date?
				if (self.endDate && entry.datev > self.endDate) {
					return false; // entry date is after the end date
				}

				// filter on the start date?
				if (self.startDate && entry.datev < self.startDate) {
					return false; // entry date is before the start date
				}

				// filter on the title? case insensitive. wildcard... text can appear anywhere.
				if (self.title) {
					if (!entry.title || !entry.title.value || entry.title.value.toLowerCase().indexOf(self.title) < 0) {
						return false; // if there is a wildcard title to search and is not in the entry title field.
					}
				}

				return true;
			});

			// ===============
			// orderBy item

			// select the orderBy function
			if (self.orderBy === FIELD.woopsRating) {
				orderByFunc = self.orderByDescending ? self.orderByWoopsRatingDescending : self.orderByWoopsRatingAscending;
			} else {
				// (!self.orderBy || self.orderBy === "date")
				orderByFunc = self.orderByDescending ? self.orderByDateDescending : self.orderByDateAscending;
			}

			// apply the sort
			filtered = filtered.sort(orderByFunc);
			
			// ===============
			// Apply the 'take top n'
			if (self.top) {
				filtered = filtered.slice(0, self.top);
			}

			return filtered;
		};

		this.orderByDateDescending = function(noInfoEntryA, noInfoEntryB) {
			return noInfoEntryB.datev - noInfoEntryA.datev;
		};

		this.orderByDateAscending = function (noInfoEntryA, noInfoEntryB) {
			return noInfoEntryA.datev - noInfoEntryB.datev;
		};

		this.orderByWoopsRatingDescending = function (entryA, entryB) {
			// woops rating is stored in the title.name field (for some reason)
			return parseInt(entryB.title.name, 10) - parseInt(entryA.title.name, 10);
		};

		this.orderByWoopsRatingAscending = function (entryA, entryB) {
			// woops rating is stored in the title.name field (for some reason)
			return parseInt(entryA.title.name, 10) - parseInt(entryB.title.name, 10);
		};

		this.getUrl = function() {
			var urlBuilder = new UrlBuilder();
			//urlBuilder.addFragment(TOKEN.ids, self.ids.split(","));
			urlBuilder.addFragment(TOKEN.startDatev, self.startDate);
			urlBuilder.addFragment(TOKEN.endDatev, self.endDate);
			return urlBuilder.url;
		};

	};

	// static functions
	DiaryFilter.createWithDateRange = function(startDateV, endDateV) {
		var diaryFilter = new DiaryFilter();
		diaryFilter.startDate = startDateV;
		diaryFilter.endDate = endDateV;
		return diaryFilter;
	};

	// Factory function to create a DiaryFilter object.
	DiaryFilter.createFromQueryString = function (queryString) {

		var diaryFilterItemsArray = (decodeURIComponent(queryString)).split(EXPRESSIONSPLITTER),
			expression,
			field,
			value,
			diaryFilter = new DiaryFilter();

		diaryFilterItemsArray.forEach(function (diaryFilterItem) {

			expression = diaryFilterItem.split('=');
			field = expression[0].toLowerCase();
			value = expression[1].trim().toLowerCase();

			if (field === '$top') {
				diaryFilter.top = parseInt(value, 10);
			} else if (field === 'enddate') {
				diaryFilter.endDate = jsDateToDatev(new Date(value));
			} else if (field === 'enddatev') {
				diaryFilter.endDate = value;
			} else if (field === 'startdate') {
				diaryFilter.startDate = jsDateToDatev(new Date(value));
			} else if (field === 'startdatev') {
				diaryFilter.startDate = value;
			} else if (field === 'title') {
				diaryFilter.title = value;
			} else if (field === 'id') {
				diaryFilter.ids = [value];	// makes the id a single array element.
			} else if (field === 'ids') {
				diaryFilter.ids = value.split(",");	// joins a comma separated list of commas into a list.
			} else if (field === '$orderby') {
				var orderByValue = value.split(' ');
				diaryFilter.orderBy = orderByValue[0];
				if (orderByValue.length > 1) {
					diaryFilter.orderByDescending = (orderByValue[1] === 'desc');
				}
			}

		});

		return diaryFilter;
	};

	jsDateToDatev = function (jsDate) {
		var year, month, day;
		year = jsDate.getFullYear().toString();
		month = stringUtil.pad(2, jsDate.getMonth() + 1, "0");
		day = stringUtil.pad(2, jsDate.getDate(), "0");

		return parseInt(year + month + day, 10);
	};

	return DiaryFilter;

});