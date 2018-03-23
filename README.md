# Sharepoint REST Utilities for C#
The Sharepoint rest api is very powerful but writing code that does something with it can be tricky and costly sometimes. Everytime i wanted to use it i had to try to remeber how to do this and that, looking through code i've written and of course googling. So i thought it would be great to have a class library which would do all the dirty work for me and this is what this project is basically all about. But I'm warning you in advance: This hasn't been fully tested! Use it on your own responsibility!

## Getting Started
After referencing this solution in your project, which can be basically anything, all you have to do is to initialize the SpRestUtilities and tell it where to operate (sp-site-url) and who you are (username and pw).

```c#
using SP_REST_UTILITY;

SpRestUtilities myUT = new SpRestUtilities();
myUT.SiteUrl = "https://yoursharepoint.com/yoursite";
myUT.Credentials = new NetworkCredential("USERNAME", "PASSWORD");
```

## List Utilities
### Get SpList by Title or its Id
```c#
SpList listA = myUT.Get_SpList_By_Title("LISTNAME");
SpList listB = myUT.Get_SpList_By_ID("GUID"); // e.g.: "bacfa614-08de-428e-be54-24d673600901"
```
### Create a new List
```c#
SpList newList = new SpList();
newList.SetProperty("Title","LISTTITLE");
newList.SetProperty("Description","DESCRIPTION");
newList.SetProperty("AllowContentTypes", "true");
newList.SetProperty("BaseTemplate", "100");
newList.SetProperty("ContentTypesEnabled", "false");
myUt.Create_SP_List(newList);
```
### Update a List
The following Properties "should" be able to be changed:
 - Title
 - Description
 - ContentTypesEnabled
 - AllowContentTypes
 - EnableAttachments
 - EnableFolderCreation
 - EnableMinorVersions
 - EnableModeration
 - EnableVersioning
 - ForceCheckout
 - HasExternalDataSource
 - Hidden
 - IrmEnabled
 - IrmExpire
 - IrmReject
 - IsApplicationList
 - IsCatalog
 - IsPrivate
 - MultipleDataList
 - NoCrawl
 - ServerTemplateCanCreateFolders
 - BaseTemplate
```c#
SpList list = myUT.Get_SpList_By_Title("LISTNAME");
list.SetProperty("Title","NEWTITLE");
myUT.Update_SpList(list);
```
### Delete a List
```c#
SpList list = myUT.Get_SpList_By_Title("LISTNAME");
myUT.Delete_SpList(list);
```

## Item Utilities
### Get SpItem by ID
```c#
int yourItemId = 100;
SpList list = myUT.Get_SpList_By_Title("LISTNAME");
SpItem item = myUT.Get_SpItem_By_ID(yourItemId,list);
```
### Get SpItemCollection by optional filter
```c#
// Without filter example
SpList listA = myUT.Get_SpList_By_Title("LISTNAMEA");
SpItemCollection collection = myUT.Get_SpItem_Collection(listA);

// With filter example
SpList listB = myUT.Get_SpList_By_Title("LISTNAMEB");
SpItemCollection collection = myUT.Get_SpItem_Collection(listB,"$filter=Fieldname eq 'Whatever'");
```
### Update SpItem
```c#
int yourItemId = 100;
SpList list = myUT.Get_SpList_By_Title("LISTNAME");
SpItem item = myUT.Get_SpItem_By_ID(yourItemId,list);
item.SetFieldValue("Title","New Title");
myUT.Update_SpItem(item,list);
```
### Delete SpItem
```c#
int yourItemId = 100;
SpList list = myUT.Get_SpList_By_Title("LISTNAME");
SpItem item = myUT.Get_SpItem_By_ID(yourItemId,list);
myUT.Delete_SpItem(item,list);
```

## Library Utilities
documentation to come...

## Field Utilities
documentation to come...

## User Utilities
documentation to come...
