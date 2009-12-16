namespace ASFTag.Net
{
    public class Attribute
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public WMT_ATTR_DATATYPE Type { get; set; }

        public Attribute(string name, string value, WMT_ATTR_DATATYPE type)
        {
            Name = name;
            Value = value;
            Type = type;
        }
    }
}