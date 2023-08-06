using Testing.DB;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Model;

namespace Testing.Model
{
    public class Price : AbstractModel, IDB<Price> {

        string _id=string.Empty;
        public string ID { get => _id; set => Set<string>(ref value, ref _id,nameof(ID)); }

        double _pricevalue;
        public double PriceValue { get => _pricevalue; set => Set<double>(ref value, ref _pricevalue,nameof(PriceValue)); }

        public Price() { }

        public Price(SQLiteDataReader reader) {
            ID = reader.GetString(0);
            PriceValue = reader.GetDouble(1);
            IsDirty = false;
        }


        public override bool CanUpdate()=>true;

        public string DeleteSelect()
        {
            throw new NotImplementedException();
        }

        public Price GetRecord(SQLiteDataReader reader) => new(reader);

        public string InsertSelect()
        {
            throw new NotImplementedException();
        }

        public void Params(SQLiteParameterCollection param)
        {
            throw new NotImplementedException();
        }

        public string SQLSelect() => "SELECT * FROM Price;";

        public string UpdateSelect()
        {
            throw new NotImplementedException();
        }

        public override string? ToString() => PriceValue.ToString();

        public void SetLastInsertedID()
        {
            throw new NotImplementedException();
        }
    }
}
