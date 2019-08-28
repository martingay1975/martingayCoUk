var settings = {
    "pagingType": "input",
    "bAutoWidth": false,
    "columnDefs": [
        {
            /*Year*/
            "bVisible": false,
            "sType": "numeric",
            "aTargets": [0],
        },
        {
            /*Month*/
            "bVisible": false,
            "sType": "numeric",
            "aTargets": [1],
        },
        {
            /*Day*/
            "bVisible": false,
            "sType": "numeric",
            "aTargets": [2],
        },
        {
            /*Id*/
            "bVisible": false,
            "aTargets": [3],
        },
        {
            /*Date Value*/
            "aTargets": [4],
            "aDataSort": [0, 1, 2],
            "width": "70px"
        }
    ],

    /* sort by the date value column, which in turn sorts via the 'aDataSort' property */
    "aaSorting": [[4, "desc"]],
    
    /* when a row is added, add an id which is the date value */
    "fnCreatedRow": function( nRow, aData, iDataIndex ) {
        $(nRow).attr('id', aData[3]);
    },

    "lengthMenu": [15, 10, 20, 250],
};

function DataTableHelpers() {
    
}

DataTableHelpers.prototype =
{
    _selector : '#tableEntries',

    _getDataTable : function() {
        return $(this._selector).dataTable();
    },

    _generateRow : function(entry) {
        return [entry.DateEntry().Year(),   // 0
                entry.DateEntry().Month(),  // 1
                entry.DateEntry().Day(),    // 2
                entry.DateEntry().Id(),     // 3 
                entry.DateEntry().Value(),  // 4
                entry.Title().Value(),      // 5
                entry.PeopleInitialsStringList()];            // 6
    },

    _getExistingRowElement: function(dateEntry) {
        var rowSelector = this._selector + " #" + dateEntry.Id;
        var trElement = $(rowSelector)[0];
        return trElement;
    },

    createEntry: function(entry, redraw) {
        this._getDataTable().fnAddData(this._generateRow(entry), redraw);
    },
    
    redraw : function() {
        this._getDataTable().fnDraw();
    },
    
    updateEntry: function(originalDate, entry) {
        var trElementToUpdate = this._getExistingRowElement(originalDate);
        var newRowData = this._generateRow(entry);

        //var ff = this._getDataTable().page();
        this._getDataTable().fnUpdate(newRowData, trElementToUpdate, undefined, false, undefined);

        // we need to do our own redraw which holds the paging position, rather than resets it back to page 1
        this._getDataTable().fnDraw("full-hold");
    },
    
    deleteAllEntries : function() {
        this._getDataTable().fnClearTable();
    },

    deleteEntry: function (dateEntry) {
        var trElementToDelete = this._getExistingRowElement(dateEntry);
        this._getDataTable().fnDeleteRow(trElementToDelete);
    },

    initialize: function () {

        viewModel.dataTable = $('#tableEntries').dataTable(settings);
        

        $('#tableEntries tbody').on("click", "tr", function (event) {

            // check if we have clicked the row that is already selected. i.e. user wish to de-select.
            if ($(this).hasClass('row_selected')) {
                $(this).removeClass('row_selected');
                viewModel.editEntry.loadActiveEntry(null);
                return;
            }

            // a new row is selected.
            $('#tableEntries tbody tr.row_selected').removeClass('row_selected');
            $(this).addClass('row_selected');

            // obtain the row data.
            var rowData = viewModel.dataTable.fnGetData(this);
            var year = rowData[0];
            var month = rowData[1];
            var day = rowData[2];
            var dateEntry = new DateEntry(year, month, day);

            // updates the active entry
            viewModel.editEntry.loadActiveEntry(dateEntry);
        });
    }
};