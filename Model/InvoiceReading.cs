using Meter.Controller;
using System;
using System.Collections;
using System.Data.SQLite;
using Testing.Controller.Interfaces;
using Testing.DB;
using Testing.Model;
using Testing.RecordSource;

namespace Meter.Model
{
    public class InvoiceReading : AbstractModel, IDB<InvoiceReading>, IRecordSource {

        Int64 _invoicereadingid;
        public Int64 InvoiceReadingID { get => _invoicereadingid; set => Set<Int64>(ref value, ref _invoicereadingid, nameof(InvoiceReadingID)); }

        Invoice? _invoice;
        public Invoice? Invoice { get => _invoice; set => Set<Invoice?>(ref value, ref _invoice,nameof(Invoice)); }

        Reading? _reading;
        public Reading? Reading { get => _reading; set => Set<Reading?>(ref value, ref _reading,nameof(Reading)); }

        public InvoiceReading() {
            IsDirty = false;
        }

        public InvoiceReading(SQLiteDataReader reader)
        {
            InvoiceReadingID = reader.GetInt64(0);
            Invoice = new() { InvoiceID = reader.GetInt32(1) };
            Reading = new();
            Reading.ReadingID = reader.GetInt32(2);
            Reading.ReadingValue = reader.GetDouble(3);
            Reading.TenantAddress=new() {TenantAddressID=reader.GetInt32(4) };
            Reading.DOB = reader.GetDateTime(5);
            IsDirty = false;
        }

        public override bool CanUpdate() => true;

        public string DeleteSelect() => "DELETE FROM InvoiceReading WHERE InvoiceReadingID=@InvoiceReadingID;";

        public InvoiceReading GetRecord(SQLiteDataReader reader) => new(reader);

        public string InsertSelect() => "INSERT INTO InvoiceReading (InvoiceID, ReadingID) VALUES (@InvoiceID, @ReadingID);";

        public void Params(SQLiteParameterCollection param)
        {
            param.AddWithValue("@InvoiceReadingID", InvoiceReadingID);
            param.AddWithValue("@InvoiceID", Invoice?.InvoiceID);
            param.AddWithValue("@ReadingID", Reading?.ReadingID);
        }

        public string SQLSelect() => "SELECT * FROM TenantInvoice;";

        public string UpdateSelect() => "";

        public string CompleteComboBoxSelectedItem() => string.Empty;

        public bool FilterRecordSource(IFilterList filter)=>
        ((InvoiceReadingFilter)filter).InvoiceID == Invoice?.InvoiceID;
        
        public IEnumerable ConvertItemSourceToRecordSource(IEnumerable ItemSource)=>
         (RecordSource<InvoiceReading>)ItemSource;

        public IEnumerable ReorderList(IEnumerable ItemSource)
        {
            throw new System.NotImplementedException();
        }

        public void SetLastInsertedID() => InvoiceReadingID = MainDB.InvoiceReading.LastInsertedID();

    }


}
