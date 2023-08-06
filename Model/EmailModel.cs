using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.DB;
using Testing.Model;

namespace Meter.Model
{
    public class EmailModel : AbstractModel, IDB<EmailModel>
    {
        string _emailaddress=string.Empty;
        string _pwd = string.Empty;
        string _subject=string.Empty;   
        string _mainbody=string.Empty;

        public string EmailAddress { get => _emailaddress; set => Set<string>(ref value, ref _emailaddress,nameof(EmailAddress)); }
        public string Pwd { get => _pwd; set => Set<string>(ref value, ref _pwd, nameof(Pwd)); }
        public string Subject { get => _subject; set => Set<string>(ref value, ref _subject,nameof(Subject)); }
        public string MainBody { get => _mainbody; set => Set<string>(ref value, ref _mainbody, nameof(MainBody)); }

        public EmailModel() { 
        
        }

        public EmailModel(SQLiteDataReader reader) {
            EmailAddress = reader.GetString(0);
            Pwd=reader.GetString(1);
            Subject=reader.GetString(2);
            MainBody=reader.GetString(3);
        }

        public override bool CanUpdate()=>
            (!string.IsNullOrEmpty(EmailAddress))
            || (!string.IsNullOrEmpty(Pwd));
            

        public string DeleteSelect()
        {
            throw new NotImplementedException();
        }

        public EmailModel GetRecord(SQLiteDataReader reader) => new(reader);

        public string InsertSelect()
        {
            throw new NotImplementedException();
        }

        public void Params(SQLiteParameterCollection param)
        {
            param.AddWithValue("EmailAddress",EmailAddress);
            param.AddWithValue("Pwd",Pwd);
            param.AddWithValue("Subject", Subject);
            param.AddWithValue("MainBody", MainBody);
        }

        public string SQLSelect() => "SELECT * FROM Email;";

        public string UpdateSelect() => "UPDATE Email SET EmailAddress=@EmailAddress, Pwd=@Pwd,Subject=@Subject,MainBody=@MainBody;";

        public override string? ToString() => $"{EmailAddress} {Pwd} {Subject} {MainBody}";

        public void SetLastInsertedID()
        {
            throw new NotImplementedException();
        }
    }
}
