using Testing.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Controller.Interfaces;

namespace Testing.RecordSource
{
    public interface IRecordSource
    {
        public string CompleteComboBoxSelectedItem();
        public bool FilterRecordSource(IFilterList filter);

        //EXAMPLE:
        //var source = (RecordSource<City>)ItemSource;
        //source.ReplaceRecords(MainDB.CityTable.RecordSource);
        //return source;
        public IEnumerable ConvertItemSourceToRecordSource(IEnumerable ItemSource);

        public IEnumerable ReorderList(IEnumerable ItemSource);

    }
}
