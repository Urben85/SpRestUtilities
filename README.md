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
SpList listA = myUT.GetSpListByTitle("LISTNAME");
SpList listB = myUT.GetSpListByID("GUID"); // e.g.: "bacfa614-08de-428e-be54-24d673600901"
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
myUt.CreateSPList(newList);
```
### Update a List
```c#
SpList list = myUT.GetSpListByTitle("LISTNAME");
list.SetProperty("Title","NEWTITLE");
myUT.UpdateSpList(list);
```
### Delete a List
```c#
SpList list = myUT.GetSpListByTitle("LISTNAME");
myUT.DeleteSpList(list);
```

## Library Utilities
### Get a SpFolder
```c#
// With a Library
SpList lilibrary = myUT.GetSpListByTitle("LIBRARYNAME");
SpFolder folder = myUT.GetSpFolderByPath("FOLDERNAME",library);

// With path only
SpFolder folder = myUT.GetSpFolderByPath("LIBRARYNAME/FOLDERNAME");

// Get a Subfolder
SpFolder folder = myUT.GetSpFolderByPath("LIBRARYNAME/FOLDERNAME/SUBFOLDERNAME");
```
### Get all SpFolders from SpFolder
```c#
// With path and Library
SpList lilibrary = myUT.GetSpListByTitle("LIBRARYNAME");
SpFolderCollection folderCollection = myUT.GetSpFolderCollectionByPath("FOLDERNAME",library);

// With path only
SpFolderCollection folderCollection = myUT.GetSpFolderCollectionByPath("LIBRARYNAME/FOLDERNAME");

// Get all Folders in Subfolder
SpFolderCollection folderCollection = myUT.GetSpFolderCollectionByPath("LIBRARYNAME/FOLDERNAME/SUBFOLDERNAME");

// By SpFolder
SpFolder folder = myUT.GetSpFolderByPath("LIBRARYNAME/FOLDERNAME");
SpFolderCollection folderCollection = myUT.GetAllSpFoldersFromSpFolder(folder);
```
### Create a new SpFolder
```c#
// With path and Library
SpList lilibrary = myUT.GetSpListByTitle("LIBRARYNAME");
SpFolder folder = myUT.CreateSpFolder("FOLDERNAME",library);

// With path only
SpFolder folder = myUT.CreateSpFolder("LIBRARYNAME/FOLDERNAME");

// Create a Subfolder
SpFolder subFolder = myUT.CreateSpFolder("LIBRARYNAME/FOLDERNAME/SUBFOLDERNAME"); // Parent Folders must exist!
```
### Delete a SpFolder
```c#
SpFolder folder = myUT.GetSpFolderByPath("LIBRARYNAME/FOLDERNAME");
myUT.DeleteSpFolder(folder);
```
### Get a SpFile by FileName from SpFolder
```c#
SpFolder folder = myUT.GetSpFolderByPath("LIBRARYNAME/FOLDERNAME");
SpFile file = myUT.GetSpFileByFilenameFromSpFolder("FILENAME.txt",folder);
```
### Get all SpFiles by FileName from SpFolder
```c#
SpFolder folder = myUT.GetSpFolderByPath("LIBRARYNAME/FOLDERNAME");
SpFileCollection file = myUT.GetAllSpFilesFromSpFolder(folder);
```
### Upload a File into a SpFolder
```c#
string path = @"C:\yourpath\FILENAME.txt";
SpFolder folder = myUT.GetSpFolderByPath("LIBRARYNAME/FOLDERNAME");

// Auto overwrite
SpFile file = myUT.UploadSpFileToSpFolder(path,folder); // if overwrite is undefined it is set to true

// overwrite false
SpFile file = myUT.UploadSpFileToSpFolder(path,folder,false);
```
### Delete SpFile
```c#
SpFolder folder = myUT.GetSpFolderByPath("LIBRARYNAME/FOLDERNAME");
SpFile file = myUT.GetSpFileByFilenameFromSpFolder("FILENAME.txt",folder);
myUT.DeleteSpFile(file);
```
### Move SpFile to another SpFolder
```c#
SpFolder sourceFolder = myUT.GetSpFolderByPath("LIBRARYNAME/FOLDERNAME");
SpFile file = myUT.GetSpFileByFilenameFromSpFolder("FILENAME.txt",sourceFolder);
SpFolder destFolder = myUT.GetSpFolderByPath("LIBRARYNAME/DESTINATIONFOLDERNAME");
myUT.MoveSpFile(file,destFolder);
```
### Copy SpFile to another SpFolder
```c#
SpFolder sourceFolder = myUT.GetSpFolderByPath("LIBRARYNAME/FOLDERNAME");
SpFile file = myUT.GetSpFileByFilenameFromSpFolder("FILENAME.txt",sourceFolder);
SpFolder destFolder = myUT.GetSpFolderByPath("LIBRARYNAME/DESTINATIONFOLDERNAME");

// Auto overwrite
myUT.CopySpFile(file,destFolder);

// overwrite false
myUT.CopySpFile(file,destFolder,false);
```

## Item Utilities
### Get SpItem by ID
```c#
int yourItemId = 100;
SpList list = myUT.GetSpListByTitle("LISTNAME");
SpItem item = myUT.GetSpItemByID(yourItemId,list);
```
### Get SpItemCollection by optional filter
```c#
// Without filter example
SpList listA = myUT.GetSpListByTitle("LISTNAMEA");
SpItemCollection collection = myUT.GetSpItemCollection(listA);

