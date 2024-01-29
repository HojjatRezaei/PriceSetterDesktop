namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    using System.Collections.Generic;
    using PriceSetterDesktop.Libraries.Types.Data;

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
