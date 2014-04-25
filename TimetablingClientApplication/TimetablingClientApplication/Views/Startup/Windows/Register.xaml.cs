using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace TimetablingClientApplication.Views.Startup.Windows
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        TimetablingService.TimetablingServiceClient _client = new TimetablingService.TimetablingServiceClient();
        SolidColorBrush alert = new SolidColorBrush(Colors.Red);
        SolidColorBrush completed = new SolidColorBrush(Colors.Green);
        public ObservableCollection<string> list = new ObservableCollection<string>();
        //http://www.codegateway.com/2012/03/c-regex-for-email-address.html
        private readonly Regex _regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        public Register()
        {
            InitializeComponent();
           
                list.Add("Mr");
                list.Add("Mrs");
                list.Add("Miss");
                list.Add("Ms");
                list.Add("Dr");
                Title.ItemsSource = list;
                Title.Text = "";
        }

        public void SubmitNewUser(object sender, RoutedEventArgs e)
        {
            bool displayValidation = false;

            if (String.IsNullOrEmpty(Title.Text))
            {
                TitleValidationAlert();
                displayValidation = true;
            }

            if (String.IsNullOrEmpty(UserForenameText.Text))
            {
                ForenameValidationAlert();
                displayValidation = true;
            }

            if (String.IsNullOrEmpty(UserSurnameText.Text))
            {
                SurnameValidationAlert();
                displayValidation = true;
            }

            if (String.IsNullOrEmpty(UserEmailText.Text))
            {
                EmailValidationAlert();
                displayValidation = true;
            }
            else
            {
                if (!_client.Check_Email_Not_Exist(UserEmailText.Text))
                {
                   EmailValidationAlert();
                   EmailValidation.Text = String.Format("This Email address is already \n associated with an account.");
                }
                else
                {
                    EmailValidation.Visibility = Visibility.Hidden;
                }
                      
            }

            if (String.IsNullOrEmpty(Password1Text.Password))
            {
                Password1ValidationAlert();
                displayValidation = true;
            }

            if(String.IsNullOrEmpty(Password2Text.Password))
            {
                Password2ValidationAlert();
                displayValidation = true;
            }

            if (displayValidation)
            {
                FieldsValidation.Visibility = Visibility.Visible;
            }
                       

            if (Password1Text.Password != Password2Text.Password && !String.IsNullOrEmpty(Password1Text.Password))
            {
                Password1ValidationAlert();
                Password2ValidationAlert();
                return;
            }


            if (!String.IsNullOrEmpty(Title.Text) && !String.IsNullOrEmpty(UserForenameText.Text) && !String.IsNullOrEmpty(UserSurnameText.Text) && !String.IsNullOrEmpty(UserEmailText.Text) && !String.IsNullOrEmpty(Password1Text.Password) && !String.IsNullOrEmpty(Password2Text.Password) && Password1Text.Password == Password2Text.Password)
            {

                FieldsValidation.Visibility = Visibility.Hidden;

                if (EmailValidation.Visibility == Visibility.Hidden)
                {
                  var  newUser = new TimetablingService.User()
                     {
                         UserTitle = Title.Text,
                         UserForename = UserForenameText.Text,
                         UserSurname = UserSurnameText.Text,
                         UserEmail = UserEmailText.Text,
                         Password = _client.Encrypt(Password2Text.Password)
                     };

                  _client.Register_User(newUser);

                }
                
            }
                    
        }

          public void OnTitleChange(object sender, RoutedEventArgs e)
            {
                if (Title.SelectedItem != null)
                {
                    UserTitle.Foreground = completed;
                    Title.BorderBrush = completed; 
                }
            }

          public void OnForenameChange(object sender, RoutedEventArgs e)
            {
                if (!String.IsNullOrEmpty(UserForenameText.Text))
                {
                    ForenameValidationComplete();
                }
            }
        public void OnSurnameChange(object sender, RoutedEventArgs e)
            {
                if (!String.IsNullOrEmpty(UserSurnameText.Text))
                {
                    SurnameValidationComplete();
                }
            }

        public void OnEmailChange(object sender, RoutedEventArgs e)
            {

                if (!String.IsNullOrEmpty(UserEmailText.Text))
                {
                    var temp = UserEmailText.Text;
                    if (!_client.Check_Email_Not_Exist(temp))
                    {
                        EmailValidationAlert();
                        EmailValidation.Text = String.Format("This Email address is already \n associated with an account.");
                        EmailValidation.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        Match match = _regex.Match(UserEmailText.Text);
                        if (match.Success)
                        {
                            EmailValidationComplete();
                        }
                        else
                        {
                            EmailValidationAlert();
                            EmailValidation.Text = String.Format("The Email address you have \n entered is not valid");
                            EmailValidation.Visibility = Visibility.Visible;
                        }
                    }
                }
            }

        public void OnPassword1Change(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Password1Text.Password))
            {
                Password1ValidationComplete();
            }
        }

        public void OnPassword2Change(object sender, RoutedEventArgs e)
        {
            if (Password2Text.Password != Password1Text.Password)
            {
               Password1ValidationAlert();
               Password2ValidationAlert();
            }
            else
            {
                Password1ValidationComplete();
                Password2ValidationComplete();
            }

        }

        public void TitleValidationAlert()
        {
            UserTitle.Foreground = alert;
            Title.BorderBrush = alert;
        }

        public void TitleValidationComplete()
        {
            UserTitle.Foreground = completed;
            Title.BorderBrush = completed;
        } 
        
        public void ForenameValidationAlert()
        {
            UserForename.Foreground = alert;
            UserForenameText.BorderBrush = alert; 
        }  
        
        public void ForenameValidationComplete()
        {
            UserForename.Foreground = completed;
            UserForenameText.BorderBrush = completed; 
        } 
        
        public void SurnameValidationAlert()
        {
            UserSurname.Foreground = alert;
            UserSurnameText.BorderBrush = alert; 
        } 
        
        public void SurnameValidationComplete()
        {
            UserSurname.Foreground = completed;
            UserSurnameText.BorderBrush = completed; 
        } 
        
        public void EmailValidationAlert()
        {
            UserEmail.Foreground = alert;
            UserEmailText.BorderBrush = alert;
            UserEmailText.Foreground = alert;
        }

        public void EmailValidationComplete()
        {
            UserEmail.Foreground = completed;
            UserEmailText.BorderBrush = completed;
            UserEmailText.Foreground = completed;
        } 
        
        public void Password1ValidationAlert()
        {
            Password1.Foreground = alert;
            Password1Text.BorderBrush = alert;
        }

        public void Password1ValidationComplete()
        {
            Password1.Foreground = completed;
            Password1Text.BorderBrush = completed;
        }

        public void Password2ValidationAlert()
        {
            Password2.Foreground = alert;
            Password2Text.BorderBrush = alert;
            PasswordValidation.Visibility = Visibility.Visible;
        } 
        
        public void Password2ValidationComplete()
        {
            Password2.Foreground = completed;
            Password2Text.BorderBrush = completed;
            PasswordValidation.Visibility = Visibility.Hidden;
        }
    }
}
