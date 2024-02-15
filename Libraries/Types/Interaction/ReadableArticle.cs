namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    using PriceSetterDesktop.Libraries.Types.Data;
    using System.Collections.Generic;

    public class ReadableArticle : IComparable<ReadableArticle>
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ColorView> Colors { get; set; } = [];
        public List<Provider> Providers { get; set; } = [];
        public int CompareTo(ReadableArticle? other)
        {
            if (other == null)
                return 0;
            return Name.CompareTo(other.Name);
        }
    }
}
