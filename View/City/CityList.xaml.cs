using Meter.Controller;
using System.Windows.Controls;

namespace Meter.View
{
    /// <summary>
    /// Interaction logic for CityList.xaml
    /// </summary>
    public partial class CityList : Page
    {
        public CityListController Controller { get; set; }

        public CityList()
        {
            InitializeComponent();
            Controller = (CityListController) DataContext;  
        }
    }
}
