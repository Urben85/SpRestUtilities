using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP_REST_UTILITY
{
    public class SpFile
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

    public class SpFileCollection
    {
        public List<SpFile> Files = new List<SpFile>();
    }
}
