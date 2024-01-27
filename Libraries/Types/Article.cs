namespace PriceSetterDesktop.Libraries.Types
{
    using PriceSetterDesktop.Libraries.Statics;
    using System;
    using System.Xml;
    using WPFCollection.Data.Attributes;
    using WPFCollection.Data.Interface;
    using WPFCollection.Data.List;

    [XmlMarker(nameof(Article))]
    public partial class Article : IGeneratable, IXmlItem
    {
        public Article()
        {
            
        }
        [XmlItem(nameof(Name), "string")]
        public string Name { get; set; }
        public IEnumerable<Prices> Prices
        {
            get
            {
                var db = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
                var tb = db.GetTable<Prices>(nameof(Prices));
                return tb.List.Where(x => x.ArticleID == ElementSeed);
            }
        }
        public int ElementSeed { get; set; }

        public IXmlItem CreateObject()
        {
            return this;
        }

        public IXmlItem CreateObjectFromNode(XmlNodeList nodeList, int seed)
        {
            var newObject = new Article();
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
            var CompareObject = obj as Article;
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
