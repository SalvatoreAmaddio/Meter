using Testing.Controller;
using Testing.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Testing.Model;
using Testing.RecordSource;
using Testing.Controller.Interfaces;
using Meter.Controller;
using System.Windows;
using Testing.View;

namespace Meter.Model {

    public class Invoice : AbstractModel, IDB<Invoice>, IRecordSource {
        Int64 _invoiceid;
        DateTime? _paymentdate;
        DateTime _topayon;
        double _amount;
        double _amountpaid;
        bool _fullypaid;

        OverdueTracker _overduetracker=null!;
        public OverdueTracker OverdueTracker 
        { 
           get => _overduetracker;
           set => Set<OverdueTracker>(ref value, ref _overduetracker, nameof(OverdueTracker)); 
        }
    

        Tenant _tenant = null!;
        public Tenant Tenant { get => _tenant; set => Set<Tenant>(ref value, ref _tenant, nameof(Tenant)); }

        public Int64 InvoiceID { get => _invoiceid; set => Set<Int64>(ref value, ref _invoiceid, nameof(InvoiceID)); }
        public DateTime? PaymentDate { 
            get => _paymentdate; 
            set => Set<DateTime?>(ref value, ref _paymentdate, nameof(PaymentDate)); 
        }

        public DateTime ToPayOn { get => _topayon; set => Set<DateTime>(ref value, ref _topayon, nameof(ToPayOn)); }

        public double Amount { get => _amount; set => Set<Double>(ref value, ref _amount, nameof(Amount)); } 
        
        public double AmountPaid { get => _amountpaid; set => Set<Double>(ref value, ref _amountpaid, nameof(AmountPaid)); }
        
        public bool FullyPaid { get => _fullypaid;set =>Set<bool>(ref value, ref _fullypaid, nameof(FullyPaid));}

        public double CurrentAmountDue { get => Amount + OverdueTracker.Overdue; }

        public double AmountDue2
        {
            get
            {
                var val = (Amount) - AmountPaid;
                return (val < 0) ? 0 : val;
            }
        }


        public double AmountDue 
        {
            get 
            { 
                var val = (Amount + OverdueTracker.Overdue) - AmountPaid;
                return (val < 0) ? 0 : val;
            }  
        }

        public bool LockAmountPaid { get => !FullyPaid; }

        public Visibility ShowAmountPaid { get => (AmountPaid > 0) ? Visibility.Visible : Visibility.Collapsed; }

        #region Constructors
        public Invoice() {
            PaymentDate = DateTime.Now;
            IsDirty = false;
            Tenant = new();
            ToPayOn = DateTime.Today;
            ToPayOn.AddMonths(1);
            AfterPropChangedEvt += Invoice_BeforePropChangedEvt;
            OverdueTracker = new();
        }

        public Invoice(RecordSource<Reading> values, Tenant tenant) : this()
        {
            Tenant = tenant;
            Calculate(values, tenant.TenantID);
            MainDB.InvoiceTable.Record = this;
            MainDB.InvoiceTable.Insert();
            OverdueTracker = new(this,true);
            RegisterReadings(values);
        }

        public Invoice(SQLiteDataReader reader)
        {
            InvoiceID = reader.GetInt64(0);
            PaymentDate = reader.GetDateTime(1);
            Amount = reader.GetDouble(2);
            AmountPaid = reader.GetDouble(3);
            FullyPaid = reader.GetBoolean(4);

            try
            {
                ToPayOn = reader.GetDateTime(6);
            }
            catch { }

            if (reader.FieldCount <= 7) return;

            Tenant = new();
            Tenant.TenantID = reader.GetInt32(7);
            Tenant.FirstName = reader.GetString(8);
            Tenant.LastName = reader.GetString(9);

            try
            {
                Tenant.DOB = reader.GetDateTime(10);
            }
            catch
            {

            }
            Tenant.PhoneNumber = reader.GetInt32(11);
            Tenant.Email = reader.GetString(12);
            Tenant.Notes = reader.GetString(13);

            IsDirty = false;
            OverdueTracker = new(this,false);
            AfterPropChangedEvt += Invoice_BeforePropChangedEvt;
        }
        #endregion

