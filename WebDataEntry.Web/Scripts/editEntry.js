
	var EditEntry = function() {
		this.activeEntry = ko.observable(null);
		this._originalDate = null;
		this.imagesObservable = ko.observableArray([]);
		this.isEntryValid = ko.observable(false);
	};

	EditEntry.prototype = {

		_originalDate : null,

		saveActiveEntry: function (data) {

			if (!data.editEntry.isEntryValidInternal()) {
				alert("Entry is invalid");
			}

			var updatedPromise;
			if (!data.editEntry._originalDate) {
				updatedPromise = viewModelHelpers.createEntry(data.editEntry.activeEntry());
				data.editEntry._originalDate = ko.toJS(data.editEntry.activeEntry().DateEntry());
			} else {
				data.editEntry.activeEntry().Version += 1;	// increase the version of the entry.
				updatedPromise = viewModelHelpers.updateEntry(data.editEntry._originalDate, data.editEntry.activeEntry());
			}

			return updatedPromise.done(function() { viewModelHelpers.testValidDiary(); });
		},

		insertImage: function (data) {

			var dataContext = viewModel.editEntry;
			for (var index = 0; index < dataContext.imagesObservable().length; index++) {

				var iteratingImage = viewModel.editEntry.imagesObservable()[index];
				if (data.path === iteratingImage.path) {

					// add it to the info part of the entry
					var orig = dataContext.activeEntry().Info().Content();
					var addition = String.format('\r\n<image forweb="50">\r\n<src>{0}</src>\r\n<caption>{1}</caption>\r\n</image>', data.path, data.captionObservable());
					dataContext.activeEntry().Info().Content(orig + addition);

					// remove it from the list
					viewModel.editEntry.imagesObservable.splice(index, 1);

					return;
				}
			}

			//var index = viewModel.editEntry.imagesObservable.indexOf(data);
			//var caption;
			//if (index > -1) {
			//	viewModel.editEntry.imagesObservable.splice(index, 1);
			//	captionElement = document.getElementById('caption' + index);
			//	caption = captionElement.value;

			//	var orig = dataContext.activeEntry().Info().Content();
			//	var additions = String.format('\r\n<image forweb="50">\r\n<src>{0}</src>\r\n<caption></caption>\r\n</image>', imagePath);
			//	dataContext.activeEntry().Info().Content(orig + additions);
			//}


		},

		removeAllImages: function () {
			viewModel.editEntry.imagesObservable([]);
		},
		
		createActiveEntry: function(data, event) {
			var newEntryJs = EntryFactory.create();
			var newEntry = new Entry(newEntryJs);
			data.editEntry.activeEntry(newEntry);
			data.editEntry._originalDate = null;
			
			$('#entryTitle').focus();
		},

		_createValueChangedSubscription : function() {
			viewModel.editEntry.activeEntry().childValueChanged.subscribe(this.isEntryValidInternal);
		},

		loadActiveEntry: function (dateIndex) {

			// store the original date, just in case this changes as part of the edit.
			this._originalDate = ko.toJS(dateIndex);
			
			if (dateIndex) {
				// selecting new entry
				var entry = viewModelHelpers.findCachedEntry(dateIndex);

				if (entry == null)
					throw String.format("Unable to find the entry: Year-{0}, Month-{1}, Day-{2}", dateIndex.Year(), dateIndex.Month(), dateIndex.Day());

				// make a copy (rather than take a reference)
				var activeEntryCopy = new Entry(ko.toJS(entry));
				if (activeEntryCopy.Info().Content()) {
					var beaut = vkbeautify.xml(activeEntryCopy.Info().Content(), 4);
					activeEntryCopy.Info().Content(beaut);
				}

				// set the copied value
				viewModel.editEntry.activeEntry(activeEntryCopy);
				this._createValueChangedSubscription();
				
				$('#entryTitle').focus();

			} else {
				// deselecting entry
				viewModel.editEntry.activeEntry(null);
			}
		},

		isEntryValidInternal: function () {
			var errorCount = $("#mainEntry .has-error").length;
			var warningCount = $("#mainEntry .has-warning").length;
			console.log("errors: " + errorCount + "   warnings: " + warningCount);

			var isValid = (errorCount == 0);
			viewModel.editEntry.isEntryValid(isValid);
			return isValid;
		},
		
		addImage: function() {

			var dataContext = viewModel.editEntry;
			dataContext.imagesObservable();
			var files = $("#file1").get(0).files;
			if (files.length > 0) {
				if (window.FormData !== undefined) {
					var data = new FormData();
					for (var i = 0; i < files.length; i++) {
						data.append("file" + i, files[i]);
					}

					var url = viewModel.baseUrl + "api/rpc/UploadImages";
					$.ajax({
						type: "POST",
						url: url,
						contentType: false,
						processData: false,
						data: data,
						success: function (relativePaths) {
							for (var index = 0; index < relativePaths.length; index++) {
								var imagePath = relativePaths[index];
								dataContext.imagesObservable.push({ path: imagePath, captionObservable: ko.observable("") });
							}
						}
					});
				} else {
					alert("This browser doesn't support HTML5 multiple file uploads!");
				}
			}

		},

		addYouTube: function () {
			var orig = viewModel.editEntry.activeEntry().Info().Content();
			orig += '\r\n<div class="youtube"><iframe width="560" height="315" src="https://www.youtube.com/embed/111111" frameborder="0" allowfullscreen="true"></iframe></div>\r\n';
			viewModel.editEntry.activeEntry().Info().Content(orig);
		},

		addStrava: function () {
			var orig = viewModel.editEntry.activeEntry().Info().Content();
			orig += '\r\n' + '<a href="https://www.strava.com/activities/11111" target="_blank">Route<img class="image-icon" src="/Scripts/components/navigation/images/strava.png" /></a>';
			viewModel.editEntry.activeEntry().Info().Content(orig);
		}
	};
