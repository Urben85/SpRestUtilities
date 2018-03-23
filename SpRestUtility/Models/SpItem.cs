using System.Collections.Generic;

namespace SpRestUtility
{
    public class SpItem
    {
        public int Id { get; set; }
        public Dictionary<string, string> Data = new Dictionary<string, string>();

        public void SetFieldValue(string fieldName, string value)
        {
            if (Data.ContainsKey(fieldName))
                Data[fieldName] = value;
            else
                Data.Add(fieldName, value);
        }
    }

    public class SpItemCollection
    {
        public List<SpItem> Items = new List<SpItem>();
    }
}