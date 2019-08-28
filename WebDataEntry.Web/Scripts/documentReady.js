

$(document).ready(function () {

    viewModelHelpers.initialize();
    viewModelHelpers.crudCallbackContext = viewModel.dataTableHelpers;

    var peoplePromise = viewModelHelpers.getPeople();
    var locationsPromise = peoplePromise.done(function() { viewModelHelpers.getLocations(); });

    $.when(peoplePromise, locationsPromise).done(function () {
        ko.applyBindings(viewModel);
        viewModel.dataTableHelpers.initialize();
    });

    $('#buttonSaveXml').click(function () { viewModelHelpers.save(); });

    $('#downloadDatabase').click(function() {
        var downloaded = viewModelHelpers.downloadDatabase();
        downloaded.done(function() {
            var dt = new Date();
            var date2014 = new DateEntry(dt.getFullYear(), 0, 0);
            viewModelHelpers.getEntries(date2014);
        });
    });

    $('#uploadDatabase').click(function () {
    	viewModelHelpers.uploadDatabase();
    });

    $('#buttonLoadThisYear').click(function () {
        var dt = new Date();
        var dateThisYear = new DateEntry(dt.getFullYear(), 0, 0);
        viewModelHelpers.getEntries(dateThisYear);
    });

    $('#buttonLoadAll').click(function () {
        viewModelHelpers.getEntries();
    });
});
