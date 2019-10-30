/*global define*/
define(['jquery', 'text!./stravaData.html'], function ($, componentTemplate) {

	"use strict";

	var StravaDataComponentViewModel = function (params) {

        if (!(this instanceof StravaDataComponentViewModel)) {
			throw "Must invoke the function StravaDataComponentViewModel with the new operator";
        }
        
        var init = function() {
            //var access_token = "ab5309fea6e5993a127421eba4218f5bf7140829";
            // http://www.strava.com/oauth/authorize?client_id=9912&response_type=code&redirect_uri=http://localhost:53001/strava-exchange.html&approval_prompt=force&scope=read,activity:read&state=some-State
            
        };

        var getAccessTokenViaCode = function() {

            //var url = "http://www.strava.com/oauth/authorize?client_id=9912&response_type=code&redirect_uri=http://localhost:53001&approval_prompt=force&scope=read,activity:read";
            var clientId = "9912";
            var clientIdUrl = "client_id=" + clientId;

            var clientSecret = "64dc88eaf43bfa3d3b0f4f624e5b7aeefd1059c6";
            var clientSecretUrl = "client_secret"; + clientSecret;
            
            var authorizationCode = "2a3a05fba9bbf47ad01a521677e8374c3a5554ea";
            var authorizationCodeUrl = "code=" + authorizationCode;

            var authorizationTypeUrl = "grant_type=authorization_code";
            
            var url = "https://www.strava.com/oauth/token?" + clientIdUrl + "&" + clientSecretUrl + "&" + authorizationCodeUrl + "&" + authorizationTypeUrl;
            var promise = $.post(url);


            "https://www.strava.com/oauth/token?client_id=9912&client_secret&code=2a3a05fba9bbf47ad01a521677e8374c3a5554ea&grant_type=authorization_code"

            promise.done(function() {
                alert("done");
            });

            promise.fail(function() {
                alert("fail");
            });

            // $.getJSON( url, function (data) {
            //     alert("hello " + data.length);
            // });
        };

        init();
	};
    
    

	return { viewModel: StravaDataComponentViewModel, template: componentTemplate };
});
