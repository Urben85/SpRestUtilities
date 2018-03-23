using System.Collections.Generic;

namespace SP_REST_UTILITY
{
    public class SpField
    {
        public string Id { get; set; }
        public string InternalName { get; set; }
        public SpFieldTypeKind FieldTypeKind { get; set; }
        public Dictionary<string, string> Properties = new Dictionary<string, string>();

        public void SetProperty(string propertyName, string value)
        {
            if (Properties.ContainsKey(propertyName))
                Properties[propertyName] = value;
            else
                Properties.Add(propertyName, value);
        }
    }
    public enum SpFieldTypeKind
    {
        None = 0,
        Integer = 1,
        Text = 2,
        Note = 3,
        DateTime = 4,
        Counter = 5,
        Choice = 6,
        Lookup = 7,
        Boolean = 8,
        Number = 9,
        Currency = 10,
        URL = 11,
        Computed = 12,
        Threading = 13,
        Guid = 14,
        MultiChoice = 15,
        GridChoice = 16,
        Calculated = 17,
        File = 18,
        Attachments = 19,
        User = 20,
        Recurrence = 21,
        CrossProjectLink = 22,
        ModStat = 23,
        Error = 24,
        ContentTypeId = 25,
        PageSeparator = 26,
        ThreadIndex = 27,
        WorkflowStatus = 28,
        AllDayEvent = 29,
        WorkflowEventType = 30,
        MaxItems = 31
    }
    public class SpFieldCollection
    {
        public List<SpField> Fields = new List<SpField>();
    }

}