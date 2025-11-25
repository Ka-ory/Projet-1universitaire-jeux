using System;
using System.Collections.Generic;
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



namespace SAE_PROG
{
    public partial class IntroWindow : Window
    {
        public IntroWindow()
        {
            InitializeComponent();
            
        }

        
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.introo = 1;
            this.DialogResult = true;
            
        }

       
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.introo = 2;
            this.DialogResult = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.introo = 1;
            this.DialogResult = true;
        }
    }
}
