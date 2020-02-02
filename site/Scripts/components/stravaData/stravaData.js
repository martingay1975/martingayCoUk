/*global define*/
define(["jquery", "gmapLoader", "text!./stravaData.html"], function ($, gmapLoader, componentTemplate) {

	"use strict";

    const GoogleMapsApiKey = "AIzaSyBYemck0mQmHpfMmqrxgERNPaYZ99RbiuQ";
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
                    // find out how much local storage is being used.
                    var space = escape(encodeURIComponent(JSON.stringify(localStorage))).length;

                    // line below Error: DOMException: Failed to execute 'setItem' on 'Storage': Setting the value of '/res/strava/Run/437607297.json' exceeded the quota.
                    try
                    {
                         var stringJson = JSON.stringify(response);
                         window.localStorage.setItem(jsonPath, stringJson);
                    }
                    catch (e) 
                    {
                        console.log("Failed. " + e);
                    }
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

                const isAllActivityTypes = !activityTypeFilter;
                for (const key in gpxFileSystemDictionary) {

                    if (isAllActivityTypes || key === activityTypeFilter)
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

        this.getActivitiesAsync = function(drawActivityFn, activityJsonPaths, filterOptions)
        {
            const getActivityPromises = [];
            const numActivities = activityJsonPaths.length;

            console.log(`Getting {numActivities} activities`);
            for (var index = 0; index < numActivities; index++)
            {
                // gets the activity
                var getActivityPromise = getJSONAsync(activityJsonPaths[index]);
                getActivityPromises.push(getActivityPromise);
            }

            var overallPromise = $.when.all(getActivityPromises).then(function(activities) {
                
                console.log(`Got {numActivities} activities`);
                for (var index = 0; index < activities.length; index++)
                {
                    var heatMapJson = activities[index];

                    if (filterOptions)
                    {
                        if (filterOptions.year)
                        {
                            if (heatMapJson.StartDateTime.slice(0,4) !== filterOptions.year)
                            {
                                continue;
                            }
                        }
                    }

                    drawActivityFn(heatMapJson);
                    //window.setTimeout(function() {drawActivityFn(heatMapJson)}, 300);
                }
                console.log("Finished drawing");
            });

            return overallPromise;
        };
    };

    var GoogleMap = function() {

        var self = this;
        var allPolylines = [];
        this.currentActivityType = "Run";
        this.filterOptions = {
            year: null
        };

        this.runClick = function() {
            self.currentActivityType = "Run";
            self.runAsync();
        };

        this.bikeClick = function() {
            self.currentActivityType = "Ride";
            self.runAsync();
        };

        this.walkClick = function() {
            self.currentActivityType = "Walk";
            self.runAsync();
        };

        this.kayakClick = function() {
            self.currentActivityType = "Kayaking";
            self.runAsync();
        };

        this.allActivityClick = function() {
            self.currentActivityType = null;
            self.runAsync();
        };

        this.year2020Click = function() {
            self.filterOptions = {
                year: "2020"
            }
            self.runAsync();
        }

        this.yearAllClick = function() {
            self.filterOptions = {
                year: null
            }
            self.runAsync();
        }

        this.drawActivity = function(heatMapJson) {
            var activityCount = 1600;
            const encodedPolyline = heatMapJson.Polyline;
            try
            {
                if (encodedPolyline === null)
                {
                    return;
                }
                
                var polyline = google.maps.geometry.encoding.decodePath(encodedPolyline);
                self.drawPolyline(polyline, activityCount);
            }
            catch (e)
            {
                console.error(e);
            }
        };

        this.drawPolyline = function(polyline, activityCount)
        {
            var hotOpacity = 0.2 * Math.pow(activityCount, -0.3);
            var hot = new google.maps.Polyline({
                clickable: true,
                map: self.map,
                strokeColor: "#FFFF00",
                strokeWeight: 5,
                strokeOpacity: hotOpacity,
                zIndex: 4,
                path: polyline
            });
            allPolylines.push(hot);

            var medOpacity = 0.54 * Math.pow(activityCount, -0.292);
            var medium = new google.maps.Polyline({
                clickable: true,
                map: self.map,
                strokeColor: "#FF0000",
                strokeWeight: 4,
                strokeOpacity: medOpacity,
                zIndex: 3,
                path: polyline
            });
            allPolylines.push(medium);

            var cold = new google.maps.Polyline({
                clickable: true,
                map: self.map,
                strokeColor: "#0000FF",
                strokeWeight: 3,
                strokeOpacity: 0.6,
                zIndex: 2,
                path: polyline
            });
            allPolylines.push(cold);
        };

        this.removeAllPolylines = function() {
            for (var index=0; index < allPolylines.length; index ++) {
                var polyline = allPolylines[index];
                polyline.setMap(null);
            }
        };

        this.runAsync = function() {
            self.removeAllPolylines();

            const stavaActivitiesLoader = new StavaActivitiesLoader();
            const fileSystemPromise = stavaActivitiesLoader.getFileSystemAsync(this.currentActivityType);
            fileSystemPromise.then(function(activityJsonPaths) {
                return stavaActivitiesLoader.getActivitiesAsync(self.drawActivity, activityJsonPaths, self.filterOptions);
            });

            return fileSystemPromise;
        };

        this.initAsync = function() {
            self.map = null;
            const promiseMapLoader = gmapLoader({
                key: GoogleMapsApiKey,
                libraries: ["geometry"]
            });

            promiseMapLoader.then(function(googleMaps) {
                const mapProp= {
                    // Center the map around Frimley Green
                    center: new google.maps.LatLng(51.3132412,-0.7379628),
                    zoom: 12,
                };
        
                const googleMapElement = document.getElementById("googleMap");
                self.map = new googleMaps.Map(googleMapElement, mapProp);

                return self.runAsync();
            });

            return promiseMapLoader;
        }

        this.initAsync();
    };
    
	return { viewModel: GoogleMap, template: componentTemplate };
});
