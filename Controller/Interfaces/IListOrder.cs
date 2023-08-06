using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.RecordSource;

namespace Meter.Controller.Interfaces
{
   public interface IListOrder
    {
        public IRecordSource Obj { get; set; }
    }
}
