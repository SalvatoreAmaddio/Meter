using Meter.Controller;
using System;
using System.Collections;
using System.Data.SQLite;
using System.Linq;
using Testing.Controller.Interfaces;
using Testing.DB;
using Testing.RecordSource;

namespace Testing.Model
{
    public class Reading : AbstractModel, IDB<Reading>, IRecordSource {

        Int64 _readingid;
        double _readingvalue;
        TenantAddress? _tenantaddress;
        DateTime _dob;

        public Int64 ReadingID { get => _readingid; set => Set<Int64>(ref value, ref _readingid,nameof(ReadingID)); }
        public double ReadingValue { get => _readingvalue; set => Set<double>(ref value, ref _readingvalue, nameof(ReadingValue)); }
        public TenantAddress? TenantAddress { get => _tenantaddress; set => Set<TenantAddress?>(ref value, ref _tenantaddress, nameof(TenantAddress)); }
        public DateTime DOB { get => _dob; set => Set<DateTime>(ref value, ref _dob, nameof(DOB)); }
        
        public Reading() { 
            DOB=DateTime.Now;
            ReadingValue = 0;
            IsDirty=false;
        }

        public Reading(SQLiteDataReader reader) {
            ReadingID = reader.GetInt64(0);
            ReadingValue = reader.GetDouble(1);
            TenantAddress = new() { TenantAddressID = reader.GetInt32(2) };
            DOB = reader.GetDateTime(3);
            IsDirty = false;
        }

        public string SQLSelect() => "SELECT * FROM Reading ORDER BY DOB DESC;";

        public string UpdateSelect() => "UPDATE Reading SET ReadingValue=@ReadingValue, DOB=@DOB WHERE ReadingID=@ReadingID;";

        public string InsertSelect() => "INSERT INTO Reading (TenantAddressID, ReadingValue, DOB) VALUES(@TenantAddressID, @ReadingValue, @DOB);";

        public string DeleteSelect() => "DELETE FROM Reading WHERE ReadingID=@ReadingID;";

        public Reading GetRecord(SQLiteDataReader reader) => new(reader);

        public void Params(SQLiteParameterCollection param)
        {
            param.AddWithValue("@ReadingID", ReadingID);
            param.AddWithValue("@ReadingValue", ReadingValue);
            param.AddWithValue("@TenantAddressID", TenantAddress?.TenantAddressID);
            param.AddWithValue("@DOB", DOB);
        }

        public override bool Equals(object? obj)
        {
            return obj is Reading reading &&
                   ReadingID == reading.ReadingID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ReadingID);
        }

        public override string? ToString() => $"{ReadingID}, {ReadingValue}, {TenantAddress}, on {DOB}";

        public string CompleteComboBoxSelectedItem() => string.Empty;

        public bool FilterRecordSource(IFilterList filter)
        {
            var fil= (ReadingFilter)filter;
            var datefrom= ReadingFilter.DateFrom;
            var dateto = ReadingFilter.DateTo;
            var id = fil.TenantAddressID;
            return (fil.IgnoreDate) ? (TenantAddress?.TenantAddressID == id) : (TenantAddress?.TenantAddressID == id) && (DOB>=datefrom && DOB<=dateto);
        }

        public IEnumerable ConvertItemSourceToRecordSource(IEnumerable ItemSource)
        {
            var rec = (RecordSource<Reading>) ItemSource;
            rec.ReplaceRecords(MainDB.ReadingTable.RecordSource);
            return rec;
        }

        public override bool CanUpdate() => true;

        public IEnumerable ReorderList(IEnumerable ItemSource)
        {
            throw new NotImplementedException();
        }

        public void SetLastInsertedID() => ReadingID = MainDB.ReadingTable.LastInsertedID();
    }
}
