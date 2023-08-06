using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.RecordSource
{
    public interface IRefreshRecordSourceEvent
    {
        public void Refresh();
        public void RemoveRecord(object obj);
        public void RemoveRecord(object obj, bool allowEvt);
        public object GetFirstRecord();
        public void RemoveRecords(IEnumerable records);
        public void RemoveRecords(IEnumerable records, bool allowEvt);

    }
}
