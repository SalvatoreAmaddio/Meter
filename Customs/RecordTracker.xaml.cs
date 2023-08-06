using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

namespace Testing.Customs
{
    /// <summary>
    /// Interaction logic for RecordTracker.xaml
    /// </summary>
    public partial class RecordTracker : Border
    {

        public static DependencyProperty RecordTrackerProperty =
        DependencyProperty.Register(
        nameof(Tracker), typeof(Testing.Controller.RecordTracker), typeof(RecordTracker),
        new FrameworkPropertyMetadata()
        {
            DefaultValue = null,
            PropertyChangedCallback=RecordTrackerPropertyChanged
        }
        );

        private static void RecordTrackerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RecordTracker)d).Label.Content = e.NewValue?.ToString();
        }

        public Testing.Controller.RecordTracker Tracker
        {
            get => (Testing.Controller.RecordTracker)GetValue(RecordTrackerProperty);
            set => SetValue(RecordTrackerProperty, value);
        }

        public RecordTracker()
        {
            InitializeComponent();
        }
    }
}
