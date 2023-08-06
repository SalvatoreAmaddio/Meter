using Testing.DB;
using MvvmHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using System.Windows.Input;
using Testing.Model;
using Meter.Model;

namespace Testing.RecordSource {

    public class RecordSource<M> : ObservableRangeCollection<M>, IRefreshRecordSourceEvent {

        public IRefreshRecordSourceEvent? ParentListToBeNotified { get; set; }
        public event EventHandler? OnRecordCountChanged;
        public event EventHandler<OnCascadeQueryEventArgs<M>>? OnDeleteCascadeEvent;
        public event EventHandler<OnCascadeQueryEventArgs<M>>? OnAddingCascadeEvent;
        public OnCascadeQueryEventArgs<M> args=null!;
        public Order Order=Order.ASC;

        public RecordSource() {
        }
        
        public RecordSource(IEnumerable<M> collection) : base(collection) { 
        
        }

        public void ReplaceRecords(IEnumerable<M> records,Crud crud=Crud.UPDATE)
        {
            foreach (var record in records)
            {
                args = new(record, crud);
                OnAddingCascadeEvent?.Invoke(this, args);
            }
            ReplaceRange(records);
            ParentListToBeNotified?.Refresh();
            OnRecordCountChanged?.Invoke(this,EventArgs.Empty);
        }

        public void AddRecords(IEnumerable<M> records,Crud crud=Crud.INSERT)
        {
            if (!crud.Equals(Crud.VOID)) {
                foreach (var record in records)
                {
                    args = new(record, crud);
                    OnAddingCascadeEvent?.Invoke(this, args);
                }
            }
            AddRange(records);
            ParentListToBeNotified?.Refresh();
            OnRecordCountChanged?.Invoke(this, EventArgs.Empty);
        }

        public void AddRecords(Crud crud = Crud.INSERT,params M[] records)
        {
            if (!crud.Equals(Crud.VOID))
            {
                foreach (var record in records)
                {
                    args = new(record, crud);
                    OnAddingCascadeEvent?.Invoke(this, args);
                }
            }
            AddRange(records);
            ParentListToBeNotified?.Refresh();
            OnRecordCountChanged?.Invoke(this, EventArgs.Empty);
        }

        public void AddRecord(M record,Crud crud = Crud.INSERT)
        {
            Add(record);
            if (ParentListToBeNotified != null) {
                ParentListToBeNotified.Refresh();
            }

            OnRecordCountChanged?.Invoke(this, EventArgs.Empty);
            if (crud.Equals(Crud.VOID)) return;
            args = new(record, crud);
            OnAddingCascadeEvent?.Invoke(this, args);
        }

        public void Refresh() {
           var x= CollectionViewSource.GetDefaultView(this);
            x.Refresh();
            
        }
        
        public object GetFirstRecord() => this.FirstOrDefault();

        public void RemoveRecord(object obj)
        {
            M record=(M)obj;
            args = new(record, Crud.DELETE);
            OnDeleteCascadeEvent?.Invoke(this, args);
            Remove(record);
            ParentListToBeNotified?.Refresh();
            OnRecordCountChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ReorderOrderBy<TKey>(Func<M, TKey> keySelector)
        {
            RecordSource<M> temp;
            if (Order.Equals(Order.ASC))
            {
                temp = new(this.OrderBy(keySelector));
            }
            else {
                temp = new(this.OrderByDescending(keySelector));
            }

            Clear();
            AddRecords(temp, Crud.VOID);
        }

        public void RemoveRecord(object obj, bool allowEvent)
        {
            M record = (M)obj;
            args = new(record, Crud.DELETE);
                if (allowEvent) {
                    OnDeleteCascadeEvent?.Invoke(this, args);
                }
            Remove(record);
            ParentListToBeNotified?.Refresh();
            OnRecordCountChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ReplaceRecord(M record,Crud crud=Crud.UPDATE) {
            try {
                var index = this.IndexOf(record);
                this[index] = record;
                if (crud.Equals(Crud.VOID)) return;
                args = new(record, crud);
                OnAddingCascadeEvent?.Invoke(this, args);
            }
            catch { }
        }

        public void RemoveRecords(IEnumerable records)
        {
            
            foreach (M record in records) {
                args = new(record, Crud.DELETE);
                OnDeleteCascadeEvent?.Invoke(this, args);
            }

            RemoveRange(records.Cast<M>().ToList());
            ParentListToBeNotified?.Refresh();
        }

        public void RemoveRecords(IEnumerable records, bool allowEvt)
        {
            if (allowEvt) {
                foreach (M record in records)
                {
                    args = new(record,Crud.DELETE);
                    OnDeleteCascadeEvent?.Invoke(this, args);
                }
            }
            RemoveRange(records.Cast<M>().ToList());
            ParentListToBeNotified?.Refresh();

        }

    }

    public enum Crud { 
        SELECT,
        DELETE,
        INSERT,
        UPDATE,
        VOID
    }

    public class OnCascadeQueryEventArgs<M> : EventArgs
    {
        public M Record { get; set; }
        public Crud Crud { get; set; }

        public OnCascadeQueryEventArgs(M record, Crud crud)
        {
            Record = record;
            Crud = crud;
        }
    }

    public enum Order
    {
        ASC,
        DESC,    
    };

}
