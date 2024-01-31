namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    using PriceSetterDesktop.Libraries.Types.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ArticleDetails
    {
        public Provider? Provider { get; set; }
        public string Color { get; set; } = string.Empty;
        public List<TagView> Tags { get; set; } = [];
        public string ErrorDescription { get; set; } = string.Empty;
        public bool IsError { get; set; } = false;
    }
}
