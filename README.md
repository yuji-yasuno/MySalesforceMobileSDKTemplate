MySalesforceMobileSDKTemplate
=============================

- Installation -
1. Download "MySalesforceMobileSDKTemplate"
2. Double click the "MySalesforceMobileSDKTemplate.vsi" file and then automatically start the installation into the Visual Studio 2012. (You cannot use the previous versions of Visual Studio because it needs to use WinRT environment)

After installation, you can create new projects using "MySalesforceMobileSDKTemplate".
And you can set the salesforce.com application information by Resource.resw, please edit using the Visual Studio.

- Introduction -
Using this template, you only have to do care the logic after OAuth autholization.
And you can use the wrapper class of REST API of salesforce.com to access your organization data.
Please design you application from MainPage.xaml which is set to show after OAuth autholization.

Examples:

1.Create a Account record

            MySFRestAPI api = getApi();
            Dictionary<String, String> fields = new Dictionary<string, string>();
            fields.Add("Name", t"Please fill out new Account name here");
            MySFRestRequest request = api.requestForCreateWithObjectType("Account", fields);
            api.send(request);

2.Get a Account record

            MySFRestAPI api = getApi();
            MySFRestRequest request = api.requestForRetrieveWithObjectType("Account", txtObjectIdForGet.Text, new List<String> { "Name", "Id" });
            api.send(request);

3.Update a Account record

            MySFRestAPI api = getApi();
            Dictionary<String, String> fields = new Dictionary<String, String>();
            fields.Add("Name", "Test");
            fields.Add("NumberOfEmployees", "10000");
            MySFRestRequest request = api.requestForUpdateWithObjectType("Account", txtObjectIdForUpdate.Text, fields);
            api.send(request);
			
4.Delete a Account record

            MySFRestAPI api = getApi();
            MySFRestRequest request = api.requestForDeleteWithObjectType("Account", "fill out Account ID here");
            api.send(request);


5.Query

            MySFRestAPI api = getApi();
            MySFRestRequest request = api.requestForQuery("Select Id,Name From Account");
            api.send(request);
			
6.Search

            MySFRestAPI api = getApi();
            MySFRestRequest request = api.requestForSearch("FIND 'map*' IN ALL FIELDS RETURNING Account (Id, Name), Contact, Opportunity, Lead");
            api.send(request);


- Events -
You can add events for get the request response which including the JSON(JsonValue class for handling these data) data.

onCompletedLoadResponse event
-> Happen after successing access REST API

onFailedLoadResponse event
-> Happen after failing access REST API

onCanceldLoadResponse event
-> Happen when cancel the request.

onTimeoutLoadResponse event
-> Happen when request is timeout.

onFailedRetry;
-> MySFRestAPI automatically refresh token when token is expired and then retry the request. If the this sequence is failed, event will happen.