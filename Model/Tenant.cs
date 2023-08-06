using Testing.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Testing.Model;
using Testing.RecordSource;
using System.Windows;
using Meter.Utilities;
using Testing.Controller.Interfaces;
using System.Windows.Media.Media3D;

namespace Testing.Model {

    public class Tenant : AbstractModel, IDB<Tenant>, IRecordSource {
        string _firstName=string.Empty;
        string _lastName=string.Empty;
        DateTime? _dob;
        string _email = string.Empty;
        Int64 _phoneNumber;
        string _notes = string.Empty;
        string _tencod = string.Empty;
        TenantAddress? _tenantaddress =null!;

        public TenantAddress? TenantAddress { get => _tenantaddress; set => Set<TenantAddress?>(ref value, ref _tenantaddress,nameof(TenantAddress)); }

        public Int64 TenantID { get; set; }

        public string FirstName { get=>_firstName; set=>Set<string>(ref value, ref _firstName,nameof(FirstName)); }
        public string LastName { get => _lastName; set => Set<string>(ref value, ref _lastName, nameof(LastName)); }
        public DateTime? DOB { get => _dob; set => Set<DateTime?>(ref value,ref _dob,nameof(DOB)); }
        public string Email { get => _email; set => Set<string>(ref value, ref _email, nameof(Email)); }
        public Int64 PhoneNumber { get => _phoneNumber; set => Set<Int64>(ref value, ref _phoneNumber, nameof(PhoneNumber)); }
        public string Notes { get => _notes; set => Set<string>(ref value, ref _notes, nameof(Notes)); }
        public string TenCod { get => _tencod; set => Set<string>(ref value, ref _tencod, nameof(TenCod)); }

        public string FullName { get => $"{FirstName} {LastName}"; }

        public Tenant() {
            IsDirty = false;
            BeforePropChangedEvt += Tenant_BeforePropChangedEvt;
        }

        private void Tenant_BeforePropChangedEvt(object? sender, Controller.NotifierArgs e) {
            string val = string.Empty;
            if (e.PropIs(nameof(FirstName)) || e.PropIs(nameof(LastName))) {
                val=e.GetValue<string>();
                Sys.FirstLetterUpper(ref val);
                e.SetValue<string>(val);
            }

            if (e.PropIs(nameof(PhoneNumber))) {
                if (!Sys.IsNumericInt64(e.GetValue<Int64>()))
                {
                    MessageBox.Show("Only numbers are allowed", "WRONG INPUT");
                    e.Cancel = true;
                }
            }

            if (e.PropIs(nameof(TenCod))) {
                val = e.GetValue<string>().ToUpper();
                e.SetValue<string>(val);    
            }
        }

        public Tenant(SQLiteDataReader reader) {
            TenantID = reader.GetInt64(0);
            FirstName = reader.GetString(1);
            LastName = reader.GetString(2);

            try
            {
                DOB = reader.GetDateTime(3);
            }
            catch { DOB = null; }

            PhoneNumber = reader.GetInt64(4);

            try {
                Email = reader.GetString(5);

            } catch { }

            try {
                Notes = reader.GetString(6);

            } catch { }


            try
            {
                TenCod = reader.GetString(7);

            }
            catch { }

            TenantAddress = new();
            TenantAddress.Tenant = this;
            IsDirty = false;
            BeforePropChangedEvt += Tenant_BeforePropChangedEvt;
        }


        public string SQLSelect() => "SELECT * FROM Tenant ORDER BY TenCod ASC;";
        public string UpdateSelect() => "UPDATE Tenant SET FirstName=@FirstName, LastName=@LastName, DOB=@DOB, PhoneNumber=@PhoneNumber, Email=@Email, Notes=@Notes, TenCod=@TenCod WHERE TenantID=@TenantID;";
        public string InsertSelect() => "INSERT INTO Tenant (FirstName,LastName,DOB,Email,PhoneNumber,Notes,TenCod) VALUES (@FirstName,@LastName,@DOB,@Email,@PhoneNumber,@Notes,@TenCod)";
        public string DeleteSelect() => "DELETE FROM Tenant WHERE TenantID=@TenantID;";

        public Tenant GetRecord(SQLiteDataReader reader) => new(reader);

        public override bool Equals(object? obj)=>obj is Tenant tenant && TenantID == tenant.TenantID;
        
        public override int GetHashCode()=>HashCode.Combine(TenantID);
        
        public override string? ToString()=>$"{FirstName}, {IsDirty}";
        
        public bool HasAddress()=> TenantAddress?.Address.AddressID>0;

        public void Params(SQLiteParameterCollection param)
        {
            param.AddWithValue("@TenantID", TenantID);
            param.AddWithValue("@FirstName", FirstName);
            param.AddWithValue("@LastName", LastName);
            param.AddWithValue("@DOB", DOB);
            param.AddWithValue("@Email", Email);
            param.AddWithValue("@PhoneNumber", PhoneNumber);
            param.AddWithValue("@Notes", Notes);
            param.AddWithValue("@TenCod", TenCod);
        }

        public string CompleteComboBoxSelectedItem()=>string.Empty;


        public bool FilterRecordSource(IFilterList filter)
        {
            return FullName.ToLower().Contains(filter.ToString().ToLower()) ||
                   TenCod.ToLower().Contains(filter.ToString().ToLower())
                  ;
        }
        

        public IEnumerable ConvertItemSourceToRecordSource(IEnumerable ItemSource)
        {
            var source=(RecordSource<Tenant>)ItemSource;
            source.ReplaceRecords(MainDB.TenantTable.RecordSource);
            return source;
        }

        public override bool CanUpdate() {
            var FirstNameIsNull=string.IsNullOrEmpty(FirstName);
            var LastNameIsNull=string.IsNullOrEmpty(LastName);
            var EmailIsNull = string.IsNullOrEmpty(Email);
            return Evaluate(FirstNameIsNull,LastNameIsNull,EmailIsNull);
        }

        public IEnumerable ReorderList(IEnumerable ItemSource)
        {
            var source = (RecordSource<Tenant>)ItemSource;
            MainDB.TenantTable.RecordSource.ReorderOrderBy(s=>s.TenCod);
            source.ReplaceRecords(MainDB.TenantTable.RecordSource);
            return source;
        }

        public void SetLastInsertedID()=>TenantID = MainDB.TenantTable.LastInsertedID();
    }
}
