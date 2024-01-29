namespace PriceSetterDesktop.Libraries.Types
{
    using System.Collections.Generic;

    public class ProviderView
    {
        public int ElementSeed { get; set; } = -1;
        public string ProviderName { get; set; } = "";
        public string URL { get; set; } = "";
        public List<XPathItem> XPathCollection { get; set; } = [];
        public string ClickPath { get; set; } = "";
        public bool HaveURl { get { return URL != string.Empty; } }
        public bool HaveXPathCollection { get { return XPathCollection != null; } }
    }
}
