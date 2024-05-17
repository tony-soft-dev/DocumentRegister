using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Data.Pdf;
using Windows.Storage.Streams;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using System.Collections.ObjectModel;
using DocumentRegister.Helpers;
using DocumentRegister.Models;
using Syncfusion.XlsIO;
using System.Reflection;
using Windows.Storage.Pickers;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;

namespace DocumentRegister
{
    public sealed partial class MainWindow : Microsoft.UI.Xaml.Window
    {
        int caseIndex = 0;
        List<Case> cases = new List<Case>();
        int selectedIndex = 0;

        public MainWindow()
        {
            this.InitializeComponent();
        }
        private void runScript_Click(object sender, RoutedEventArgs e)
        {
            //String path = "C:\\Users\\ants\\Downloads\\employee_pdf_register\\Employment";
            String path = "C:\\Users\\ants\\Downloads\\Employment";
            //String path = "\\server1\\Data\\LAW\\Clients\\Employment";
            runscript_button.IsEnabled = false;
            save_button.IsEnabled = true;

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
            ErrorMessage.Text = "";
            CaseName.Text = cases[caseIndex].CaseName;

            ToBeProcessedList.DeselectRange(new ItemIndexRange(selectedIndex, 1));
            ToBeProcessedList.ItemsSource = cases[caseIndex].ToProcessList;

            selectedIndex = 0;
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


            PDFPreview.Source = null;
            ErrorMessage.Text = String.Empty;
            if (ToBeProcessedList.SelectedItem != null)
            {
                selectedIndex = ToBeProcessedList.SelectedIndex;

                if (cases[caseIndex].ToProcessList[selectedIndex].PForm.Saved)
                {
                    Saved.Text = "(SAVED) -- Will process when you run script to save";
                } else
                {
                    Saved.Text = "";
                }

                Description.Text = cases[caseIndex].ToProcessList[selectedIndex].PForm.Description;
                Date.Date = cases[caseIndex].ToProcessList[selectedIndex].PForm.Date;
                To.Text = cases[caseIndex].ToProcessList[selectedIndex].PForm.To;
                From.Text = cases[caseIndex].ToProcessList[selectedIndex].PForm.From;
                DocType.SelectedItem = cases[caseIndex].ToProcessList[selectedIndex].PForm.DocType;
                PrivilegedCheckbox.IsChecked = cases[caseIndex].ToProcessList[selectedIndex].PForm.Privilaged;
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
            cases[caseIndex].ToProcessList[selectedIndex].PForm.DocType = DocType.SelectedItem.ToString();
            cases[caseIndex].ToProcessList[selectedIndex].PForm.Privilaged = PrivilegedCheckbox.IsChecked.Value;

            // disable
            cases[caseIndex].ToProcessList[selectedIndex].PForm.Saved = true;
            Saved.Text = "(SAVED) -- Will process when you run script to save";
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            runscript_button.IsEnabled = true;
            save_button.IsEnabled = false;

            foreach (Case c in  cases)
            {
                for (int i = 0; i < c.ToProcessList.Count; i++)
                {
                    if (c.ToProcessList[i].PForm.Saved)
                    {
                        XLWorkbook wb = new XLWorkbook(c.ExcelPath);
                        IXLWorksheet ws = wb.Worksheet("Sheet1");
                        string linkText = $"{c.CaseNumber}.{Date.Date.ToString("MMddyy")}.{i + 1}";
                        
                        IXLRow lastRow = ws.LastRowUsed().RowBelow();
                        Form d = c.ToProcessList[i].PForm;
                        var data = new[]
                        {
                            new object[]{ linkText, d.Description, d.Date.Date.ToString("MM/dd/yyyy"), d.To, d.From, d.DocType, d.Privilaged }
                        };
                        string destPath = $"{c.ParentPath}/PDF/{c.ToProcessList[i].Name}";

                        lastRow.FirstCell().InsertData(data);
                        lastRow.FirstCell().SetHyperlink(new XLHyperlink(@$"{destPath}"));

                        wb.SaveAs(c.ExcelPath);
                        // move file
                        File.Move(c.ToProcessList[i].Path, destPath);
                    }
                }
            }
        }
    }
}
