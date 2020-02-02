define(["jquery", "knockout", "fullYearDiaryRepository", "router", "entryUtil"], function ($, ko, fullYearDiaryRepository, router, entryUtil) {

	"use strict";
	// public
	var entryExtension = {

		/* Description: Extend the entry to include state data for the page. */
		extendEntry : function (entry, isContentDisplayed) {
			$.extend(entry, {
				isContentDisplayed: ko.observable(isContentDisplayed),

				getUrl: function() {
					var url = router.getDiaryUrl({
						filter: "id=" + this.getId(),
						expanded: true
					});
					return "/#" + url;
				},

				gotoEntryOnly: function() {
					window.open(this.getUrl());
				},

				getId: function() {
					return entryUtil.getId(entry.date);
				},

				getDateValue: ko.pureComputed(function() {
					return entryUtil.getDateValue(entry.date);
				}, entry),

				getJSDateValue: function() {
					var day = 1,
						month = 0,
						year = 1900;

					if (entry.date.d) {
						day = entry.date.d;
					}

					if (entry.date.m) {
						month = entry.date.m - 1;
					}

					if (entry.date.y) {
						year = entry.date.y;
					}

					var date = new Date();
					date.setFullYear(year, month, day);
					return date;
				},

				toggleContentDisplayed: function(forceValue) {
					// potentially need to do two things:
					// 1) toggle the isContentDisplayed flag.
					// 2) if toggling to show, make sure there is something to show. If not lazy load it.

					var newIsContentDisplayed,
						repositoryEntryPromise,
						self = this;

					if (typeof forceValue === "boolean") {
						if (this.isContentDisplayed() === forceValue) {
							return;
						}
						newIsContentDisplayed = forceValue;
					} else {
						newIsContentDisplayed = !this.isContentDisplayed();
					}

					// if new value is to display content
					if (newIsContentDisplayed) {

						// if there is no content to show, probably because need to go off and load it. Lazy loading.
						if (!this.info || !this.info.content || this.info.content.length === 0) {
							repositoryEntryPromise = fullYearDiaryRepository.getFullEntryAsync(this.date);
							repositoryEntryPromise.done(function(reposEntry) {
								if (reposEntry.info && reposEntry.info.content) {
									self.info.content(reposEntry.info.content);
								}
								self.isContentDisplayed(newIsContentDisplayed);
							});
						}
					}

					this.isContentDisplayed(newIsContentDisplayed);
				},

				hasYouTube: entryUtil.hasYouTube.call(entry),
				hasImages: entryUtil.hasImages.call(entry)
			});

			if (!entry.info) {
				entry.info = { content: ko.observable() };
			}



		}
	};

	return entryExtension;
});
