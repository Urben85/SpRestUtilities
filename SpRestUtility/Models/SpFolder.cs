using System.Collections.Generic;

namespace SP_REST_UTILITY
{
    public class SpFolder
    {
        public Dictionary<string, string> Properties = new Dictionary<string, string>();

        public void SetProperty(string fieldName, string value)
        {
            if (Properties.ContainsKey(fieldName))
                Properties[fieldName] = value;
            else
                Properties.Add(fieldName, value);
        }
    }
    public class SpFolderCollection
    {
        public List<SpFolder> Folders = new List<SpFolder>();
    }

}
