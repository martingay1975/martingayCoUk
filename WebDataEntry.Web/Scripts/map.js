
define(['googlemaps'], function(googlemaps) {

    var mapDiv = document.getElementById('mapcontainer'),
        locations,

        googleMapOptions = {
            center: new googlemaps.LatLng(51.4, -0.65),
            zoom: 10,
            mapTypeId: googlemaps.MapTypeId.ROADMAP,
            navigationControl: true,
            navigationControlOptions: {
                style: googlemaps.NavigationControlStyle.SMALL
            }
        },

        googleMap = new googlemaps.Map(mapDiv, googleMapOptions),

        plotLocation = function(viewModelLocation) {
            var latlng = new googlemaps.LatLng(viewModelLocation.Lattitude(), viewModelLocation.Longitude()),
                marker = new googlemaps.Marker({
                    position: latlng,
                    map: googleMap,
                    title: viewModelLocation.Name(),
                    viewModel: viewModelLocation
                });

            googlemaps.event.addListener(marker, 'click', function() {
                var infoWindow = new googlemaps.InfoWindow({
                        content: viewModelLocation.Name(),
                        size: new googlemaps.Size(150, 50)
                    });

                infoWindow.open(googlemaps, marker);
            });
        },

        plotLoadedLocations = function() {
            for (var locationIndex = 0; locationIndex < locations.length; locationIndex ++) {
                plotLocation(locations[locationIndex]);
            }
        },

        getNextLocationId = function() {
            var maxId = 0;
            for (var locationIndex = 0; locationIndex < viewModel.locations.length; locationIndex ++) {
                var location = viewModel.locations[locationIndex];
                if (location.Id() > maxId) {
                    maxId = location.Id();
                }
            }
            return maxId;
        },

        onNewPlot = function(newLocation) {
            var handlerIndex;
            for (handlerIndex = 0; handlerIndex < handlers.length; handlerIndex++) {
                try {
                    handlers[handlerIndex](newLocation);
                } catch (e) {

                }
            }
        },

        handlers = [],

        map = function() {

        };


    map.prototype = {

        loadLocations: function () {
            viewModelHelpers.getLocations().done(function() {
                locations = viewModel.locations();
                plotLoadedLocations();
            });

            googlemaps.event.addListener(googleMap, 'rightclick', function(event) {
                var newLocation = new Location({
                    Id : -1,
                    Name: "TO BE FILLED",
                    Longitude: event.latLng.lng(),
                    Lattitude: event.latLng.lat()
                });

                // calls any listeners that a new location has been proposed to be added on the map surface.
                onNewPlot(newLocation);
            });
        },

        addLocation: function (newLocation) {

            // set a new Id
            newLocation.Id = getNextLocationId(),

            // update the viewModel
            viewModel.locations.push(newLocation);

            // update the map canvas
            plotLocation(newLocation);
        },

        saveLocations:function() {
            
        },

        addListener : function(handler) {
            handlers.push(handler);
        },
    };

    return map;
});
