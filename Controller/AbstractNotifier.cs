using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Testing.Controller {

    public abstract class AbstractNotifier : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<NotifierArgs>? AfterPropChangedEvt;
        public event EventHandler<NotifierArgs>? BeforePropChangedEvt;
        private NotifierArgs myArgs = null!;

        public virtual void Set<M>(ref M value, ref M back, string propertyName)
        {

            myArgs = new(propertyName, value);
            BeforePropChangedEvt?.Invoke(this, myArgs);

            if (BeforePropChangedEvt != null)
            {
                if (myArgs.Cancel) return;
                value = myArgs.GetValue<M>();
            }

            back = value;
            NotifyView(propertyName);

            myArgs.Value = value;
            AfterPropChangedEvt?.Invoke(this, myArgs);
           
        }

        public void NotifyView(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
    }

    public class NotifierArgs : EventArgs {
        public string PropName { get; set; }
        public bool Cancel { get; set; } = false;
        public dynamic Value;

        public NotifierArgs(string propName) => PropName = propName;

        public NotifierArgs(string propName, object value) : this(propName)
        {
            Value = value;
        }

        public M GetValue<M>() => Value;
        public void SetValue<M>(M value) => Value = value;

        public bool PropIs(string propname) => PropName.Equals(propname);
    }

}
