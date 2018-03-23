# Sharepoint REST Utilities for C#
The Sharepoint rest api is very powerful but writing code that does something with it can be tricky and costly sometimes. Everytime i wanted to use it i had to try to remember how to do this and that, looking through code i've written and of course googling. So i thought it would be great to have a class library which would do all the dirty work for me and this is what this project is basically all about. But I'm warning you in advance: This hasn't been fully tested! Use it on your own risk! It's also still work in progress and may or my be not further developed in the future.

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
### Get a SpFolder
```c#
// With a Library
SpList lilibrary = myUT.Get_SpList_By_Title("LIBRARYNAME");
SpFolder folder = myUT.Get_SpFolder_By_Path("FOLDERNAME",library);

// With path only
SpFolder folder = myUT.Get_SpFolder_By_Path("LIBRARYNAME/FOLDERNAME");

// Get a Subfolder
SpFolder folder = myUT.Get_SpFolder_By_Path("LIBRARYNAME/FOLDERNAME/SUBFOLDERNAME");
```
### Get all SpFolders from SpFolder
```c#
// With path and Library
SpList lilibrary = myUT.Get_SpList_By_Title("LIBRARYNAME");
SpFolderCollection folderCollection = myUT.Get_SpFolderCollection_By_Path("FOLDERNAME",library);

// With path only
SpFolderCollection folderCollection = myUT.Get_SpFolderCollection_By_Path("LIBRARYNAME/FOLDERNAME");

// Get all Folders in Subfolder
SpFolderCollection folderCollection = myUT.Get_SpFolderCollection_By_Path("LIBRARYNAME/FOLDERNAME/SUBFOLDERNAME");

// By SpFolder
SpFolder folder = myUT.Get_SpFolder_By_Path("LIBRARYNAME/FOLDERNAME");
SpFolderCollection folderCollection = Get_All_SpFolders_From_SpFolder(folder);
```
### Create a new SpFolder
```c#
// With path and Library
SpList lilibrary = myUT.Get_SpList_By_Title("LIBRARYNAME");
SpFolder folder = myUT.Create_SpFolder("FOLDERNAME",library);

// With path only
SpFolder folder = myUT.Create_SpFolder("LIBRARYNAME/FOLDERNAME");

// Create a Subfolder
SpFolder subFolder = myUT.Create_SpFolder("LIBRARYNAME/FOLDERNAME/SUBFOLDERNAME"); // Parent Folders must exist!
```
### Delete a SpFolder
```c#
SpFolder folder = myUT.Get_SpFolder_By_Path("LIBRARYNAME/FOLDERNAME");
myUT.Delete_SpFolder(folder);
```
### Get a SpFile by FileName from SpFolder
```c#
SpFolder folder = myUT.Get_SpFolder_By_Path("LIBRARYNAME/FOLDERNAME");
SpFile file = myUT.Get_SpFile_By_Filename_From_SpFolder("FILENAME.txt",folder);
```
### Get all SpFiles by FileName from SpFolder
```c#
SpFolder folder = myUT.Get_SpFolder_By_Path("LIBRARYNAME/FOLDERNAME");
SpFileCollection file = myUT.Get_All_SpFiles_From_SpFolder(folder);
```
### Upload a File into a SpFolder
```c#
string path = @"C:\yourpath\FILENAME.txt";
SpFolder folder = myUT.Get_SpFolder_By_Path("LIBRARYNAME/FOLDERNAME");

// Auto overwrite
SpFile file = myUT.Upload_SpFile_To_SpFolder(path,folder); // if overwrite is undefined it is set to true

// overwrite false
SpFile file = myUT.Upload_SpFile_To_SpFolder(path,folder,false);
```
### Delete SpFile
```c#
SpFolder folder = myUT.Get_SpFolder_By_Path("LIBRARYNAME/FOLDERNAME");
SpFile file = myUT.Get_SpFile_By_Filename_From_SpFolder("FILENAME.txt",folder);
myUT.Delete_SpFile(file);
```
### Move SpFile to another SpFolder
```c#
SpFolder sourceFolder = myUT.Get_SpFolder_By_Path("LIBRARYNAME/FOLDERNAME");
SpFile file = myUT.Get_SpFile_By_Filename_From_SpFolder("FILENAME.txt",sourceFolder);
SpFolder destFolder = myUT.Get_SpFolder_By_Path("LIBRARYNAME/DESTINATIONFOLDERNAME");
myUT.Move_SpFile(file,destFolder);
```
### Copy SpFile to another SpFolder
```c#
SpFolder sourceFolder = myUT.Get_SpFolder_By_Path("LIBRARYNAME/FOLDERNAME");
SpFile file = myUT.Get_SpFile_By_Filename_From_SpFolder("FILENAME.txt",sourceFolder);
SpFolder destFolder = myUT.Get_SpFolder_By_Path("LIBRARYNAME/DESTINATIONFOLDERNAME");

// Auto overwrite
myUT.Copy_SpFile(file,destFolder);

// overwrite false
myUT.Copy_SpFile(file,destFolder,false);
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
### Upload an Attachment to a SpItem
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
