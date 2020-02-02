/*global environment*/
var version = "1.0";
var require = {
	baseUrl: "Scripts",
	paths: {
		// third-party
		crossroads:					"ThirdParty/crossroads/dist/crossroads",
		hasher:						"ThirdParty/hasher/dist/js/hasher",
		jquery:						"//ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min",
		knockout:					"//cdnjs.cloudflare.com/ajax/libs/knockout/3.5.0/knockout",
		//knockout: 					"ThirdParty/knockout/knockout-3.5.0",	// debug
		"knockout-projections":		"ThirdParty/knockout-projections/dist/knockout-projections",
		"knockout-mapping":			"ThirdParty/knockout-mapping/knockout.mapping",
		signals:					"ThirdParty/js-signals/dist/signals",
		text:						"ThirdParty/requirejs-text/text",
		"jquery-dateFormat":		"ThirdParty/jquery-dateFormat/dateFormat",
		moment:						"ThirdParty/moment/moment.min",
		"load-google-maps-api":		"../node_modules/load-google-maps-api/index",
		
		// app
		router:						"app/router",											// Routing / url hash
		routerConfig:				"app/routerConfig",										// Config for the routing,
		siteOptions:				"app/siteOptions",										// Representation of siteOptions.js
		siteViewModel:				"app/siteViewModel",									// The root object, allow easy access of objects
		stringUtil:					"app/stringUtil",										// String utility class
		urlBuilder:					"app/urlBuilder",										// Url building utility class
		menuOption:					"app/menuOption",										// A menu option for the calendar / special 
		googleAnalytics:			"app/googleAnalytics",									// Google Analytics api initiator
		gmapLoader:					"app/gmapLoader",										// Google Map loader

		// app/bindingHandlers
		bindingHandlersFormatDate:	"app/bindingHandlers/formatDate",						// format date binding handler

		// app/repository
		fullYearDiaryRepository:	"app/repository/fullYearDiaryRepository.v1",				// Entry point to get diary entries from cache or server
		noInfoRepository:			"app/repository/noInfoRepository",						// Entry point to get noInfo entries from cache or server
		whoopsRepository:			"app/repository/whoopsRepository",						// Entry point to get whops entries from cache or server
		
		// app/diary
		diaryFilter:				"app/diary/diaryFilter",								// Diary filtering container
		entryUtil:					"app/diary/entryUtil",									// Diary entry utilities.
		entryExtension:				"app/diary/entryExtension",								// Diary extended with knockout
		diaryJsonReader:			"app/diary/diaryJsonReader",

		// polyfill
		polyfillArrayFind:          "polyfill/arrayFind",                                   // Polyfill for the array Find method

		// components
		provider:					"components/entries/diaryProvider/provider",			// Broker to get correct diaryProvider
		defaultProvider:			"components/entries/diaryProvider/defaultProvider.v1",		// Provides default data for the entries page
		whoopsProvider:				"components/entries/diaryProvider/whoopsProvider"		// Provides whoops data for the entries page

	}
};
