using Meter.View.EmailGuide;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Shapes;

namespace Meter.View
{
    /// <summary>
    /// Interaction logic for EmailError.xaml
    /// </summary>
    public partial class EmailError : Window
    {
        int stepsCount = 1;
        List<UIElement> Steps = new();

        public EmailError()
        {
            InitializeComponent();
            Steps.Add(Step1);
            Steps.Add(Step2);
            Steps.Add(Step3);
            Steps.Add(Step4);
            Steps.Add(Step5);
            Steps.Add(Step6);
            Steps.Add(Step7);
            Steps.Add(Step8);
            Steps.Add(Step9);

            ShowStep();
        }

        private void HideAll() {
            foreach (UIElement element in Steps) {
                element.Visibility = Visibility.Collapsed;
            }
        }

        private void NextStep(object sender, RoutedEventArgs e)
        {
            stepsCount++;
            if (stepsCount > 9) {
                Close();
                return;
            }
            HideAll();
            ShowStep();
        }

        private void ShowStep() {
            switch (stepsCount)
            {
                case 1:
                    Step1.Visibility = Visibility.Visible;
                    break;
                case 2:
                    Step2.Visibility = Visibility.Visible;
                    break;
                case 3:
                    Step3.Visibility = Visibility.Visible;
                    break;
                case 4:
                    Step4.Visibility = Visibility.Visible;
                    break;
                case 5:
                    Step5.Visibility = Visibility.Visible;
                    break;
                case 6:
                    Step6.Visibility = Visibility.Visible;
                    break;
                case 7:
                    Step7.Visibility = Visibility.Visible;
                    break;
                case 8:
                    Step8.Visibility = Visibility.Visible;
                    break;
                case 9:
                    Step9.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void PreviousStep(object sender, RoutedEventArgs e)
        {
            stepsCount--;

            if (stepsCount < 1) {
                stepsCount = 1;
                return;
            }
            
            HideAll();
            ShowStep();
        }
    }
}
