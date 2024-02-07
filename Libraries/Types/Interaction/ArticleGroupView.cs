namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ArticleGroupView
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ArticleColorView> Colors { get; set; } = [];
    }
}
