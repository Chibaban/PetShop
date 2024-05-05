using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Contexts;
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
using System.Xml.Linq;

namespace PetShop
{
    /// <summary>
    /// Interaction logic for Owner_Page.xaml
    /// </summary>
    public partial class Staff_Page : Window
    {
        PetShopDataContext _PSDC = null;

        public Staff_Page()
        {
            InitializeComponent();
            lbGetDate.Content = DateTime.Now.ToString("yyyy-MM-dd");

            _PSDC = new PetShopDataContext
                (Properties.Settings.Default.Pet_Shop_DatabaseConnectionString);

            var StaffsData = from staffs in _PSDC.Staffs
                            select staffs.Staff_LastName + ", " + staffs.Staff_FirstName;
            lbStaffs.ItemsSource = StaffsData.ToList();
        }

        private void btHome_Click(object sender, RoutedEventArgs e)
        {
            Home_Page HP = new Home_Page();
            HP.Show();
            this.Close();
        }

        private void lbStaffs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbStaffs.SelectedIndex >= 0 && lbStaffs.SelectedIndex < lbStaffs.Items.Count)
            {
                btnUpload.IsEnabled = true;
                var selectedItem = lbStaffs.SelectedItem.ToString();
                var StaffsInfo = _PSDC.Staffs.FirstOrDefault(o => o.Staff_LastName + ", " + o.Staff_FirstName == selectedItem);
                if (StaffsInfo != null)
                {
                    tbStaffID.Text = StaffsInfo.Staff_ID;
                    tbStaffFName.Text = StaffsInfo.Staff_FirstName;
                    tbStaffLName.Text = StaffsInfo.Staff_LastName;
                    tbStaffRoleID.Text = StaffsInfo.StaffRole_ID;
                    tbStaffStatus.Text = StaffsInfo.Staff_Status;

                    if (StaffsInfo.Staff_Photo != null)
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        using (MemoryStream stream = new MemoryStream(StaffsInfo.Staff_Photo.ToArray()))
                        {
                            bitmapImage.BeginInit();
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.StreamSource = stream;
                            bitmapImage.EndInit();
                        }

                        // Assuming 'imagePicture' is the name of the Image control in your XAML
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

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            tbStaffID.Text = null;
            tbStaffFName.Text = null;
            tbStaffLName.Text = null;
            tbStaffRoleID.Text = null;
            tbStaffStatus.Text = null;
            imagePicture.Source = null;

            lbStaffs.ItemsSource = null;

            btnUpload.IsEnabled = true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var existingStaffs = _PSDC.Staffs.FirstOrDefault(o => o.Staff_ID == tbStaffID.Text.ToString());

            if (existingStaffs != null)
            {
                // Update the existing owner's information
                existingStaffs.Staff_FirstName = tbStaffFName.Text;
                existingStaffs.Staff_LastName = tbStaffLName.Text;
                existingStaffs.StaffRole_ID = tbStaffRoleID.Text;
                existingStaffs.Staff_Status= tbStaffStatus.Text;

                if (imagePicture.Source != null)
                {
                    byte[] imageData = ConvertImageToByteArray(imagePicture);
                    existingStaffs.Staff_Photo = imageData;
                }

                _PSDC.SubmitChanges();
                MessageBox.Show("Staff information updated successfully.");

                tbStaffID.Text = null;
                tbStaffFName.Text = null;
                tbStaffLName.Text = null;
                tbStaffRoleID.Text = null;
                tbStaffStatus.Text = null;
                imagePicture.Source = null;

                lbStaffs.ItemsSource = null;
            }
            else
            {
                _PSDC.Proc_Add_Staff
            (tbStaffID.Text, tbStaffFName.Text, tbStaffLName.Text, tbStaffRoleID.Text, tbStaffStatus.Text, ConvertImageToByteArray(imagePicture));

                _PSDC.SubmitChanges();
                MessageBox.Show("Staff information added successfully.");

                tbStaffID.Text = null;
                tbStaffFName.Text = null;
                tbStaffLName.Text = null;
                tbStaffRoleID.Text = null;
                tbStaffStatus.Text = null;
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

        private void btViewing_Click(object sender, RoutedEventArgs e)
        {
            var StaffsData = from staffs in _PSDC.Staffs
                            select staffs.Staff_LastName + ", " + staffs.Staff_FirstName;

            lbStaffs.ItemsSource = StaffsData.ToList();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            lbStaffs.ItemsSource = null;

            string searchStaff = tbSearch.Text;

            var query = from entry in _PSDC.Staffs
                        where entry.Staff_LastName.Contains(searchStaff)
                           || entry.Staff_FirstName.Contains(searchStaff)
                        select entry;

            List<string> staffNames = new List<string>();
            foreach (var entry in query)
            {
                // Add the formatted owner name to the list
                staffNames.Add(entry.Staff_LastName + ", " + entry.Staff_FirstName);
            }

            // Set the new list as the ItemsSource
            lbStaffs.ItemsSource = staffNames;
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
    }
}
