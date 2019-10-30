define(['router', 'whoopsProvider', 'defaultProvider'], function (router, whoopsProvider, defaultProvider) {

	"use strict";

	return {

		// Description: Choose where we are getting our diary entries from. Currently there is the default diary (using noInfoRepository) or the Whoops repository.
		getFromRoute: function() {
			var providerName = router.currentRouteObservable().diaryProvider;

			switch (providerName) {
				case "whoops":
					return whoopsProvider;
				default:
					return defaultProvider;
			}
		}
	};

});