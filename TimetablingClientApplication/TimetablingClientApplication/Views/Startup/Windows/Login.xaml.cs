using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TimetablingClientApplication.TimetablingService;
using TimetablingClientApplication.Views.MasterViews;

namespace TimetablingClientApplication.Views.Startup.Windows
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login 
    {   //Webservice functionality exposed through servce refernce
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        //Registration page 
        private Register _registrationPage;
        //Alert for validation and reset
        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush _normal = new SolidColorBrush(Colors.Black);
        
        public Login()
        {
             InitializeComponent();
        }

        /// <summary>
        /// Provides Validation and Access to the Login methods on the Webservice 
        /// </summary>
        public void ClickLoginButton(object value, RoutedEventArgs e)
        {
            ValidationError.Visibility = Visibility.Hidden;

            #region Validation
            //Check to ensure fields are completed and if not validation applied
            if (String.IsNullOrEmpty(UserNameTextBox.Text) && String.IsNullOrEmpty(PasswordTextBox.Password))
            {
               UserNameAlert();
                PasswordAlert();
                ValidationError.Content = "Login Failed. Please Complete All Highlighted Fields";
                return;
            }
            //If username field populated checks made to ensure details are valid
            if (String.IsNullOrEmpty(UserNameTextBox.Text) && !String.IsNullOrEmpty(PasswordTextBox.Password))
            {
                RemovePasswordAlert();
                UserNameAlert();
                ValidationError.Content = "The Username is a required Field";
                return;
            }

            //Checks the password submitted against the email provided
            if (String.IsNullOrEmpty(PasswordTextBox.Password) && !String.IsNullOrEmpty(UserNameTextBox.Text))
            {
                //Check to ensure email address is registered
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

            //Login attempt result saved
            var loginStatus = _client.Login(UserNameTextBox.Text, _client.Encrypt(PasswordTextBox.Password));
            //User id will alwas be 0+
            if (loginStatus != 0)
            {
                ValidationError.Content = "Login Success";     
                //redirect
                var applicationLogon = new MasterView(loginStatus);
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

        //User name field text changed
        private void UserNameTextChanged(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(UserNameTextBox.Text))
            {
               UserNameAlert();
               ValidationError.Content = "The Username is a required Field";             
            }
            else
            {
                RemoveUserNameAlert();
            }
                   
        }

        //text in password field changed validation to indictae it is required
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

        //Setting validation alert styling to field
        private void PasswordAlert()
        {
            PasswordLabel.Foreground = _alert;
            PasswordTextBox.BorderBrush = _alert;
            ValidationError.Visibility = Visibility.Visible;
        }

        //Removal of alert styling from password field
        private void RemovePasswordAlert()
        {
            PasswordLabel.Foreground = _normal;
            PasswordTextBox.ClearValue(Border.BorderBrushProperty);
            ValidationError.Visibility = Visibility.Hidden;
        }

        //Setting validation alert styling to field
        private void UserNameAlert()
        {
            UserNameLabel.Foreground = _alert;
            UserNameTextBox.BorderBrush = _alert;

            ValidationError.Visibility = Visibility.Visible;
        }

        //Removal of styling from username field
        private void RemoveUserNameAlert()
        {
            PasswordLabel.Foreground = _normal;
            PasswordTextBox.ClearValue(Border.BorderBrushProperty);
            ValidationError.Visibility = Visibility.Hidden;
        }
    }
}
