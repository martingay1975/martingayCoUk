
// =============================
function Entry(serverEntry) {

    if (!serverEntry) {
        return null;
    }

    var self = this;

    this.DateEntry = ko.observable(new DateEntry(serverEntry.DateEntry));
    this.Title = ko.observable(new Title(serverEntry.Title));
    this.First = ko.observable(new First(serverEntry.First));
    this.Info = ko.observable(new Info(serverEntry.Info));
	this.Version = serverEntry.Version;

    // Need to make a string list so that the 'checked' binding works in knockout.
    this.Locations = ko.observableArray(stringedAssembler.convertToStringArray(serverEntry.Locations));
    this.People = ko.observableArray(stringedAssembler.convertToStringArray(serverEntry.People));

    this.Images = ko.observableArray();
    if (serverEntry.Images != null) {
        /*jslint unparam: true*/
        $.each(serverEntry.Images, function (index, value) {
            this.Images.push(new DiaryImage(value.Source, value.Description));
        });
        /*jslint unparam: false*/
    }

    this.childValueChanged = ko.observable(0);
    this.isDirty = ko.observable(false);

    this.changedValue = function () {
        var newValue = self.childValueChanged() + 1;
        self.childValueChanged(newValue);
        self.isDirty(true);
    };

    this.PeopleInitialsStringList = function() {
        var list = self.People().map(function (personId) {
            var foundPerson = viewModel.people().find(function (person)
            {
                return personId === person.Id();
            });

            if (foundPerson) {
                return foundPerson.Name()[0];
            }

            return null;
        });

        return list.sort();
    }
   
    this.Title().Value.subscribe(self.changedValue);
    this.DateEntry().Day.subscribe(self.changedValue);
    this.DateEntry().Month.subscribe(self.changedValue);
    this.DateEntry().Year.subscribe(self.changedValue);
    this.Locations.subscribe(self.changedValue);
    this.People.subscribe(self.changedValue);
}

// =============================
function DiaryImage(source, description) {
    this.Source = ko.observable(source);
    this.Description = ko.observable(description);
}

function DateEntry(year, month, day) {
    
    if (arguments.length == 1) {
        month = arguments[0].Month;
        day = arguments[0].Day;
        year = arguments[0].Year;
    }

    this.Year = ko.observable();
    this.Month = ko.observable();
    this.Day = ko.observable();
    this.Value = ko.computed(this.getValue, this);
    this.Id = ko.computed(this.getId, this);
    
    this.update(parseInt(year), parseInt(month), parseInt(day));
}

DateEntry.prototype = {

    update : function(year, month, day) {
        this.Year(year);
        this.Month(month);
        this.Day(day);
    },
    
    getValue : function() {

        var ret = "";
        if (this.Day())
            ret += this.Day();

        if (this.Month())
            ret += "-" + this.Month();

        ret += "-" + this.Year();

        return ret;
    },

    getId: function () {

        var ret = "";
        if (this.Day())
            ret += this.Day();
        else
            ret += "0";

        ret += "z";
        if (this.Month())
            ret += this.Month();
        else 
            ret += "0";
        
        ret += "z" + this.Year();

        return ret;
    },

    
    getParam : function() {
        return String.format("year={0}&month={1}&day={2}", this.Year(), this.Month(), this.Day());
    },
};


// =============================
function First(serverFirst) {
    if (serverFirst == null) return;
    
    this.Name = ko.observable(serverFirst.Name);
    this.Value = ko.observable(serverFirst.Value);
}


// =============================
function Info(serverInfo) {
    if (serverInfo == null) return;
    this.Content = ko.observable(serverInfo.Content);
}


// =============================
function Location(serverLocation) {
    this.Id = ko.observable(serverLocation.Id.toString());  // needs to be a string, so knockout can match when using selectedOptions or checked.
    this.Name = ko.observable(serverLocation.Name);
    this.Lattitude = ko.observable(serverLocation.Lattitude);
    this.Longitude = ko.observable(serverLocation.Longitude);
}


// =============================
function Person(serverPerson) {
    this.Id = ko.observable(serverPerson.Id.toString());    // needs to be a string, so knockout can match when using selectedOptions or checked.
    this.Name = ko.observable(serverPerson.Name);
}

// =============================
function Title(serverTitle) {
    if (serverTitle == null) {
        this.Name = ko.observable("");
        this.Value = ko.observable("");
        return;
    }
    
    this.Name = ko.observable(serverTitle.Name);
    this.Value = ko.observable(serverTitle.Value);
}
