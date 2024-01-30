namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PriceView
    {
        public string ArticleName { get; set; } = string.Empty;
        public string ProviderName { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        //extract 35 tag for 4 colors
        public List<TagView> Tags { get; set; } = [];
        public string ErrorDescription { get; set; } = string.Empty;
        public bool IsError { get; set; } = false;
    }
}
