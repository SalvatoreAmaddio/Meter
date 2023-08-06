using Meter.Controller;
using Meter.Model;
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
using Testing.DB;

namespace Meter.View
{
    /// <summary>
    /// Interaction logic for IncomeStatement.xaml
    /// </summary>
    public partial class IncomeStatement : Page {
        public IncomeStatementController? Controller { get; }

        public IncomeStatement()
        {
            InitializeComponent();
            Controller = (IncomeStatementController)DataContext;
            Controller.CountTotals(EarningsList.ItemsSource,false);
            Controller.CountTotals(OutstandingList.ItemsSource,true);
        }

        private void ListEarning_OnRecordSourceChanged(object sender, Testing.Customs.OnRecordSourceChangedEvtArgs e) =>
            Controller?.CountTotals(e.Source,false);

        private void ListOutstanding_OnRecordSourceChanged(object sender, Testing.Customs.OnRecordSourceChangedEvtArgs e)=>
                        Controller?.CountTotals(e.Source, true);

    }
}
