using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.UWP.Views
{
    public sealed partial class FileNameDialog : ContentDialog
    {
        public FileNameDialog()
        {
            InitializeComponent();
        }

        private void FileNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(FileNameTextBox.Text))
            {
                IsPrimaryButtonEnabled = false;
                ValidationTextBlock.Text = "Invalid file name";
            }
            else
            {
                IsPrimaryButtonEnabled = false;
                ValidationTextBlock.Text = "";
            }
        }
    }
}
