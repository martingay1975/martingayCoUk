/*global require, window*/
define(['knockout', 'provider', 'router', 'text!./entries.html?version=1', 'bindingHandlersFormatDate'],
	function (ko, provider, router, componentTemplate) {

		"use strict";

		// http://localhost:56836/#/whoops/false/$top=3&$sort=woopsRating&$sortDirection=asc

		var EntriesComponentViewModel = function (params) {

			var self = this, init, diaryProvider;

			this.diaryEntries = ko.observableArray();
			this.title = ko.observable("");
			this.iconPath = ko.observable("");
			this.buttons = [];

			this.buttonClick = function(button) {
				router.setUrl("#/whoops/false/" + button.hash);
			}

			init = function() {

				// get the diary provider and filtered search
				diaryProvider = provider.getFromRoute();
				if (!params.filter) {
					params.filter = diaryProvider.getDefaultFilter();
				}

				if (!params.title) {
					params.title = diaryProvider.defaultName;
				}

				self.buttons = diaryProvider.buttons;

				self.title(params.title);

				self.iconPath(diaryProvider.getIconPath(params));

				// go and get the entries from the JSON file.
				var getEntriesPromise = diaryProvider.getEntriesAsync(params);

				// Set the knockout observable with the entries
				getEntriesPromise.done(function (arrayOfEntries) {
					self.diaryEntries(arrayOfEntries);
				});

				// Handle if unable to get the diary entries
				getEntriesPromise.fail(function (err) {
					window.alert("There is an issue. " + JSON.stringify(err));
				});
			};

			init();
		};

		return { viewModel: EntriesComponentViewModel, template: componentTemplate };
	});
