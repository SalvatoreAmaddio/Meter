using System.Windows;
using System.Windows.Controls;

namespace Testing.Customs
{
    public abstract class AbstractDatePicker : DatePicker {

        #region Mandatory
        public static readonly DependencyProperty IsMandatoryProperty =
        DependencyProperty.Register(
            nameof(IsMandatory),
            typeof(bool),
            typeof(AbstractDatePicker),
            new FrameworkPropertyMetadata()
            {
                BindsTwoWayByDefault = true,
                DefaultValue = true,
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
            typeof(AbstractDatePicker),
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
