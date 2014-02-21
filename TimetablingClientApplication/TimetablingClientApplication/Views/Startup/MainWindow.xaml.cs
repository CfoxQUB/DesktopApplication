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

namespace TimetablingClientApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TimetablingService.TimetablingServiceClient client = new TimetablingService.TimetablingServiceClient();

        private Register _registrationPage;
        private SolidColorBrush alert = new SolidColorBrush(Colors.Red);
        private SolidColorBrush normal = new SolidColorBrush(Colors.Black);
        public MainWindow()
        {
             InitializeComponent();
            // NavigationService ns = NavigationService.GetNavigationService(this);
        }

        /// <summary>
        /// Written: 21/11/21/2013
        /// Provides Validation and Access to the Login methods on the Webservice 
        /// </summary>
        public void clickLoginButton(object value, RoutedEventArgs e)
        {
            validationError.Visibility = Visibility.Hidden;

            #region Validation
            if (userNameTextBox.Text == "" && passwordTextBox.Password == "")
            {
                userNameLabel.Foreground = alert;
                userNameTextBox.BorderBrush = alert;

                passwordLabel.Foreground = alert;
                passwordTextBox.BorderBrush = alert;
                validationError.Visibility = Visibility.Visible;
                validationError.Content = "Login Failed. Please Complete All Highlighted Fields";
                return;
            }

            if (userNameTextBox.Text == "" && passwordTextBox.Password != "")
            {
                passwordLabel.Foreground = normal;
                passwordTextBox.ClearValue(Border.BorderBrushProperty);

                userNameLabel.Foreground = alert;
                userNameTextBox.BorderBrush = alert;
                validationError.Visibility = Visibility.Visible;
                validationError.Content = "The Username is a required Field";
                return;
            }

            if (passwordTextBox.Password == "" && userNameTextBox.Text != "")
            {
                userNameLabel.Foreground = normal;
                userNameTextBox.ClearValue(Border.BorderBrushProperty);

                passwordLabel.Foreground = alert;
                passwordTextBox.BorderBrush = alert;
                validationError.Visibility = Visibility.Visible;
                validationError.Content = "The Password is a required Field";
                return;
            }

            #endregion

            var loginStatus = client.Login(userNameTextBox.Text, passwordTextBox.Password);

            if (loginStatus != 0)
            {
                validationError.Content = "Login Success";

                Event openApplication= new Event(loginStatus);
                openApplication.Show();
                this.Close();
            }
            else 
            {
                userNameLabel.Foreground = alert;
                userNameTextBox.BorderBrush = alert;

                passwordLabel.Foreground = alert;
                passwordTextBox.BorderBrush = alert;

                validationError.Visibility = Visibility.Visible;
                validationError.Content = "Login Failed. Please Check Your Details Are Correct";
            }
           
        }

        /// <summary>
        /// Written: 21/11/2013
        /// Opens a new registration page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clickRegisterButton(object sender, RoutedEventArgs e)
        {
            _registrationPage = new TimetablingClientApplication.Register();
            _registrationPage.Visibility = System.Windows.Visibility.Visible;
            _registrationPage.Focus();
        }


        private void UserNameTextChanged(object sender, RoutedEventArgs e)
        {
            if (userNameTextBox.Text != "")
            {
                if (client.Check_Email_Not_Exist(userNameTextBox.Text))
                {
                    userNameLabel.Foreground = alert;
                    userNameTextBox.BorderBrush = alert;

                    validationError.Content = "This Email Does not exist as a User";
                    validationError.Visibility = Visibility.Visible;
                    
                    return;
                }
                else
                {

                    userNameLabel.Foreground = normal;
                    userNameTextBox.ClearValue(Border.BorderBrushProperty);
                    validationError.Visibility = Visibility.Hidden;
                    return;
                }
            }
            else
            {
                userNameLabel.Foreground = alert;
                userNameTextBox.BorderBrush = alert;

                validationError.Content = "The Username is a required Field";
                validationError.Visibility = Visibility.Visible;

                return;
            }
                   
        }

        private void PasswordTextChanged(object sender, RoutedEventArgs e)
        {
            if (passwordTextBox.Password != "")
            {
                passwordLabel.Foreground = normal;
                passwordTextBox.ClearValue(Border.BorderBrushProperty);
                validationError.Visibility = Visibility.Hidden;
                return;
            }
            else
            {
                passwordLabel.Foreground = alert;
                passwordTextBox.BorderBrush = alert;

                validationError.Content = "The Password is a required Field";
                validationError.Visibility = Visibility.Visible;
                return;
            }

                       
        }
    }
}
