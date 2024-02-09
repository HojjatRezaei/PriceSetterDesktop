namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    using System.Collections.Generic;

    public class ArticleGroupView
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ArticleColorView> Colors { get; set; } = [];
    }
}
