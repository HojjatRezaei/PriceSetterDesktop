namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    using PriceSetterDesktop.Libraries.Types.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ArticleView
    {
        public string Name { get; set; }
        public List<ArticleDetails> ArticleDetails { get; set; } = [];
    }
}
