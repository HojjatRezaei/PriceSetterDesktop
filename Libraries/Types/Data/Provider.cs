namespace PriceSetterDesktop.Libraries.Types.Data
{
    using PriceSetterDesktop.Libraries.Statics;
    using System;
    using System.Xml;
    using WPFCollection.Data.Attributes;
    using WPFCollection.Data.Interface;

    [XmlMarker(nameof(Provider))]
    public partial class Provider : IGeneratable, IXmlItem
    {
        public Provider()
        {
        }
        [XmlItem(nameof(Name), "string")]
        public string Name { get; set; } = "";
        public List<ContainerXPath> Containers { get; set; } = [];
        public IEnumerable<Prices> Prices
        {
            get
            {
                var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
                var tb = db.GetTable<Prices>(nameof(Prices));
                return tb.List.Where(x => x.ProviderID == ElementSeed);
            }
        }
        public int ElementSeed { get; set; } = -1;

        public IXmlItem CreateObject()
        {
            return this;
        }

        public IXmlItem CreateObjectFromNode(XmlNodeList nodeList, int seed)
        {
            var newObject = new Provider();
            foreach (XmlNode node in nodeList)
            {
                var searchProperty = newObject.GetType().GetProperty(node.Name);
                if (searchProperty != null)
                {
                    var newVal = Convert.ChangeType(node.InnerText, searchProperty.PropertyType);
                    searchProperty.SetValue(newObject, newVal);
                }
            }
            ElementSeed = seed;
            return newObject;
        }
        public string GenerateIdentifier()
        {
            //propertyHash;
            return "";
        }
        public override bool Equals(object? obj)
        {
            var CompareObject = obj as Provider;
            if (CompareObject == null)
                return false;
            return Name == CompareObject.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
