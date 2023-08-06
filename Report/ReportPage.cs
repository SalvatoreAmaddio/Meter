using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Printing;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Testing.Customs;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Testing.Report
{
    public class ReportPage {

        private PageContent PageContent;
        private FixedPage FixedPage;
        private Size sz;
        public PaperBase Paper;

        public ReportPage(PaperBase element)
        {
            Paper = element;
            PageContent = new();
            FixedPage = new();
            sz = new Size(96 * 8.3, 96 * 11.7);

            PageContent.Width = sz.Width;
            PageContent.Height = sz.Height;

            FixedPage.Children.Add((UIElement)element);
            FixedPage.Width = sz.Width;
            FixedPage.Height = sz.Height;
            PageContent.Child = FixedPage;
            Paper.Width = sz.Width;

            FixedPage.InvalidateArrange();
            FixedPage.UpdateLayout();
            FixedPage.Measure(sz);
            FixedPage.Arrange(new Rect(new Point(), sz));
        }

        public ReportPage(PaperBase element, int PageNum) : this(element)
        {
            PageNumber = PageNum;
        }

        public double HeaderHeight { get => Paper.HeaderFrame.ActualHeight; }
        public double FooterHeight { get => Paper.FooterFrame.ActualHeight; }
        public double MainBodyHeight { get => Paper.MainBody.ActualHeight; }
        public double ContentHeight { get => Paper.ActualHeight; }
        public double PageHeight { get => FixedPage.ActualHeight; }

        public PageContent GetPage() => PageContent;

        public override string? ToString() => $"Header Height: {HeaderHeight}, Footer Height: {FooterHeight}, Main Body: {MainBodyHeight}, Content: {ContentHeight}, Fixed Page: {PageHeight}";
        
        public int PageNumber { get=>Paper.PagNum; set=>Paper.PageNumber=value; }

        public bool ShouldGrow { get => Paper.MainBody.MustGrow; }
    }
}
