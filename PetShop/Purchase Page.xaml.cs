using System;
using System.Collections.Generic;
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
    /// Interaction logic for Purchase_Page.xaml
    /// </summary>
    public partial class Purchase_Page : Window
    {
        PetShopDataContext _PSDC = null;

        public Purchase_Page()
        {
            InitializeComponent();
            lbGetDate.Content = DateTime.Now.ToString("yyyy-MM-dd");

            _PSDC = new PetShopDataContext
                (Properties.Settings.Default.Pet_Shop_DatabaseConnectionString);

            var PurchasesData = from purchases in _PSDC.Purchases
                            select purchases.Purchase_ID;
            lbPurchases.ItemsSource = PurchasesData.ToList();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var existingPurchases = _PSDC.Purchases.FirstOrDefault(o => o.Purchase_ID == tbPurchaseID.Text);

            if (existingPurchases != null)
            {
                // Update the existing owner's information
                existingPurchases.Purchase_ID = tbPurchaseID.Text;
                existingPurchases.Pet_ID = tbPetID.Text;
                existingPurchases.Item_ID = tbItemID.Text;
                existingPurchases.Item_Quantity = tbItemQuantity.Text;
                existingPurchases.Purchase_Total = int.Parse(tbPurchaseTotal.Text);
                existingPurchases.Purchase_Date = DateTime.Parse(tbPurchaseDate.Text).Date;
                existingPurchases.Owners_ID = tbOwnerID.Text;
                existingPurchases.Customer_Number = tbCustomerNum.Text;
                existingPurchases.Staff_ID = tbStaffID.Text;

                _PSDC.SubmitChanges();
                MessageBox.Show("Purchases information updated successfully.");

                tbPurchaseID.Text = null;
                tbPetID.Text = null;
                tbItemID.Text = null;
                tbItemQuantity.Text = null;
                tbPurchaseTotal.Text = null;
                tbPurchaseDate.Text = null;
                tbOwnerID.Text = null;
                tbCustomerNum.Text = null;
                tbStaffID.Text = null;
            }
            else
            {
                _PSDC.Proc_Add_Purchase
            (tbPurchaseID.Text, tbPetID.Text, tbItemID.Text, int.Parse(tbItemQuantity.Text), int.Parse(tbPurchaseTotal.Text),
            DateTime.Parse(tbPurchaseDate.Text), tbOwnerID.Text, tbCustomerNum.Text, tbStaffID.Text);

                _PSDC.SubmitChanges();
                MessageBox.Show("Purchases information added successfully.");

                tbPurchaseID.Text = null;
                tbPetID.Text = null;
                tbItemID.Text = null;
                tbItemQuantity.Text = null;
                tbPurchaseTotal.Text = null;
                tbPurchaseDate.Text = null;
                tbOwnerID.Text = null;
                tbCustomerNum.Text = null;
                tbStaffID.Text = null;
            }
        }

        private void lbStaffs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbPurchases.SelectedIndex >= 0 && lbPurchases.SelectedIndex < lbPurchases.Items.Count)
            {
                var selectedPurchased = lbPurchases.SelectedItem.ToString();
                var PurchasesInfo = _PSDC.Purchases.FirstOrDefault(o => o.Purchase_ID == selectedPurchased);
                if (PurchasesInfo != null)
                {
                    tbPurchaseID.Text = PurchasesInfo.Purchase_ID;
                    tbPetID.Text = PurchasesInfo.Pet_ID;
                    tbItemID.Text = PurchasesInfo.Item_ID;
                    tbItemQuantity.Text = PurchasesInfo.Item_Quantity;
                    tbPurchaseTotal.Text = PurchasesInfo.Purchase_Total.ToString();
                    tbPurchaseDate.Text = PurchasesInfo.Purchase_Date.ToShortDateString();
                    tbOwnerID.Text = PurchasesInfo.Owners_ID;
                    tbCustomerNum.Text = PurchasesInfo.Customer_Number;
                    tbStaffID.Text = PurchasesInfo.Staff_ID;
                }
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            tbPurchaseID.Text = null;
            tbPetID.Text = null;
            tbItemID.Text = null;
            tbItemQuantity.Text = null;
            tbPurchaseTotal.Text = null;
            tbPurchaseDate.Text = null;
            tbOwnerID.Text = null;
            tbCustomerNum.Text = null;
            tbStaffID.Text = null;

            lbPurchases.ItemsSource = null;
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            lbPurchases.ItemsSource = null;

            string searchPurchases = tbSearch.Text;

            var query = from entry in _PSDC.Purchases
                        where entry.Purchase_ID.Contains(searchPurchases)
                        select entry;

            List<string> purchasesNames = new List<string>();
            foreach (var entry in query)
            {
                // Add the formatted owner name to the list
                purchasesNames.Add(entry.Purchase_ID);
            }

            // Set the new list as the ItemsSource
            lbPurchases.ItemsSource = purchasesNames;
        }

        private void btViewing_Click(object sender, RoutedEventArgs e)
        {
            var PurchasesData = from purchases in _PSDC.Purchases
                            select purchases.Purchase_ID;

            lbPurchases.ItemsSource = PurchasesData.ToList();

            tbPurchaseID.Text = null;
            tbPetID.Text = null;
            tbItemID.Text = null;
            tbItemQuantity.Text = null;
            tbPurchaseTotal.Text = null;
            tbPurchaseDate.Text = null;
            tbOwnerID.Text = null;
            tbCustomerNum.Text = null;
            tbStaffID.Text = null;
        }

        private void btHome_Click(object sender, RoutedEventArgs e)
        {
            Home_Page HP = new Home_Page();
            HP.Show();
            this.Close();
        }
    }
}
