using Testing.Model;
using MvvmHelpers.Commands;
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Testing.RecordSource;
using System.Windows.Media.Media3D;
using Testing.DB;

namespace Testing.Controller
{

    public abstract class AbstractDataController<O> : AbstractNotifier where O : AbstractModel, new()
    {

        O _record = default(O)!;

        public O Record
        {
            get => _record;
            set => Set<O>(ref value, ref _record, nameof(Record));
        }

        RecordTracker? _recordTracker;

        public RecordTracker? RecordTracker 
        { 
            get => _recordTracker; 
            set => Set<RecordTracker?>(ref value, ref _recordTracker,nameof(RecordTracker)); 
        }

        public void SetRecordTracker(RecordSource<O> RecordSource)
        {
            RecordTracker.Source= RecordSource;
            RecordTracker.RecordCount = RecordSource.Count;
            RecordTracker.CurrentRecord = RecordSource.IndexOf(Record) + 1;
        }

        public ICommand GoNextCMD { get; protected set; }
        public ICommand GoPreviousCMD { get; protected set; }
        public ICommand DeleteRecordCMD { get; }
        public ICommand SaveRecordCMD { get; }

        public AbstractDataController()
        {
            Record = new();
            SaveRecordCMD = new Command<O>(SaveRecord);
            DeleteRecordCMD = new Command<O>(DeleteRecord);
            GoNextCMD = new Command(NextRecord);
            GoPreviousCMD = new Command(PreviousRecord);
            RecordTracker = new();
            AfterPropChangedEvt += AfterPropChanged;
        }

        public virtual void Save() => SaveRecord(Record);

        public void Delete() => DeleteRecord(Record);

        #region SaveDeleteRecord
        protected abstract void SaveRecord(O record);
        protected abstract void DeleteRecord(O record);

        protected virtual void NextRecord() {
            if (Record.IsDirty) Save();
            if (!RecordTracker.CanGoNext) return;
            RecordTracker.CurrentRecord++;
            Record=(RecordTracker.IsNewRecord) ? new() : RecordTracker.FindRecordByCurrentRecord<O>();
            Record.IsDirty = false;
        }

        protected virtual void PreviousRecord() {
            if (Record.IsDirty) Save();
            if (!RecordTracker.CanGoPrevious) return;
            RecordTracker.CurrentRecord--;
            Record = RecordTracker.FindRecordByCurrentRecord<O>();
            Record.IsDirty = false;
        }

        #endregion

        public MessageBoxResult ConfirmDialog() =>
        MessageBox.Show("Are you sure you want to delete this record?", "Confirm", MessageBoxButton.YesNo);

        public abstract void SetRecordTrackerSource();

        private void AfterPropChanged(object? sender, NotifierArgs e)
        {
            if (e.PropName.Equals(nameof(Record)))
            {
                SetRecordTrackerSource();
                RecordTracker.SetCurrentRecordByRecord(Record);
                var temp = RecordTracker;
                RecordTracker = null;
                RecordTracker = temp;
                Record.IsDirty = false;
            }
        }
    }


    #region RecordTracker
    public class RecordTracker : AbstractNotifier {
        private int _currentRecord;

        public bool IsNewRecord { get; private set; }
        public bool AllowNewRecord { get; set; } = true;
        public bool IsEmpty { get => RecordCount == 0; }
        public bool EOF { get; private set; }
        public bool BOF { get; private set; }
        public IEnumerable? Source { get; set; }
        public event EventHandler? OnNewRecordEvt;
        public event EventHandler<OnNextRecordEvtArgs>? OnNextRecordEvt;

        public bool CanGoNext
        {
            get
            {
                if (IsEmpty) return false;
                if (IsNewRecord) return false;
                if (BOF && !EOF) return true;
                if (!BOF && EOF && AllowNewRecord) return true;
                if (BOF && EOF && AllowNewRecord) return true;
                if (AllowNewRecord) return true;
                return false;
            }
        }

        public bool CanGoPrevious
        {
            get
            {
                if (IsEmpty) return false;
                if (EOF && BOF) return false;
                if (CurrentRecord <= 0) return false;
                if (EOF) return true;
                if (IsNewRecord) return true;
                if (!EOF && !BOF) return true;
                return false;
            }
        }

        public int CurrentRecord {
            get => _currentRecord;
            set
            {
                if (IsEmpty) return;
                BOF = false;
                EOF = false;
                IsNewRecord = false;

                if (value == 1) BOF = true;
                if (value == RecordCount) EOF = true;
                if (value > RecordCount) {
                    IsNewRecord = true;
                    OnNewRecordEvt?.Invoke(this,EventArgs.Empty);
                }
                Set<int>(ref value, ref _currentRecord, nameof(CurrentRecord));
                OnNextRecordEvt?.Invoke(this, new(BOF, EOF,CanGoNext,CanGoPrevious,value));
            }
        }

        public int RecordCount
        {
            get;
            set;
        }

        public RecordTracker()
        {
            RecordCount = 0;
            CurrentRecord = 0;
        }

        public void TriggerEvent() =>
        OnNextRecordEvt?.Invoke(this, new(BOF, EOF, CanGoNext, CanGoPrevious, CurrentRecord));

        public M FindRecordByCurrentRecord<M>()
        {
            if (Source == null) return default;
            return Source.Cast<M>().ElementAt(CurrentRecord - 1);
        }

        public void SetCurrentRecordByRecord<M>(M obj)
        {
            if (Source == null) return;
            int index = Source.Cast<M>().ToList().IndexOf(obj);
            if (index < 0)
            {
                if (AllowNewRecord)
                {
                    IsNewRecord = true;
                    return;
                }
                return;
            }
            index++;
            CurrentRecord = index;
        }

        public string Status => $"EOF={EOF}, BOF={BOF}, Empty={IsEmpty}, NewRec={IsNewRecord}, AllowNewRec={AllowNewRecord}, CurrentRec={CurrentRecord}, RecCount={RecordCount}";

        public override string? ToString() {
            if (Source != null) {
                RecordCount = Source.Cast<object>().Count();
            }
            if (IsEmpty) return "NO RECORDS";
            if (IsNewRecord) return "NEW RECORD";
            return $"Record {CurrentRecord} of {RecordCount}";  
        }
    }
    #endregion

    #region event
    public class OnNextRecordEvtArgs : EventArgs {
        public bool IsMOF { get =>!IsBOF && !IsEOF; }
        public bool IsBOF { get; }
        public bool IsEOF { get; }
        public bool CanGoNext { get; }
        public bool CanGoPrev { get; }
        public int CurrentRecord { get; }

        public OnNextRecordEvtArgs(bool isBOF, bool isEOF, bool canGoNext, bool canGoPrev, int currentRecord)
        {
            IsEOF = isEOF;
            IsBOF = isBOF;
            CanGoNext = canGoNext;
            CanGoPrev = canGoPrev;
            CurrentRecord = currentRecord;
        }
    }
    #endregion

}
