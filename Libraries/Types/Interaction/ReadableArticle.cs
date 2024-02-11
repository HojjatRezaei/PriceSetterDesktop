namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    using System.Collections.Generic;

    public class ReadableArticle : IComparable<ReadableArticle>
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ColorView> Colors { get; set; } = [];

        public int CompareTo(ReadableArticle? other)
        {
            if (other == null)
                return 0;
            return Name.CompareTo(other.Name);
        }
    }
}
