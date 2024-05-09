using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
    public partial class Owner_Page : Window
    {
        PetShopDataContext _PSDC = null;

        public Owner_Page()
        {
            InitializeComponent();
            lbGetDate.Content = "Date :  " + DateTime.Now.ToString("MMM-dd-yyyy");

            _PSDC = new PetShopDataContext
                (Properties.Settings.Default.Pet_Shop_DatabaseConnectionString);

            var OwnerData = from owners in _PSDC.Owners
                            select owners.Owners_LastName + ", " + owners.Owners_FirstName;
            lbOwners.ItemsSource = OwnerData.ToList();
        }

        private void btHome_Click(object sender, RoutedEventArgs e)
        {
            Home_Page HP = new Home_Page();
            HP.Show();
            this.Close();
        }

        private void lbOwners_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbOwners.SelectedIndex >= 0 && lbOwners.SelectedIndex < lbOwners.Items.Count)
            {
                var selectedItem = lbOwners.SelectedItem.ToString();
                var OwnersInfo = _PSDC.Owners.FirstOrDefault(o => o.Owners_LastName + ", " + o.Owners_FirstName == selectedItem);
                if (OwnersInfo != null)
                {
                    tbOwnerID.Text = OwnersInfo.Owners_ID;
                    tbOwnerFName.Text = OwnersInfo.Owners_FirstName;
                    tbOwnerLName.Text = OwnersInfo.Owners_LastName;
                    tbOwnerContact.Text = OwnersInfo.Owners_ContactNumber;
                    tbOwnerEmail.Text = OwnersInfo.Owners_Email;
                }
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            tbOwnerID.Text = null;
            tbOwnerFName.Text = null;
            tbOwnerLName.Text = null;
            tbOwnerContact.Text = null;
            tbOwnerEmail.Text = null;

            lbOwners.ItemsSource = null;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var existingOwner = _PSDC.Owners.FirstOrDefault(o => o.Owners_ID == tbOwnerID.Text.ToString());

            if (existingOwner != null)
            {
                // Update the existing owner's information
                existingOwner.Owners_FirstName = tbOwnerFName.Text;
                existingOwner.Owners_LastName = tbOwnerLName.Text;
                existingOwner.Owners_ContactNumber = tbOwnerContact.Text;
                existingOwner.Owners_Email = tbOwnerEmail.Text;

                _PSDC.SubmitChanges();
                MessageBox.Show("Owner information updated successfully.");

                tbOwnerID.Text = null;
                tbOwnerFName.Text = null;
                tbOwnerLName.Text = null;
                tbOwnerContact.Text = null;
                tbOwnerEmail.Text = null;

                lbOwners.ItemsSource = null;
            }
            else
            {
                _PSDC.Proc_Add_Owner
            (tbOwnerID.Text, tbOwnerFName.Text, tbOwnerLName.Text, tbOwnerContact.Text, tbOwnerEmail.Text);

                _PSDC.SubmitChanges();
                MessageBox.Show("Owner information added successfully.");

                tbOwnerID.Text = null;
                tbOwnerFName.Text = null;
                tbOwnerLName.Text = null;
                tbOwnerContact.Text = null;
                tbOwnerEmail.Text = null;
            }
        }

        private void btViewing_Click(object sender, RoutedEventArgs e)
        {
            var OwnerData = from owners in _PSDC.Owners
                            select owners.Owners_LastName + ", " + owners.Owners_FirstName;

            lbOwners.ItemsSource = OwnerData.ToList();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            lbOwners.ItemsSource = null;

            string searchItem = tbSearch.Text;

            var query = from entry in _PSDC.Owners
                        where entry.Owners_LastName.Contains(searchItem)
                           || entry.Owners_FirstName.Contains(searchItem)
                        select entry;

            List<string> ownerNames = new List<string>();
            foreach (var entry in query)
            {
                // Add the formatted owner name to the list
                ownerNames.Add(entry.Owners_LastName + ", " + entry.Owners_FirstName);
            }

            // Set the new list as the ItemsSource
            lbOwners.ItemsSource = ownerNames;
        }
    }
}
