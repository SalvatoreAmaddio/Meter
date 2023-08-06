using Meter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Controller;
using Testing.DB;

namespace Meter.Controller
{
    public class EmailController : AbstractDataController<EmailModel> {

        public EmailController() {
            Record = MainDB.Email.RecordSource.FirstOrDefault();
        }

        protected override void DeleteRecord(EmailModel record)
        {
            throw new NotImplementedException();
        }

        protected override void SaveRecord(EmailModel record)
        {
            MainDB.Email.Record = record;
            MainDB.Email.Update();
        }

        public override void SetRecordTrackerSource()
        {
        }

    }
}
