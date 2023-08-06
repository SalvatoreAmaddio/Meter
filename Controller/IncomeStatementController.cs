using Meter.Model;
using MvvmHelpers.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Testing.Controller;
using Testing.Controller.Interfaces;
using Testing.DB;
using Testing.Model;
using Testing.RecordSource;
using Testing.View;

namespace Meter.Controller
{
    public class IncomeStatementController : AbstractNotifier
    {
        public RecordSource<Invoice> IncomeIn { get; set; } = new();
        public ICommand OpenInvoiceCMD { get; }
        public ICommand OpenRecordCMD { get; }
        public IncomeFilter IncomeFilterEarnings { get=>_incomeFilterEarning; set=>Set<IncomeFilter>(ref value,ref  _incomeFilterEarning,nameof(IncomeFilterEarnings)); }
        public IncomeFilter IncomeFilterOutstanding { get => _incomeFilterOutstanding; set=> Set<IncomeFilter>(ref value, ref _incomeFilterOutstanding, nameof(IncomeFilterOutstanding)); }

        IncomeFilter _incomeFilterEarning=null!;
        IncomeFilter _incomeFilterOutstanding=null!;
        double _totalearnings;
        double _totaloutstanding;

        public double TotalEarning { get => _totalearnings; set => Set<double>(ref value,ref _totalearnings,nameof(TotalEarning)); }
        public double TotalOutstanding { get => _totaloutstanding; set => Set<double>(ref value, ref _totaloutstanding, nameof(TotalOutstanding)); }

        DateTime _datefrom;
        DateTime _dateto;

        public DateTime DateFrom { get => _datefrom; set => Set<DateTime>(ref value, ref _datefrom, nameof(DateFrom)); }
        public DateTime DateTo { get => _dateto; set => Set<DateTime>(ref value, ref _dateto, nameof(DateTo)); }


        public IncomeStatementController() {
            DateFrom=new DateTime(DateTime.Today.Year,1,1);
            DateTo= new DateTime(DateTime.Today.Year, 12, 31);
            IncomeFilter.DateFrom = DateFrom;
            IncomeFilter.DateTo = DateTo;
            OpenInvoiceCMD = new Command<Invoice>(OpenInvoice);
            OpenRecordCMD = new Command<Invoice>(OpenRecord);
            AfterPropChangedEvt += IncomeStatementController_AfterPropChangedEvt;
            Refresh();  
        }

        private void IncomeStatementController_AfterPropChangedEvt(object? sender, NotifierArgs e)
        {
            if (e.PropIs(nameof(DateFrom)) || e.PropIs(nameof(DateTo))) {
                IncomeFilter.DateFrom = DateFrom;
                IncomeFilter.DateTo = DateTo;
                IncomeFilterEarnings = new(true);
                IncomeFilterOutstanding = new(false);
            }
        }

        private void OpenInvoice(Invoice invoice)=>Invoice.OpenInvoice(invoice);

        private void OpenRecord(Invoice invoice)
        {
            InvoiceForm invoiceForm = new(invoice);
            invoiceForm.ShowDialog();
        }

        public void CountTotals(IEnumerable Source,bool IsDue) {
            var s = Source.Cast<Invoice>();
                switch (IsDue) { 
                    case false:
                        TotalEarning = s.Sum(s=>s.AmountPaid);
                    break;
                    case true:
                        TotalOutstanding = s.Sum(s => s.AmountDue2);
                    break;
                }
        }

        public void Refresh()
        {
            IncomeFilterEarnings = new(true);
            IncomeFilterOutstanding = new(false);
            IncomeIn.ReplaceRecords(MainDB.InvoiceTable.RecordSource);
        }
    }


    #region IncomeFilter
    public class IncomeFilter : IFilterList
    {
        public string? Search { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IRecordSource Obj { get ; set; }
        public bool IsPaid { get; set; } = false;
        public static DateTime DateFrom { get; set; }
        public static DateTime DateTo { get; set; }

        public IncomeFilter(bool isPaid)
        {
            Obj = new Invoice();
            IsPaid = isPaid;
        }

        public override string? ToString()
        {
            return IsPaid.ToString();
        }
    }
    #endregion
}
