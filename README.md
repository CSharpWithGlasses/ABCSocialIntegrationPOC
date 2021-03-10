# ABCSocialIntegrationPOC
Replace the Oath Token Details in the appsettings.json of the WebApi Project "AccessToken": "", "ApiTokenUrl": "https://api.twitter.com/oauth2/token", "ConsumerKey": "", "ConsumerSecret": "", Run the app from the WebApi project
You can modify the throttle of max connections and tweets per connection in the app settings as well:
    "MaxConnectAttempts": "",  "MaxTweets": "",
This endpont starts the connection and processing, http://localhost:60017/api/SocialMedia/startprocessing

This one allows you to get stats while processing continues. http://localhost:60017/api/SocialMedia/stats
