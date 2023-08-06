using Testing.Model;
using MvvmHelpers.Commands;
using System;
using System.Linq;
using System.Windows.Input;
using Testing.RecordSource;

namespace Testing.Controller {

    public abstract class AbstractListDataController<O> : AbstractDataController<O> 
        where O : AbstractModel, new() {

        string? _search;
        public string? Search { get => _search; set => Set<string?>(ref value, ref _search, nameof(Search)); }

        public ICommand OpenRecordCMD { get; }
        public ICommand OpenNewRecordCMD { get; }
        public RecordSource<O> RecordSource { get; set; }
        private O? _selectedRecord;
        public O? SelectedRecord 
        { 
            get => _selectedRecord; 
            set => Set<O?>(ref value, ref _selectedRecord, nameof(SelectedRecord)); 
        }

        public AbstractListDataController() : base()
        {
            RecordSource = new();
            RecordTracker.Source = RecordSource;
            RecordSource.OnRecordCountChanged += OnRecordCountChanged;
            SelectedRecord = new();
            OpenRecordCMD = new Command<O>(OpenRecord);
            OpenNewRecordCMD = new Command(OpenNewRecord);
            AfterPropChangedEvt += AbstractListDataController_AfterUpdate;
        }

        private void OnRecordCountChanged(object? sender, EventArgs e)
        {
            RecordTracker.RecordCount = RecordSource.Count;
            RecordTracker.Source = RecordSource;
            if (SelectedRecord == null) SelectedRecord = RecordSource.FirstOrDefault();
            var temp = RecordTracker;
            RecordTracker = null;
            RecordTracker = temp;
        }

        private void AbstractListDataController_AfterUpdate(object? sender, Testing.Controller.NotifierArgs e)
        {

            if (e.PropName.Equals(nameof(SelectedRecord)))
            {
                SetRecordTrackerSource();
                RecordTracker.SetCurrentRecordByRecord(SelectedRecord);
                var temp = RecordTracker;
                RecordTracker = null;
                RecordTracker = temp;
            }
        }

        protected abstract void OpenNewRecord();
        protected abstract void OpenRecord(O record);

        public override void Save()=>SaveRecord(SelectedRecord);

        protected override void NextRecord()
        {
            if (SelectedRecord.IsDirty) Save();
            if (!RecordTracker.CanGoNext) return;
            RecordTracker.CurrentRecord++;
            SelectedRecord = (RecordTracker.IsNewRecord) ? new() : RecordTracker.FindRecordByCurrentRecord<O>();
            RecordTracker.TriggerEvent();
            SelectedRecord.IsDirty = false;
        }

        protected override void PreviousRecord()
        {
            if (SelectedRecord == null) return;
            if (SelectedRecord.IsDirty) Save();
            if (!RecordTracker.CanGoPrevious) return;
            RecordTracker.CurrentRecord--;
            SelectedRecord = RecordTracker.FindRecordByCurrentRecord<O>();
            SelectedRecord.IsDirty = false;
            RecordTracker.TriggerEvent();
        }
    }
}
