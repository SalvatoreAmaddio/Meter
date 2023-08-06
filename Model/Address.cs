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
using Testing.Controller.Interfaces;
using Meter.Utilities;
using System.Windows;

namespace Testing.Model
{
    public class Address : AbstractModel, IDB<Address>, IRecordSource {

        private Int64 _addressid;
        private string _addressline=string.Empty;
        private string _postcode = string.Empty;
        private City _city = null!;
        private string _doornumber = string.Empty;

        public Int64 AddressID { get =>_addressid;  set=>Set<Int64>(ref value, ref _addressid,nameof(AddressID)); }
        public string AddressLine { get => _addressline; set => Set<string>(ref value, ref _addressline, nameof(AddressLine)); }
        public string DoorNumber { get => _doornumber; set => Set<string>(ref value, ref _doornumber, nameof(DoorNumber)); }
        public string PostCode { get => _postcode; set => Set<string>(ref value, ref _postcode, nameof(PostCode)); }
        public City City { get => _city; set => Set<City>(ref value, ref _city,nameof(City)); }

        public string AddressAndDoorNumber { get => $"{DoorNumber}, {AddressLine}"; }

        public string PostCodeCity { get => $"{PostCode}, {City.CityName}"; }

        public Address() {
           AddressID = 0;
           City = new City();
           IsDirty = false;
           BeforePropChangedEvt += Address_BeforePropChangedEvt;
        }

        public Address(SQLiteDataReader reader)
        {
            AddressID = reader.GetInt32(0);
            AddressLine = reader.GetString(1);
            try
            {
                PostCode = reader.GetString(2);
            }
            catch
            {
                PostCode = reader.GetInt32(2).ToString();
            }

            City = new()
            {
                CityID = reader.GetInt32(3),
                CityName = reader.GetString(5)
            };

            try
            {
                DoorNumber = reader.GetString(4);
            }
            catch (Exception)
            {
                try
                {
                    DoorNumber = reader.GetInt32(4).ToString();
                }
                catch
                {
                    DoorNumber = string.Empty;
                }
            }

            IsDirty = false;
            BeforePropChangedEvt += Address_BeforePropChangedEvt;
        }

        private void Address_BeforePropChangedEvt(object? sender, Controller.NotifierArgs e)
        {
            if (e.PropIs(nameof(PostCode)))
            {
                e.SetValue<string>(e.GetValue<string>().ToUpper());
            }

            if (e.PropIs(nameof(AddressLine))) {
                var val = e.GetValue<string>();
                val = Sys.EachFirstLetterUpper(ref val);
                e.SetValue<string>(val);
            }

            if (e.PropIs(nameof(DoorNumber)))
            {
                e.SetValue<string>(e.GetValue<string>().ToUpper());
            }
        }


        public string SQLSelect() => "SELECT AddressCity.* FROM  AddressCity;";

        public string UpdateSelect() => "UPDATE Address SET AddressLine=@AddressLine, PostCode=@PostCode, CityID=@CityID, DoorNumber=@DoorNumber WHERE AddressID=@AddressID;";

        public string InsertSelect() => "INSERT INTO Address (AddressLine, PostCode, CityID,DoorNumber) VALUES (@AddressLine, @PostCode, @CityID,@DoorNumber);";

        public string DeleteSelect() => "DELETE FROM Address WHERE AddressID=@AddressID";

        public Address GetRecord(SQLiteDataReader reader) => new(reader);

        public override bool Equals(object? obj)
        {
            return obj is Address address &&
                   AddressID == address.AddressID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AddressID);
        }

        public override string? ToString() => $"{DoorNumber} {AddressLine} {PostCode} {City}";

        public void Params(SQLiteParameterCollection param)
        {
            param.AddWithValue("@AddressID", AddressID);
            param.AddWithValue("@AddressLine", AddressLine);
            param.AddWithValue("@PostCode", PostCode);
            param.AddWithValue("@CityID", (City == null) ? 0 : City.CityID);
            param.AddWithValue("@DoorNumber", DoorNumber);
        }

        public string CompleteComboBoxSelectedItem()
        {
            return string.Empty;
        }

        public bool FilterRecordSource(IFilterList filter) => ToString().ToLower().Contains(filter.ToString().ToLower());
        
        public IEnumerable ConvertItemSourceToRecordSource(IEnumerable ItemSource)
        {
            var source = (RecordSource<Address>)ItemSource;
            source.ReplaceRecords(MainDB.AddressTable.RecordSource);
            return source;
        }

        public override bool CanUpdate() {
            bool CityIsNull = (City == null) ? true : string.IsNullOrEmpty(City.CityName);
            bool DoorNumberIsNull = string.IsNullOrEmpty(DoorNumber);
            bool AddressLineIsNull = string.IsNullOrEmpty(AddressLine);
            bool PostCodeIsNull = string.IsNullOrEmpty(PostCode);

            //var x= (!DoorNumberIsNull && !CityIsNull && !AddressLineIsNull && !PostCodeIsNull);
            //if (x is false) {
            //    MessageBox.Show("Please fill all the mandatory fields","Something is missing");
            //}
            //return x;

            return Evaluate(CityIsNull,DoorNumberIsNull,AddressLineIsNull,PostCodeIsNull);
        } 

        public IEnumerable ReorderList(IEnumerable ItemSource)
        {
            throw new NotImplementedException();
        }

        public void SetLastInsertedID()=>AddressID = MainDB.AddressTable.LastInsertedID();

    }
}
