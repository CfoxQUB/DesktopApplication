using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using TimetablingClientApplication.TimetablingService;

namespace TimetablingClientApplication.Views.Startup.Windows
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register 
    {
        //Webservice functionality exposed through webservice reference
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        //validaiton colours
        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush _completed = new SolidColorBrush(Colors.Green);
        //titles list
        private readonly ObservableCollection<string> _list = new ObservableCollection<string>();

        //regular expression used to generate valid email addresses
        //http://www.codegateway.com/2012/03/c-regex-for-email-address.html
        private readonly Regex _regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
       
        public Register()
        {
            InitializeComponent();
                //Title setup as is only field which requires it
                _list.Add("Mr");
                _list.Add("Mrs");
                _list.Add("Miss");
                _list.Add("Ms");
                _list.Add("Dr");
                Title.ItemsSource = _list;
                Title.Text = "";
        }

        //submission of details entered in the appropriate fields
        public void SubmitNewUser(object sender, RoutedEventArgs e)
        {
            //navigation reset
            bool displayValidation = false;
            bool emailValidation = false;
            //check Title field has been completed
            if (String.IsNullOrEmpty(Title.Text))
            {
                TitleValidationAlert();
                displayValidation = true;
            }
            //check Forename field has been completed
            if (String.IsNullOrEmpty(UserForenameText.Text))
            {
                ForenameValidationAlert();
                displayValidation = true;
            }
            //check surname field has been completed
            if (String.IsNullOrEmpty(UserSurnameText.Text))
            {
                SurnameValidationAlert();
                displayValidation = true;
            }
            //check email field has been completed
            if (String.IsNullOrEmpty(UserEmailText.Text))
            {
                EmailValidationAlert();
                displayValidation = true;

            }
            else
            {
                //check to ensure email field contains a valid email address
                if (!_client.Check_Email_Not_Exist(UserEmailText.Text))
                {
                    //validation failed
                    EmailValidationAlert();
                    EmailValidation.Text = String.Format("This Email address is already \n associated with an account.");
                    emailValidation = true;
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
                        emailValidation = true;
                    }
                }

            }
            //check that password field has been passed
            if (String.IsNullOrEmpty(Password1Text.Password))
            {
                Password1ValidationAlert();
                displayValidation = true;
            }

            //check that password confirmation field has passed
            if(String.IsNullOrEmpty(Password2Text.Password))
            {
                Password2ValidationAlert();
                displayValidation = true;
            }

            //if an field has not passed basic validation, alert dispalyed
            if (displayValidation)
            {
                FieldsValidation.Visibility = Visibility.Visible;
            }
            
            //if an email has not passed basic validation, alert dispalyed
            if (emailValidation)
            {
                EmailValidation.Visibility = Visibility.Visible;
            }
                       
            //check to ensure passswords entered match
            if (Password1Text.Password != Password2Text.Password && !String.IsNullOrEmpty(Password1Text.Password))
            {
                Password1ValidationAlert();
                Password2ValidationAlert();
                return;
            }

            //if all fields completed new user creted for submission
            if (!String.IsNullOrEmpty(Title.Text) && !String.IsNullOrEmpty(UserForenameText.Text) && !String.IsNullOrEmpty(UserSurnameText.Text) && !String.IsNullOrEmpty(UserEmailText.Text) && !String.IsNullOrEmpty(Password1Text.Password) && !String.IsNullOrEmpty(Password2Text.Password) && Password1Text.Password == Password2Text.Password)
            {
                FieldsValidation.Visibility = Visibility.Hidden;
                if (!displayValidation && !emailValidation)
                {
                    //new user created and user password hashed
                    var  newUser = new User
                     {
                         UserTitle = Title.Text,
                         UserForename = UserForenameText.Text,
                         UserSurname = UserSurnameText.Text,
                         UserEmail = UserEmailText.Text,
                         Password = _client.Encrypt(Password2Text.Password)
                     };
                    //new user submitted
                  _client.Register_User(newUser);

                }
                
            }
                    
        }
          //Validation to show title field has been completed
          public void OnTitleChange(object sender, RoutedEventArgs e)
            {
                if (Title.SelectedItem != null)
                {
                    UserTitle.Foreground = _completed;
                    Title.BorderBrush = _completed; 
                }
            }

          //Validation to show forename field has been completed
          public void OnForenameChange(object sender, RoutedEventArgs e)
            {
                if (!String.IsNullOrEmpty(UserForenameText.Text))
                {
                    ForenameValidationComplete();
                }
            }
          //Validation to show surname field has been completed
        public void OnSurnameChange(object sender, RoutedEventArgs e)
            {
                if (!String.IsNullOrEmpty(UserSurnameText.Text))
                {
                    SurnameValidationComplete();
                }
            }
        //Validation to show email field has been completed
        public void OnEmailChange(object sender, RoutedEventArgs e)
            {

                if (!String.IsNullOrEmpty(UserEmailText.Text))
                {
                    var temp = UserEmailText.Text;
                    //check to ensure email does not exist with another staff member
                    if (!_client.Check_Email_Not_Exist(temp))
                    {
                        EmailValidationAlert();
                        EmailValidation.Text = String.Format("This Email address is already \n associated with an account.");
                        EmailValidation.Visibility = Visibility.Visible;
                    }
                    else
                    {  //Check email address that is not associated with a staff member is a valid email address
                        Match match = _regex.Match(UserEmailText.Text);
                        if (match.Success)
                        {
                            EmailValidationComplete();
                            EmailValidation.Visibility = Visibility.Hidden;
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
        //Validation to show first password field has been completed       
        public void OnPassword1Change(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(Password1Text.Password))
            {
                Password1ValidationComplete();
            }
        }
        //Validation to show second password has been completed
        public void OnPassword2Change(object sender, RoutedEventArgs e)
        {
            //Password fields must match
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

        #region Alert and completion validation displayed on fields

        public void TitleValidationAlert()
        {
            UserTitle.Foreground = _alert;
            Title.BorderBrush = _alert;
        }

        public void TitleValidationComplete()
        {
            UserTitle.Foreground = _completed;
            Title.BorderBrush = _completed;
        } 
        
        public void ForenameValidationAlert()
        {
            UserForename.Foreground = _alert;
            UserForenameText.BorderBrush = _alert; 
        }  
        
        public void ForenameValidationComplete()
        {
            UserForename.Foreground = _completed;
            UserForenameText.BorderBrush = _completed; 
        } 
        
        public void SurnameValidationAlert()
        {
            UserSurname.Foreground = _alert;
            UserSurnameText.BorderBrush = _alert; 
        } 
        
        public void SurnameValidationComplete()
        {
            UserSurname.Foreground = _completed;
            UserSurnameText.BorderBrush = _completed; 
        } 
        
        public void EmailValidationAlert()
        {
            UserEmail.Foreground = _alert;
            UserEmailText.BorderBrush = _alert;
            UserEmailText.Foreground = _alert;
        }

        public void EmailValidationComplete()
        {
            UserEmail.Foreground = _completed;
            UserEmailText.BorderBrush = _completed;
            UserEmailText.Foreground = _completed;
        } 
        
        public void Password1ValidationAlert()
        {
            Password1.Foreground = _alert;
            Password1Text.BorderBrush = _alert;
        }

        public void Password1ValidationComplete()
        {
            Password1.Foreground = _completed;
            Password1Text.BorderBrush = _completed;
        }

        public void Password2ValidationAlert()
        {
            Password2.Foreground = _alert;
            Password2Text.BorderBrush = _alert;
            PasswordValidation.Visibility = Visibility.Visible;
        } 
        
        public void Password2ValidationComplete()
        {
            Password2.Foreground = _completed;
            Password2Text.BorderBrush = _completed;
            PasswordValidation.Visibility = Visibility.Hidden;
        }
        #endregion
    }
}
