using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Testing.Customs;

namespace Testing.Customs
{
    public abstract class AbstractSearchBox : TextBox
    {
        public ICommand ResetTextCmd { get; set; }

        public AbstractSearchBox()
        {
            ResetTextCmd = new Command(ResetText);
        }

        protected void ResetText() => Text = string.Empty;

        #region MyMargin3
        public static readonly DependencyProperty MyMargin3Property =
        DependencyProperty.Register(
            nameof(MyMargin3),
            typeof(Thickness),
            typeof(AbstractSearchBox),
             new FrameworkPropertyMetadata()
             {
                 BindsTwoWayByDefault = true,
                 DefaultValue = new Thickness(0)
             }
        );

        public Thickness MyMargin3
        {
            get => (Thickness)GetValue(MyMargin3Property);
            set => SetValue(MyMargin3Property, value);
        }
        #endregion

        #region MyMargin2
        public static readonly DependencyProperty MyMargin2Property =
        DependencyProperty.Register(
            nameof(MyMargin2),
            typeof(Thickness),
            typeof(AbstractSearchBox),
             new FrameworkPropertyMetadata()
             {
                 BindsTwoWayByDefault = true,
                 DefaultValue = new Thickness(0)
             }
        );

        public Thickness MyMargin2
        {
            get => (Thickness)GetValue(MyMargin2Property);
            set => SetValue(MyMargin2Property, value);
        }
        #endregion

        #region MyMargin
        public static readonly DependencyProperty MyMarginProperty =
        DependencyProperty.Register(
            nameof(MyMargin),
            typeof(Thickness),
            typeof(AbstractSearchBox),
             new FrameworkPropertyMetadata()
             {
                 BindsTwoWayByDefault = true,
                 DefaultValue = new Thickness(0,0,1,0)
             }
        );

        public Thickness MyMargin
        {
            get => (Thickness)GetValue(MyMarginProperty);
            set => SetValue(MyMarginProperty, value);
        }
        #endregion

        #region CornerRadius
        public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(AbstractSearchBox),
             new FrameworkPropertyMetadata()
             {
                 BindsTwoWayByDefault = true,
                 DefaultValue = new CornerRadius(0),
                 PropertyChangedCallback=CornerRadiusPropertyChanged
             }
        );

        private static void CornerRadiusPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AbstractSearchBox txt = (AbstractSearchBox)d;

            CornerRadius con = (CornerRadius) e.NewValue;
            if (con.TopRight == 15)
            {
                txt.MyMargin = new Thickness(0, 0, 11, 0);
                txt.MyMargin2 = new Thickness(5, 0, 30, 0);
                txt.MyMargin3 = new Thickness(5, 0, 0, 0);
            }
            else {
                txt.MyMargin = new Thickness(0, 0, 5, 0);
                txt.MyMargin2 = new Thickness(0, 0, 0, 0);
                txt.MyMargin3 = new Thickness(0, 0, 0, 0);
            }
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        #endregion


        #region PlaceHolder
        public static readonly DependencyProperty PlaceHolderTextProperty =
        DependencyProperty.Register(
            nameof(PlaceHolderText),
            typeof(string),
            typeof(AbstractSearchBox),
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

        #region ShowButton
        public static readonly DependencyProperty ShowButtonProperty =
        DependencyProperty.Register(
            nameof(ShowButton),
            typeof(Visibility),
            typeof(AbstractSearchBox),
             new FrameworkPropertyMetadata()
             {
                 BindsTwoWayByDefault = true,
                 DefaultValue = Visibility.Hidden
             }
        );

        public Visibility ShowButton
        {
            get => (Visibility)GetValue(ShowButtonProperty);
            set => SetValue(ShowButtonProperty, value);
        }

        #endregion

        #region ShowPlaceHolder
        public static readonly DependencyProperty ShowPlaceHolderProperty =
        DependencyProperty.Register(
        nameof(ShowPlaceHolder),
        typeof(Visibility),
        typeof(AbstractSearchBox),
        new FrameworkPropertyMetadata()
        {
            BindsTwoWayByDefault = true,
            DefaultValue = Visibility.Visible
        }
        );

        public Visibility ShowPlaceHolder
        {
            get => (Visibility)GetValue(ShowButtonProperty);
            set => SetValue(ShowButtonProperty, value);
        }
        #endregion

        #region Mandatory
        public static readonly DependencyProperty IsMandatoryProperty =
        DependencyProperty.Register(
            nameof(IsMandatory),
            typeof(bool),
            typeof(AbstractSearchBox),
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
            typeof(AbstractSearchBox),
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
    }
}