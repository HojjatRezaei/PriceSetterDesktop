namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    public class ProviderView
    {
        public string ProviderName { get; set; } = string.Empty;
        public List<TagView> ScrappedData { get; set; } = [];
    }
}
