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
    using OpenQA.Selenium.Support.UI;
    using WPFCollection.Network.Error;

    public class ReportViewModel : BaseControl
    {
        public ReportViewModel()
        {
            _dataBase = DataHolder.XMLData.GetDataBase(DataHolder.XMLDataBaseName);
            _articleTable = _dataBase.GetTable<Article>(nameof(Article));
            _providerTable = _dataBase.GetTable<Provider>(nameof(Provider));
            _urlTypeTable = _dataBase.GetTable<URLType>(nameof(URLType));
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

        public ViewCollection<CollectionView> PriceList
        { get => _priceList; set { _priceList = value; PropertyCall(); } }
        public ICommand ExcelOutputCommand { get; set; } = new FastCommand
            ((object parameter) => { ReportViewModel model = (ReportViewModel)parameter; model.ExcelOutputCommandHandler(); }, (object parameter) => { return true; });
        public ICommand UpdatePricesCommand { get; set; } = new FastCommand
            ((object parameter) => { ReportViewModel model = (ReportViewModel)parameter; model.UpdatePricesCommandHandler(); }, (object parameter) => { return true; });
        public ICommand RetryScrap { get; set; } = new FastCommand
            ((object parameter) => 
            {
                object[] pars = (object[])parameter;
                ReportViewModel model = (ReportViewModel)pars[0];
                var prop = pars[1].GetType().GetProperty("DataContext");
                model.RetryScrapHandler((ProviderView)pars[1] , model);
            }, (object parameter) => { return true; });

        private void RetryScrapHandler(ProviderView provider , ReportViewModel vm)
        {
            //import pricelist to local variable
            List<CollectionView> importedList = PriceList.ToList();
            //search for editable object in PriceList
            var collectionViewSearchResult = PriceList.FirstOrDefault(x => x.ArticleID == provider.ArticleID);
            if(collectionViewSearchResult != null)
            {
                var collectionViewIndex = PriceList.IndexOf(collectionViewSearchResult);
                //search for provider object in articleSearchResult ProviderList
                var providerSearchResult = collectionViewSearchResult.Providers.FirstOrDefault(y => y.ProviderID == provider.ProviderID);
                if(providerSearchResult != null)
                {
                    var providerIndex = collectionViewSearchResult.Providers.IndexOf(providerSearchResult);
                    //try to scrap again
                    var newScrapedData = CreateProvider(provider.URLTypeInstance);
                    if (newScrapedData.Equals(provider))
                    {
                        //open dialog box
                    }
                    else
                    {
                        importedList[collectionViewIndex].Providers[providerIndex] = newScrapedData;
                        PriceList = importedList;
                    }

                }
            }
        }
        private void ExcelOutputCommandHandler()
        {

        }
        
        private void UpdatePricesCommandHandler()
        {
            ChromeOptions chromeOption = new();
            chromeOption.AddArgument("--start-maximized");
            WebDriver chromeDrive = new ChromeDriver(chromeOption);
            //get a list of articles
            List<CollectionView> priceList = _articleTable.List.Select((x) =>
            {
                return new CollectionView()
                {
                    ArticleName = x.Name,
                    ArticleID = x.ElementSeed,
                    Providers = _urlTypeTable.List.Where(y => y.ArticleID == x.ElementSeed).Select((i) => 
                    {
                        return CreateProvider(i, chromeDrive);
                    }).ToList(),
                };
            }).ToList();
            PriceList = priceList;
            chromeDrive.Quit();
        }
        private ProviderView CreateProvider(URLType urlObject , WebDriver? drive = null)
        {
            WebDriver chromeDrive;
            if(drive == null)
            {
                ChromeOptions chromeOption = new();
                chromeOption.AddArgument("--start-maximized");
                chromeDrive = new ChromeDriver(chromeOption);
            }
            else
            {
                chromeDrive = drive;
            }
            var scrapResult = urlObject.GetPriceFromWeb(chromeDrive);
            bool parsed = double.TryParse(scrapResult, out double priceResult);
            if (drive == null)
            {
                AllocConsole();
                Console.WriteLine("Enter New Value");
                var res = Console.ReadLine();
                WaitForChromeToClose(chromeDrive);
                chromeDrive.Quit();
            }
            return new ProviderView()
            {
                ProviderName = urlObject.GetProviderName(),
                ProviderPrice = parsed ? priceResult.ToString("#,##0") : "can't parse value",
                IsValid = !parsed,
                ErrorText = scrapResult,
                ProviderID = urlObject.ElementSeed,
                ArticleID = urlObject.ArticleID
            };

        }
        private void WaitForChromeToClose(WebDriver driver)
        {
            WebDriverWait wait = new(driver, new(0, 5, 0));
            IJavaScriptExecutor scripter = driver;
            try
            {
                _ = wait.Until(x => scripter.ExecuteScript($"return document.documentElement,document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue", null) == null);
            }
            catch (Exception e)
            {
                ErrorManager.SendError(e);
                return;
            }

        }
        private ViewCollection<CollectionView> _priceList;
        private readonly XMLDataBase _dataBase;
        private readonly XMLTable<Article> _articleTable;
        private readonly XMLTable<Provider> _providerTable;
        private readonly XMLTable<URLType> _urlTypeTable;
        public class CollectionView
        {
            public int ArticleID { get; set; } = -1;
            public string ArticleName { get; set; } = "";
            public List<ProviderView> Providers { get; set; } = [];
        }
        public class ProviderView
        {
            public string ProviderName { get; set; } = "";
            public string ProviderPrice { get; set; } = "";
            public bool IsValid { get; set; } = false;
            public string ErrorText { get; set; } = "";
            public int ProviderID { get; set; } = new();
            public int ArticleID { get; set; } = new();
            public URLType URLTypeInstance { get; set; } = new();

        }
    }
}
