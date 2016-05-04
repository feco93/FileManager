using System;
using System.Collections.Generic;
using System.IO;
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

namespace FileManager
{
    /// <summary>
    /// Interaction logic for DriveChooserDialog.xaml
    /// </summary>
    public partial class DriveChooserDialog : Window
    {
        public DriveChooserDialog(string errorMessage)
        {            
            InitializeComponent();
            ErrorMessage.Content = errorMessage;
            driveCb.ItemsSource = Directory.GetLogicalDrives();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
