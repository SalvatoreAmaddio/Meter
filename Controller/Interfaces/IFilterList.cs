using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.RecordSource;

namespace Testing.Controller.Interfaces
{
    public interface IFilterList
    {
        public string? Search { get; set; }
        public IRecordSource Obj { get; set; }
    }
}
