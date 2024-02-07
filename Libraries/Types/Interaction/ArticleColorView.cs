namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ArticleColorView
    {
        public int ColorMetaID { get; set; } = -1;
        public string ColorName { get; set; } = string.Empty;
        public int PriceMetaID { get; set; } = -1;
        public double PriceValue { get; set; } = 0;
    }
}
