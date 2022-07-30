# bisportalservice
Simple Windows Service to execute a batch file which runs a java command. Monitors for changes, capable of restarting on change.

see **Overview.txt** for more details

I wrote this pretty much at night to help with an issue deploying our spring application primarilly into the DEV environment.

Once the CI/CD pipeline had generated the new build and swagger files for our service, the front end guys were keen to work with the latest APIs. But it was troublesome for them. This service basically allowed them to drop a zip file in a monitored folder and a minute later the latest API is up and running.

There's a couple more tools, such as base64 encode/decode etc. that were useful for other things.

Project was hastily put together, but is useful for a reference.
