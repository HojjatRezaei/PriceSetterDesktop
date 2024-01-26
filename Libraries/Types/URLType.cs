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
    [XmlMarker(nameof(URLType))]
    public class URLType : BaseFileType<URLType> , IXmlItem
    {
        public URLType()
        {
            
        }
        [XmlItem(nameof(URL), "string")]
        public string URL { get; set; }
        [XmlItem(nameof(XPath), "string")]
        public string XPath { get; set; }
        [XmlItem(nameof(ProviderID), "int")]
        public int ProviderID { get; set; }
        [XmlItem(nameof(ArticleID), "int")]
        public int ArticleID { get; set; }
    }
}
