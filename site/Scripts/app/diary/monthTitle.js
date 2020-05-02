define(['moment'], function (moment) {

	"use strict";

	var MonthTitle = function () {

		const self = this;

        // settings
        // settings.datev - optional - pass the date in datev format
        // settings.moment - optional - pass the date as a moment object
        // settings.monthAdjust - optional - adjust the month of the passed date
        this.getMonth = function(settings) {
            let theMomentMonth = moment;

            if (settings.datev)
            {
                const year = parseInt(settings.datev.substr(0, 4));
                const month = parseInt(settings.datev.substr(4, 2));
                
                theMomentMonth = moment().year(year).month(month - 1).date(1);
            } 
            else if (settings.moment) {
                theMomentMonth = settings.moment;
            }

            if (settings.monthAdjust) {
                theMomentMonth.add(settings.monthAdjust, "months");
            }

            const title = theMomentMonth.format("MMM YYYY");
            return {
                moment: theMomentMonth.clone(),
                title: title,
                hash: "#/entries/false/" + encodeURIComponent(title) + "/startdatev=" + theMomentMonth.format("YYYYMM") + "00" + "&enddatev=" + theMomentMonth.format("YYYYMM") + "32"
            };
        };

        this.getYear = function(year) {
            return {
                title: year,
                hash: "#/entries/false/" + encodeURIComponent(year) + "/startdatev=" + year + "0100" + "&enddatev=" + year + "1232"
            };
        };
    };

    return MonthTitle;
});