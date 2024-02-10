namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    using System.Collections.Generic;

    public class ArticleGroupView : IComparable<ArticleGroupView>
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ArticleColorView> Colors { get; set; } = [];

        public int CompareTo(ArticleGroupView? other)
        {
            if (other == null)
                return 0;
            return Name.CompareTo(other.Name);
        }
    }
}
