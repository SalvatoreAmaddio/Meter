using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Testing.Customs;

namespace Testing.Report
{
    abstract public class PaperBase : Canvas
    {
        public event EventHandler<MyArgs> GrowEvent;

        public Bordo MainBody { get; set; }
        public Border HeaderFrame { get; set; }
        public Border FooterFrame { get; set; }

        public PaperBase()
        {
            NameScope.SetNameScope(this, new NameScope());
            CreateHeader();
            CreateBody();
            CreateFooter();
            MainBody.Paper = this;
            PageNumber = 1;
        }

        private void CreateFooter()
        {
            FooterFrame = new();
            FooterFrame.Width = 797;
            FooterFrame.Name = nameof(FooterFrame);
            RegisterName(FooterFrame.Name, FooterFrame);
            FooterTop = 1079;
            Children.Add(FooterFrame);
        }

        private void CreateHeader()
        {
            HeaderFrame = new();
            HeaderFrame.Width = 797;
            HeaderFrame.Name = nameof(HeaderFrame);
            RegisterName(HeaderFrame.Name, HeaderFrame);
            SetTop(HeaderFrame, -1);
            Children.Add(HeaderFrame);
        }

        private void CreateBody()
        {
            MainBody = new();
            //MainBody.BorderBrush = Brushes.Red;
            //MainBody.BorderThickness = new Thickness(1);
            MainBody.Width = 796;
            MainBody.Name = nameof(MainBody);
            RegisterName(MainBody.Name, MainBody);

            Binding b = new("ActualHeight");
            b.ElementName = nameof(HeaderFrame);
            BindingOperations.SetBinding(this, BodyTopProperty, b);

            BindingOperations.SetBinding(MainBody, Bordo.HeaderHeightProperty, b);

            Children.Add(MainBody);
        }

        #region footertop
        public static readonly DependencyProperty FooterTopProperty = DependencyProperty.Register(
        nameof(FooterTop),
        typeof(double),
        typeof(PaperBase),
        new PropertyMetadata() { PropertyChangedCallback = FooterTopPropertyChanged });

        private static void FooterTopPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PaperBase)d;
            SetTop(control.FooterFrame, (double)e.NewValue);
            control.MainBody.FooterTop = (double)e.NewValue;
        }

        public double FooterTop
        {
            get { return (double)GetValue(FooterTopProperty); }
            set { SetValue(FooterTopProperty, value); }
        }
        #endregion

        #region BodyTop
        public static readonly DependencyProperty BodyTopProperty = DependencyProperty.Register(
        nameof(BodyTop),
        typeof(double),
        typeof(PaperBase),
        new PropertyMetadata() { PropertyChangedCallback = BodyTopPropertyChanged });


        private static void BodyTopPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PaperBase)d;
            SetTop(control.MainBody, (double)e.NewValue);
        }

        public double BodyTop
        {
            get { return (double)GetValue(BodyTopProperty); }
            set { SetValue(BodyTopProperty, value); }
        }
        #endregion



        public bool Grow
        {
            set
            {
                MyArgs args = new(PagNum, BodyChild, this, MainBody, HeaderFrame,FooterFrame);
                GrowEvent?.Invoke(this, args);
            }
        }

        #region PageNum
        public static readonly DependencyProperty PageNumberProperty =
        DependencyProperty.Register(
        nameof(PageNumber), typeof(object), typeof(PaperBase),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = null,
        }
        );

        public object PageNumber
        {
            get => GetValue(PageNumberProperty);
            set => SetValue(PageNumberProperty, "Page: " + value);
        }

        public int PagNum { get => int.Parse(PageNumber?.ToString().Split(" ")[1]); }
        #endregion

        #region MainBody
        public static readonly DependencyProperty BodyChildProperty = DependencyProperty.Register(
           nameof(BodyChild),
           typeof(UIElement),
           typeof(PaperBase),
           new PropertyMetadata()
           {
               PropertyChangedCallback = BodyChildPropertyChanged
           });


        private static void BodyChildPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var a = (PaperBase)d;
            a.MainBody.Child = (UIElement)e.NewValue;
        }

        public UIElement BodyChild
        {
            get { return (UIElement)GetValue(BodyChildProperty); }
            set { SetValue(BodyChildProperty, value); }
        }

        #endregion

        #region Header
        public static readonly DependencyProperty HeaderChildProperty = DependencyProperty.Register(
           nameof(HeaderChild),
           typeof(UIElement),
           typeof(PaperBase),
           new PropertyMetadata()
           {
               PropertyChangedCallback = HeaderChildPropertyChanged
           });


        private static void HeaderChildPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var a = (PaperBase)d;
            a.HeaderFrame.Child = (UIElement)e.NewValue;
        }

        public UIElement HeaderChild
        {
            get { return (UIElement)GetValue(HeaderChildProperty); }
            set { SetValue(HeaderChildProperty, value); }
        }
        #endregion

        #region Footer
        public static readonly DependencyProperty FooterChildProperty = DependencyProperty.Register(
   nameof(FooterChild),
   typeof(UIElement),
   typeof(PaperBase),
   new PropertyMetadata()
   {
       PropertyChangedCallback = FooterChildPropertyChanged
   });


        private static void FooterChildPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var a = (PaperBase)d;
            a.FooterFrame.Child = (UIElement)e.NewValue;
        }

        public UIElement FooterChild
        {
            get { return (UIElement)GetValue(FooterChildProperty); }
            set { SetValue(FooterChildProperty, value); }
        }
        #endregion

        abstract public PaperBase ReturnNew();

    }

    public class MyArgs : EventArgs
    {
        private PaperBase Paper { get; }
        public int PageNumber { get; }
        public UIElement BodyChild { get; }
        public Bordo Body { get; }
        public Border Header { get; }
        public Border Footer { get; }
        public double FooterTop { get => Body.FooterTop; }
        public PaperBase NewPaper { get => Paper.ReturnNew(); }
        
        public bool BodyChildIsStackPanel { get => BodyChild is StackPanel; }
        public bool BodyChildIsGrid { get => BodyChild is Grid; }

        public Grid GetBodyChildAsGrid { get => (Grid)BodyChild; }

        private double ExceededHeight { get; set; }

        public bool HasContentToMove { get=> ExceededHeight > FooterTop; }

        public MyArgs(int pageNumber, UIElement bodyChild, 
                                      PaperBase paper, 
                      Bordo body, Border header, Border footer)
        {
            PageNumber = pageNumber;
            BodyChild = bodyChild;
            Paper = paper;
            Body = body;
            Header = header;
            Footer = footer;    
        }

        public void UpdateLayouts() {
            Header.UpdateLayout();
            Body.UpdateLayout();
            Footer.UpdateLayout();
            ExceededHeight=Body.ActualHeight + Header.ActualHeight;
        }

    }
}