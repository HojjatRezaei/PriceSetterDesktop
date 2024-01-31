namespace PriceSetterDesktop.ViewModel
{
    using PriceSetterDesktop.Libraries.Types;
    using System.Windows.Input;
    using WPFCollection.Data.Types.Generic;
    using WPFCollection.Data.Types;
    using WPFCollection.Style.Base;
    using PriceSetterDesktop.Libraries.Statics;
    using WPFCollection.Data.List;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium;
    using PriceSetterDesktop.Libraries.Types.Data;
    using PriceSetterDesktop.Libraries.Types.Interaction;
    using WPFCollection.Data.Statics;

    public class ReportViewModel : BaseControl
    {
        public ReportViewModel()
        {
            _dataBase = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            _articleTable = _dataBase.GetTable<Article>(nameof(Article));
            _providerTable = _dataBase.GetTable<Provider>(nameof(Provider));
            _urlTable = _dataBase.GetTable<URLType>(nameof(URLType));
            //create visual elements for debugging
            //#if DEBUG
            //            PriceList =
            //            [
            //                new CollectionView() 
            //                {
            //                    ArticleName = "گوشی ایفون مدل اپل برای تست",
            //                    Providers =
            //                    [
            //                        new ProviderView()
            //                        {
            //                            ErrorText = "مشکل در پیدا کردن قیمت",
            //                            IsValid = true,
            //                            ProviderName = "دیجی کالا",
            //                            ProviderPrice = "مشکل در پیدا کردن قیمت",
            //                        },
            //                        new ProviderView()
            //                        {
            //                            ErrorText = "این یه ارور نیست",
            //                            IsValid = false,
            //                            ProviderName = "ایرانسل",
            //                            ProviderPrice = "34,555,999"
            //                        },
            //                    ]
            //                },
            //                new CollectionView()
            //                {
            //                    ArticleName = "گوشی ایفون مدل اپل برای تست2",
            //                    Providers =
            //                    [
            //                        new ProviderView()
            //                        {
            //                            ErrorText = "مشکل در پیدا کردن قیمت",
            //                            IsValid = true,
            //                            ProviderName = "دیجی کالا",
            //                            ProviderPrice = "مشکل در پیدا کردن قیمت",
            //                        },
            //                        new ProviderView()
            //                        {
            //                            ErrorText = "این یه ارور نیست",
            //                            IsValid = false,
            //                            ProviderName = "ایرانسل",
            //                            ProviderPrice = "34,555,999"
            //                        },
            //                    ]
            //                },
            //                new CollectionView()
            //                {
            //                    ArticleName = "گوشی ایفون مدل اپل برای تست3",
            //                    Providers =
            //                    [
            //                        new ProviderView()
            //                        {
            //                            ErrorText = "مشکل در پیدا کردن قیمت",
            //                            IsValid = true,
            //                            ProviderName = "دیجی کالا",
            //                            ProviderPrice = "مشکل در پیدا کردن قیمت",
            //                        },
            //                        new ProviderView()
            //                        {
            //                            ErrorText = "این یه ارور نیست",
            //                            IsValid = false,
            //                            ProviderName = "ایرانسل",
            //                            ProviderPrice = "34,555,999"
            //                        },
            //                    ]
            //                },
            //            ];
            //#endif
        }

        private ViewCollection<ArticleView> _articleList;
        public ViewCollection<ArticleView> ArticleList
        { get => _articleList; set { _articleList = value; PropertyCall(); } }
        public ICommand ExcelOutputCommand { get; set; } = new FastCommand
            ((object parameter) => { ReportViewModel model = (ReportViewModel)parameter; model.ExcelOutputCommandHandler(); }, (object parameter) => { return true; });
        public ICommand UpdatePricesCommand { get; set; } = new FastCommand
            ((object parameter) => { ReportViewModel model = (ReportViewModel)parameter; model.UpdatePricesCommandHandler(); }, (object parameter) => { return true; });

        private void ExcelOutputCommandHandler()
        {
            
            //UpdatePricesCommandHandler();
            //ExcelFile.WriteFile(new() { new(ArticleList , "newExtractedData") });
        }
        private void UpdatePricesCommandHandler()
        {
            ChromeOptions chromeOption = new();
            chromeOption.AddArgument("--start-maximized");
            WebDriver chromeDrive = new ChromeDriver(chromeOption);
            chromeDrive.Manage().Window.Minimize();
            //create priceViewList
            var articleViewList = new List<ArticleView>();
            //get a list of urls
            var urlList = _urlTable.List.ToList();
            foreach (URLType url in urlList)
            {
                var articleName = url.GetArticleName();
                ArticleView articleView = new()
                {
                    Name = articleName,
                };
                List<ArticleDetails> scrappedArticleViewList = url.Scrap(chromeDrive);
                foreach (var scrappedItem in scrappedArticleViewList)
                {
                    articleView.ArticleDetails.Add(scrappedItem);
                }
                articleViewList.Add(articleView);
            }
            ArticleList = articleViewList;
            chromeDrive.Quit();
        }

        private readonly XMLDataBase _dataBase;
        private readonly XMLTable<Article> _articleTable;
        private readonly XMLTable<Provider> _providerTable;
        private readonly XMLTable<URLType> _urlTable;
    }
}