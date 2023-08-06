using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Model;

namespace Testing.DB { 

    public interface IDB<M> where M : AbstractModel {


        public string SQLSelect();
        public string UpdateSelect();
        public string InsertSelect();
        public string DeleteSelect();
        public M GetRecord(SQLiteDataReader reader);
        public void Params(SQLiteParameterCollection param);
        public void SetLastInsertedID();

    }
}
