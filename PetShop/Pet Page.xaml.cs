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
    /// Interaction logic for Pet_Page.xaml
    /// </summary>
    public partial class Pet_Page : Window
    {
        PetShopDataContext _PSDC = null;
        private int _CheckedCount = 0;

        public Pet_Page()
        {
            InitializeComponent();
            lbGetDate.Content = "Date :  " + DateTime.Now.ToString("MMM-dd-yyyy");

            _PSDC = new PetShopDataContext
                (Properties.Settings.Default.Pet_Shop_DatabaseConnectionString);

            var PetsData = from pets in _PSDC.Pets
                           select pets.Pet_Name;
            lbPets.ItemsSource = PetsData.ToList();
        }

        private void lbStaffs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbPets.SelectedIndex >= 0 && lbPets.SelectedIndex < lbPets.Items.Count)
            {
                btnUpload.IsEnabled = true;
                var selectedPet = lbPets.SelectedItem.ToString();
                var PetInfo = _PSDC.Pets.FirstOrDefault(o => o.Pet_Name == selectedPet);
                if (PetInfo != null)
                {
                    tbPetID.Text = PetInfo.Pet_ID;
                    tbPetName.Text = PetInfo.Pet_Name;
                    tbPetType.Text = PetInfo.Pet_Type;
                    tbPetSex.Text = PetInfo.Pet_Sex;
                    tbPetPedigree.Text = PetInfo.Pet_Pedigree;
                    tbPetColor.Text = PetInfo.Pet_Color;
                    tbPetBirthday.Text = PetInfo.Pet_Birthday.ToString();
                    tbPetGrowth.Text = PetInfo.Pet_Growth;
                    tbPetVaccine.Text = PetInfo.Vaccine_ID;
                    tbPetInfo.Text = PetInfo.Pet_Info;
                    tbPetPrice.Text = PetInfo.Pet_Price.ToString();
                    tbPetStatus.Text = PetInfo.Pet_Status;
                    tbOriginID.Text = PetInfo.Origin_ID;
                    tbOwnersID.Text = PetInfo.Owners_ID;

                    if (PetInfo.Pet_Photo != null)
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        using (MemoryStream stream = new MemoryStream(PetInfo.Pet_Photo.ToArray()))
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

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            tbPetID.Text = null;
            tbPetName.Text = null;
            tbPetType.Text = null;
            tbPetSex.Text = null;
            tbPetPedigree.Text = null;
            tbPetColor.Text = null;
            tbPetBirthday.Text = null;
            tbPetGrowth.Text = null;
            tbPetVaccine.Text = null;
            tbPetInfo.Text = null;
            tbPetPrice.Text = null;
            tbPetStatus.Text = null;
            tbOriginID.Text = null;
            tbOwnersID.Text = null;

            lbPets.ItemsSource = null;

            btnUpload.IsEnabled = true;
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            lbPets.ItemsSource = null;

            string searchPet = tbSearch.Text;

            var query = from entry in _PSDC.Pets
                        where entry.Pet_Name.Contains(searchPet)
                        select entry;

            List<string> PetDescription = new List<string>();
            foreach (var entry in query)
            {
                // Add the formatted owner name to the list
                PetDescription.Add(entry.Pet_Name);
            }

            // Set the new list as the ItemsSource
            lbPets.ItemsSource = PetDescription;
        }

        private void btViewing_Click(object sender, RoutedEventArgs e)
        {
            var PetData = from pets in _PSDC.Pets
                           select pets.Pet_Name;

            lbPets.ItemsSource = PetData.ToList();
        }

        private void btHome_Click(object sender, RoutedEventArgs e)
        {
            Home_Page HP = new Home_Page();
            HP.Show();
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var existingPets = _PSDC.Pets.FirstOrDefault(o => o.Pet_ID == tbPetID.Text);

            if (existingPets != null)
            {
                // Update the existing owner's information
                existingPets.Pet_Name = tbPetName.Text;
                existingPets.Pet_Type = tbPetType.Text;
                existingPets.Pet_Sex = tbPetSex.Text;
                existingPets.Pet_Pedigree = tbPetPedigree.Text;
                existingPets.Pet_Color = tbPetColor.Text;
                existingPets.Pet_Birthday = DateTime.Parse(tbPetBirthday.Text);
                existingPets.Pet_Growth = tbPetGrowth.Text;
                existingPets.Vaccine_ID = tbPetVaccine.Text;
                existingPets.Pet_Info = tbPetInfo.Text;
                existingPets.Pet_Price = int.Parse(tbPetPrice.Text);
                existingPets.Pet_Status = tbPetStatus.Text;
                existingPets.Origin_ID = tbOriginID.Text;
                existingPets.Owners_ID = tbOwnersID.Text;

                if (imagePicture.Source != null)
                {
                    byte[] imageData = ConvertImageToByteArray(imagePicture);
                    existingPets.Pet_Photo = imageData;
                }

                if (tbOwnersID.Text == null || tbOwnersID.Text == "")
                {
                    tbOwnersID.Text = "-";
                    existingPets.Origin_ID = tbOriginID.Text;
                }

                if (tbPetVaccine.Text == null || tbPetVaccine.Text == "")
                {
                    tbPetVaccine.Text = "NO-VAX";
                    existingPets.Vaccine_ID = tbPetVaccine.Text;
                }

                if (tbPetInfo.Text == null || tbPetInfo.Text == "")
                {
                    tbPetInfo.Text = "No Info";
                    existingPets.Pet_Info = tbPetInfo.Text;
                }

                _PSDC.SubmitChanges();
                MessageBox.Show("Pet information updated successfully.");

                tbPetID.Text = null;
                tbPetName.Text = null;
                tbPetType.Text = null;
                tbPetSex.Text = null;
                tbPetPedigree.Text = null;
                tbPetColor.Text = null;
                tbPetBirthday.Text = null;
                tbPetGrowth.Text = null;
                tbPetVaccine.Text = null;
                tbPetInfo.Text = null;
                tbPetPrice.Text = null;
                tbPetStatus.Text = null;
                tbOriginID.Text = null;
                tbOwnersID.Text = null;

                lbPets.ItemsSource = null;

                imagePicture.Source = null;

                btnUpload.IsEnabled = false;
            }
            else
            {
                if (tbOwnersID.Text == null)
                {
                    tbOwnersID.Text = "-";
                    existingPets.Origin_ID = tbOriginID.Text;
                }

                if (tbPetVaccine.Text == null)
                {
                    tbPetVaccine.Text = "NO-VAX";
                    existingPets.Vaccine_ID = tbPetVaccine.Text;
                }

                if (tbPetInfo.Text == null)
                {
                    tbPetInfo.Text = "No Info";
                    existingPets.Pet_Info = tbPetInfo.Text;
                }

                    _PSDC.Proc_Add_Pet
            (tbPetID.Text, tbPetName.Text, tbPetType.Text, tbPetSex.Text, tbPetPedigree.Text, tbPetColor.Text, DateTime.Parse(tbPetBirthday.Text),
            tbPetGrowth.Text, tbPetVaccine.Text, tbPetInfo.Text, int.Parse(tbPetPrice.Text), tbPetStatus.Text, tbOriginID.Text, tbOwnersID.Text, 
            ConvertImageToByteArray(imagePicture));

                _PSDC.SubmitChanges();
                MessageBox.Show("Pet information added successfully.");

                tbPetID.Text = null;
                tbPetName.Text = null;
                tbPetType.Text = null;
                tbPetSex.Text = null;
                tbPetPedigree.Text = null;
                tbPetColor.Text = null;
                tbPetBirthday.Text = null;
                tbPetGrowth.Text = null;
                tbPetVaccine.Text = null;
                tbPetInfo.Text = null;
                tbPetPrice.Text = null;
                tbPetStatus.Text = null;
                tbOriginID.Text = null;
                tbOwnersID.Text = null;

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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //foreach (var category in categoryBox.Items)
            //{
            //    CheckBox checkcategory = (CheckBox)category;
            //    if ((bool)checkcategory.IsChecked)
            //    {
            //        _CheckedCount++;
            //    }
            //    else
            //    {
            //        _CheckedCount--;
            //    }
            //}

            MessageBox.Show($"Count: {_CheckedCount}");
        }

        private void cbDog_Checked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbDog_Unchecked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbCat_Checked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbCat_Unchecked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbMale_Checked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbMale_Unchecked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbFemale_Checked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbFemale_Unchecked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbYoung_Checked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbYoung_Unchecked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbAdult_Checked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbAdult_Unchecked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbSenior_Checked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbSenior_Unchecked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbNoVax_Checked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbNoVax_Unchecked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbRB_Checked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbRB_Unchecked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbF5_Checked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbF5_Unchecked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbF5RB_Checked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbF5RB_Unchecked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbDA2PP_Checked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbDA2PP_Unchecked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbDA2PPRB_Checked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbDA2PPRB_Unchecked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbAvailable_Checked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbAvailable_Unchecked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbSold_Checked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void cbSold_Unchecked(object sender, RoutedEventArgs e)
        {
            FilterPets();
        }

        private void FilterPets()
        {
            var filteredPets = _PSDC.Pets.ToList(); // Assuming _PSDC.Pets contains all pets

            if (cbDog.IsChecked == true)
            {
                filteredPets = filteredPets.Where(p => p.Pet_Type == "Dog").ToList();
            }

            if (cbCat.IsChecked == true)
            {
                filteredPets = filteredPets.Where(p => p.Pet_Type == "Cat").ToList();
            }

            if (cbMale.IsChecked == true)
            {
                filteredPets = filteredPets.Where(p => p.Pet_Sex == "Male").ToList();
            }

            if (cbFemale.IsChecked == true)
            {
                filteredPets = filteredPets.Where(p => p.Pet_Sex == "Female").ToList();
            }

            if (cbYoung.IsChecked == true)
            {
                filteredPets = filteredPets.Where(p => p.Pet_Growth == "Young").ToList();
            }

            if (cbAdult.IsChecked == true)
            {
                filteredPets = filteredPets.Where(p => p.Pet_Growth == "Adult").ToList();
            }

            if (cbSenior.IsChecked == true)
            {
                filteredPets = filteredPets.Where(p => p.Pet_Growth == "Senior").ToList();
            }

            if (cbNoVax.IsChecked == true)
            {
                filteredPets = filteredPets.Where(p => p.Vaccine_ID == "NO-VAX").ToList();
            }

            if (cbF5.IsChecked == true)
            {
                filteredPets = filteredPets.Where(p => p.Vaccine_ID == "F5").ToList();
            }

            if (cbRB.IsChecked == true)
            {
                filteredPets = filteredPets.Where(p => p.Vaccine_ID == "RB").ToList();
            }

            if (cbF5RB.IsChecked == true)
            {
                filteredPets = filteredPets.Where(p => p.Vaccine_ID == "F5-RB").ToList();
            }

            if (cbDA2PP.IsChecked == true)
            {
                filteredPets = filteredPets.Where(p => p.Vaccine_ID == "DA2PP").ToList();
            }

            if (cbDA2PPRB.IsChecked == true)
            {
                filteredPets = filteredPets.Where(p => p.Vaccine_ID == "DA2PP-RB").ToList();
            }

            if (cbAvailable.IsChecked == true)
            {
                filteredPets = filteredPets.Where(p => p.Pet_Status == "Available").ToList();
            }

            if (cbSold.IsChecked == true)
            {
                filteredPets = filteredPets.Where(p => p.Pet_Status == "Sold").ToList();
            }

            // Update the listbox with the filtered pets
            lbPets.ItemsSource = filteredPets.Select(p => p.Pet_Name).ToList();
        }

        private void categoryBox_DropDownClosed(object sender, EventArgs e)
        {

        }
    }
}
