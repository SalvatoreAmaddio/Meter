using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Testing.RecordSource;
using MvvmHelpers;
using Testing;
using Testing.Controller.Interfaces;

namespace Testing.Customs {

    public partial class Combo : ComboBox {
        private IEnumerable Source;
        private IRecordSource obj;

        public Combo() => InitializeComponent();

        #region Mandatory
        public static readonly DependencyProperty IsMandatoryProperty =
        DependencyProperty.Register(
            nameof(IsMandatory),
            typeof(bool),
            typeof(Combo),
            new FrameworkPropertyMetadata()
            {
                BindsTwoWayByDefault = true,
                DefaultValue = false,
            }
        );

        public bool IsMandatory
        {
            get { return (bool)GetValue(IsMandatoryProperty); }
            set { SetValue(IsMandatoryProperty, value); }
        }
        #endregion Mandatory

        #region ShowMandatory
        public static readonly DependencyProperty ShowMandatoryLabelProperty =
        DependencyProperty.Register(
            nameof(ShowMandatoryLabel),
            typeof(Visibility),
            typeof(Combo),
            new FrameworkPropertyMetadata()
            {
                BindsTwoWayByDefault = true,
                DefaultValue = Visibility.Collapsed,
            }
        );

        public Visibility ShowMandatoryLabel
        {
            get { return (Visibility)GetValue(ShowMandatoryLabelProperty); }
            set { SetValue(ShowMandatoryLabelProperty, value); }
        }
        #endregion Mandatory


        #region /*Filter*/
        public static readonly DependencyProperty FilterProperty =
        DependencyProperty.Register(
        nameof(Filter), typeof(IFilterList), typeof(Combo),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue= null,
            PropertyChangedCallback = FilterPropertyChanged
        }
        );

        private static void FilterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var combo = (Combo)d;
            var ItemsSourceIsNull = combo.ItemsSource == null;
            var SharedItemsSourceIsNull = combo.SharedItemsSource==null;

            if (SharedItemsSourceIsNull && !ItemsSourceIsNull) {
                combo.obj.ConvertItemSourceToRecordSource(combo.ItemsSource);
                combo.Refresh();
                return;
            }

