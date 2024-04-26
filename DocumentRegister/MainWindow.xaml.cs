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
        int caseIndex = 0;
        List<Case> cases = new List<Case>();
        int selectedIndex = 0;
        Form currentForm = new Form();
        string cn = "";

        public MainWindow()
        {
            this.InitializeComponent();
        }
        private void runScript_Click(object sender, RoutedEventArgs e)
        {
            String path = "C:\\Users\\ants\\Downloads\\employee_pdf_register\\Employment";

            foreach(string p in Directory.GetDirectories(path))
            {
                if (PathParsing.HasFilesToProcess(Directory.GetDirectories(p)))
                {
                    cases.Add(new Case(p));
                }
            }


            getEmployeeDisplayValues();
        }

        private void getEmployeeDisplayValues()
        {
            DisableForm();

            ErrorMessage.Text = "";
            CaseName.Text = cases[caseIndex].CaseName;
            cn = cases[caseIndex].CaseName;

            ToBeProcessedList.DeselectRange(new ItemIndexRange(selectedIndex, 1));
            ToBeProcessedList.ItemsSource = cases[caseIndex].ToProcessList;

            selectedIndex = 0;
            currentForm = cases[caseIndex].ToProcessList[selectedIndex].PForm;

            Description.Text = cases[caseIndex].ToProcessList[selectedIndex].PForm.Description;
            Date.Date = cases[caseIndex].ToProcessList[selectedIndex].PForm.Date;
            To.Text = cases[caseIndex].ToProcessList[selectedIndex].PForm.To;
            From.Text = cases[caseIndex].ToProcessList[selectedIndex].PForm.From;
            Type.Text = cases[caseIndex].ToProcessList[selectedIndex].PForm.Type;
            PrivilegedCheckbox.IsChecked = cases[caseIndex].ToProcessList[selectedIndex].PForm.Privilaged;


        }

        private void previous_Click(object sender, RoutedEventArgs e)
        {
            selectedIndex = 0;
            if (caseIndex != 0)
            {
                caseIndex--;
                getEmployeeDisplayValues();
            }

        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            selectedIndex = 0;
            if (caseIndex != cases.Count - 1)
            {
                caseIndex++;
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
            EnableForm();

            Description.Text = cases[caseIndex].ToProcessList[selectedIndex].PForm.Description;
            Date.Date = cases[caseIndex].ToProcessList[selectedIndex].PForm.Date;
            To.Text = cases[caseIndex].ToProcessList[selectedIndex].PForm.To;
            From.Text = cases[caseIndex].ToProcessList[selectedIndex].PForm.From;
            Type.Text = cases[caseIndex].ToProcessList[selectedIndex].PForm.Type;
            PrivilegedCheckbox.IsChecked = cases[caseIndex].ToProcessList[selectedIndex].PForm.Privilaged;

            PDFPreview.Source = null;
            ErrorMessage.Text = String.Empty;
            if (ToBeProcessedList.SelectedItem != null)
            {
                string tempPath = cases[caseIndex].ToProcessList[selectedIndex].Path;

                switch (Path.GetExtension(tempPath))
                {
                    case ".pdf":
                        OpenPDF(tempPath);
                        break;
                    case ".jpg":
                        OpenImage(tempPath);
                        break;
                    case ".png":
                        OpenImage(tempPath);
                        break;
                    default:
                        ErrorMessage.Text = "Cannot display this file extention type";
                        break;
                }
            }
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
            cases[caseIndex].ToProcessList[selectedIndex].PForm.Description = Description.Text;
            cases[caseIndex].ToProcessList[selectedIndex].PForm.Date = Date.Date;
            cases[caseIndex].ToProcessList[selectedIndex].PForm.To = To.Text;
            cases[caseIndex].ToProcessList[selectedIndex].PForm.From = From.Text;
            cases[caseIndex].ToProcessList[selectedIndex].PForm.Type = Type.Text;
            cases[caseIndex].ToProcessList[selectedIndex].PForm.Privilaged = PrivilegedCheckbox.IsChecked.Value;
        }
    }
}
