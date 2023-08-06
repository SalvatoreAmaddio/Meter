using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Testing.Customs;

namespace Meter.Customs
{
    abstract public class AbstractLista : ListView {

        public AbstractLista() {
        }


        #region Header
        public static readonly DependencyProperty HeaderChildProperty = DependencyProperty.Register(
           nameof(HeaderChild),
           typeof(UIElement),
           typeof(Lista),
           new PropertyMetadata()
           {
               PropertyChangedCallback = HeaderChildPropertyChanged
           });


        private static void HeaderChildPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;
            System.Windows.Application.Current?.Dispatcher?.InvokeAsync(() => {
                var control = (AbstractLista)d;
                control.ApplyTemplate();

                Grid HeaderGrid = (Grid)control.Template.FindName("HeaderGrid", control);
                ItemsPresenter ItemsPresenter = (ItemsPresenter)control.Template.FindName("ItemsPresenter", control);

                Grid header = (Grid)e.NewValue;

                List<UIElement> lista = new();

                foreach (var child in header.Children) lista.Add((UIElement)child);
                header.Children.Clear();

                int colc = (header.ColumnDefinitions.Count == 0) ? 1 : header.ColumnDefinitions.Count;

                Grid.SetColumnSpan(ItemsPresenter, colc);
                
                foreach (var col in header.ColumnDefinitions) HeaderGrid.ColumnDefinitions.Add(new() { Width = col.Width });
                header.ColumnDefinitions.Clear();
                
                int colindex = 0;
                foreach (var child in lista)
                {
                    Grid.SetColumn((UIElement)child, colindex);
                    HeaderGrid.Children.Add((UIElement)child);
                    colindex++;
                }
                lista.Clear();
                control.HeaderChild = null!;
            });

        }

        public UIElement HeaderChild
        {
            get { return (UIElement)GetValue(HeaderChildProperty); }
            set { SetValue(HeaderChildProperty, value); }
        }
        #endregion

    }
}
