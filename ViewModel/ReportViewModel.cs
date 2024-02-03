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
    using OpenQA.Selenium.Internal;
    using OpenQA.Selenium.Support.UI;

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

        public ViewCollection<ArticleView> ArticleList
        { get => _articleList; set { _articleList = value; PropertyCall(); } }
        public ICommand ExcelOutputCommand { get; set; } = new FastCommand
            ((object parameter) => { ReportViewModel model = (ReportViewModel)parameter; model.ExcelOutputCommandHandler(); }, (object parameter) => { return true; });
        public ICommand UpdatePricesCommand { get; set; } = new FastCommand
            ((object parameter) => { ReportViewModel model = (ReportViewModel)parameter; model.UpdatePricesCommandHandler(); }, (object parameter) => { return true; });

        private void ExcelOutputCommandHandler()
        {
            //UpdatePricesCommandHandler();
            var excelIns = new ExcelFile();
            excelIns.WriteFile(ArticleList);
        }
        private void UpdatePricesCommandHandler()
        {
            //create chrome option
            ChromeOptions chromeOption = new();
            // add maximized parameter to the chromeoption instance
            chromeOption.AddArgument("--start-maximized");
            //create new driver for navigating through pages
            WebDriver chromeDrive = new ChromeDriver(chromeOption);
            //maximize windows
            chromeDrive.Manage().Window.Maximize();
            //minimize window
            chromeDrive.Manage().Window.Minimize();
            //create priceViewList
            var scrappedArticleList = new List<ScrappedAritcles>();
            //get a list of urls
            var urlList = _urlTable.List.ToList();
            //loop through urls
            foreach (URLType url in urlList)
            {
                //get article name based on url
                var articleName = url.GetArticleName();
                //create new intance of scrappedArticles
                ScrappedAritcles scrappedArticles = new()
                {
                    //add extracted name 
                    Name = articleName,
                };
                //call scrap function based on current url and store result 
                List<ArticleDetails> scrappedArticleDetailsList = url.Scrap(chromeDrive);
                //loop through scrapped items from web
                foreach (var scrappedArticleDetails in scrappedArticleDetailsList)
                {
                    //gather scrapped resoucres 
                    scrappedArticles.ArticleDetails.Add(scrappedArticleDetails);
                }
                //add object to the list 
                scrappedArticleList.Add(scrappedArticles);
            }
            _finalList.Clear();
            //cast scapped Article List to a readable object 
            foreach (var scrappedItem in scrappedArticleList)
            {
                AddCleanValue(scrappedItem);
            }
            //asign castedList
            ArticleList = _finalList.ToList();
            _finalList.Clear();
            _finalList = [];
            chromeDrive.Quit();
        }
        private void AddCleanValue(ScrappedAritcles item)
        {
            //**passed item is a type of scrappedArticles**\\
            //store article name 
            string articleName = item.Name;
            //loop through ArticleDetails 
            foreach (var details in item.ArticleDetails)
            {
                //add new ArticleView object and add stored article name and founded color inside loop
                ArticleView newArticleView = new ArticleView()
                {
                    ArticleColor = details.Color,
                    ArticleName = articleName,
                };


                ProviderView newProviderView;
                //check if provider object is not null
                if (details.Provider == null)
                {
                    //create new ProviderView Object inside loop
                    newProviderView = new ProviderView()
                    {
                        //set the providerview object name to scrappedArticle provider
                        ProviderName = string.Empty,
                    };
                }
                else
                {
                    //create new ProviderView Object inside loop
                    newProviderView = new ProviderView()
                    {
                        //set the providerview object name to scrappedArticle provider
                        ProviderName = details.Provider.Name,
                    };
                }
                //add scrapped tags to the providerview object 
                foreach (var providerTag in details.Tags)
                {
                    newProviderView.ScrappedData.Add(providerTag);
                }
                //check if article with the specified name and color exist in list
                var searchResult = _finalList.FirstOrDefault(x => x.ArticleName == newArticleView.ArticleName && x.ArticleColor == newArticleView.ArticleColor);
                if (searchResult == null)
                {
                    //add new specified provider to new Created Article 
                    newArticleView.Providers.Add(newProviderView);
                    _finalList.Add(newArticleView);
                }
                else
                {
                    //Find Index of New Item
                    int itemIndex = _finalList.IndexOf(searchResult);
                    //Add New Specified Provider To Existing Article
                    _finalList[itemIndex].Providers.Add(newProviderView);
                }
            }
        }
        private List<ArticleView> _finalList = [];
        private readonly XMLDataBase _dataBase;
        private readonly XMLTable<Article> _articleTable;
        private readonly XMLTable<Provider> _providerTable;
        private readonly XMLTable<URLType> _urlTable;
        private ViewCollection<ArticleView> _articleList;
    }
}