using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Testing.Customs;
using Testing.Report;

namespace Testing.Customs
{
    /// <summary>
    /// Interaction logic for Bordo.xaml
    /// </summary>
    public partial class Bordo : Border {

        public PaperBase Paper { get; set; }

        #region HeightHasExceeded
        public static readonly DependencyProperty HeightHasExceededProperty =
        DependencyProperty.Register(
        nameof(HeightHasExceeded), typeof(double), typeof(Bordo),
        new FrameworkPropertyMetadata()
        {
        PropertyChangedCallback = HeightHasExceededPropertyChanged
        }
        );

        private static void HeightHasExceededPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var control = (Bordo)d;
            var body = (double)e.NewValue;
            if (body> control.FooterTop) control.Paper.Grow = true;             
        }

        public double HeightHasExceeded
        {
            get => (double)GetValue(HeightHasExceededProperty);
            set => SetValue(HeightHasExceededProperty, value);
        }
        #endregion

        #region HeaderHeight
        public static readonly DependencyProperty HeaderHeightProperty =
        DependencyProperty.Register(
        nameof(HeaderHeight), typeof(double), typeof(Bordo),
        new FrameworkPropertyMetadata() {PropertyChangedCallback = HeaderHeightPropertyChanged});

        private static void HeaderHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Bordo)d).HeightHasExceeded += (double)e.NewValue;
        }

        public double HeaderHeight
        {
            get => (double)GetValue(HeaderHeightProperty);
            set => SetValue(HeaderHeightProperty, value);
        }
        #endregion

        #region FooterTop
        public static readonly DependencyProperty FooterTopProperty =
        DependencyProperty.Register(nameof(FooterTop), typeof(double), typeof(Bordo),
        new FrameworkPropertyMetadata() { PropertyChangedCallback = FooterTopPropertyChanged });

        private static void FooterTopPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var control = (Bordo)d;
            //var x=(double)e.NewValue;
        }

        public double FooterTop
        {
            get => (double)GetValue(FooterTopProperty);
            set => SetValue(FooterTopProperty, value);
        }

        #endregion

        #region Grow
        public static readonly DependencyProperty MustGrowProperty =
        DependencyProperty.Register(
        nameof(MustGrow), typeof(bool), typeof(Bordo),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = false,
        }
        );

        public bool MustGrow
        {
            get { return (bool)GetValue(MustGrowProperty); }
            set { SetValue(MustGrowProperty, value); }
        }
        #endregion

        public Bordo()
        {
            InitializeComponent();             
        }
    }
}
