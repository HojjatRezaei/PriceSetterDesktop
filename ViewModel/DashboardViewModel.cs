﻿namespace PriceSetterDesktop.ViewModel
{
    using PriceSetterDesktop.Libraries.Statics;
    using PriceSetterDesktop.Libraries.Types.Data;
    using PriceSetterDesktop.Libraries.Types.Interaction;
    using System.Net.Http.Headers;
    using System.Windows.Input;
    using WPFCollection.Data.Statics;
    using WPFCollection.Data.Types;
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
