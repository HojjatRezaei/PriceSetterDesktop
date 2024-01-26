namespace PriceSetterDesktop.Libraries.Types
{
    using PriceSetterDesktop.Libraries.Types.Base;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using WPFCollection.Data.Attributes;
    using WPFCollection.Data.Interface;
    [XmlMarker(nameof(Prices))]
    public class Prices : BaseFileType<Prices> , IXmlItem
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

    }
}
