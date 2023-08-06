using System;
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
    /// Interaction logic for TitledBorder.xaml
    /// </summary>
    public partial class TitledBorder : Border {
        public Label TitleLabel { get; set; }

        #region TitleProperty
        public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(TitledBorder),
        new FrameworkPropertyMetadata() { PropertyChangedCallback=TitlePropertyChanged});

        private static void TitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TitledBorder)d;
            if (control.TitleLabel == null) return;
            control.TitleLabel.Content = e.NewValue.ToString();
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
        #endregion

        public TitledBorder()
        {
            InitializeComponent();
        }

        public override UIElement Child 
        { 
            get => base.Child;
            set => base.Child = SettingChild(value);
        }

        private Grid SettingChild(UIElement child) {
            TitleLabel = new();
            TitleLabel.Content = Title;
            TitleLabel.HorizontalAlignment = HorizontalAlignment.Center;
            TitleLabel.VerticalAlignment = VerticalAlignment.Top;
            TitleLabel.Background = Brushes.White;

            if (child is DockPanel)
            {
                TitleLabel.Margin = new Thickness(0, -15, 0, 0);
            }

            if (child is Grid) {
                TitleLabel.Margin = new Thickness(0, -20, 0, 0);
            }

            TitleLabel.FontStyle = FontStyles.Italic;
            
            Grid grid = new();
            grid.RowDefinitions.Add(new());
            grid.Margin = new Thickness(0);
            
            Grid.SetRow(TitleLabel, 0);
            Grid.SetRow(child, 0);

            grid.Children.Add(TitleLabel);
            grid.Children.Add(child);
            return grid;
        }
    }
}
