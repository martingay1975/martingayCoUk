/*global define*/
define(["jquery", "gmapLoader", "text!./stravaData.html"], function ($, gmapLoader, componentTemplate) {

	"use strict";

    var StavaActivitiesLoader;

    var extendJQuery = function() {
        if (typeof jQuery.when.all === 'undefined') {
            jQuery.when.all = function (deferreds) {
                return $.Deferred(function (def) {
                    $.when.apply(jQuery, deferreds).then(
                        function () {
                            def.resolveWith(this, [Array.prototype.slice.call(arguments)]);
                        },
                        function () {
                            def.rejectWith(this, [Array.prototype.slice.call(arguments)]);
                        });
                });
            }
        }
    };

    StavaActivitiesLoader = function() {
        
        extendJQuery();
        var self = this;

        // Helper to lazily get JSON entries,
        var getJSONAsync = function(jsonPath) {

            var deferred = new $.Deferred();

            var cacheValue = window.localStorage.getItem(jsonPath);
            if (cacheValue != null) {
                var ret = JSON.parse(cacheValue);
                deferred.resolve(ret);
            } 
            else {
                var promise = $.getJSON(jsonPath);
                promise.done(function(response) {
                    // line below Error: DOMException: Failed to execute 'setItem' on 'Storage': Setting the value of '/res/strava/Run/437607297.json' exceeded the quota.
                    //var stringJson = JSON.stringify(response);
                    //window.localStorage.setItem(jsonPath, stringJson);
                    deferred.resolve(response);
                });

                promise.fail(function (msg) {
                    deferred.reject(msg);
                });
            }

            return deferred.promise();
        };

        this.getFileSystemAsync = function(activityTypeFilter)
        {
            // make a new request each time.... don't rely on cache
            let jsonFilePath = "/res/strava/fileSystem.json?" + new Date().getUTCMilliseconds();
            let getFileSystemPromise = $.getJSON(jsonFilePath);
            let activityIdListToDraw = [];
            var promise = getFileSystemPromise.then(function(gpxFileSystemDictionary) {

                for (const key in gpxFileSystemDictionary) {

                    if (!key || key === activityTypeFilter)
                    {
                        let activitiesForType = gpxFileSystemDictionary[key];
                        if (activitiesForType) {
                            for (var acitivityIndex=0; acitivityIndex < activitiesForType.length; acitivityIndex ++)
                            {
                                var activityId = activitiesForType[acitivityIndex];
                                var jsonPath = "/res/strava/" + key + "/" + activityId + ".json";
                                activityIdListToDraw.push(jsonPath);
                            }
                        }
                    }
                }

                return activityIdListToDraw;
            });

            return promise;
        }

        this.getActivitiesAsync = function(drawActivityFn, activityPaths)
        {
            const promises = [];
            for (var index = 0; index < activityPaths.length; index++)
            {
                // gets the activity
                var promise = getJSONAsync(activityPaths[index]);
                promises.push(promise);
            }

            var overallPromise = $.when.all(promises).then(function(activities) {
                for (var index = 0; index < activities.length; index++)
                {
                    var activity = activities[index];
                    drawActivityFn(activity);
                }
            });

            return overallPromise;
        };
    };

    var GoogleMap = function() {

        var self = this;

        this.createGoogleLatLng = function(stravaLatLng) {
            var gMapLngLatArray = [];
            for (var index = 0; index < stravaLatLng.length; index ++)
            {
                var latlng = stravaLatLng[index];
                var lat = latlng[0];
                var lng = latlng[1];
                gMapLngLatArray.push(new google.maps.LatLng(lat, lng));
            }

            return gMapLngLatArray;
        };

        this.drawRoute = function(stravaActivity) {
            var googleLatLng = self.createGoogleLatLng(stravaActivity.Latlng.data);
            var activityCount = 300;
            self.drawPolyline(googleLatLng, activityCount);
        };

        this.drawPolyline = function(googleLatLng, activityCount)
        {
            var hotOpacity = 0.2 * Math.pow(activityCount, -0.3);
            var hot = new google.maps.Polyline({
                clickable: true,
                map: self.map,
                strokeColor: "#FFFF00",
                strokeWeight: 5,
                strokeOpacity: hotOpacity,
                zIndex: 4,
                path: googleLatLng
            });

            var medOpacity = 0.54 * Math.pow(activityCount, -0.292);
            var medium = new google.maps.Polyline({
                clickable: true,
                map: self.map,
                strokeColor: "#FF0000",
                strokeWeight: 4,
                strokeOpacity: medOpacity,
                zIndex: 3,
                path: googleLatLng
            });

            var cold = new google.maps.Polyline({
                clickable: true,
                map: self.map,
                strokeColor: "#0000FF",
                strokeWeight: 3,
                strokeOpacity: 0.6,
                zIndex: 2,
                path: googleLatLng
            });
        };

        this.initAsync = function() {
            self.map = null;
            const promiseMapLoader = gmapLoader({key: "AIzaSyBYemck0mQmHpfMmqrxgERNPaYZ99RbiuQ"})
            promiseMapLoader.then(function(googleMaps) {
                const mapProp= {
                    // Center the map around Frimley Green
                    center: new google.maps.LatLng(51.3132412,-0.7379628),
                    zoom: 12,
                };
        
                const googleMapElement = document.getElementById("googleMap");
                self.map = new googleMaps.Map(googleMapElement, mapProp);

                const stavaActivitiesLoader = new StavaActivitiesLoader();

                let fileSystemPromise = stavaActivitiesLoader.getFileSystemAsync("Run");
                fileSystemPromise.then(function(activityPaths) {
                    return stavaActivitiesLoader.getActivitiesAsync(self.drawRoute, activityPaths);
                });
            });

            return promiseMapLoader;
        }

        this.initAsync();
    };
    
	return { viewModel: GoogleMap, template: componentTemplate };
});
