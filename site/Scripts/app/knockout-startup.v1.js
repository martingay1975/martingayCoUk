/*global define, require, window, document, navigator, environment*/
/* jslint browser: true */
define(['knockout', 'siteViewModel'], function (ko, siteViewModel) {

	"use strict";

	// this is invoked from index.html and is run immediately. See the data-main attribute where the require.js gets included.
	// Registering the knockout components: These are keyed by their name, and "require" a javaScript module to initialize the component. 
	// They key, e.g. network can be used statically in the html, or can be part of the parameter of a component knockout binding.

	// ==========================
	// controls

	// controls - shared (currently shared, consumed by more than one component
	ko.components.register('site-header', { require: 'components/site-header/site-header' });
	ko.components.register('titles', { require: 'components/titles/titles' });
	ko.components.register('search', { require: 'components/search/search' });
	ko.components.register('entries', { require: 'components/entries/entries.v1' });
	ko.components.register('whoopsprogressbar', { require: 'components/whoopsProgressBar/whoopsProgressBar' });
	ko.components.register('navigation', { require: 'components/navigation/navigation' });
	ko.components.register('calendar', { require: 'components/calendar/calendar' });
	ko.components.register('calendar-tile', { require: 'components/calendar-tile/calendar-tile' });
	ko.components.register('calendar-year', { require: 'components/calendar-year/calendar-year' });
	ko.components.register('special-days', { require: 'components/special-days/special-days' });
	ko.components.register('strava', { require: 'components/strava/strava' });
	ko.components.register('spotify', { require: 'components/spotify/spotify' });
	ko.components.register('lm2016', { require: 'components/lm2016/lm2016' });
	ko.components.register('diabetes', { require: 'components/diabetes/diabetes' });
	ko.components.register('stravaData', { require: 'components/stravaData/stravaData' });

	// ==========================
	try {
		// Start the application
		ko.applyBindings(siteViewModel);
	} catch (e) {
		window.alert("There was an error initializing. " + e);
	}
});
