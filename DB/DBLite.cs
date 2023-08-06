using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Testing.Model;
using Testing.RecordSource;

namespace Testing.DB {

     public class DBLite<M> where M : AbstractModel, IDB<M>, new() {
     private const string OriginalDBPath= @".\DB\MyDb.db";
     private SQLiteConnection connection;
     private SQLiteCommand command=null!;
     public M Record { get; set; } = new();
     public RecordSource<M> RecordSource { get; set; } = new(); 
        
     public DBLite() {
            connection = new ($"Data Source={OriginalDBPath};Version=3;");
            connection.Open();
        }

        public void Close() {
            connection.Close();
        }

        public void Update(string sql)
        {
            command = new(sql, connection);
            Record.Params(command.Parameters);
            command.ExecuteNonQuery();
            command.Parameters.Clear();
        }

        public void Update(M record) { 
            Record=record;
            Update();
        }

        public void Update() {
            command = new(Record.UpdateSelect(), connection);
            Record.Params(command.Parameters);
            command.ExecuteNonQuery();
            command.Parameters.Clear();
            RecordSource.ReplaceRecord(Record);
        }

        public void Insert(M record) { 
            Record=record;
            Insert();
        }

        public void Insert()
        {
            command = new(Record.InsertSelect(), connection);
            Record.Params(command.Parameters);
            command.ExecuteNonQuery();
            command.Parameters.Clear();
            Record.SetLastInsertedID();
            RecordSource.AddRecord(Record);
        }

        public void Delete(M record) { 
            Record=record;
            Delete();
        }

        public void Delete()
        {
            command = new("PRAGMA foreign_keys = ON;", connection);
            command.ExecuteNonQuery();

            command = new(Record.DeleteSelect(), connection);            
            Record.Params(command.Parameters);

            command.ExecuteNonQuery();
            command.Parameters.Clear();
            RecordSource.RemoveRecord(Record);
        }

        public RecordSource<M> Select(string sql) {
            RecordSource<M> res = new();
            command = new(sql, connection);
            var result = command.ExecuteReader();            
            while (result.Read())
            {
                res.AddRecord(Record.GetRecord(result));
            }
            return res;
        }

        public M GetRecord(Func<M, bool> predicate) {
            Record= RecordSource.Where(predicate).FirstOrDefault();
            return Record;
        }

        public Int64 LastInsertedID()
        {
            command = new("SELECT last_insert_rowid()", connection);
            return (Int64)command.ExecuteScalar();

        }

        public void Select() {
            RecordSource.Clear();
            command = new(Record.SQLSelect(),connection);
            var result=command.ExecuteReader();
            while (result.Read()) {
                RecordSource.AddRecord(Record.GetRecord(result));            
            }        
        }
    }

}
