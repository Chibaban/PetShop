using Microsoft.Win32;
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

namespace PetShop
{
    /// <summary>
    /// Interaction logic for Item_Page.xaml
    /// </summary>
    public partial class Item_Page : Window
    {
        PetShopDataContext _PSDC = null;

        public Item_Page()
        {
            InitializeComponent();

            lbGetDate.Content = DateTime.Now.ToString("yyyy-MM-dd");

            _PSDC = new PetShopDataContext
                (Properties.Settings.Default.Pet_Shop_DatabaseConnectionString);

            var ItemsData = from items in _PSDC.Items
                             select items.Item_Description;
            lbItems.ItemsSource = ItemsData.ToList();
        }

        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Image files|*.bmp;*.jpg;*.png";
            openDialog.FilterIndex = 1;

            if (openDialog.ShowDialog() == true)
            {
                imagePicture.Source = new BitmapImage(new Uri(openDialog.FileName));
            }
        }

        private void lbItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbItems.SelectedIndex >= 0 && lbItems.SelectedIndex < lbItems.Items.Count)
            {
                btnUpload.IsEnabled = true;
                var selectedItem = lbItems.SelectedItem.ToString();
                var ItemsInfo = _PSDC.Items.FirstOrDefault(o => o.Item_Description == selectedItem);
                if (ItemsInfo != null)
                {
                    tbItemID.Text = ItemsInfo.Item_ID;
                    tbItemDesc.Text = ItemsInfo.Item_Description;
                    tbItemPrice.Text = ItemsInfo.Item_Price.ToString();
                    tbItemStock.Text = ItemsInfo.Item_Stock.ToString();
                    tbItemAvail.Text = ItemsInfo.Item_Availability;

                    if (ItemsInfo.Item_Photo != null)
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        using (MemoryStream stream = new MemoryStream(ItemsInfo.Item_Photo.ToArray()))
                        {
                            bitmapImage.BeginInit();
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.StreamSource = stream;
                            bitmapImage.EndInit();
                        }

                        imagePicture.Source = bitmapImage;
                    }
                    else
                    {
                        // If no photo is available, clear the image control
                        imagePicture.Source = null;
                    }
                }
            }
            else
            {
                btnUpload.IsEnabled = false;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var existingItems = _PSDC.Items.FirstOrDefault(o => o.Item_ID == tbItemID.Text);

            if (existingItems != null)
            {
                // Update the existing owner's information
                existingItems.Item_Description = tbItemDesc.Text;
                existingItems.Item_Price = int.Parse(tbItemPrice.Text);
                existingItems.Item_Stock = int.Parse(tbItemStock.Text);
                existingItems.Item_Availability = tbItemAvail.Text;

                if (imagePicture.Source != null)
                {
                    byte[] imageData = ConvertImageToByteArray(imagePicture);
                    existingItems.Item_Photo = imageData;
                }

                _PSDC.SubmitChanges();
                MessageBox.Show("Item information updated successfully.");

                tbItemID.Text = null;
                tbItemDesc.Text = null;
                tbItemPrice.Text = null;
                tbItemStock.Text = null;
                tbItemAvail.Text = null;
                imagePicture.Source = null;

                lbItems.ItemsSource = null;
            }
            else
            {
                _PSDC.Proc_Add_Item
            (tbItemID.Text, tbItemDesc.Text, int.Parse(tbItemPrice.Text), int.Parse(tbItemStock.Text), tbItemAvail.Text, ConvertImageToByteArray(imagePicture));

                _PSDC.SubmitChanges();
                MessageBox.Show("Item information added successfully.");

                tbItemID.Text = null;
                tbItemDesc.Text = null;
                tbItemPrice.Text = null;
                tbItemStock.Text = null;
                tbItemAvail.Text = null;
                imagePicture.Source = null;

                btnUpload.IsEnabled = false;
            }
        }

        private byte[] ConvertImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image.Source));
                encoder.Save(ms);
                return ms.ToArray();
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            tbItemID.Text = null;
            tbItemDesc.Text = null;
            tbItemPrice.Text = null;
            tbItemStock.Text = null;
            tbItemAvail.Text = null;

            lbItems.ItemsSource = null;

            btnUpload.IsEnabled = true;
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            lbItems.ItemsSource = null;

            string searchItem = tbSearch.Text;

            var query = from entry in _PSDC.Items
                        where entry.Item_Description.Contains(searchItem)
                        select entry;

            List<string> itemDescription = new List<string>();
            foreach (var entry in query)
            {
                // Add the formatted owner name to the list
                itemDescription.Add(entry.Item_Description);
            }

            // Set the new list as the ItemsSource
            lbItems.ItemsSource = itemDescription;
        }

        private void btViewing_Click(object sender, RoutedEventArgs e)
        {
            var ItemData = from items in _PSDC.Items
                             select items.Item_Description;

            lbItems.ItemsSource = ItemData.ToList();
        }

        private void btHome_Click(object sender, RoutedEventArgs e)
        {
            Home_Page HP = new Home_Page();
            HP.Show();
            this.Close();
        }
    }
}
