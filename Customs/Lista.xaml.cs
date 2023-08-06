using Testing.Controller.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Testing.RecordSource;
using Meter.Controller.Interfaces;
using Testing.Report;
using Meter.Customs;

namespace Testing.Customs {

    public partial class Lista : AbstractLista {

        event EventHandler<OnRecordSourceChangedEvtArgs>? _onrecordsourcechanged;

        public event EventHandler<OnRecordSourceChangedEvtArgs>? OnRecordSourceChanged
        {
            add
            {
                _onrecordsourcechanged += value;
            }
            remove
            {
                _onrecordsourcechanged -= value;
            }
        }

        private IEnumerable? Source;

        public Lista() {
            InitializeComponent();

        }


        #region /*OrderBy*/
        public static readonly DependencyProperty OrderByProperty =
        DependencyProperty.Register(
        nameof(OrderBy), typeof(IListOrder), typeof(ListView),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = null,
            PropertyChangedCallback = OrderByPropertyChanged
        }
        );

        private static void OrderByPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var list = (Lista)d;
            if (list.ItemsSource == null) return;
            list.Source = ((IListOrder)e.NewValue).Obj.ReorderList(list.ItemsSource); 
        }

        public IListOrder OrderBy
        {
            get => (IListOrder)GetValue(OrderByProperty);
            set => SetValue(OrderByProperty, value);
        }
        #endregion

        #region /*Filter*/
        public static readonly DependencyProperty FilterProperty =
        DependencyProperty.Register(
        nameof(Filter), typeof(IFilterList), typeof(ListView),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = null,
            PropertyChangedCallback = FilterPropertyChanged
        }
        );

        private static void FilterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var list = (Lista)d;
            if (list.ItemsSource == null) return;
             list.Source = ((IFilterList)e.NewValue).Obj.ConvertItemSourceToRecordSource(list.ItemsSource);
            if (list.IsSourceShared)
                list.RefreshSharedItemSource();
            else
                list.Refresh();
        }

        public IFilterList Filter
        {
            get => (IFilterList)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }
        #endregion

        #region /*SetSource*/
        public static readonly DependencyProperty SetSourceProperty =
        DependencyProperty.Register(
        nameof(SetSource), typeof(bool), typeof(Lista),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = false,
            PropertyChangedCallback = SetSourcePropertyChanged
        }
        );

        private static void SetSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var isSet = bool.Parse(e.NewValue.ToString());
            if (isSet)
            {
                var control = (Lista)d;

                var obj = control.ItemsSource.Cast<IRecordSource>().ToList().FirstOrDefault();
                if (obj == null) return;
                control.Source = obj.ConvertItemSourceToRecordSource(control.ItemsSource);
                if (control.IsSourceShared) return;
                control.Refresh();
                control.ItemsSource = control.Source;
            }
        }

        public bool SetSource
        {
            get => (bool)GetValue(SetSourceProperty);
            set => SetValue(SetSourceProperty, value);
        }

        public void Refresh()
        {
            if (ItemsSource == null) return;
            List<object> toKeep = new();

            foreach (var item in Source)
            {
                var record = (IRecordSource)item;

                if (Filter != null)
                    if (!record.FilterRecordSource(Filter)) toKeep.Add(item);
            }

            if (Filter == null) return;
            var x = (IRefreshRecordSourceEvent)Source;
            foreach (var record in toKeep) x.RemoveRecord(record,false);
            SelectedItem = x.GetFirstRecord();
        }

        #endregion

        #region /*SharedItemSource*/
        public static DependencyProperty IsSourceSharedProperty =
        DependencyProperty.Register(
        nameof(IsSourceShared), typeof(bool), typeof(Lista),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = false,
            PropertyChangedCallback = SetSharedItemsSourcePropertyChanged
        }
        );

        private static void SetSharedItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Lista control = (Lista)d;
            control.RefreshSharedItemSource();
        }

        public bool IsSourceShared
        {
            get => (bool)GetValue(IsSourceSharedProperty);
            set => SetValue(IsSourceSharedProperty, value);
        }

        public void RefreshSharedItemSource()
        {
            RecordSource<IRecordSource> toKeep = new();

            if (Filter == null)
            {
                ItemsSource = Source;
                return;
            }

            foreach (var item in Source)
            {
                var record = (IRecordSource)item;

                if (Filter != null) if (record.FilterRecordSource(Filter)) toKeep.Add(record);
            }
            ItemsSource = toKeep;
            SelectedItem = toKeep.FirstOrDefault();
            _onrecordsourcechanged?.Invoke(this, new OnRecordSourceChangedEvtArgs(ItemsSource));
        }


        #endregion

    }

    #region OnRecordSourceChangedEvtArgs
    public class OnRecordSourceChangedEvtArgs : EventArgs { 
        public IEnumerable Source { get; }

        public OnRecordSourceChangedEvtArgs(IEnumerable source) {
            Source = source;
        }
    
    }
    #endregion
}
