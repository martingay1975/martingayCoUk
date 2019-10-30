/*global define*/
define(['jquery', 'stringUtil', 'noInfoRepository', 'diaryFilter', 'fullYearDiaryRepository', 'entryExtension'],
	function ($, stringUtil, noInfoRepository, diaryFilter, fullYearDiaryRepository, entryExtension) {

		"use strict";

		var DefaultProvider;
		
		DefaultProvider = function() {

			this.getEntriesAsync = function(params) {
				var filter,
					readPromise,
					getEntryPromise,
					entryCounter,
					entry,
					expanded = params.expanded,
					tempDiaryEntries = [],
					getEntryPromises = [],
					returnedPromise;


				returnedPromise = new $.Deferred();

				// create a filter object from the query string
				filter = diaryFilter.createFromQueryString(params.filter);

				// go and get the noInfo entries that adhere to the filter
				readPromise = noInfoRepository.readAsync(filter);

				readPromise.done(function(noInfoEntries) {

					// We have all the noInfoEntries. Do we need to get the content as well.... or are we just giving away the meta data for now?
					if (params.expanded) {
						try {

							// got the metadata from the result of the search.
							noInfoEntries.forEach(function(noInfoEntry) {
								// go and get the full diary entry from the fullYearDiaryRepository
								getEntryPromise = fullYearDiaryRepository.getFullEntryAsync(noInfoEntry.date);
								getEntryPromises.push(getEntryPromise);
							});

							// Once all the promises are in...... extend the data. 
							$.when.apply($, getEntryPromises).then(function() {

								
								for (entryCounter = 0; entryCounter < arguments.length; entryCounter += 1) {
									entry = arguments[entryCounter];
									if (entry === null) {
										var message = "The entry was null for the " + entryCounter + " element.";
										console.error(message);
										continue;
									}
									
									entryExtension.extendEntry(entry, expanded);
									tempDiaryEntries.push(entry);
								}

								returnedPromise.resolve(tempDiaryEntries);
							});
						} catch (ex) {
							var message = "exception getting content. " + JSON.stringify(ex);
							console.error(message);
							returnedPromise.reject(ex);
						}
					} else {
						try {
							for (entryCounter = 0; entryCounter < noInfoEntries.length; entryCounter += 1) {
								entry = noInfoEntries[entryCounter];
								entryExtension.extendEntry(entry, expanded);
								tempDiaryEntries.push(entry);
							}
						} catch (ex) {
							console.error("exception making noInfo entries. " + JSON.stringify(ex));
							returnedPromise.reject(ex);
						}

						returnedPromise.resolve(tempDiaryEntries);
					}
				});

				readPromise.fail(function (err) {
					var message = "failed to read noInfoRepository. " + JSON.stringify(err);
					console.error(message);
					returnedPromise.reject(err);
				});

				return returnedPromise;
			};

			this.buttons = [];

			this.getDefaultFilter = function() {
				return stringUtil.format('$top=10&$orderBy=date desc&startDate={0}-{1}-{2}', new Date().getFullYear() - 1, 11, 1);
			};

			this.defaultName = "Diary";

			this.getIconPath = function(params) {

				var chopppedTitle, year;

				// latest
				if (params.filter === this.getDefaultFilter()) {
					return "/Scripts/components/navigation/images/calendar/latest.png";
				}

				// archive
				chopppedTitle = params.title.split(' ');
				if (chopppedTitle.length === 2) {
					year = parseInt(chopppedTitle[1], 10);
					if (year > 1900 && year < 2030) {
						return "/Scripts/components/navigation/images/calendar/archive.png";
					}
				}

				// search
				return "/Scripts/components/navigation/images/calendar/search-icon.png";
			};
		};

		return new DefaultProvider();
	});