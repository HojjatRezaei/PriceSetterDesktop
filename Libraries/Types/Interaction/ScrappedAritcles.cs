namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    using System.Collections.Generic;

    public class ScrappedAritcles
    {
        public string Name { get; set; }
        public List<ArticleDetails> ArticleDetails { get; set; } = [];
    }
}
