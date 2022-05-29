using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ImageToIcon
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        byte[] LoadImageByte;


        public MainWindow()
        {
            InitializeComponent();

            ComboBoxSize.Items.Add("16x16");
            ComboBoxSize.Items.Add("32x32");
            ComboBoxSize.Items.Add("48x48");
            ComboBoxSize.Items.Add("256x256");

            ComboBoxSize.SelectedItem = "32x32";
            ComboBoxSize.SelectionChanged += ComboBoxSize_SelectionChanged;

            ButtonLoad.Click += ButtonLoad_Click;
            ButtonSave.Click += ButtonSave_Click;
        }

        void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "Image|*.bmp;*.jpg'*.jpeg;*.png;";
            if (ofd.ShowDialog() == true)
            {
                LoadImageByte = File.ReadAllBytes(ofd.FileName);

                ImageOrigin.Source = GetBitmapImage(LoadImageByte);

                ComboBoxSize_SelectionChanged(ComboBoxSize, null);
            }
        }

        void ComboBoxSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ImageOrigin.Source != null)
            {
                System.Drawing.Size size = GetSelectedSize();

                ImageIcon.Source = GetBitmapImage(LoadImageByte, size.Width, size.Height);
            }
        }

        void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (ImageOrigin.Source != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "ico|*.ico";
                sfd.OverwritePrompt = true;
                if (sfd.ShowDialog() == true)
                {
                    MemoryStream outStream = new MemoryStream();

                    BitmapEncoder enc = new BmpBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create((BitmapImage)ImageOrigin.Source));
                    enc.Save(outStream);

                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                    System.Drawing.Bitmap bitmapSize = new System.Drawing.Bitmap(bitmap, GetSelectedSize());
                    bitmapSize.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Icon);
                    bitmapSize.Dispose();

                    bitmap.Dispose();

                    outStream.Close();
                    outStream.Dispose();
                }
            }
        }


        BitmapImage GetBitmapImage(byte[] bytes, int width = 0, int height = 0)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.DecodePixelHeight = height;
            img.DecodePixelWidth = width;
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.StreamSource = new MemoryStream(bytes);
            img.EndInit();
            return img;
        }

        System.Drawing.Size GetSelectedSize()
        {
            System.Drawing.Size size;

            if (ComboBoxSize.SelectedItem != null)
            {
                switch (ComboBoxSize.SelectedItem.ToString())
                {
                    case "16x16": size = new System.Drawing.Size(16, 16); break;
                    case "32x32": size = new System.Drawing.Size(32, 32); break;
                    case "48x48": size = new System.Drawing.Size(48, 48); break;
                    case "256x256": size = new System.Drawing.Size(256, 256); break;
                    default: size = new System.Drawing.Size(32, 32); break;
                }
            }
            else
            {
                size = new System.Drawing.Size(32, 32);
            }

            return size;
        }
    }
}