// With filter example
SpList listB = myUT.GetSpListByTitle("LISTNAMEB");
SpItemCollection collection = myUT.GetSpItemCollection(listB,"$filter=Fieldname eq 'Whatever'");
```
### Access Field Values
```c#
int yourItemId = 100;
SpList list = myUT.GetSpListByTitle("LISTNAME");
SpItem item = myUT.GetSpItemByID(yourItemId,list);
string title = item.Data["Title"];
```
### Update SpItem
```c#
int yourItemId = 100;
SpList list = myUT.GetSpListByTitle("LISTNAME");
SpItem item = myUT.GetSpItemByID(yourItemId,list);
item.SetFieldValue("Title","New Title");
myUT.UpdateSpItem(item,list);
```
### Setting Lookups, User and URL Fields
```c#
int yourItemId = 100;
SpList list = myUT.GetSpListByTitle("LISTNAME");
SpItem item = myUT.GetSpItemByID(yourItemId,list);

// Set LookupSingle and UserSingle
item.Data["SingleId"] = "100"; // "Id" must be applied at the end of the Fieldname!
item.Data["SingleId"] = "-1"; // Resets the Field to empty

// Set LookupMulti and UserMulti
item.Data["MultiId"] = "{'results':[100,101]}"; // "Id" must be applied at the end of the Fieldname!
item.Data["MultiId"] = "{'results':[]}"; // Resets the Field to empty

// Set URL Field
item.Data["UrlField"] = "{'Url':'https://github.com','Description':'GitHub'}";
item.Data["UrlField"] = "{'Url':'','Description':''}"; // Resets the Field to empty

myUT.UpdateSpItem(item,list);
```
### Delete SpItem
```c#
int yourItemId = 100;
SpList list = myUT.GetSpListByTitle("LISTNAME");
SpItem item = myUT.GetSpItemByID(yourItemId,list);
myUT.DeleteSpItem(item,list);
```
### Get an Attachment from a SpItem by Filename
```c#
int yourItemId = 100;
SpList list = myUT.GetSpListByTitle("LISTNAME");
SpItem item = myUT.GetSpItemByID(yourItemId,list);
SpFile attachment = myUT.GetAttachmentFromSpItem("FILENAME.txt",item,list);
```
### Get all Attachments from a SpItem
```c#
int yourItemId = 100;
SpList list = myUT.GetSpListByTitle("LISTNAME");
SpItem item = myUT.GetSpItemByID(yourItemId,list);
SpFileCollection attachments = myUT.GetAllAttachmentsFromSpItem(item,list);
```
### Upload an Attachment to a SpItem
```c#
int yourItemId = 100;
string path = @"C:\yourpath\FILENAME.txt";
SpList list = myUT.GetSpListByTitle("LISTNAME");
SpItem item = myUT.GetSpItemByID(yourItemId,list);
SpFile attachment = myUT.UploadSpItemAttachment(path,item,list);
```
### Rename an Attachment of a SpItem
```c#
SpList list = ut.GetSpListByTitle("LISTNAME");
SpItem item = ut.GetSpItemByID(1, list);
SpFile file = ut.GetAttachmentFromSpItem("FILENAME.TYPE", item, list);
ut.RenameSpItemAttachment("NEWFILENAME.TYPE", file);
```
## Field Utilities
### Get a SpField by InternalName or Title
```c#
SpList list = myUT.GetSpListByTitle("LISTNAME");
SpField field = myUT.GetSpFieldByInternalNameOrTitle("InterNameOrTitle",list);
```
### Get a SpField by ID
```c#
SpList list = myUT.GetSpListByTitle("LISTNAME");
SpField field = myUT.GetSpFieldByID("GUID",list); // e.g.: "bacfa614-08de-428e-be54-24d673600901"
```
### Get all SpFields from List
```c#
SpList list = myUT.GetSpListByTitle("LISTNAME");
SpFieldCollection fieldCollection = myUT.GetSpFieldsFromList(list);
```
### Create a SpField on a List
```c#
SpList list = myUT.GetSpListByTitle("LISTNAME");
SpField field = new SpField();
field.InternalName = "MyNewField";
field.SpFieldTypeKind = SpField.TypeKind.Text;
field.SetProperty("SchemaXml","<Field Type=\"Text\" DisplayName=\"My new Field\" Required=\"FALSE\" />");
myUT.CreateSpField(field,list);
```
### Update a SpField on a List
```c#
string newXmlSchema = "SchemaXmlString"; // look above
SpList list = myUT.GetSpListByTitle("LISTNAME");
SpField field = myUT.GetSpFieldByInternalNameOrTitle("InterNameOrTitle",list);
field.Properties["SchemaXml"] = newXmlSchema;
myUT.UpdateSpField(field,list);
```
### Delete a SpField from a List
```c#
SpList list = myUT.GetSpListByTitle("LISTNAME");
SpField field = myUT.GetSpFieldByInternalNameOrTitle("InterNameOrTitle",list);
myUT.DeleteSpField(field,list);
```

## User Utilities
### Get SpUser by UserName
```c#
SpUser user = myUT.GetSpUserByUserName("USERNAME");
```
### Get SpUser by Id
```c#
int userId = 100;
SpUser user = myUT.GetSpUserById(userId);
```