            if (ItemsSourceIsNull && !SharedItemsSourceIsNull) {
                combo.obj.ConvertItemSourceToRecordSource(combo.SharedItemsSource);
                combo.RefreshSharedItemSource();
            }
        }

        public IFilterList Filter { 
                               get => (IFilterList)GetValue(FilterProperty);
                               set => SetValue(FilterProperty,value); 
                            }

        #endregion

        #region /*SetSource*/
        public static readonly DependencyProperty SetSourceProperty =
        DependencyProperty.Register(
        nameof(SetSource),typeof(bool),typeof(Combo),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = false,
            PropertyChangedCallback=SetSourcePropertyChanged
        }
        );

        private static void SetSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue == null) return;
            var IsSet = bool.Parse(e.NewValue.ToString());
            
            if (IsSet) {
                var control = (Combo)d;
                if (control.SharedItemsSource != null) return;
                
                control.obj = control.ItemsSource.Cast<IRecordSource>().ToList().FirstOrDefault();
                if (control.obj == null) return;
                control.Source = control.obj.ConvertItemSourceToRecordSource(control.ItemsSource);
                control.Refresh();
                control.ItemsSource = control.Source;
            }
        }

        public void Refresh() {
            if (ItemsSource == null) return;
            List<object> toRemove = new();

            foreach (var item in Source)
            {
                var record = (IRecordSource)item;
                if (Filter != null)
                    if (!record.FilterRecordSource(Filter)) toRemove.Add(item);
            }
            if (Filter == null) return;
            var recordsource = (IRefreshRecordSourceEvent)Source;
            recordsource.RemoveRecords(toRemove,false);
            SelectedItem = recordsource.GetFirstRecord();
        }

        public bool SetSource
        {
            get => (bool)GetValue(SetSourceProperty);
            set => SetValue(SetSourceProperty, value);
        }
        #endregion

        #region /*SharedItemSource*/
        public static DependencyProperty SharedItemsSourceProperty =
        DependencyProperty.Register(
        nameof(SharedItemsSource), typeof(IEnumerable), typeof(Combo),
        new FrameworkPropertyMetadata()
        {
         BindsTwoWayByDefault = true,
         DefaultValue = null,
         PropertyChangedCallback = SetSharedItemsSourcePropertyChanged
        }
        );

        private static void SetSharedItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue == null) return;
                var control = (Combo)d;
                var sharedsource=(IEnumerable)e.NewValue;
                control.obj = sharedsource.Cast<IRecordSource>().ToList().FirstOrDefault();
                if (control.obj == null) return;
                control.Source = control.obj.ConvertItemSourceToRecordSource(sharedsource);
                control.RefreshSharedItemSource();
        }

        public void RefreshSharedItemSource()
        {
            if (SharedItemsSource == null) return;
            ObservableRangeCollection<IRecordSource> toKeep = new();

            foreach (var item in Source)
            {
                var record = (IRecordSource)item;
                if (Filter != null) if (record.FilterRecordSource(Filter)) toKeep.Add(record);
            }

            if (Filter == null) {
                ItemsSource = Source;
                return;
            }
            ItemsSource = toKeep;
            SelectedItem = toKeep.FirstOrDefault();
        }

        public IEnumerable SharedItemsSource
        {
            get => (IEnumerable)GetValue(SharedItemsSourceProperty);
            set => SetValue(SharedItemsSourceProperty, value);
        }
        #endregion

        #region /*ItemCheck*/
        private void CheckInput() {
            App.Current.Dispatcher.InvokeAsync(() => {
            if (ItemsSource == null) return;
            var RecordSource = ItemsSource.Cast<IRecordSource>().ToList();
            var filterSource = RecordSource.Where(s => s.Equals(SelectedItem));

            if (!filterSource.Any())
            {
                    if (Text.Length == 0) return;
                    MessageBox.Show("The item selected does not exist in this list", "INPUT ERROR");
                    SelectedItem = RecordSource.FirstOrDefault();
                    return;
                }
            });
        }


        public static readonly DependencyProperty ItemCheckProperty =
        DependencyProperty.Register(
        nameof(ItemCheck),typeof(bool),typeof(Combo),
        new FrameworkPropertyMetadata() {
            BindsTwoWayByDefault = true,
            DefaultValue = false,
            PropertyChangedCallback = ItemCheckPropertyChanged
        }
        );

        private static void ItemCheckPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var carryon = bool.Parse(e.NewValue.ToString());
            if (carryon) ((Combo)d).CheckInput();
        }

        public bool ItemCheck
        {
            get => (bool)GetValue(ItemCheckProperty);
            set => SetValue(ItemCheckProperty, value);
        }
        #endregion

        #region /*FixSelectedItem*/

        private void FixSelectedItem()
        {
            if (ItemsSource == null) return;
            var item = (IRecordSource)SelectedItem;
            if (item == null) return;
            App.Current.Dispatcher.InvokeAsync(() => {
                var x = item.CompleteComboBoxSelectedItem();
                Text = item.ToString();
            }
                                              );
        }

        public static readonly DependencyProperty FixSelectedItemProperty =
        DependencyProperty.Register(
        nameof(FixSelected),typeof(bool),typeof(Combo),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = false,
            PropertyChangedCallback = FixSelectedItemPropertyChanged
         }
        );

        private static void FixSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            var carryon = bool.Parse(e.NewValue.ToString());
            if (carryon) ((Combo)d).FixSelectedItem();
        }

        public bool FixSelected
        {
            get => (bool)GetValue(FixSelectedItemProperty);
            set => SetValue(FixSelectedItemProperty, value);
        }
        #endregion

        #region /*PlaceHolder*/
        public static readonly DependencyProperty PlaceHolderTextProperty =
        DependencyProperty.Register(
        nameof(PlaceHolderText),
        typeof(string),
        typeof(Combo),
        new FrameworkPropertyMetadata()
        {
         BindsTwoWayByDefault = true,
         DefaultValue = string.Empty
        }
        );

        public string PlaceHolderText
        {
            get => (string)GetValue(PlaceHolderTextProperty);
            set => SetValue(PlaceHolderTextProperty, value);
        }
        #endregion                   
    }
}
