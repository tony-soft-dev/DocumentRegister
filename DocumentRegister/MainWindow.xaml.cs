using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Protection.PlayReady;
using Ghostscript.NET.Rasterizer;
using Ghostscript.NET.Viewer;
using Windows.Storage.Streams;
using Windows.System;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using System.Collections.ObjectModel;
using DocumentRegister.Helpers;
using DocumentRegister.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DocumentRegister
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class MainWindow : Window
    {
        private ObservableCollection<Case> recordings = new ObservableCollection<Case>();
        List<string> employeeList = new List<string>();
        public Case currentEmployee { get; set; }
        int currentIndex = 0;
        int employeeListLength = 0;
        List<Case> cases = new List<Case>();


        public MainWindow()
        {
            this.InitializeComponent();
            currentEmployee = new Case();
        }
        private void runScript_Click(object sender, RoutedEventArgs e)
        {
            String path = "C:\\Users\\ants\\Downloads\\employee_pdf_register\\Employment";
            employeeList.AddRange(Directory.GetDirectories(path));
            currentIndex = 0;
            

            for (int i = 0; i < employeeList.Count(); i++)
            {
                if (PathParsing.HasFilesToProcess(Directory.GetDirectories(employeeList[i])))
                {
                    cases.Add(new Case(employeeList[i]));
                    //cases.Last().CurrentCase.ParentPath = employeeList[i];
                }
            }
            employeeListLength = cases.Count;
            currentEmployee = cases[currentIndex];

            getEmployeeDisplayValues();
        }

        private void getEmployeeDisplayValues()
        {
            //ToBeProcessedList.DeselectRange(new ItemIndexRange(0, 1));
            DisableForm();

            ErrorMessage.Text = "";
            CaseName.Text = cases[currentIndex].CaseName;
            //CaseName.Text = currentEmployee.ToProcessList[0].Name;

            ToBeProcessedList.ItemsSource = cases[currentIndex].ToProcessList;
            //BaseExample.ItemsSource = cases[currentIndex].ToProcessList;

            Description.Text = cases[currentIndex].FormVals.Description;
            Date.Date = cases[currentIndex].FormVals.Date;
            To.Text = cases[currentIndex].FormVals.To;
            From.Text = cases[currentIndex].FormVals.From;
            Type.Text = cases[currentIndex].FormVals.Type;
            PrivilegedCheckbox.IsChecked = cases[currentIndex].FormVals.Privilaged == "true" ? true : false;


        }

        private void previous_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex != 0)
            {
                currentIndex--;
                currentEmployee = cases[currentIndex];

                getEmployeeDisplayValues();
            }

        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            if (currentIndex != employeeListLength - 1)
            {
                currentIndex++;
                currentEmployee = cases[currentIndex];

                getEmployeeDisplayValues();
            }
        }

        private void HandleCheckbox(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                PrivilegedCheckbox.Content = "Privileged";
            } else
            {
                PrivilegedCheckbox.Content = "Unprivileged";
            }
        }

 

        public void SelectFile(object sender, RoutedEventArgs e)
        {
            // Unlock form
            EnableForm();

            // Prefill form values

            PDFPreview.Source = null;
            ErrorMessage.Text = String.Empty;
            //if (ToBeProcessedList.SelectedItem != null)
            //{
            //    switch (Path.GetExtension(ToBeProcessedList.SelectedItem.ToString()))
            //    {
            //        case ".pdf":
            //            OpenPDF(ToBeProcessedList.SelectedItem.ToString());
            //            break;
            //        case ".jpg":
            //            OpenImage(ToBeProcessedList.SelectedItem.ToString());
            //            break;
            //        case ".png":
            //            OpenImage(ToBeProcessedList.SelectedItem.ToString());
            //            break;
            //        default:
            //            ErrorMessage.Text = "Cannot display this file extention type";
            //            break;
            //    }
            //}
        }
        public void EnableForm()
        {
            Description.IsEnabled = true;
            Date.IsEnabled = true;
            To.IsEnabled = true;
            From.IsEnabled = true;
            Type.IsEnabled = true;
            PrivilegedCheckbox.IsEnabled = true;
            SaveButton.IsEnabled = true;

        }
        public void DisableForm()
        {
            Description.IsEnabled = false;
            Date.IsEnabled = false;
            To.IsEnabled = false;
            From.IsEnabled = false;
            Type.IsEnabled = false;
            PrivilegedCheckbox.IsEnabled = false;
            SaveButton.IsEnabled = false;
        }


        public async void OpenPDF(string path)
        {
            StorageFile f = await
                StorageFile.GetFileFromPathAsync("file:///" + path);
            PdfDocument doc = await PdfDocument.LoadFromFileAsync(f);
            Load(doc);
        }


        public async void Load(PdfDocument pdfDoc)
        {
            PdfPages.Clear();
            BitmapImage image = new BitmapImage();

            var page = pdfDoc.GetPage(0);

            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await page.RenderToStreamAsync(stream);
                await image.SetSourceAsync(stream);
            }
            PDFPreview.Source = image;
        }

        public static ObservableCollection<BitmapImage> PdfPages
        {
            get;
            set;
        } = new ObservableCollection<BitmapImage>();

        public void OpenImage(string path)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.UriSource = new Uri(PDFPreview.BaseUri, path);

            PDFPreview.Source = bitmapImage;
        }

        public void saveExcel(object sender, RoutedEventArgs e)
        {
            cases[currentIndex].FormVals.Description = Description.Text;
            cases[currentIndex].FormVals.Date = Date.Date;
            cases[currentIndex].FormVals.To = To.Text;
            cases[currentIndex].FormVals.From = From.Text;
            cases[currentIndex].FormVals.Type = Type.Text;
            cases[currentIndex].FormVals.Privilaged = PrivilegedCheckbox.IsChecked.Value.ToString();

        }
    }
}
