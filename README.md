# DocStoreAPI
A simple asp.net core document managment api, backed by SQL Server, Azure storage and SMB shares, and secured by Azure Active Directory or Active Directory .

At it's core DocStoreAPI is a simple API designed to be easily intergratable with other projects that require an auditable document managment system.

# In-Practice
The application stores docuemnts within `stors` such as a file share or Azure Blob Storage, and then stores the document metadat within SQL Server. Buisness Metadta is stored as simple Key Value Pairs where as the document properties are stored in a flat structure.


## To-Do (MVP)
- [x] Implement Move on `DocumentController`.
- [x] Implement a Security Controller(s) to Manage `Groups`, `ACE`s and `BuisnessArea`s.
- [x] Add ACE state of `Supervisor` to manage permissions by Buisness Area.
- [x] Inject Admin Groups via Config.
- [x] Implement Admin or 'Supervisor' Check on Security Controller(s).
- [x] Implement Metadata filtering on `DocumentMetadataController.Put` & `DocumentMetadataController.Post` to ensure generated values aren't being overwritten and document names are updated when necessary.
	- [x] Add User Edit Metod to limit the properties that can be edited by Put.
	- [x] Verify Post Data and strip Generated Data.
- [x] Redifne the Gates on the group controller and remove the exception when the object is null
- [x] Update User Is authorised by Buisness Area to support Admins.
- ~[ ] Implement a Search by Custom Metadata Keys in `DocumentMetadataController` using `SearchByCustomMetadataKey`.~
- [x] Implement a search 
	- [x] Use a class passed as the body to store the search Data with Queries for the Pagination etc.
- [x] update `BuisnessArea` and `Group` conrtollers to search by string not id.
- [ ] Go through and ensure that Auditing is being stored in the database
- [ ] Add Unit Tests
- [ ] Add a Client library.
- [ ] Add an example WPF app.

## Version 2 improvements
- Implement Background Worker and Queue to manage archival, deletion and movement of documents instead of at Runtime.
- Implement simple polices based upon BuisnessArea to define automatic movement policies based on Stors and BuisnessAreas.
	- Use [Rules Engine](https://github.com/microsoft/RulesEngine) to implement polices
- Implement a Admin management interface.
- Add Support for MySql as an alternative database.
- Add Support for Comsmos DB as an alternative database (requires .net core 3).
- Look into Support for AWS storage.

## Possible Version 3 Improvements
- Support for Metadata / Security caching possible using redis or something similar.
- Look into Supporting other Identity providers.


# Example Configs

## appsettings.json

- `ConnectionStrings.DefaultConnection` is for the SQL Server used for metadata storage.
- `StorConfigFilePath` is a Path so you can host the Stor Config on a shared drive

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "instance.onmicrosoft.com",
    "TenantId": "22222222-2222-2222-2222-222222222222",
    "ClientId": "11111111-1111-1111-11111111111111111"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=Server\\Instance;Database=DocStore;User Id=SQLUSer;Password=Password;"
  },
  "StorConfigFilePath": "D:\\DocStore\\Config\\StorConfig.json",
  "AdminGroups": {
    "ADAdminGroupNames": [ "Domain Admins", "DocStore Admins" ],
    "AZAdminGroupNames": [ "DocStore Admins" ]
  }
}
```

## StorConfig.json

- Only use SMB share auth if needed otherwise just grant access via the run as account.
- Don't add a trailing slash to the end of the file paths in `BasePath`

``` json

{
  "Stors": [
    {
      "RequiresAuth": false,
      "UserName": null,
      "Password": null,
      "Domain": null,
      "BasePath": "\\\\Sevrer\\Share\\DocStore01",
      "StorType": "FileShareStor",
      "ShortName": "DocStore01"
    },
    {
      "RequiresAuth": false,
      "UserName": null,
      "Password": null,
      "Domain": null,
      "BasePath": "\\\\Sevrer\\Share\\DocStore02",
      "StorType": "FileShareStor",
      "ShortName": "DocStore02"
    },
    {
      "ContainerName": "DocStoreContainer",
      "AccessKey": "YourKeyHere",
      "AccountName": "yourAccountNameHere",
      "StorType": "AzureBlobStor",
      "ShortName":  "AZDefaultStor"
    }
  ]
}


```


# MIT License

Copyright (c) 2019 Liam Townsend

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