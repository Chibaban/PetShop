using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PetShop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PetShopDataContext _PSDC = null;
        bool loginFlag = false;

        public MainWindow()
        {
            InitializeComponent();
            _PSDC = new PetShopDataContext
                (Properties.Settings.Default.Pet_Shop_DatabaseConnectionString);
        }

        private void btClear_Click(object sender, RoutedEventArgs e)
        {
            tbAccount.Text = "";
            pbPassword.Password = ""; 
        }

        private void btLogIn_Click(object sender, RoutedEventArgs e)
        {
            loginFlag = false;

            if (tbAccount.Text.Length > 0 && pbPassword.Password.Length > 0)
            {
                var loginQuery = from l in _PSDC.Accounts
                                 where
                                    l.StaffRole_ID == tbAccount.Text
                                 select l;

                if (loginQuery.Count() == 1)
                {
                    foreach (var login in loginQuery)
                    {
                        if (login.Account_Password == pbPassword.Password)
                        {
                            loginFlag = true;
                        }
                    }
                }

                if (loginFlag)
                {
                    MessageBox.Show("Login Successful!");
                    Home_Page HP = new Home_Page();
                    HP.Show();
                    this.Close();
                }

                else
                {
                    MessageBox.Show("Invalid Credentials!");
                    tbAccount.Text = null;
                    pbPassword.Password = null;
                }

            }
            else
            {
                MessageBox.Show("Please input a credential!");
                tbAccount.Text = null;
                pbPassword.Password = null;
            }
        }
    }
}
