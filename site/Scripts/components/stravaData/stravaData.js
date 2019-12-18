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
        var getJSONAsync = function(activityId, jsonPath) {

            var deferred = new $.Deferred();

            var cacheValue = window.localStorage.getItem(activityId);
            if (cacheValue != null) {
                var ret = JSON.parse(cacheValue);
                deferred.resolve(ret);
            } 
            else {
                var promise = $.getJSON(jsonPath);
                promise.done(function(response) {
                    var stringJson = JSON.stringify(response);
                    window.localStorage.setItem(activityId, stringJson);
                    deferred.resolve(response);
                });

                promise.fail(function (msg) {
                    deferred.reject(msg);
                });
            }

            return deferred.promise();
        };

        this.getActivitiesAsync = function(drawActivity)
        {
            var activityIds = ["2534549498", "2540249862"]
            var promises = [];

            for (var index=0; index<activityIds.length; index++)
            {
                var activityId = activityIds[index];
                var promise = self.getActivityAsync(activityId);
                promises.push(promise);
            }

            var overallPromise = $.when.all(promises).then(function(activities) {
                for (var index=0; index < activities.length; index++)
                {
                    var activity = activities[index];
                    drawActivity(activity);
                }
            });

            return overallPromise;
        };
        
        this.getActivityAsync = function(activityId) {
            var jsonFilePath = "/res/strava/Run/" + activityId + ".json";
            var activityPromise = getJSONAsync(activityId, jsonFilePath);
            return activityPromise;
        };
    };

    var GMapLngLat = function (lng, lat) {
        this.lng = lng;
        this.lat = lat;
    };

    var GoogleMap = function() {

        var self = this;

        this.drawLines = function(activity) {
          var latLngArray = activity.Latlng.data;
          var gMapLngLatArray = [];
          for (var index = 0; index < latLngArray.length; index ++)
          {
              var latlng = latLngArray[index];
              var lat = latlng[0];
              var lng = latlng[1];
              gMapLngLatArray.push(new google.maps.LatLng(lat, lng));
          }

          self.drawPolyline(gMapLngLatArray, 3);
        };

        this.drawPolyline = function(lngLatArray, activityCount)
        {
            var hotOpacity = 0.2 * Math.pow(activityCount, -0.3);
            var hot = new google.maps.Polyline({
                clickable: true,
                map: self.map,
                strokeColor: "#FFFF00",
                strokeWeight: 5,
                strokeOpacity: hotOpacity,
                zIndex: 4,
                path: lngLatArray
            });

            var medOpacity = 0.54 * Math.pow(activityCount, -0.292);
            var medium = new google.maps.Polyline({
                clickable: true,
                map: self.map,
                strokeColor: "#FF0000",
                strokeWeight: 4,
                strokeOpacity: medOpacity,
                zIndex: 3,
                path: path
            });
            var cold = new google.maps.Polyline({
                clickable: true,
                map: self.map,
                strokeColor: "#0000FF",
                strokeWeight: 3,
                strokeOpacity: 0.6,
                zIndex: 2,
                path: path
            });
        }

        this.map = null;

        var promiseMapLoader = gmapLoader({key: "AIzaSyBYemck0mQmHpfMmqrxgERNPaYZ99RbiuQ"})
        promiseMapLoader.then(function(googleMaps) {
            var mapProp= {
                center:new google.maps.LatLng(51.3132412,-0.7379628),
                zoom:12,
            };
    
            var div = document.getElementById("googleMap");
            self.map = new googleMaps.Map(div, mapProp);

            var stavaActivitiesLoader = new StavaActivitiesLoader();
            var promise = stavaActivitiesLoader.getActivitiesAsync(self.drawLines);
            promise.done(function() {
                console.log("Got all activities");
            });
    
        });

    };


	// var StravaDataComponentViewModel = function (params) {

    //     if (!(this instanceof StravaDataComponentViewModel)) {
	// 		throw "Must invoke the function StravaDataComponentViewModel with the new operator";
    //     }

    //     GoogleMap();
        
    //     var init = function() {
    //         //var access_token = "ab5309fea6e5993a127421eba4218f5bf7140829";
    //         // http://www.strava.com/oauth/authorize?client_id=9912&response_type=code&redirect_uri=http://localhost:53001/strava-exchange.html&approval_prompt=force&scope=read,activity:read&state=some-State
            
    //     };

    //     var getAccessTokenViaCode = function() {

    //         //var url = "http://www.strava.com/oauth/authorize?client_id=9912&response_type=code&redirect_uri=http://localhost:53001&approval_prompt=force&scope=read,activity:read";
    //         var clientId = "9912";
    //         var clientIdUrl = "client_id=" + clientId;

    //         var clientSecret = "64dc88eaf43bfa3d3b0f4f624e5b7aeefd1059c6";
    //         var clientSecretUrl = "client_secret"; + clientSecret;
            
    //         var authorizationCode = "2a3a05fba9bbf47ad01a521677e8374c3a5554ea";
    //         var authorizationCodeUrl = "code=" + authorizationCode;

    //         var authorizationTypeUrl = "grant_type=authorization_code";
            
    //         var url = "https://www.strava.com/oauth/token?" + clientIdUrl + "&" + clientSecretUrl + "&" + authorizationCodeUrl + "&" + authorizationTypeUrl;
    //         var promise = $.post(url);


    //         "https://www.strava.com/oauth/token?client_id=9912&client_secret&code=2a3a05fba9bbf47ad01a521677e8374c3a5554ea&grant_type=authorization_code"

    //         promise.done(function() {
    //             alert("done");
    //         });

    //         promise.fail(function() {
    //             alert("fail");
    //         });

    //         // $.getJSON( url, function (data) {
    //         //     alert("hello " + data.length);
    //         // });
    //     };

    //     init();
	// };
    
	return { viewModel: GoogleMap, template: componentTemplate };
});
