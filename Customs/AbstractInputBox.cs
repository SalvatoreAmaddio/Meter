using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Testing.Customs {

    public abstract class AbstractInputBox : TextBox {

        #region Mandatory
        public static readonly DependencyProperty IsMandatoryProperty =
        DependencyProperty.Register(
            nameof(IsMandatory),
            typeof(bool),
            typeof(AbstractInputBox),
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
            typeof(AbstractInputBox),
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

        public AbstractInputBox()
        {
          
        }
    }
}
