using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.ObjectModel;
using Windows.Data.Pdf;
using Windows.Storage.Streams;
using Windows.Storage;

namespace DocumentRegister.Helpers
{
    internal class ViewFile
    {
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
        }

        public ObservableCollection<BitmapImage> PdfPages
        {
            get;
            set;
        } = new ObservableCollection<BitmapImage>();

        public void OpenImage(string path)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.UriSource = new Uri(path);
        }
    }
}
