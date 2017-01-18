# Swift Dot Net
Sample full application (client and back-end in separate projects) that demonstrates using Swift 3 for iOS with ASP.NET 4.6.x Web API and MVC on Azure with DocumentDB and Azure Search.

####Recent Updates (1/17/17):
- Added .NET Core Project (No Identity or SignalR, yet.)
- Added .NET Core Extensions Class Library
- Added .NET Core DocumentDB Class Library

Real-time support is handled via websockets and SignalR for the purposes of demonstration. If you choose to use a backplane, you should be aware of latency or cost issues that might be incurred.

If you looking for a fully-supported development framework for quickly launching mobile applications, please e-mail: sales@dryverless.com. Ask for a license to use the Ahtau Framework.

####Azure Service Requirements for Deployment

You must configure the project with your own Azure Search Keys, Azure DocumentDB Database Connection Information, and Bing Maps API Keys.

######If you are new to Azure, you can get a free trial at the following link: 
https://azure.microsoft.com/en-us/pricing/free-trial/

######Bing Maps API (10k transactions/mo for free)
http://www.microsoft.com/maps/Licensing/licensing.aspx#mainTab2

######Azure Search Service are free excluding outbound data transfers (3 data connections / indexes / indexers, 50 MB Total Storage, 10,000 Documents).
https://azure.microsoft.com/en-us/pricing/details/search/

######Azure DocumentDB requires at least an S1 plan ($25/mo = 250 RU/sec, 10 GB Storage) for each collection used (even empty ones).
https://azure.microsoft.com/en-us/pricing/details/documentdb/

######Data Transfers Pricing Details
https://azure.microsoft.com/en-us/pricing/details/data-transfers/

#####How User Data Appears on DocDB Server:
```json
{
  "id": "463b5add-3abb-482c-8f72-9f199203e22b",
  "UserName": "demo@github.com",
  "Email": "demo@github.com",
  "EmailConfirmed": true,
  "PasswordHash": "AMZO39oQGu9eUtMcy8gho6oPxETXQ8OPmeju7JEVMeHW7LgQi/hcnEATX7294xfBKg==",
  "SecurityStamp": "31a3e067-7508-41c4-acd5-d4311da67b2c",
  "PhoneNumber": null,
  "PhoneNumberConfirmed": false,
  "TwoFactorEnabled": false,
  "LockoutEnd": "0001-01-01T00:00:00+00:00",
  "LockoutEnabled": false,
  "AccessFailedCount": 0,
  "Logins": [],
  "Claims": [],
  "Roles": []
}
```

##Related GitHub Projects / Credits

###Example Swift Apps by Mark Hamilton, Dryverless
Collection of example applications written in Swift / Objective-C for iOS 9.x (developed under 9.2.1 SDK - will be migrated to 9.3 when released)
######https://github.com/TheDarkCode/Example-Swift-Apps

###AngularAzureSearch by Mark Hamilton, Dryverless
######https://github.com/TheDarkCode/AngularAzureSearch/

###Azure Search Demos by Liam Cavanagh, Microsoft
######https://github.com/liamca/AzureSearchDemos
######http://azure.microsoft.com/en-us/documentation/services/search/

###DocumentDb with Web API by Richard J. Leopold
######https://github.com/rleopold/DocDbWebApi/
######(Article found here: http://4rjl.net/post/azure-documentdb-with-web-api)

###DocumentDB.AspNet.Identity by Adrian Fernandez, Microsoft
######https://github.com/tracker086/DocumentDB.AspNet.Identity

###GolfTracker.DocumentDB by King Wilder, Gizmo Beach
######https://github.com/kahanu/GolfTracker.DocumentDB/
######(Video / Article Series found here: http://www.nosqlcentral.net/Story/Details/videos/kahanu/1-documentdb-golf-tracker-overview)

###azure-documentdb-dotnet by Ryan CrawCour & Aravind Ramachandran, Microsoft
######https://github.com/Azure/azure-documentdb-dotnet/
######(Article found here: https://azure.microsoft.com/en-us/documentation/articles/documentdb-sharding/)

###MailService by James Bisiar
######https://github.com/bisiar

###MVA-SignalR by Jon Galloway & Brady Gaster, Microsoft
######https://github.com/jongalloway/MVA-SignalR

##Helpful Links

######http://blogs.msdn.com/b/documentdb/archive/2014/12/03/scaling-a-multi-tenant-application-with-azure-documentdb.aspx
######DocumentDB Documentation: https://msdn.microsoft.com/en-us/library/azure/dn781482.aspx
######Azure Search Documentation: https://msdn.microsoft.com/en-us/library/azure/dn798933.aspx
######W3C Recommendation on Cross-Origin-Resource-Sharing: http://www.w3.org/TR/cors/

##Related Microsoft Virtual Academy Courses

####Adding Microsoft Azure Search to Your Websites and Apps:
######https://mva.microsoft.com/en-us/training-courses/adding-microsoft-azure-search-to-your-websites-and-apps-10540

####Developing Solutions with Azure DocumentDB:
######https://mva.microsoft.com/en-us/training-courses/developing-solutions-with-azure-documentdb-10554

####Lighting Up Real-Time Web Communications with SignalR:
######https://mva.microsoft.com/en-us/training-courses/lighting-up-real-time-web-communications-with-signalr-8358

##Support:

#####Send any questions or requests to: support@dryverless.com

## Contributing

  - 1) Fork this repository!
  - 2) Create your feature branch: ```git checkout -b Your-New-Feature```
  - 3) Commit your changes: ```git commit -am 'Adding some super awesome update'```
  - 4) Push to the branch: ```git push origin Your-New-Feature```
  - 5) Submit a pull request!

## License
Copyright (c) 2016-2017 Mark Hamilton / dryverless (http://www.dryverless.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
