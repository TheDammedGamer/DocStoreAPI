# DocStoreAPI
A simple asp.net core document managment api, backed by SQL Server, AzureAd, ActiveDirectory, Azure storage and SMB shares.

At it's core DocStorAPI is a simple document management API designed to be easily intergratable with other projects that require an auditable service.

# In-Practice
The application stores docuemnts within `stors` such as a file share or Azure Blob Storage, and then links theese documents to 


## To-Do (MVP)
- [x] Implement Move on `DocumentController`
- [x] Implement a Security Controller(s) to Manage Groups, ACEs and Buisness Areas.
- [x] Add ACE state of 'Supervisor' to manage permissions
- [x] Inject Admin Groups via Config
- [x] Implement Admin or 'Supervisor' Check on Security Controller(s)
- [ ] Implement Metadata filtering on `DocumentMetadataController.Put` & `DocumentMetadataController.Post` to ensure generated values aren't being overwritten and document names are updated when nessacary. (Partially Completed)
	- [x] Add User Edit Metod to limit the properties that can be edited by Put
	- [ ] Verify Post Data and strip Generated Data
- [x] Update User Is authorsed by Buisness Area to support Admins
- [ ] Implement a Search by Custom Metadata Keys in `DocumentMetadataController` using `SearchByCustomMetadataKey`
	- [ ] Use a class passed as the body to store the search Data with Queries for the Pagination etc 
- [ ] Add a Client library.
- [ ] Add an example WPF app.

## Version 2 improvements
- Implement Background Worker and Queue to manage archival, deletion and movement of documents instead of at Runtime.
- Implement simple polices based upon BuisnessArea to define automatic movement policies based on Stors and BuisnessAreas.
- Add Support for MySql as an alternative database.
- Add Support for Comsmos DB as an alternative database (requires .net core 3).
- Look into Support for AWS storage.
- Look into Supporting other Identity providers.

## Possible Version 3 Improvements
- Support for Metadata / Security caching possible using redis or something similar.

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