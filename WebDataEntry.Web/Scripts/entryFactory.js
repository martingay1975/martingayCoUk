

var EntryFactory = {
    create: function () {
        var newEntry = new Object();
        newEntry.DateEntry = new Object();

        var now = new Date();
	    newEntry.version = 1;
        newEntry.DateEntry.Year = now.getFullYear();
        newEntry.DateEntry.Month = now.getMonth() + 1;
        newEntry.DateEntry.Day = now.getDate();
        newEntry.Title = new Object();
        newEntry.Title.Name = null;
        newEntry.Title.Value = "";
        newEntry.Info = new Object();
        newEntry.Info.Content = "<p></p>";
        newEntry.People = [500, 501, 502, 503, 504];
        return newEntry;
    }
};

var stringedAssembler = {
    convertToStringArray: function (array) {
        var stringArray = new Array();
        for (var arrayElement in array) {
            stringArray.push(array[arrayElement].toString());
        }

        return stringArray;
    }
};