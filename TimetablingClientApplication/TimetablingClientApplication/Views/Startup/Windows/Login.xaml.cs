using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using TimetablingClientApplication.Views.MasterViews;

namespace TimetablingClientApplication.Views.Startup.Windows
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        TimetablingService.TimetablingServiceClient _client = new TimetablingService.TimetablingServiceClient();

        private Register _registrationPage;
        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush _normal = new SolidColorBrush(Colors.Black);
        public Login()
        {
             InitializeComponent();
        }

        /// <summary>
        /// Written: 21/11/21/2013
        /// Provides Validation and Access to the Login methods on the Webservice 
        /// </summary>
        public void ClickLoginButton(object value, RoutedEventArgs e)
        {
            ValidationError.Visibility = Visibility.Hidden;

            #region Validation
            if (String.IsNullOrEmpty(UserNameTextBox.Text) && String.IsNullOrEmpty(PasswordTextBox.Password))
            {
               UserNameAlert();
                PasswordAlert();
                ValidationError.Content = "Login Failed. Please Complete All Highlighted Fields";
                return;
            }

            if (String.IsNullOrEmpty(UserNameTextBox.Text) && !String.IsNullOrEmpty(PasswordTextBox.Password))
            {
                RemovePasswordAlert();
                UserNameAlert();
                ValidationError.Content = "The Username is a required Field";
                return;
            }

            if (String.IsNullOrEmpty(PasswordTextBox.Password) && !String.IsNullOrEmpty(UserNameTextBox.Text))
            {


                if (_client.Check_Email_Not_Exist(UserNameTextBox.Text))
                {
                    UserNameAlert();
                    ValidationError.Content = "This Email Does not exist as a User";
                    return;
                }

                RemoveUserNameAlert();
                PasswordAlert();
                ValidationError.Content = "The Password is a required Field";
                return;
            }

            #endregion

            var loginStatus = _client.Login(UserNameTextBox.Text, _client.Encrypt(PasswordTextBox.Password));

            if (loginStatus != 0)
            {
                ValidationError.Content = "Login Success";     
         
                MasterView applicationLogon = new MasterView(loginStatus);

                applicationLogon.Visibility = Visibility.Visible;
                applicationLogon.Focus();

                Close();
            }
            else 
            {             
                UserNameAlert();
                PasswordAlert();
                ValidationError.Content = "Login Failed. Please Check Your Details Are Correct";
            }
           
        }

        /// <summary>
        /// Written: 21/11/2013
        /// Opens a new registration page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickRegisterButton(object sender, RoutedEventArgs e)
        {
            _registrationPage = new Register();
            _registrationPage.Visibility = Visibility.Visible;
            _registrationPage.Focus();
        }


        private void UserNameTextChanged(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(UserNameTextBox.Text))
            {


                if (_client.Check_Email_Not_Exist(UserNameTextBox.Text))
                {
                    UserNameAlert();
                    ValidationError.Content = "This Email Does not exist as a User";
                }
                else
                {
                   RemoveUserNameAlert();
                }
            }
            else
            {
               UserNameAlert();
               ValidationError.Content = "The Username is a required Field";             
            }
                   
        }

        private void PasswordTextChanged(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(PasswordTextBox.Password))
            {
                RemovePasswordAlert();
            }
            else
            {
                PasswordAlert();
                ValidationError.Content = "The Password is a required Field";
            }

                       
        }

        private void PasswordAlert()
        {
            PasswordLabel.Foreground = _alert;
            PasswordTextBox.BorderBrush = _alert;
            
            ValidationError.Visibility = Visibility.Visible;
        }

        private void RemovePasswordAlert()
        {
            PasswordLabel.Foreground = _normal;
            PasswordTextBox.ClearValue(Border.BorderBrushProperty);
            ValidationError.Visibility = Visibility.Hidden;
        }

        private void UserNameAlert()
        {
            UserNameLabel.Foreground = _alert;
            UserNameTextBox.BorderBrush = _alert;

            ValidationError.Visibility = Visibility.Visible;
        }

        private void RemoveUserNameAlert()
        {
            PasswordLabel.Foreground = _normal;
            PasswordTextBox.ClearValue(Border.BorderBrushProperty);
            ValidationError.Visibility = Visibility.Hidden;
        }
    }
}
