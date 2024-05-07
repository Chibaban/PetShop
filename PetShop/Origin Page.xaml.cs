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
    public partial class Origin_Page : Window
    {
        PetShopDataContext _PSDC = null;

        public Origin_Page()
        {
            InitializeComponent();
            lbGetDate.Content = DateTime.Now.ToString("yyyy-MM-dd");

            _PSDC = new PetShopDataContext
                (Properties.Settings.Default.Pet_Shop_DatabaseConnectionString);

            var OriginsData = from origins in _PSDC.Origins
                            select origins.Origin_LastName + ", " + origins.Origin_FirstName;
            lbOrigin.ItemsSource = OriginsData.ToList();
        }

        private void btHome_Click(object sender, RoutedEventArgs e)
        {
            Home_Page HP = new Home_Page();
            HP.Show();
            this.Close();
        }

        private void lbOrigin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbOrigin.SelectedIndex >= 0 && lbOrigin.SelectedIndex < lbOrigin.Items.Count)
            {
                var selectedItem = lbOrigin.SelectedItem.ToString();
                var OriginsInfo = _PSDC.Origins.FirstOrDefault(o => o.Origin_LastName + ", " + o.Origin_FirstName == selectedItem);
                if (OriginsInfo != null)
                {
                    tbOriginID.Text = OriginsInfo.Origin_ID;
                    tbOriginFName.Text = OriginsInfo.Origin_FirstName;
                    tbOriginLName.Text = OriginsInfo.Origin_LastName;
                    tbOriginContact.Text = OriginsInfo.Origin_ContactNumber;
                    tbOriginEmail.Text = OriginsInfo.Origin_Email;
                }
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            tbOriginID.Text = null;
            tbOriginFName.Text = null;
            tbOriginLName.Text = null;
            tbOriginContact.Text = null;
            tbOriginEmail.Text = null;

            lbOrigin.ItemsSource = null;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var existingOrigin = _PSDC.Origins.FirstOrDefault(o => o.Origin_ID == tbOriginID.Text.ToString());

            if (existingOrigin != null)
            {
                // Update the existing owner's information
                existingOrigin.Origin_FirstName = tbOriginFName.Text;
                existingOrigin.Origin_LastName = tbOriginLName.Text;
                existingOrigin.Origin_ContactNumber = tbOriginContact.Text;
                existingOrigin.Origin_Email = tbOriginEmail.Text;

                _PSDC.SubmitChanges();
                MessageBox.Show("Origin information updated successfully.");

                tbOriginID.Text = null;
                tbOriginFName.Text = null;
                tbOriginLName.Text = null;
                tbOriginContact.Text = null;
                tbOriginEmail.Text = null;
            }
            else
            {
                _PSDC.Proc_Add_Origin
            (tbOriginID.Text, tbOriginFName.Text, tbOriginLName.Text, tbOriginContact.Text, tbOriginEmail.Text);

                _PSDC.SubmitChanges();
                MessageBox.Show("Origin information added successfully.");

                tbOriginID.Text = null;
                tbOriginFName.Text = null;
                tbOriginLName.Text = null;
                tbOriginContact.Text = null;
                tbOriginEmail.Text = null;
            }
        }

        private void btViewing_Click(object sender, RoutedEventArgs e)
        {
            tbOriginID.Text = null;
            tbOriginFName.Text = null;
            tbOriginLName.Text = null;
            tbOriginContact.Text = null;
            tbOriginEmail.Text = null;

            var OriginsData = from origins in _PSDC.Origins
                            select origins.Origin_LastName + ", " + origins.Origin_FirstName;

            lbOrigin.ItemsSource = OriginsData.ToList();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            lbOrigin.ItemsSource = null;

            string searchItem = tbSearch.Text;

            var query = from entry in _PSDC.Origins
                        where entry.Origin_LastName.Contains(searchItem)
                           || entry.Origin_FirstName.Contains(searchItem)
                        select entry;

            List<string> originNames = new List<string>();
            foreach (var entry in query)
            {
                // Add the formatted owner name to the list
                originNames.Add(entry.Origin_LastName + ", " + entry.Origin_FirstName);
            }

            // Set the new list as the ItemsSource
            lbOrigin.ItemsSource = originNames;
        }
    }
}