        public void SetUpOverdueTracker() =>OverdueTracker = new(this,true);
        
        private void Invoice_BeforePropChangedEvt(object? sender, NotifierArgs e)
        {
            if (e.PropIs(nameof(Amount)) || e.PropIs(nameof(AmountPaid))) {
                NotifyView(nameof(AmountDue));
                NotifyView(nameof(CurrentAmountDue));
            }

            if (e.PropIs(nameof(AmountPaid))) NotifyView(nameof(ShowAmountPaid));
        
            if (!e.PropIs(nameof(FullyPaid))) return;

            var ischecked = e.GetValue<bool>();
            AmountPaid = (ischecked) ? (AmountPaid+AmountDue) : 0;
            NotifyView(nameof(LockAmountPaid));
        }

        public void RegisterReadings(RecordSource<Reading> readings) {

            var newRecord = new InvoiceReading()
            {
                Invoice = this,
                Reading = readings[0]
            };

            MainDB.InvoiceReading.Record = newRecord;
            MainDB.InvoiceReading.Insert();

            newRecord = new InvoiceReading()
            {
                Invoice = this,
                Reading = readings[1]
            };

            MainDB.InvoiceReading.Record = newRecord;
            MainDB.InvoiceReading.Insert();
        }

        public Invoice GetRecord(SQLiteDataReader reader) => new(reader);

        #region QueryStrings
        public string DeleteSelect() => "DELETE FROM Invoice WHERE InvoiceID=@InvoiceID;";
        public string InsertSelect() => "INSERT INTO Invoice (PaymentDate,Amount,AmountPaid,FullyPaid,ToPayOn) VALUES(@PaymentDate,@Amount,@AmountPaid,@FullyPaid,@ToPayOn);";
        public virtual string SQLSelect() => "SELECT * FROM InvoiceList ORDER BY InvoiceID;";
        public string UpdateSelect() => "UPDATE Invoice SET PaymentDate=@PaymentDate, Amount=@Amount, AmountPaid=@AmountPaid, FullyPaid=@FullyPaid,ToPayOn=@ToPayOn WHERE InvoiceID=@InvoiceID;";

        public void Params(SQLiteParameterCollection param)
        {
            param.AddWithValue("@InvoiceID", InvoiceID);
            param.AddWithValue("@PaymentDate", PaymentDate);
            param.AddWithValue("@Amount", Amount);
            param.AddWithValue("@AmountPaid", AmountPaid);
            param.AddWithValue("@FullyPaid", FullyPaid);
            param.AddWithValue("@ToPayOn", ToPayOn);
        }

        #endregion

        public override bool Equals(object? obj)=>obj is Invoice invoice && InvoiceID == invoice.InvoiceID;
        
        public override int GetHashCode()=>HashCode.Combine(InvoiceID);
        
        public override string? ToString() => $"{InvoiceID}, {PaymentDate}, {Amount}, {AmountPaid}, {FullyPaid}";

        public override bool CanUpdate() =>!(PaymentDate == null);
        
        public string CompleteComboBoxSelectedItem()=>string.Empty;

        public virtual bool FilterRecordSource(IFilterList filter) {

            if (filter is IncomeFilter) {
                var fil = (IncomeFilter)filter;
                var datefrom = IncomeFilter.DateFrom<=PaymentDate; 
                var dateto = IncomeFilter.DateTo>= PaymentDate;

                return (fil.IsPaid) ? (AmountPaid > 0 && datefrom && dateto) : (FullyPaid is false && datefrom && dateto);
            }

            return Tenant.FullName.ToLower().Contains(filter.ToString().ToLower());
        }

