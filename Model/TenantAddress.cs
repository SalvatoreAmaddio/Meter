using Testing.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Model;
using Testing.RecordSource;
using System.Windows;
using Meter.Utilities;
using Testing.Controller.Interfaces;
using Meter.Model;

namespace Testing.Model
{
    public class TenantAddress : AbstractModel, IDB<TenantAddress>, IRecordSource {

        public Int64 TenantAddressID { get; set; }
        private Tenant _tenant=null!;
        private Address _address=null!;
        private string _meter=string.Empty;

        public Tenant Tenant { get=>_tenant; set=>Set<Tenant>(ref value,ref _tenant,nameof(Tenant)); }
        public Address Address { get=>_address; set => Set<Address>(ref value, ref _address, nameof(Address)); }
        public string Meter { get => _meter; set => Set<string>(ref value, ref _meter, nameof(Meter)); }

        private DateTime? _movedin;
        private DateTime? _movedout;
        public DateTime? MovedIn { get=>_movedin; set=>Set<DateTime?>(ref value, ref _movedin,nameof(MovedIn)); }
        public DateTime? MovedOut { get=>_movedout; set => Set<DateTime?>(ref value, ref _movedout, nameof(MovedOut)); }

        public TenantAddress() {
            MovedIn = DateTime.Now;
            MovedOut = null;
            Tenant = new();
            Address = new();
            IsDirty = false;
            BeforePropChangedEvt += TenantAddress_BeforePropChangedEvt;
        }

        private void TenantAddress_BeforePropChangedEvt(object? sender, Controller.NotifierArgs e) {
            string val = string.Empty;
            if (e.PropIs(nameof(Meter)))
            {
                val = e.GetValue<string>();
                val = Sys.Nz<string>(val,"0");
                if (!Sys.IsNumeric(val))
                {
                    MessageBox.Show("Only numbers are allowed", "WRONG INPUT");
                    e.Cancel = true;
                }
            }
        }

        public TenantAddress(SQLiteDataReader reader) : this()
        {
            TenantAddressID = reader.GetInt32(0);
            Tenant.TenantID = reader.GetInt32(1);

            try { MovedIn = reader.GetDateTime(2); } catch { }
            try { MovedOut = reader.GetDateTime(3); } catch { }

            try
            {
                Meter = reader.GetInt32(4).ToString();
            }
            catch { }

            Address.AddressID = reader.GetInt32(5);
            Address.AddressLine = reader.GetString(6);

            try {
                Address.PostCode=reader.GetString(7);
            }
            catch {
                Address.PostCode = reader.GetInt32(7).ToString();
            }

            Address.City.CityID=reader.GetInt32(8);
            
            try {
                Address.DoorNumber = reader.GetString(9);
            }
            catch { Address.DoorNumber = string.Empty; }

            Address.City.CityName = reader.GetString(10);
            Tenant.IsDirty = false;
            Address.IsDirty = false;
            IsDirty = false;
        }

        public string SQLSelect() => "SELECT * FROM TenantAndAddress;";
        public string UpdateSelect() => "UPDATE TenantAddress SET TenantID=@TenantID, AddressID=@AddressID, MovedIn=@MovedIn, MovedOut=@MovedOut, Meter=@Meter WHERE TenantAddressID=@TenantAddressID;";
        public string InsertSelect() => "INSERT INTO TenantAddress (TenantID,AddressID,Meter) VALUES(@TenantID,@AddressID,@Meter);";
        public string DeleteSelect() => "DELETE FROM TenantAddress WHERE TenantAddressID=@TenantAddressID;";

        public TenantAddress GetRecord(SQLiteDataReader reader) => new(reader);

        public override bool Equals(object? obj)=>obj is TenantAddress address && TenantAddressID == address.TenantAddressID;
        
        public override int GetHashCode()=>HashCode.Combine(TenantAddressID);
        
        public void Params(SQLiteParameterCollection param)
        {
            param.AddWithValue("@TenantAddressID", TenantAddressID);
            param.AddWithValue("@AddressID", Address.AddressID);
            param.AddWithValue("@TenantID", Tenant.TenantID);
            param.AddWithValue("@MovedIn", MovedIn);
            param.AddWithValue("@MovedOut", MovedOut);
            param.AddWithValue("@Meter", Meter);
        }

        public override string? ToString() => $"{TenantAddressID}, {Tenant.TenantID}, {Address.AddressID}, {MovedIn}, {MovedOut}, {Meter}, {IsDirty}";

        public override bool CanUpdate() {
            bool meterIsNull = string.IsNullOrEmpty(Meter);
            bool movedInIsNull = string.IsNullOrEmpty(MovedIn.ToString());
            return Evaluate(meterIsNull,movedInIsNull);
        } 

        public string CompleteComboBoxSelectedItem() => string.Empty;

        public bool FilterRecordSource(IFilterList filter) => true;

        public IEnumerable ConvertItemSourceToRecordSource(IEnumerable ItemSource)=>
        (RecordSource<TenantAddress>) ItemSource;

        public static bool GetLastAddress(Tenant tenant) {
            TenantAddress tenantAddress;

            tenantAddress = MainDB.TenantAddressTable.RecordSource.Where(s => s.Tenant.Equals(tenant)).LastOrDefault();

            if (tenantAddress == null) tenantAddress = new();
            tenant.TenantAddress = tenantAddress;
            if (tenant.HasAddress()) return true;
            return false;            
        }

        public static TenantAddress GetInvoicedAddress(Invoice invoice)
        {
            var invoiceReading=MainDB.InvoiceReading.RecordSource.Where(s=>s.Invoice.InvoiceID==invoice.InvoiceID).FirstOrDefault();
            var tenantAddressID = MainDB.ReadingTable.RecordSource.Where(s=>s.ReadingID==invoiceReading.Reading.ReadingID).FirstOrDefault().TenantAddress.TenantAddressID;
            return MainDB.TenantAddressTable.RecordSource.Where(s => s.TenantAddressID==tenantAddressID).FirstOrDefault();
        }

        public static bool GetFirstAddress(Tenant tenant)
        {
            TenantAddress tenantAddress;

            var addressid = tenant.TenantAddress?.Address.AddressID;
            if (addressid > 0)
            {
                tenantAddress = MainDB.TenantAddressTable.RecordSource.Where(s => s.Tenant.TenantID == tenant.TenantID && s.Address.AddressID == addressid).FirstOrDefault();
            }
            else
            {
                tenantAddress = MainDB.TenantAddressTable.RecordSource.Where(s => s.Tenant.TenantID == tenant.TenantID).FirstOrDefault();
            }

            if (tenantAddress == null) tenantAddress = new();
            tenant.TenantAddress = tenantAddress;
            if (tenant.TenantAddress.TenantAddressID > 0) return true;
            return false;
        }

        public IEnumerable ReorderList(IEnumerable ItemSource)
        {
            throw new NotImplementedException();
        }

        public void SetLastInsertedID()=>TenantAddressID = MainDB.TenantAddressTable.LastInsertedID();
    }
}
