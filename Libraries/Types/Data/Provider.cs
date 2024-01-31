namespace PriceSetterDesktop.Libraries.Types.Data
{
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Enum;
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
        [XmlItem(nameof(Extraction) , "int")]
        public ExtractionTypes Extraction { get; set; } = 0;
        public List<ContainerXPath> Containers 
        {
            get
            {
                var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
                var tb = db.GetTable<ContainerXPath>(nameof(ContainerXPath));
                return tb.List.Where(x => x.ProviderID == ElementSeed).ToList();

            }
        
        }
        public IEnumerable<Prices> Prices
        {
            get
            {
                var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
                var tb = db.GetTable<Prices>(nameof(Prices));
                return tb.List.Where(x => x.ProviderID == ElementSeed);
            }
        }
        public bool HaveURL { get; set; } = false;
        public bool HaveData 
        { 
            get 
            {
                return Containers.Count != 0;
            }
        }
        public int ElementSeed { get; set; } = -1;

        public IXmlItem CreateObject()
        {
            return this;
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
