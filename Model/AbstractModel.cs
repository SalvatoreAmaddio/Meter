using Testing.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Testing.Model
{
    public abstract class AbstractModel : AbstractNotifier {

        private bool _isdirty;

        public bool IsDirty { get => _isdirty; set => _isdirty = value; }

        public override void Set<M>(ref M value, ref M back, string PropName)
        {
            base.Set(ref value, ref back, PropName);
            IsDirty = true;
        }

        public abstract bool CanUpdate();

        public bool Evaluate(params bool[] values) {

            bool x=false;
            int index = 0;

            foreach (var val in values) {

                x = (index == 0) ? !val : x && !val;
                index++;
            }
            if (x is false)
            {
                MessageBox.Show("Please fill all the mandatory fields", "Something is missing");
            }
            return x;
        }

    }
}
