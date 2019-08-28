/// <reference path="entry.js" />
/// <reference path="viewmodel.js" />

var viewModelHelpers =
{
    _baseApiUrl: "",
    _baseApiUrlRpc: "",
    crudCallbackContext: null,

    initialize : function() {
        this._baseApiUrl = viewModel.baseUrl + "api/diary";
        this._baseApiUrlRpc = viewModel.baseUrl + "api/rpc";
    },

    getEntries: function (dateIndex) {
        if (!dateIndex)
            dateIndex = new DateEntry(0, 0, 0);
        
        var url = this._baseApiUrl + "?" + dateIndex.getParam();
        var self = this;
        var ret = $.ajax({
            type: "GET",
            url: url,
            success: function (entries) {
                viewModel.entries.removeAll();
                viewModel.dataTableHelpers.deleteAllEntries();
                var temp = [];
                for (var entryIndex in entries) {
                    if (entryIndex % 20 == 0) {
                        console.log((entryIndex / entries.length) * 100 + "% + count:" + entryIndex);
                    }

                    var entry = new Entry(entries[entryIndex]);
                    {
                        temp.push(entry);
                        self.onCreateEntry(entry, false);
                    }
                }

                viewModel.entries(temp);
                
                // redraw the datatable after a bulk addition. This is much quicker than getting the datatable's fnAddData function to do it after every addition.
                viewModel.dataTableHelpers.redraw();
            }
        });

        return ret;
    },
    
    getPeople : function() {
        var url = this._baseApiUrlRpc + "/GetPeople";
        return this.makeAjaxCall(url).done(function(people) {

                            var temp = [];
                            for (var peopleIndex=0; peopleIndex < people.length; peopleIndex++) {
                                var person = new Person(people[peopleIndex]);
                                temp.push(person);
                            }

                            viewModel.people(temp);
                        });
    },

    downloadDatabase : function() {
        var url = this._baseApiUrlRpc + "/DownloadDatabase";
        return this.makeAjaxCall(url).done(function() {
            alert("downloaded");
        });
    },

    uploadDatabase: function () {
        var url = this._baseApiUrlRpc + "/UploadDatabase";
        var promise = this.makeAjaxCall(url);
        promise.done(function () {
            alert("uploaded");
        });

	    return promise;
    },

    getLocations : function() {
        var url = this._baseApiUrlRpc + "/GetLocations";
        return this.makeAjaxCall(url).done(function(locations) {
            for (var locationIndex in locations) {
                var location = new Location(locations[locationIndex]);
                viewModel.locations.push(location);
            }
        });
    },
    
    makeAjaxCall: function (url) {
        var promise = $.ajax({
            url: url,
            type: "GET"
        });

        promise.fail(function(err) {
             alert("Failed to get " + url + " successfully. " + err.responseJSON.ExceptionMessage);
        });
        return promise;
    },

    save: function () {
        var url = this._baseApiUrlRpc + "/Save";
        var savePromise = this.makeAjaxCall(url);
        var self = this;
        savePromise.done(function() {
            self.saveFormats();
        });

        return savePromise;
    },

    testValidDiary : function() {
        var url = this._baseApiUrlRpc + "/TestValidDiary";
        this.makeAjaxCall(url);
    },

    findCachedEntry: function (dateIndex) {

        console.log("find entry: " + dateIndex.Value());

        var ret = ko.utils.arrayFirst(viewModel.entries(), function (entry) {
            var ret1 = (entry.DateEntry().Year() == dateIndex.Year() &&
                entry.DateEntry().Month() == dateIndex.Month() &&
                (isNaN(entry.DateEntry().Day()) && isNaN(dateIndex.Day()) || entry.DateEntry().Day() == dateIndex.Day()));

            return ret1;
        });

        return ret;
    },

    deleteCachedEntry: function (dateIndex) {
        
        dateIndex = ko.toJS(dateIndex);
        viewModel.entries.remove(function (entry) {
            if (dateIndex.Year > 0 && entry.DateEntry().Year() != dateIndex.Year)
                return false;

            if (dateIndex.Month > 0 && entry.DateEntry().Month() != dateIndex.Month)
                return false;

            if (dateIndex.Day > 0 && entry.DateEntry().Day() != dateIndex.Day)
                return false;

            return true;
        });
    },
    
    deleteEntry: function(dateIndex) {
        var url = this._baseApiUrl + "?" + $.param(dateIndex);
        var self = this;
        
        var deleteResponse = $.ajax({
            type: "DELETE",
            url: url,
        });

        deleteResponse.done(function() {

            self.deleteCachedEntry(dateIndex);

            // inform any listeners
            self.crudCallback.updateEntry();

            if (this.crudDeleteCallback) {
                this.crudDeleteCallback(dateIndex);
            }

            return deleteResponse;
        });
    },

    saveFormats: function() {
    	var saveRequest = { OldEntriesJson: true, LatestEntriesJson: true, WhoopsJson: true, AllJson: true };
        return this._sendRpcPost(saveRequest).done(function() {
            console.log("saved");
        });
    },

    createEntry: function (entry) {

        var createEntryRequest = viewModelHelpers.prepareEntryForSending(entry);
        var self = this;
        var createResponse = this._sendCommand(createEntryRequest, "POST");
        createResponse.done(function (entryResponse) {
            var koEntry = new Entry(entryResponse);
            viewModel.entries.push(koEntry);
            self.onCreateEntry(entry);
        });

        return createResponse;
    },
    
    onCreateEntry: function(entry, redraw)
    {
        if (this.crudCallbackContext && this.crudCallbackContext.createEntry) {
            this.crudCallbackContext.createEntry(entry, redraw);
        }
    },

    updateEntry: function (originalDate, entry) {

        var updatePromise = new $.Deferred();

        var updateEntryRequest = new UpdateEntryRequest(originalDate, entry);
        var updateResponse = this._sendCommand(updateEntryRequest, "PUT");
        var self = this;
        
        updateResponse.done(function () {

            self.deleteCachedEntry(originalDate);
            self.deleteCachedEntry(entry.DateEntry);
            viewModel.entries.push(entry);
            
            // inform any listeners
            if (self.crudCallbackContext && self.crudCallbackContext.updateEntry) {
                self.crudCallbackContext.updateEntry(originalDate, entry);
            }

            updatePromise.resolve();
        });

        updateResponse.fail(function(err) { updatePromise.rejectWith(err); });

        return updatePromise;
    },

    prepareEntryForSending: function (entry) {
        if (entry == null)
            return null;
            
        var koStrippedEntry = ko.toJS(entry);
        delete koStrippedEntry.DateEntry.Value;
        return koStrippedEntry;
    },
    
    _sendCommand: function (jsDataObject, verb) {

        var jsonData = JSON.stringify(jsDataObject);
        return $.ajax({
            type: verb,
            url: this._baseApiUrl,
            data: jsonData,
            contentType: 'application/json;charset=utf-8',
        });  
    },
    
    _sendRpcPost : function(objectToSend) {

        var jsonData = JSON.stringify(objectToSend);
        return $.ajax({
            type: "POST",
            url: this._baseApiUrlRpc + "/SaveFormats",
            data: jsonData,
            contentType: 'application/json;charset=utf-8',
        });
    },
};

function UpdateEntryRequest(originalDate, entry) {
    this.OriginalDate = originalDate;
    this.Entry = viewModelHelpers.prepareEntryForSending(entry);
}