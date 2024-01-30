namespace PriceSetterDesktop.Libraries.Types.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using WPFCollection.Data.Attributes;
    using WPFCollection.Data.Interface;
    [XmlMarker(nameof(Prices))]
    public partial class Prices : IGeneratable, IXmlItem
    {
        public Prices()
        {

        }
        [XmlItem(nameof(Date), "DateTime")]
        public DateTime Date { get; set; }
        [XmlItem(nameof(Price), "double")]
        public double Price { get; set; }
        [XmlItem(nameof(ProviderID), "int")]
        public int ProviderID { get; set; }
        [XmlItem(nameof(ArticleID), "int")]
        public int ArticleID { get; set; }
        public int ElementSeed { get; set; }

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
            var CompareObject = obj as Prices;
            if (CompareObject == null)
                return false;
            return Date == CompareObject.Date &&
                   Price == CompareObject.Price &&
                   ProviderID == CompareObject.ProviderID &&
                   ArticleID == CompareObject.ArticleID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Date, Price, ProviderID, ArticleID);
        }
        public override string ToString()
        {
            return $"{Date} / {Price}";
        }
    }
}
