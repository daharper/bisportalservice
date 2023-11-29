# bisportalservice
Executes a batch file which runs a java command. 

Monitors for changes, kills existing processes, restarts new REST Service on change.

see **Overview.txt** for more details

Written in a rush to help with an issue deploying our spring application into the DEV environment.

Once the CI/CD pipeline had generated the new build and swagger files for our service, the front end guys were keen to work with the latest APIs. But it was troublesome for them. This service basically allowed them to drop a zip file in a monitored folder and a minute later the latest API is up and running.

It uses WMI to detect running related java processes, and stops them, before starting the newly uploaded service.

There's a couple more tools, such as base64 encode/decode etc. that was useful for other things. The console program allows the development of the service and debugging - on the server, the service is registered as a Windows Service.

Correctly identifying change can be deceiving, more events than expected are rasied at times.

Project was hastily put together, but is useful for a reference.
