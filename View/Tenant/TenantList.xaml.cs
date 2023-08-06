using Meter.Controller;
using System.Windows.Controls;

namespace Meter.View
{
    public partial class TenantList : Page
    {
        public TenantListController? Controller { get; }

        public TenantList()
        {
            InitializeComponent();
            Controller = (TenantListController)DataContext;
        }

    }
}
