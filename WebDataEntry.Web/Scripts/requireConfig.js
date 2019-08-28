/*global requirejs*/

requirejs.config({
    baseUrl: "Scripts",
    paths: {
        //jquery:                 ["ThirdParty/jQuery-2.1.0.min"],
        //bootstrap:              ["Thirdparty/bootstrap.min"],
        //knockout:               ["ThirdParty/knockout-3.1.0.debug"],
        async:                  ["ThirdParty/requirejs-plugins-master/src/async"],
        googleMapApi:           ["async!http://maps.google.com/maps/api/js?v=3&sensor=false"],

        /* local */
        map :                   ["map"]
        
    },
    shim : {
        bootstrap : {
            deps: ["jquery"],
            exports: "$.fn.tooltip"
        }
    }
});

// convert Google Maps into an AMD module
define('googlemaps', ['async!http://maps.google.com/maps/api/js?v=3&sensor=false'],
function () {
    // return the gmaps namespace for brevity
    return window.google.maps;
});
