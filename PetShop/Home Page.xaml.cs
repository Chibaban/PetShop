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
    /// Interaction logic for Home_Page.xaml
    /// </summary>
    public partial class Home_Page : Window
    {
        public Home_Page()
        {
            InitializeComponent();
            lbGetDate.Content = "Date : " + DateTime.Now.ToString("MMM-dd-yyyy");
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Logging out...");
            MainWindow LoginPage = new MainWindow();
            LoginPage.Show();
            this.Close();
        }

        private void btPet_Click(object sender, RoutedEventArgs e)
        {
            Pet_Page PtP = new Pet_Page();
            PtP.Show();
            this.Close();
        }

        private void btOwner_Click(object sender, RoutedEventArgs e)
        {
            Owner_Page OwP = new Owner_Page();
            OwP.Show();
            this.Close();
        }

        private void btOrigin_Click(object sender, RoutedEventArgs e)
        {
            Origin_Page OrP = new Origin_Page();
            OrP.Show();
            this.Close();
        }

        private void btStaff_Click(object sender, RoutedEventArgs e)
        {
            Staff_Page SP = new Staff_Page();
            SP.Show();
            this.Close();
        }

        private void btItems_Click(object sender, RoutedEventArgs e)
        {
            Item_Page IP = new Item_Page();
            IP.Show();
            this.Close();
        }

        private void btPurchase_Click(object sender, RoutedEventArgs e)
        {
            Purchase_Page PP = new Purchase_Page();
            PP.Show();
            this.Close();
        }
    }
}