        public virtual IEnumerable ConvertItemSourceToRecordSource(IEnumerable ItemSource) {
            try
            {
                var source = (RecordSource<IRecordSource>)ItemSource;
                source.ReplaceRecords(MainDB.InvoiceTable.RecordSource);
                return source;
            }
            catch {
                var source = (RecordSource<Invoice>)ItemSource;
                source.ReplaceRecords(MainDB.InvoiceTable.RecordSource);
                return source;
            }

        }

        public static void OpenInvoice(Invoice invoice) {
            invoice.Tenant.TenantAddress = TenantAddress.GetInvoicedAddress(invoice);
            InvoiceViewer invoiceViewer = new(
                                              new(
                                                  new(
                                                      MainDB.InvoiceReading.RecordSource.Where(s => s.Invoice.InvoiceID == invoice.InvoiceID)),
                                                      invoice));
            invoiceViewer.ShowDialog();
        }

        public void Calculate(RecordSource<Reading> values, Int64 tenantID) {
            Amount = ReadingsCalculator.CalcReadings(values[0].ReadingValue, values[1].ReadingValue);
            IsDirty = false;
        }

        public IEnumerable ReorderList(IEnumerable ItemSource)
        {
            var source = (RecordSource<Invoice>)ItemSource;
            MainDB.InvoiceTable.RecordSource.Order = Order.DESC;
            MainDB.InvoiceTable.RecordSource.ReorderOrderBy(s => s.PaymentDate);
            source.ReplaceRecords(MainDB.InvoiceTable.RecordSource,Crud.VOID);
            return source;
        }

        public void SetLastInsertedID() => InvoiceID = MainDB.InvoiceTable.LastInsertedID();

    }

    #region reading calculator
    public static class ReadingsCalculator
    {
        private static double FirstMeter { get; set; }
        private static double SecondMeter { get; set; }
        static double PricePerMeter = MainDB.PriceTable.RecordSource.Where(s => s.ID.Equals("PRICE PER METER")).FirstOrDefault().PriceValue;
        static double MasterMeter = MainDB.PriceTable.RecordSource.Where(s => s.ID.Equals("MASTER READING")).FirstOrDefault().PriceValue;

        private static double Formula()
        {
            var sub = Math.Abs(SecondMeter - FirstMeter);
            return (sub * PricePerMeter) + MasterMeter;
        }

        public static double CalcReadings(double val1, double val2)
        {
            FirstMeter = (val1 > val2) ? val2 : val1;
            SecondMeter = (val1 > val2) ? val1 : val2;
            return Formula();
        }

    }
    #endregion

    #region InvoiceStream
    public class InvoiceStream { 
        public Invoice? Invoice { get; set; }
        public DateTime Anterior { get; set; }
        public DateTime Actual { get; set; }
        public double Reading1 { get; set; }
        public double Reading2 { get; set; }
        public string Meter { get; }

        public double Consumo { get => Math.Abs(Reading1 - Reading2); }
        public double PricePerMeter { get; }
        public double MasterMeter { get; }

        public InvoiceStream(List<InvoiceReading> Readings, Invoice? invoice) {
            Invoice = invoice;
            Readings[0].Reading.TenantAddress = MainDB.TenantAddressTable.RecordSource.Where(s=>s.TenantAddressID == Readings[0].Reading.TenantAddress.TenantAddressID).FirstOrDefault();
            Meter =Readings[0].Reading.TenantAddress.Meter;
            OrderByGreater(Readings[0].Reading, Readings[1].Reading);
        }

        private void OrderByGreater(Reading reading1, Reading reading2) {

            if (reading1.ReadingValue > reading2.ReadingValue)
            {
                Reading1 = reading2.ReadingValue;
                Anterior = reading2.DOB;

                Reading2 = reading1.ReadingValue;
                Actual = reading1.DOB;
            }
            else {
                Reading1 = reading1.ReadingValue;
                Anterior = reading1.DOB;

                Reading2 = reading2.ReadingValue;
                Actual = reading2.DOB;
            }
        } 
    }

    #endregion
}

