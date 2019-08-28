/// <reference path="ThirdParty/knockout-2.1.0.js" />
/// <reference path="entry.js" />
/// <reference path="viewmodelHelpers.js" />
/// <reference path="dataTableHelpers.js" />

function ViewModel() {
    var self = this;

    this.entries = ko.observableArray();
    this.people= ko.observableArray();
    this.locations = ko.observableArray();
    this.editEntry = new EditEntry();
    this.dataTableHelpers = new DataTableHelpers();
    this.baseUrl = null;
    this.names = ['martin', 'laura', 'katie', 'ben', 'jessica'];
    this.familyPeopleComputed = ko.computed(function() {
        var ret = self.people().filter(function (person) { 
            return person.Id() >= 500 && person.Id() < 506; 
        });
        return ko.observableArray(ret);
    }, this);

    this.anyEntries= ko.computed(function() {
        return this.entries().length > 0;
    }, this);

    this.dispose = function() {
        if (this.familyPeopleComputed) {
            this.familyPeopleComputed.dispose();
        }
    }
}

var viewModel = new ViewModel();
