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
        public Owner_Page()
        {
            InitializeComponent();
            lbGetDate.Content = DateTime.Now.ToString("yyyy-MM-dd");

            using (var context = new PetShopDataContext())
            {
                var data = from owners in context.Owners
                           select owners.Owners_LastName + ", " + owners.Owners_FirstName;
                lbOwners.ItemsSource = data.ToList();
            }
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
                using (var context = new PetShopDataContext())
                {
                    var selectedItem = lbOwners.SelectedItem.ToString();
                    var contact = context.Owners.FirstOrDefault(u => u.Owners_LastName + ", " + u.Owners_FirstName == selectedItem);
                    if (contact != null)
                    {
                        tbOwnerID.Text = contact.Owners_ID;
                        tbOwnerFName.Text = contact.Owners_FirstName;
                        tbOwnerLName.Text = contact.Owners_LastName;
                        tbOwnerContact.Text = contact.Owners_ContactNumber;
                        tbOwnerEmail.Text = contact.Owners_Email;
                    }
                }
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new PetShopDataContext())
            {
                var newRecord = context.Owners;


                {
                    newRecord.Owners_ID = tbOwnerID.Text;

                    var temp = tbOwnerID.Text;
                    newRecord.Owners_ID = temp;

                    


                    Owners_FirstName = tbOwnerFName.Text,
                    Owners_LastName = tbOwnerLName.Text,
                    Owners_ContactNumber = tbOwnerContact.Text,
                    Owners_Email = tbOwnerEmail.Text
                };

                context.Owners.InsertOnSubmit(newRecord);
                context.SubmitChanges();
            }

            UpdateListView();

            tbOwnerID.Text = "";
            tbOwnerFName.Text = "";
            tbOwnerLName.Text = "";
            tbOwnerContact.Text = "";
            tbOwnerEmail.Text = "";
        }
    }
}
