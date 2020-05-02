/*global define*/
define(['jquery', 'moment', 'stringUtil', 'noInfoRepository', 'diaryFilter', 'fullYearDiaryRepository', 'monthTitle', 'entryExtension'],
	function ($, moment, stringUtil, noInfoRepository, diaryFilter, fullYearDiaryRepository, MonthTitle, entryExtension) {

		"use strict";

		let DefaultProvider;
		
		DefaultProvider = function(params) {

			this.getEntriesAsync = function(params) {
				let filter,
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
										let message = "The entry was null for the " + entryCounter + " element.";
										console.error(message);
										continue;
									}
									
									entryExtension.extendEntry(entry, expanded);
									tempDiaryEntries.push(entry);
								}

								returnedPromise.resolve(tempDiaryEntries);
							});
						} catch (ex) {
							let message = "exception getting content. " + JSON.stringify(ex);
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
					let message = "failed to read noInfoRepository. " + JSON.stringify(err);
					console.error(message);
					returnedPromise.reject(err);
				});

				return returnedPromise;
			};

			this.buttons = this.getButtons = function(params) {
				const filter = diaryFilter.createFromQueryString(params.filter);
				
				// if there is a filter with 00 day of the month as the startDate and 32 as the endDate then we know we are looking 
				// at month views and should offer navigational buttons back and forth.
				

				if (filter.startDate && filter.endDate)
				{
					var today = moment();
					let startMonth = parseInt(filter.startDate.substr(4,2));
					let endMonth = parseInt(filter.endDate.substr(4,2));
					let isStartOfMonth = filter.startDate.toString().endsWith('00');
					let isEndOfMonth = filter.endDate.toString().endsWith('32');
					let startYear = parseInt(filter.startDate.substr(0,4));
					let endYear = parseInt(filter.endDate.substr(0,4));
					
					if (startYear < 1970)
					{
						return [];
					}
	
					if (isStartOfMonth && isEndOfMonth)
					{
						if (startYear == endYear)
						{
							// start and end dates are in same month and year
							const monthTitle = new MonthTitle();
							
							if (startMonth == endMonth)
							{
								let diaryTitlePrev = monthTitle.getMonth({datev: filter.startDate, monthAdjust: -1});
								let diaryCurrentYear = monthTitle.getYear(startYear);

								const returnButtons = [];
								returnButtons.push({label: diaryCurrentYear.title, hash: diaryCurrentYear.hash});
								returnButtons.push({label: diaryTitlePrev.title, hash: diaryTitlePrev.hash});

								if (!(startYear == today.year() && startMonth == today.month()+1)) {
									let diaryTitleNext = monthTitle.getMonth({datev: filter.startDate, monthAdjust: 1});
									returnButtons.push({label: diaryTitleNext.title, hash: diaryTitleNext.hash});
								}

								return returnButtons;
							} else {
								// start date is start of a month, end date is different month - both same year

								const returnButtons = [];
								const diaryPrevYear = monthTitle.getYear(startYear - 1);
								returnButtons.push({label: diaryPrevYear.title, hash: diaryPrevYear.hash});

								if (startYear < today.year())
								{
									const diaryNextYear = monthTitle.getYear(startYear + 1);
									returnButtons.push({label: diaryNextYear.title, hash: diaryNextYear.hash});
								}

								return returnButtons;
							}
						}
					}
				}

				return [];
			};

			this.getDefaultFilter = function() {
				return stringUtil.format('$top=10&$orderBy=date desc&startDate={0}-{1}-{2}', new Date().getFullYear() - 1, 11, 1);
			};

			this.defaultName = "Diary";

			this.getIconPath = function(params) {

				let chopppedTitle, year;

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