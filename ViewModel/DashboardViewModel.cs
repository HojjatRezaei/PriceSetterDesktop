namespace PriceSetterDesktop.ViewModel
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Data;
    using PriceSetterDesktop.Libraries.Types.Interaction;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using WPFCollection.Data.Statics;
    using WPFCollection.Data.Types;
    using WPFCollection.Data.Types.Generic;
    using WPFCollection.Style.Base;

    public class DashboardViewModel : BasePage
    {
        public DashboardViewModel()
        {
            var db = DataHolder.XMLData.CreateDataBase(DataHolder.XMLDataBaseName);
            db.CreateTable<Article>(nameof(Article));
            db.CreateTable<ContainerXPath>(nameof(ContainerXPath));
            db.CreateTable<Prices>(nameof(Prices));
            db.CreateTable<Provider>(nameof(Provider));
            db.CreateTable<URLType>(nameof(URLType));
            db.CreateTable<XPathItem>(nameof(XPathItem));
            CurrentContent = new ArticleViewModel();
            var hc = new HttpClient();
            //add data
            var generator = new Generator<ArticleView>();
            ArticleView articleView = generator.GenerateFromScrath();
            //var jsonType = JsonConvert.SerializeObject(articleView);
            if (articleView != null)
            {
                //JObject ojsonObject = new()
                //{
                //    { "ArticleName", "ایفون 13 نرمال " },
                //    { "ArticleColor", "مشکی" },
                //    { "ProviderName", "دیجیکالا" },
                //    { "AnnouncedPrice", "401000000" },
                //};
                //var content = new StringContent(ojsonObject.ToString(), Encoding.UTF8, "application/json");
                //var task = hc.PostAsync("https://vetos-mobile.com/hojjatDebugTest/", content);
                //task.Wait();
                var res = hc.GetAsync("https://vetos-mobile.com/hojjatDebugTest/").Result;
                res.EnsureSuccessStatusCode();
                string message = res.Content.ReadAsStringAsync().Result;
                string parsedString = Regex.Unescape(message);
                byte[] isoBites = Encoding.UTF8.GetBytes(parsedString);
                var finalRes =  Encoding.UTF8.GetString(isoBites, 0, isoBites.Length);
                var jsonObject = JObject.Parse(finalRes);
            }
        }
        private object _currentContent;
        
        public object CurrentContent
        {
            get { return _currentContent; }
            set { _currentContent = value; PropertyCall(); }
        }


        public ICommand GotoReportPage { get; set; } = new FastCommand
            ((object parameter) => { DashboardViewModel model = (DashboardViewModel)parameter; model.GotoReportPageHandler(); }, (object parameter) => { return true; });
        public ICommand GotoCreateArticlePage { get; set; } = new FastCommand
            ((object parameter) => { DashboardViewModel model = (DashboardViewModel)parameter; model.GotoCreateArticlePageHandler(); }, (object parameter) => { return true; });

        private void GotoReportPageHandler()
        {
            CurrentContent = new ReportViewModel();
        }
        private void GotoCreateArticlePageHandler()
        {
            CurrentContent = new ArticleViewModel();
        }
    }
}
