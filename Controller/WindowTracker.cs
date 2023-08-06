using Meter.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Testing.View;

namespace Testing.Controller
{
    public static class WindowTracker
    {
        public static MainTab MainTab { get; set; } = null!;

        public static TenantForm? TenantForm { get; set; }
        public static AssignAddress? AssignAddress { get; set; }

        public static TenantList? TenantList { get; set; }
        public static AddressList? AddressList {get;set;}
        public static InvoiceList? InvoiceList {get; set; }
        public static CityList? CityList { get; set; }

        public static IncomeStatement? IncomeStatement { get; set; } 

        public static InvoiceViewer? InvoiceViewer { get; set; } 
        public static InvoiceForm? InvoiceForm { get; set; }
    }
}
