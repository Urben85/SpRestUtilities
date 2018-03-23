using System;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace SP_REST_UTILITY
{
    public class SpRestUtilities
    {
        #region Properties
        private string _siteUrl;
        public string SiteUrl
        {
            get
            {
                return _siteUrl;
            }
            set
            {
                // Fix given SiteUrl
                _siteUrl = (!value.EndsWith("/")) ? value += "/" : value;
            }
        }
        public NetworkCredential Credentials { get; set; }
        private string _formDigest { get; set; }
        #endregion

        #region All Utilities
        #region List Utilities
        public SpList Get_SpList_By_Title(string title)
        {
            try
            {
                SpList foundList = new SpList();
                XmlDocument listXml = new XmlDocument();
                listXml = GetListXMLByTitle(title);
                foundList = ReturnListByXML(listXml);
                foundList.SpListTemplateType = (SpListTemplateType)int.Parse(foundList.Properties["BaseTemplate"]);
                return foundList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SpList Get_SpList_By_ID(string listID)
        {
            try
            {
                SpList foundList = new SpList();
                XmlDocument listXml = new XmlDocument();
                listXml = GetListXMLByGuid(listID);
                foundList = ReturnListByXML(listXml);
                foundList.SpListTemplateType = (SpListTemplateType)int.Parse(foundList.Properties["BaseTemplate"]);
                return foundList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SpList Create_SpList(SpList list)
        {
            try
            {
                GetAndSetFormDigest();
                list.SetProperty("BaseTemplate", ((int)list.SpListTemplateType).ToString());
                string listDataString = ReturnDataStringForRestRequest(list);

                string postBody = "{'__metadata':{'type':'SP.List'}, " + listDataString + "}";
                byte[] postData = Encoding.UTF8.GetBytes(postBody);
                postBody = Encoding.UTF8.GetString(postData);

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists");
                restRequest.Method = "POST";
                restRequest.Credentials = Credentials;
                restRequest.ContentType = "application/json;odata=verbose";
                restRequest.Accept = "application/atom+xml";
                restRequest.Headers.Add("X-RequestDigest", _formDigest);

                Stream restRequestStream = restRequest.GetRequestStream();
                restRequestStream.Write(postData, 0, postData.Length);
                restRequestStream.Close();

                XmlDocument responseXmlDoc = new XmlDocument();
                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();
                StreamReader streamReader = new StreamReader(restResponse.GetResponseStream());
                responseXmlDoc.LoadXml(streamReader.ReadToEnd());

                return ReturnListByXML(responseXmlDoc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HttpWebResponse Update_SpList(SpList list)
        {
            try
            {
                GetAndSetFormDigest();
                string listDataString = ReturnDataStringForRestRequest(list);

                string listPostBody = "{'__metadata':{'type':'SP.List'}, " + listDataString + "}";
                byte[] listPostData = Encoding.UTF8.GetBytes(listPostBody);
                listPostBody = Encoding.UTF8.GetString(listPostData);

                HttpWebRequest listRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists(guid'" + list.Id + "')");
                listRequest.Method = "POST";
                listRequest.Credentials = Credentials;
                listRequest.Accept = "application/atom+xml";
                listRequest.ContentType = "application/json;odata=verbose";
                listRequest.Headers.Add("X-HTTP-Method", "MERGE");
                listRequest.Headers.Add("IF-MATCH", "*");
                listRequest.Headers.Add("X-RequestDigest", _formDigest);

                Stream listRequestStream = listRequest.GetRequestStream();
                listRequestStream.Write(listPostData, 0, listPostData.Length);
                listRequestStream.Close();

                HttpWebResponse itemResponse = (HttpWebResponse)listRequest.GetResponse();
                return itemResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HttpWebResponse Delete_SpList(SpList list)
        {
            try
            {
                GetAndSetFormDigest();

                HttpWebRequest listRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists(guid'" + list.Id + "')");
                listRequest.Method = "POST";
                listRequest.Credentials = Credentials;
                listRequest.Accept = "application/atom+xml";
                listRequest.ContentType = "application/json;odata=verbose";
                listRequest.ContentLength = 0;
                listRequest.Headers.Add("X-HTTP-Method", "DELETE");
                listRequest.Headers.Add("IF-MATCH", "*");
                listRequest.Headers.Add("X-RequestDigest", _formDigest);

                HttpWebResponse listResponse = (HttpWebResponse)listRequest.GetResponse();
                return listResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private XmlDocument GetListXMLByTitle(string title)
        {
            try
            {
                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists/GetByTitle('" + title + "')");
                restRequest.Method = "GET";
                restRequest.Credentials = Credentials;
                restRequest.Accept = "application/atom+xml";
                restRequest.ContentType = "application/atom+xml;type=entry";

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();

                XmlDocument responseXmlDoc = new XmlDocument();
                StreamReader listReader = new StreamReader(restResponse.GetResponseStream());
                responseXmlDoc.LoadXml(listReader.ReadToEnd());

                return responseXmlDoc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private XmlDocument GetListXMLByGuid(string guid)
        {
            try
            {
                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists(guid'" + guid + "')");
                restRequest.Method = "GET";
                restRequest.Credentials = Credentials;
                restRequest.Accept = "application/atom+xml";
                restRequest.ContentType = "application/atom+xml;type=entry";

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();

                XmlDocument responseXmlDoc = new XmlDocument();
                StreamReader listReader = new StreamReader(restResponse.GetResponseStream());
                responseXmlDoc.LoadXml(listReader.ReadToEnd());

                return responseXmlDoc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private SpList ReturnListByXML(XmlDocument listXmlDoc)
        {
            try
            {
                XmlNamespaceManager lManager = ReturnSpXmlNameSpaceManager(listXmlDoc);

                XmlNode listIdNode = listXmlDoc.SelectSingleNode("//atom:entry/atom:content/m:properties/d:Id", lManager);
                XmlNodeList propertyNodes = listXmlDoc.SelectNodes("//atom:entry/atom:content/m:properties", lManager);

                SpList list = new SpList();
                list.Id = listIdNode.InnerText;
                foreach (XmlNode node in propertyNodes)
                {
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        list.SetProperty(childNode.Name.Replace("d:", ""), childNode.InnerText);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Library Utilities
        public SpFolder Get_SpFolder_By_Path(string folderPath, SpList list = null)
        {
            try
            {
                if (list != null)
                    folderPath = list.Properties["Title"] + FixedPath(folderPath);

                HttpWebRequest listRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/GetFolderByServerRelativeUrl('" + folderPath + "')");
                listRequest.Method = "GET";
                listRequest.Credentials = Credentials;
                listRequest.Accept = "application/atom+xml";

                HttpWebResponse listResponse = (HttpWebResponse)listRequest.GetResponse();

                XmlDocument responseXmlDoc = new XmlDocument();
                StreamReader listReader = new StreamReader(listResponse.GetResponseStream());
                responseXmlDoc.LoadXml(listReader.ReadToEnd());

                return ReturnFolderByXML(responseXmlDoc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SpFolderCollection Get_SpFolderCollection_By_Path(string folderPath, SpList list = null)
        {
            try
            {
                if (list != null)
                    folderPath = list.Properties["Title"] + FixedPath(folderPath);

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/GetFolderByServerRelativeUrl('" + folderPath + "')/Folders");
                restRequest.Method = "GET";
                restRequest.Credentials = Credentials;
                restRequest.Accept = "application/atom+xml";

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();

                XmlDocument responseXmlDoc = new XmlDocument();
                StreamReader responseReader = new StreamReader(restResponse.GetResponseStream());
                responseXmlDoc.LoadXml(responseReader.ReadToEnd());

                XmlNamespaceManager iManager = ReturnSpXmlNameSpaceManager(responseXmlDoc);
                XmlNodeList responseNodes = responseXmlDoc.SelectNodes("//atom:entry", iManager);

                SpFolderCollection folderCollection = new SpFolderCollection();
                foreach (XmlNode node in responseNodes)
                {
                    XmlDocument folderXmlDoc = new XmlDocument();
                    folderXmlDoc.LoadXml(node.OuterXml);
                    SpFolder folder = ReturnFolderByXML(folderXmlDoc);
                    folderCollection.Folders.Add(ReturnFolderByXML(folderXmlDoc));
                }

                if (folderCollection.Folders.Count > 0)
                    folderCollection.Folders.RemoveAt(0);

                return folderCollection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SpFolderCollection Get_All_SpFolders_From_SpFolder(SpFolder folder)
        {
            try
            {
                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/GetFolderByServerRelativeUrl('" + folder.Properties["ServerRelativeUrl"] + "')/Folders");
                restRequest.Method = "GET";
                restRequest.Credentials = Credentials;
                restRequest.Accept = "application/atom+xml";

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();

                XmlDocument responseXmlDoc = new XmlDocument();
                StreamReader responseReader = new StreamReader(restResponse.GetResponseStream());
                responseXmlDoc.LoadXml(responseReader.ReadToEnd());

                XmlNamespaceManager iManager = ReturnSpXmlNameSpaceManager(responseXmlDoc);
                XmlNodeList responseNodes = responseXmlDoc.SelectNodes("//atom:entry", iManager);

                SpFolderCollection folderCollection = new SpFolderCollection();
                foreach (XmlNode node in responseNodes)
                {
                    XmlDocument folderXmlDoc = new XmlDocument();
                    folderXmlDoc.LoadXml(node.OuterXml);
                    folderCollection.Folders.Add(ReturnFolderByXML(folderXmlDoc));
                }

                if (folderCollection.Folders.Count > 0)
                    folderCollection.Folders.RemoveAt(0);

                return folderCollection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SpFile Get_SpFile_By_Filename_From_SpFolder(string fileName, SpFolder folder)
        {
            try
            {
                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/GetFolderByServerRelativeUrl('" + folder.Properties["ServerRelativeUrl"] + "')/Files('" + fileName + "')");
                restRequest.Method = "GET";
                restRequest.Credentials = Credentials;
                restRequest.Accept = "application/atom+xml";

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();

                XmlDocument responseXmlDoc = new XmlDocument();
                StreamReader responseReader = new StreamReader(restResponse.GetResponseStream());
                responseXmlDoc.LoadXml(responseReader.ReadToEnd());

                XmlNamespaceManager iManager = ReturnSpXmlNameSpaceManager(responseXmlDoc);
                XmlNode responseNode = responseXmlDoc.SelectSingleNode("//atom:entry", iManager);
                XmlDocument responseNodeXmlDoc = new XmlDocument();
                responseNodeXmlDoc.LoadXml(responseNode.OuterXml);

                return ReturnFileByXML(responseNodeXmlDoc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    
        public SpFileCollection Get_All_SpFiles_From_SpFolder(SpFolder folder)
        {
            try
            {
                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/GetFolderByServerRelativeUrl('" + folder.Properties["ServerRelativeUrl"] + "')/Files");
                restRequest.Method = "GET";
                restRequest.Credentials = Credentials;
                restRequest.Accept = "application/atom+xml";

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();

                XmlDocument responseXmlDoc = new XmlDocument();
                StreamReader responseReader = new StreamReader(restResponse.GetResponseStream());
                responseXmlDoc.LoadXml(responseReader.ReadToEnd());

                XmlNamespaceManager iManager = ReturnSpXmlNameSpaceManager(responseXmlDoc);
                XmlNode responseNode = responseXmlDoc.SelectSingleNode("//atom:feed", iManager);

                XmlDocument responseNodeXmlDoc = new XmlDocument();
                responseNodeXmlDoc.LoadXml(responseNode.OuterXml);
                XmlNodeList fileNodes = responseNodeXmlDoc.SelectNodes("//atom:entry", iManager);

                SpFileCollection fileCollection = new SpFileCollection();
                foreach (XmlNode node in fileNodes)
                {
                    XmlDocument fileXmlDoc = new XmlDocument();
                    fileXmlDoc.LoadXml(node.OuterXml);
                    SpFile file = ReturnFileByXML(fileXmlDoc);
                    fileCollection.Files.Add(file);
                }

                return fileCollection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SpFolder Create_SpFolder(string folderPath, SpList list = null)
        {
            try
            {
                GetAndSetFormDigest();

                if (list != null)
                    folderPath = list.Properties["Title"] + FixedPath(folderPath);

                string postBody = "{'__metadata': {'type': 'SP.Folder'}, 'ServerRelativeUrl': '" + folderPath + "'}";
                byte[] postData = Encoding.UTF8.GetBytes(postBody);
                postBody = Encoding.UTF8.GetString(postData);

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/folders");
                restRequest.Method = "POST";
                restRequest.Credentials = Credentials;
                restRequest.ContentLength = postData.Length;
                restRequest.ContentType = "application/json;odata=verbose";
                restRequest.Accept = "application/atom+xml";
                restRequest.Headers.Add("X-RequestDigest", _formDigest);

                Stream responseReader = restRequest.GetRequestStream();
                responseReader.Write(postData, 0, postData.Length);
                responseReader.Close();

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();

                XmlDocument responseXmlDoc = new XmlDocument();
                StreamReader streamReader = new StreamReader(restResponse.GetResponseStream());
                responseXmlDoc.LoadXml(streamReader.ReadToEnd());

                return ReturnFolderByXML(responseXmlDoc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Somehow this isn't working!
        private HttpWebResponse Rename_SpFolder(string newName, SpFolder folder)
        {
            try
            {
                GetAndSetFormDigest();

                string postBody = "{'__metadata': {'type': 'SP.Folder'}, 'Name': '" + newName + "'}";
                byte[] postData = Encoding.UTF8.GetBytes(postBody);
                postBody = Encoding.UTF8.GetString(postData);

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/GetFolderByServerRelativeUrl('" + folder.Properties["ServerRelativeUrl"] + "')");
                restRequest.Method = "POST";
                restRequest.Credentials = Credentials;
                restRequest.ContentLength = postData.Length;
                restRequest.Accept = "application/atom+xml";
                restRequest.ContentType = "application/json;odata=verbose";
                restRequest.Headers.Add("X-HTTP-Method", "MERGE");
                restRequest.Headers.Add("IF-MATCH", "*");
                restRequest.Headers.Add("X-RequestDigest", _formDigest);

                Stream requestStream = restRequest.GetRequestStream();
                requestStream.Write(postData, 0, postData.Length);
                requestStream.Close();

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();
                return restResponse;
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HttpWebResponse Delete_SpFolder(SpFolder folder)
        {
            try
            {
                GetAndSetFormDigest();

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/GetFolderByServerRelativeUrl('" + folder.Properties["ServerRelativeUrl"] + "')");
                restRequest.Method = "POST";
                restRequest.Credentials = Credentials;
                restRequest.ContentLength = 0;
                restRequest.Headers.Add("X-HTTP-Method", "DELETE");
                restRequest.Headers.Add("IF-MATCH", "*");
                restRequest.Headers.Add("X-RequestDigest", _formDigest);

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();
                return restResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SpFile Upload_SpFile_To_SpFolder(string filePath, SpFolder folder, bool overwrite = true)
        {
            try
            {
                GetAndSetFormDigest();

                string fileName = Path.GetFileName(filePath);
                byte[] postBody = File.ReadAllBytes(filePath);
                string url = _siteUrl + "_api/web/GetFolderByServerRelativeUrl('" + folder.Properties["ServerRelativeUrl"] + "')/Files/add(url='" + fileName + "',overwrite=" + overwrite.ToString().ToLower() + ")";

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(url);
                restRequest.Method = "POST";
                restRequest.Credentials = Credentials;
                restRequest.Headers.Add("X-RequestDigest", _formDigest);

                Stream requestStream = restRequest.GetRequestStream();
                requestStream.Write(postBody, 0, postBody.Length);
                requestStream.Close();

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();

                XmlDocument responseXmlDoc = new XmlDocument();
                StreamReader responseReader = new StreamReader(restResponse.GetResponseStream());
                responseXmlDoc.Load(responseReader);

                XmlNamespaceManager iManager = ReturnSpXmlNameSpaceManager(responseXmlDoc);
                XmlNode responseNode = responseXmlDoc.SelectSingleNode("//atom:entry", iManager);
                XmlDocument fileXmlDoc = new XmlDocument();
                fileXmlDoc.LoadXml(responseNode.OuterXml);

                return ReturnFileByXML(fileXmlDoc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HttpWebResponse Delete_SpFile(SpFile file)
        {
            try
            {
                GetAndSetFormDigest();

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/GetFileByServerRelativeUrl('" + file.Properties["ServerRelativeUrl"] + "')");
                restRequest.Method = "POST";
                restRequest.Credentials = Credentials;
                restRequest.Headers.Add("X-RequestDigest", _formDigest);
                restRequest.ContentLength = 0;
                restRequest.Headers.Add("X-HTTP-Method", "DELETE");
                restRequest.Headers.Add("IF-MATCH", "*");

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();
                return restResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HttpWebResponse Move_SpFile(SpFile file, SpFolder destinationFolder)
        {
            try
            {
                GetAndSetFormDigest();

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/GetFileByServerRelativeUrl('" + file.Properties["ServerRelativeUrl"] + "')/moveto(newurl='" + destinationFolder.Properties["ServerRelativeUrl"] + "/" + file.Properties["Name"] + "',flags=1)");
                restRequest.Method = "POST";
                restRequest.Credentials = Credentials;
                restRequest.ContentLength = 0;
                restRequest.Headers.Add("X-RequestDigest", _formDigest);

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();
                return restResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HttpWebResponse Copy_SpFile(SpFile file, SpFolder destinationFolder, bool overwrite = true)
        {
            try
            {
                GetAndSetFormDigest();

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/GetFileByServerRelativeUrl('" + file.Properties["ServerRelativeUrl"] + "')/copyto(strnewurl='" + destinationFolder.Properties["ServerRelativeUrl"] + "/" + file.Properties["Name"] + "',boverwrite=" + overwrite.ToString().ToLower() + ")");
                restRequest.Method = "POST";
                restRequest.Credentials = Credentials;
                restRequest.ContentLength = 0;
                restRequest.Headers.Add("X-RequestDigest", _formDigest);

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();
                return restResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private SpFolder ReturnFolderByXML(XmlDocument folderXmlDoc)
        {
            try
            {
                XmlNamespaceManager iManager = ReturnSpXmlNameSpaceManager(folderXmlDoc);
                XmlNodeList propertyNodes = folderXmlDoc.SelectNodes("//atom:content/m:properties", iManager);

                SpFolder folder = new SpFolder();
                foreach (XmlNode node in propertyNodes)
                {
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        folder.SetProperty(childNode.Name.Replace("d:", ""), childNode.InnerText);
                    }
                }

                return folder;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private SpFile ReturnFileByXML(XmlDocument fileXmlDoc)
        {
            try
            {
                XmlNamespaceManager iManager = ReturnSpXmlNameSpaceManager(fileXmlDoc);
                XmlNodeList propertyNodes = fileXmlDoc.SelectNodes("//atom:content/m:properties", iManager);

                SpFile file = new SpFile();
                foreach (XmlNode node in propertyNodes)
                {
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        file.SetProperty(childNode.Name.Replace("d:", ""), childNode.InnerText);
                    }
                }

                return file;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Field Utilities
        public SpField Get_SpField_By_InternalName_Or_Title(string interNalnameOrTitle, SpList list)
        {
            try
            {
                XmlDocument fieldXmlDoc = new XmlDocument();

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists(guid'" + list.Id + "')/fields/getbyinternalnameortitle('" + interNalnameOrTitle + "')");
                restRequest.Method = "GET";
                restRequest.Credentials = Credentials;
                restRequest.Accept = "application/atom+xml";

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();

                StreamReader responseReader = new StreamReader(restResponse.GetResponseStream());

                fieldXmlDoc.LoadXml(responseReader.ReadToEnd());

                return ReturnFieldByXML(fieldXmlDoc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public SpField Get_SpField_By_ID(string fieldID, SpList list)
        {
            try
            {
                XmlDocument fieldXmlDoc = new XmlDocument();

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists(guid'" + list.Id + "')/fields('" + fieldID + "')");
                restRequest.Method = "GET";
                restRequest.Credentials = Credentials;
                restRequest.Accept = "application/atom+xml";

                HttpWebResponse fieldResponse = (HttpWebResponse)restRequest.GetResponse();

                StreamReader responseReader = new StreamReader(fieldResponse.GetResponseStream());

                fieldXmlDoc.LoadXml(responseReader.ReadToEnd());

                return ReturnFieldByXML(fieldXmlDoc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SpFieldCollection Get_SpFields_From_List (SpList list)
        {
            try
            {
                SpFieldCollection fieldColl = new SpFieldCollection();
                XmlDocument fieldsXmlDoc = new XmlDocument();

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists(guid'" + list.Id + "')/fields");
                restRequest.Method = "GET";
                restRequest.Credentials = Credentials;
                restRequest.Accept = "application/atom+xml";

                HttpWebResponse fieldsResponse = (HttpWebResponse)restRequest.GetResponse();

                StreamReader responseReader = new StreamReader(fieldsResponse.GetResponseStream());

                fieldsXmlDoc.LoadXml(responseReader.ReadToEnd());
                XmlNamespaceManager fManager = ReturnSpXmlNameSpaceManager(fieldsXmlDoc);
                XmlNodeList fieldsNodes = fieldsXmlDoc.SelectNodes("//atom:entry", fManager);

                foreach (XmlNode node in fieldsNodes)
                {
                    XmlDocument fieldXmlDoc = new XmlDocument();
                    fieldXmlDoc.LoadXml(node.OuterXml);
                    fieldColl.Fields.Add(ReturnFieldByXML(fieldXmlDoc));
                }

                return fieldColl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SpField Create_SpField (SpField field, SpList list)
        {
            try
            {
                GetAndSetFormDigest();
                string fieldXmlString = field.Properties["SchemaXml"];

                string postBody = "{'__metadata': {'type': 'SP.Field'}, 'FieldTypeKind': " + (int)field.FieldTypeKind + ", 'Title': '" + field.InternalName + "', 'SchemaXml': '" + fieldXmlString + "'}";
                byte[] postData = Encoding.UTF8.GetBytes(postBody);
                postBody = Encoding.UTF8.GetString(postData);

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists(guid'" + list.Id + "')/fields");
                restRequest.Method = "POST";
                restRequest.Credentials = Credentials;
                restRequest.ContentLength = postData.Length;
                restRequest.ContentType = "application/json;odata=verbose";
                restRequest.Accept = "application/atom+xml";
                restRequest.Headers.Add("X-RequestDigest", _formDigest);

                Stream responseReader = restRequest.GetRequestStream();
                responseReader.Write(postData, 0, postData.Length);
                responseReader.Close();

                XmlDocument responseXmlDoc = new XmlDocument();
                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();
                StreamReader streamReader = new StreamReader(restResponse.GetResponseStream());
                responseXmlDoc.LoadXml(streamReader.ReadToEnd());

                return ReturnFieldByXML(responseXmlDoc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public HttpWebResponse Update_SpField (SpField field, SpList list)
        {
            GetAndSetFormDigest();
            string fieldXmlString = field.Properties["SchemaXml"];

            string postBody = "{'__metadata': {'type': 'SP.Field'}, 'FieldTypeKind': " + (int)field.FieldTypeKind + ", 'Title': '" + field.InternalName + "', 'SchemaXml': '" + fieldXmlString + "'}";
            byte[] postData = Encoding.UTF8.GetBytes(postBody);
            postBody = Encoding.UTF8.GetString(postData);

            HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists(guid'" + list.Id + "')/fields('" + field.Id + "')");
            restRequest.Method = "POST";
            restRequest.Credentials = Credentials;
            restRequest.ContentLength = postData.Length;
            restRequest.ContentType = "application/json;odata=verbose";
            restRequest.Headers.Add("X-RequestDigest", _formDigest);
            restRequest.Headers.Add("X-HTTP-Method","MERGE");

            Stream restRequestStream = restRequest.GetRequestStream();
            restRequestStream.Write(postData, 0, postData.Length);
            restRequestStream.Close();

            HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();
            return restResponse;
        }
        
        public HttpWebResponse Delete_SpField (SpField field, SpList list)
        {
            try
            {
                GetAndSetFormDigest();

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists(guid'" + list.Id + "')/fields('" + field.Id + "')");
                restRequest.Method = "POST";
                restRequest.Credentials = Credentials;
                restRequest.ContentLength = 0;
                restRequest.Headers.Add("X-RequestDigest", _formDigest);
                restRequest.Headers.Add("X-HTTP-Method", "DELETE");

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();
                return restResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private SpField ReturnFieldByXML(XmlDocument fieldXmlDoc)
        {
            try
            {
                XmlNamespaceManager fManager = ReturnSpXmlNameSpaceManager(fieldXmlDoc);

                XmlNode fieldIdNode = fieldXmlDoc.SelectSingleNode("//atom:content/m:properties/d:Id", fManager);
                XmlNode fieldTypeKindXmlNode = fieldXmlDoc.SelectSingleNode("//atom:content/m:properties/d:FieldTypeKind", fManager);
                XmlNodeList propertyNodes = fieldXmlDoc.SelectNodes("//atom:content/m:properties", fManager);

                SpField field = new SpField();
                field.Id = fieldIdNode.InnerText;
                field.FieldTypeKind = (SpFieldTypeKind)Int32.Parse(fieldTypeKindXmlNode.InnerText);
                foreach (XmlNode node in propertyNodes)
                {
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        field.SetProperty(childNode.Name.Replace("d:", ""), childNode.InnerText);
                    }
                }

                return field;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region ListItem Utilities
        public SpItem Get_SpItem_By_ID(int itemID, SpList list)
        {
            try
            {
                XmlDocument responseXmlDoc = new XmlDocument();
                XmlDocument itemEntryXmlDoc = new XmlDocument();

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists(guid'" + list.Id + "')/items(" + itemID.ToString() + ")");
                restRequest.Method = "GET";
                restRequest.Credentials = Credentials;
                restRequest.Accept = "application/atom+xml";

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();

                StreamReader responseReader = new StreamReader(restResponse.GetResponseStream());
                responseXmlDoc.Load(responseReader);

                XmlNamespaceManager iManager = ReturnSpXmlNameSpaceManager(responseXmlDoc);
                XmlNode itemNode = responseXmlDoc.SelectSingleNode("//atom:entry", iManager);
                itemEntryXmlDoc.LoadXml(itemNode.OuterXml);

                return ReturnItemByXML(itemEntryXmlDoc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SpItemCollection Get_SpItem_Collection(SpList list, string filter = "")
        {
            try
            {
                HttpWebRequest itemRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists(guid'" + list.Id + "')/items?" + filter);
                itemRequest.Method = "GET";
                itemRequest.Credentials = Credentials;
                itemRequest.Accept = "application/atom+xml";

                HttpWebResponse itemResponse = (HttpWebResponse)itemRequest.GetResponse();

                XmlDocument itemsXmlDoc = new XmlDocument();
                StreamReader itemReader = new StreamReader(itemResponse.GetResponseStream());
                itemsXmlDoc.Load(itemReader);

                XmlNamespaceManager iManager = ReturnSpXmlNameSpaceManager(itemsXmlDoc);
                XmlNodeList itemNodes = itemsXmlDoc.SelectNodes("//atom:entry", iManager);

                SpItemCollection itemCollection = new SpItemCollection();
                foreach (XmlNode node in itemNodes)
                {
                    XmlDocument itemXmlDoc = new XmlDocument();
                    itemXmlDoc.LoadXml(node.OuterXml);
                    itemCollection.Items.Add(ReturnItemByXML(itemXmlDoc));
                }

                return itemCollection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SpItem Create_SpItem(SpItem item, SpList list)
        {
            try
            {
                GetAndSetFormDigest();
                string itemDataString = ReturnDataStringForRestRequest(item);

                string postBody = "{'__metadata':{'type':'" + list.Properties["ListItemEntityTypeFullName"] + "'}, " + itemDataString + "}";
                byte[] postData = Encoding.UTF8.GetBytes(postBody);
                postBody = Encoding.UTF8.GetString(postData);

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists(guid'" + list.Id + "')/items");
                restRequest.Method = "POST";
                restRequest.Credentials = Credentials;
                restRequest.ContentLength = postData.Length;
                restRequest.ContentType = "application/json;odata=verbose";
                restRequest.Accept = "application/atom+xml";
                restRequest.Headers.Add("X-RequestDigest", _formDigest);

                Stream requestStream = restRequest.GetRequestStream();
                requestStream.Write(postData, 0, postData.Length);
                requestStream.Close();

                XmlDocument responseXmlDoc = new XmlDocument();
                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();
                StreamReader streamReader = new StreamReader(restResponse.GetResponseStream());
                responseXmlDoc.LoadXml(streamReader.ReadToEnd());
                XmlNamespaceManager xmlNameSpaceManager = ReturnSpXmlNameSpaceManager(responseXmlDoc);
                XmlNode itemNode = responseXmlDoc.SelectSingleNode("//atom:entry", xmlNameSpaceManager);
                responseXmlDoc.LoadXml(itemNode.OuterXml);

                return ReturnItemByXML(responseXmlDoc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public HttpWebResponse Update_SpItem(SpItem item, SpList list)
        {
            try
            {
                GetAndSetFormDigest();
                string itemDataString = ReturnDataStringForRestRequest(item);

                string postBody = "{'__metadata':{'type':'" + list.Properties["ListItemEntityTypeFullName"] + "'}, " + itemDataString + "}";
                byte[] postData = Encoding.UTF8.GetBytes(postBody);
                postBody = Encoding.UTF8.GetString(postData);

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists(guid'" + list.Id + "')/items(" + item.Id.ToString() + ")");
                restRequest.Method = "POST";
                restRequest.Credentials = Credentials;
                restRequest.Accept = "application/atom+xml";
                restRequest.ContentType = "application/json;odata=verbose";
                restRequest.Headers.Add("X-HTTP-Method", "MERGE");
                restRequest.Headers.Add("IF-MATCH", "*");
                restRequest.Headers.Add("X-RequestDigest", _formDigest);

                Stream requestStream = restRequest.GetRequestStream();
                requestStream.Write(postData, 0, postData.Length);
                requestStream.Close();

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();
                return restResponse;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public HttpWebResponse Delete_SpItem(SpItem item, SpList list)
        {
            try
            {
                GetAndSetFormDigest();

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists(guid'" + list.Id + "')/items(" + item.Id.ToString() + ")");
                restRequest.Method = "POST";
                restRequest.Credentials = Credentials;
                restRequest.Accept = "application/atom+xml";
                restRequest.ContentType = "application/json;odata=verbose";
                restRequest.ContentLength = 0;
                restRequest.Headers.Add("X-HTTP-Method", "DELETE");
                restRequest.Headers.Add("IF-MATCH", "*");
                restRequest.Headers.Add("X-RequestDigest", _formDigest);

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();
                return restResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SpFile Get_Attachment_From_SpItem(string fileName, SpItem item, SpList list)
        {
            try
            {
                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists(guid'" + list.Id + "')/items(" + item.Id.ToString() + ")/AttachmentFiles('" + fileName + "')");
                restRequest.Method = "GET";
                restRequest.Credentials = Credentials;
                restRequest.Accept = "application/atom+xml";

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();

                XmlDocument responseXmlDoc = new XmlDocument();
                StreamReader responseReader = new StreamReader(restResponse.GetResponseStream());
                responseXmlDoc.LoadXml(responseReader.ReadToEnd());

                XmlNamespaceManager iManager = ReturnSpXmlNameSpaceManager(responseXmlDoc);
                XmlNode responseNode = responseXmlDoc.SelectSingleNode("//atom:entry", iManager);
                XmlDocument responseNodeXmlDoc = new XmlDocument();
                responseNodeXmlDoc.LoadXml(responseNode.OuterXml);

                return ReturnFileByXML(responseNodeXmlDoc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SpFileCollection Get_All_Attachments_From_SpItem(SpItem item, SpList list)
        {
            try
            {
                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists(guid'" + list.Id + "')/items(" + item.Id.ToString() + ")/AttachmentFiles");
                restRequest.Method = "GET";
                restRequest.Credentials = Credentials;
                restRequest.Accept = "application/atom+xml";

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();

                XmlDocument responseXmlDoc = new XmlDocument();
                StreamReader responseReader = new StreamReader(restResponse.GetResponseStream());
                responseXmlDoc.LoadXml(responseReader.ReadToEnd());

                XmlNamespaceManager iManager = ReturnSpXmlNameSpaceManager(responseXmlDoc);
                XmlNode responseNode = responseXmlDoc.SelectSingleNode("//atom:feed", iManager);

                XmlDocument responseNodeXmlDoc = new XmlDocument();
                responseNodeXmlDoc.LoadXml(responseNode.OuterXml);
                XmlNodeList fileNodes = responseNodeXmlDoc.SelectNodes("//atom:entry", iManager);

                SpFileCollection fileCollection = new SpFileCollection();
                foreach (XmlNode node in fileNodes)
                {
                    XmlDocument fileXmlDoc = new XmlDocument();
                    fileXmlDoc.LoadXml(node.OuterXml);
                    SpFile file = ReturnFileByXML(fileXmlDoc);
                    fileCollection.Files.Add(file);
                }

                return fileCollection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SpFile Upload_SpItem_Attachment(string filePath, SpItem item, SpList list)
        {
            try
            {
                GetAndSetFormDigest();

                string fileName = Path.GetFileName(filePath);
                byte[] postBody = File.ReadAllBytes(filePath);

                HttpWebRequest restRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/web/lists(guid'" + list.Id + "')/items(" + item.Id.ToString() + ")/AttachmentFiles/add(FileName='" + fileName + "')");
                restRequest.Method = "POST";
                restRequest.Credentials = Credentials;
                restRequest.Headers.Add("X-RequestDigest", _formDigest);

                Stream requestStream = restRequest.GetRequestStream();
                requestStream.Write(postBody, 0, postBody.Length);
                requestStream.Close();

                HttpWebResponse restResponse = (HttpWebResponse)restRequest.GetResponse();

                XmlDocument responseXmlDoc = new XmlDocument();
                StreamReader responseReader = new StreamReader(restResponse.GetResponseStream());
                responseXmlDoc.LoadXml(responseReader.ReadToEnd());

                XmlNamespaceManager iManager = ReturnSpXmlNameSpaceManager(responseXmlDoc);
                XmlNode responseNode = responseXmlDoc.SelectSingleNode("//atom:entry", iManager);
                XmlDocument responseNodeXmlDoc = new XmlDocument();
                responseNodeXmlDoc.LoadXml(responseNode.OuterXml);

                return ReturnFileByXML(responseNodeXmlDoc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private SpItem ReturnItemByXML(XmlDocument itemXmlDoc)
        {
            try
            {
                XmlNamespaceManager iManager = ReturnSpXmlNameSpaceManager(itemXmlDoc);

                XmlNode itemIdNode = itemXmlDoc.SelectSingleNode("//atom:content/m:properties/d:Id", iManager);
                XmlNodeList dataNodes = itemXmlDoc.SelectNodes("//atom:content/m:properties", iManager);

                SpItem item = new SpItem();
                item.Id = int.Parse(itemIdNode.InnerText);
                foreach (XmlNode node in dataNodes)
                {
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        XmlDocument outerXmlDoc = new XmlDocument();
                        outerXmlDoc.LoadXml(childNode.OuterXml);
                        XmlNode fieldTypeNode = outerXmlDoc.FirstChild;
                        string fieldType = (fieldTypeNode.Attributes["m:type"] == null) ? "noType" : fieldTypeNode.Attributes["m:type"].Value;

                        if (fieldType == "SP.FieldUrlValue")
                        {
                            XmlNode descriptionNode = fieldTypeNode.SelectSingleNode("d:Description", iManager);
                            XmlNode urlNode = fieldTypeNode.SelectSingleNode("d:Url", iManager);
                            item.SetFieldValue(childNode.Name.Replace("d:", ""), "{'Url':'" + urlNode.InnerText + "','Description':'" + descriptionNode.InnerText + "'}");
                        }
                        else if (fieldType == "Collection(Edm.Int32)")
                        {
                            XmlNodeList nodeList = fieldTypeNode.SelectNodes("d:element", iManager);
                            StringBuilder str = new StringBuilder();                           
                            str.Append("{'results':[");
                            foreach (XmlNode idNode in nodeList)
                            {
                                str.Append(idNode.InnerText + ",");
                            }
                            if (nodeList.Count > 0)
                                str.Length--;
                            str.Append("]}");
                            item.SetFieldValue(childNode.Name.Replace("d:", ""), str.ToString());
                        }
                        else
                            item.SetFieldValue(childNode.Name.Replace("d:", ""), childNode.InnerText);
                    }
                }

                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region User Utilities
        public SpUser Get_SpUser_By_UserName(string userName)
        {
            string url = _siteUrl + "_api/web/siteusers(@v)?@v=%27i%3A0%23.w%7Casbnet%5C" + userName + "%27";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = Credentials;
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (XmlReader xmlReader = XmlReader.Create(responseStream))
                    {
                        XmlDocument userXmlDoc = new XmlDocument();
                        userXmlDoc.Load(xmlReader);
                        SpUser spUser = ReturnUserByXML(userXmlDoc);
                        return spUser;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SpUser Get_SpUser_By_Id(int id)
        {
            string url = _siteUrl + "_api/Web/GetUserById(" + id.ToString() + ")";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Credentials = Credentials;
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (XmlReader xmlReader = XmlReader.Create(responseStream))
                    {
                        XmlDocument userXmlDoc = new XmlDocument();
                        userXmlDoc.Load(xmlReader);
                        SpUser spUser = ReturnUserByXML(userXmlDoc);
                        return spUser;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private SpUser ReturnUserByXML(XmlDocument userXmlDoc)
        {
            XmlNamespaceManager uManager = ReturnSpXmlNameSpaceManager(userXmlDoc);

            XmlNode uIdNode = userXmlDoc.SelectSingleNode("//atom:entry/atom:content/m:properties/d:Id", uManager);
            XmlNode uIsHiddenInUiNode = userXmlDoc.SelectSingleNode("//atom:entry/atom:content/m:properties/d:IsHiddenInUI", uManager);
            XmlNode uLoginNameNode = userXmlDoc.SelectSingleNode("//atom:entry/atom:content/m:properties/d:LoginName", uManager);
            XmlNode uTitleNode = userXmlDoc.SelectSingleNode("//atom:entry/atom:content/m:properties/d:Title", uManager);
            XmlNode uPrincipalTypeNode = userXmlDoc.SelectSingleNode("//atom:entry/atom:content/m:properties/d:PrincipalType", uManager);
            XmlNode uEmailNode = userXmlDoc.SelectSingleNode("//atom:entry/atom:content/m:properties/d:Email", uManager);
            XmlNode uIsSiteAdminNode = userXmlDoc.SelectSingleNode("//atom:entry/atom:content/m:properties/d:IsSiteAdmin", uManager);
            XmlNode uUserIdNode = userXmlDoc.SelectSingleNode("//atom:entry/atom:content/m:properties/d:UserId", uManager);

            SpUser spUser = new SpUser();
            spUser.Id = Int32.Parse(uIdNode.InnerText);
            spUser.IsHiddenInUi = bool.Parse(uIsHiddenInUiNode.InnerText);
            spUser.LoginName = uLoginNameNode.InnerText;
            spUser.Title = uTitleNode.InnerText;
            spUser.PrincipalType = (SpUserPrincipalType)Int32.Parse(uPrincipalTypeNode.InnerText);
            spUser.Email = uEmailNode.InnerText;
            spUser.IsSiteAdmin = bool.Parse(uIsSiteAdminNode.InnerText);
            spUser.UserId = uUserIdNode.InnerText;

            return spUser;
        }
        #endregion
        #endregion

        #region Private General Methods
        private void GetAndSetFormDigest()
        {
            try
            {
                HttpWebRequest ctxInfoRequest = (HttpWebRequest)WebRequest.Create(_siteUrl + "_api/contextinfo");
                ctxInfoRequest.Method = "POST";
                ctxInfoRequest.ContentType = "text/xml;charset=utf-8";
                ctxInfoRequest.ContentLength = 0;
                ctxInfoRequest.Credentials = Credentials;

                HttpWebResponse ctxInfoResponse = (HttpWebResponse)ctxInfoRequest.GetResponse();

                StreamReader contextinfoReader = new StreamReader(ctxInfoResponse.GetResponseStream(), System.Text.Encoding.UTF8);

                XmlDocument formDigestXML = new XmlDocument();
                formDigestXML.LoadXml(contextinfoReader.ReadToEnd());
                XmlNamespaceManager fdManager = new XmlNamespaceManager(formDigestXML.NameTable);
                fdManager.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
                XmlNode formDigestNode = formDigestXML.SelectSingleNode("//d:FormDigestValue", fdManager);
                _formDigest = formDigestNode.InnerXml;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string FixedPath(string path)
        {
            path = (path.StartsWith("/")) ? path : "/" + path;
            return (path.EndsWith("/")) ? path.Remove(path.Length -1) : path;
        }

        private string ReturnDataStringForRestRequest(SpList list)
        {
            StringBuilder str = new StringBuilder();
            foreach (KeyValuePair<string, string> property in list.Properties)
            {
                if (property.Key == "Title" ||
                    property.Key == "Description" ||
                    property.Key == "ContentTypesEnabled" ||
                    property.Key == "AllowContentTypes" ||
                    property.Key == "EnableAttachments" ||
                    property.Key == "EnableFolderCreation" ||
                    property.Key == "EnableMinorVersions" ||
                    property.Key == "EnableModeration" ||
                    property.Key == "EnableVersioning" ||
                    property.Key == "ForceCheckout" ||
                    property.Key == "HasExternalDataSource" ||
                    property.Key == "Hidden" ||
                    property.Key == "IrmEnabled" ||
                    property.Key == "IrmExpire" ||
                    property.Key == "IrmReject" ||
                    property.Key == "IsApplicationList" ||
                    property.Key == "IsCatalog" ||
                    property.Key == "IsPrivate" ||
                    property.Key == "MultipleDataList" ||
                    property.Key == "NoCrawl" ||
                    property.Key == "ServerTemplateCanCreateFolders" ||
                    property.Key == "BaseTemplate")
                    str.Append("'" + property.Key + "':'" + property.Value + "',");
            }
            str.Length--;
            return str.ToString();
        }

        private string ReturnDataStringForRestRequest(SpItem item)
        {
            StringBuilder str = new StringBuilder();
            foreach (KeyValuePair<string, string> data in item.Data)
            {
                if (!string.IsNullOrEmpty(data.Value))
                {
                    if (data.Value.StartsWith("{"))
                        str.Append("'" + data.Key + "':" + data.Value + ",");
                    else
                        str.Append("'" + data.Key + "':'" + data.Value + "',");
                }
            }
            str.Length--;
            return str.ToString();
        }

        private XmlNamespaceManager ReturnSpXmlNameSpaceManager(XmlDocument xmlDoc)
        {
            try
            {
                XmlNamespaceManager manager = new XmlNamespaceManager(xmlDoc.NameTable);
                manager.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
                manager.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
                manager.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                return manager;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}