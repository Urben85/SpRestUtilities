# Sharepoint REST Utilities for C#
The Sharepoint rest api is very powerful but writing code that does something with it can be tricky and costly sometimes. Everytime i wanted to use it i had to try to remeber how to do this and that, looking through code i've written and of course googling. So i thought it would be great to have a class library which would do all the dirty work for me and this is what this project is basically all about. But I'm warning you in advance: This hasn't been fully tested! Use it on your own responsibility!

## Getting Started
After referencing this solution in your project, which can be basically anything, all you have to do is to initialize the SpRestUtilities and tell it where to operate (sp-site-url) and who you are (username and pw).

```c#
using SpRestUtility;

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
### Changeable Listproperties
The following Properties "should" be able to be set or changed:
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
### Create a new List
```c#
SpList newList = new SpList();
newList.SpListTemplateType = SpListTemplateType.GenericList;
newList.SetProperty("Title","LISTTITLE");
newList.SetProperty("Description","DESCRIPTION");
newList.SetProperty("AllowContentTypes", "true");
newList.SetProperty("ContentTypesEnabled", "false");
myUt.Create_SP_List(newList);
```
### Update a List
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

## Library Utilities
documentation follows..

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
### Access Field Values
```c#
int yourItemId = 100;
SpList list = myUT.Get_SpList_By_Title("LISTNAME");
SpItem item = myUT.Get_SpItem_By_ID(yourItemId,list);
string title = item.Data["Title"];
```
### Update SpItem
```c#
int yourItemId = 100;
SpList list = myUT.Get_SpList_By_Title("LISTNAME");
SpItem item = myUT.Get_SpItem_By_ID(yourItemId,list);
item.SetFieldValue("Title","New Title");
myUT.Update_SpItem(item,list);
```
### Setting Lookups, User and URL Fields
```c#
int yourItemId = 100;
SpList list = myUT.Get_SpList_By_Title("LISTNAME");
SpItem item = myUT.Get_SpItem_By_ID(yourItemId,list);

// Set LookupSingle and UserSingle
item.Data["SingleId"] = "100"; // "Id" must be applied at the end of the Fieldname!
item.Data["SingleId"] = "-1"; // Resets the Field to empty

// Set LookupMulti and UserMulti
item.Data["MultiId"] = "{'results':[100,101]}"; // "Id" must be applied at the end of the Fieldname!
item.Data["SingleId"] = "{'results':[]}"; // Resets the Field to empty

// Set URL Field
item.Data["UrlField"] = "{'Url':'https://github.com','Description':'GitHub'}";
item.Data["UrlField"] = "{'Url':'','Description':''}"; // Resets the Field to empty

myUT.Update_SpItem(item,list);
```
### Delete SpItem
```c#
int yourItemId = 100;
SpList list = myUT.Get_SpList_By_Title("LISTNAME");
SpItem item = myUT.Get_SpItem_By_ID(yourItemId,list);
myUT.Delete_SpItem(item,list);
```
### Get an Attachment from a SpItem by Filename
```c#
int yourItemId = 100;
SpList list = myUT.Get_SpList_By_Title("LISTNAME");
SpItem item = myUT.Get_SpItem_By_ID(yourItemId,list);
SpFile attachment = myUT.Get_Attachment_From_SpItem("FILENAME.txt",item,list);
```
### Get all Attachments from a SpItem
```c#
int yourItemId = 100;
SpList list = myUT.Get_SpList_By_Title("LISTNAME");
SpItem item = myUT.Get_SpItem_By_ID(yourItemId,list);
SpFileCollection attachments = myUT.Get_All_Attachments_From_SpItem(item,list);
```
### Uplad an Attachment to a SpItem
```c#
int yourItemId = 100;
string path = @"C:\yourpath\FILENAME.txt";
SpList list = myUT.Get_SpList_By_Title("LISTNAME");
SpItem item = myUT.Get_SpItem_By_ID(yourItemId,list);
SpFile attachment = myUT.Upload_SpItem_Attachment(path,item,list);
```

## Field Utilities
### Get a SpField by InternalName or Title
```c#
SpList list = myUT.Get_SpList_By_Title("LISTNAME");
SpField field = myUT.Get_SpField_By_InternalName_Or_Title("InterNameOrTitle",list);
```
### Get a SpField by ID
```c#
SpList list = myUT.Get_SpList_By_Title("LISTNAME");
SpField field = myUT.Get_SpField_By_ID("GUID",list); // e.g.: "bacfa614-08de-428e-be54-24d673600901"
```
### Get all SpFields from List
```c#
SpList list = myUT.Get_SpList_By_Title("LISTNAME");
SpFieldCollection fieldCollection = myUT.Get_SpFields_From_List(list);
```
### Create a SpField on a List
```c#
SpList list = myUT.Get_SpList_By_Title("LISTNAME");
SpField field = new SpField();
field.InternalName = "MyNewField";
field.SpFieldTypeKind = SpField.TypeKind.Text;
field.SetProperty("SchemaXml","<Field Type=\"Text\" DisplayName=\"My new Field\" Required=\"FALSE\" />");
myUT.Create_SpField(field,list);
```
### Update a SpField on a List
```c#
string newXmlSchema = "SchemaXmlString"; // look above
SpList list = myUT.Get_SpList_By_Title("LISTNAME");
SpField field = myUT.Get_SpField_By_InternalName_Or_Title("InterNameOrTitle",list);
field.Properties["SchemaXml"] = newXmlSchema;
myUT.Update_SpField(field,list);
```
### Delete a SpField from a List
```c#
SpList list = myUT.Get_SpList_By_Title("LISTNAME");
SpField field = myUT.Get_SpField_By_InternalName_Or_Title("InterNameOrTitle",list);
myUT.Delete_SpField(field,list);
```

## User Utilities
### Get SpUser by UserName
```c#
SpUser user = myUT.Get_SpUser_By_UserName("USERNAME");
```
### Get SpUser by Id
```c#
int userId = 100;
SpUser user = myUT.Get_SpUser_By_Id(userId);
```
