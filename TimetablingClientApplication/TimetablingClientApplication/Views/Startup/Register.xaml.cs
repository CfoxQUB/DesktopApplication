using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace TimetablingClientApplication
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        TimetablingService.TimetablingServiceClient client = new TimetablingService.TimetablingServiceClient();
        SolidColorBrush alert = new SolidColorBrush(Colors.Red);
        SolidColorBrush completed = new SolidColorBrush(Colors.Green);
        public ObservableCollection<string> list = new ObservableCollection<string>();

        public Register()
        {
            InitializeComponent();
           
                list.Add("Mr");
                list.Add("Mrs");
                list.Add("Ms");
                list.Add("Dr");
                this.Title.ItemsSource = list;
                this.Title.Text = "";
        }

        public void SubmitNewUser(object sender, RoutedEventArgs e)
        {
            bool displayValidation = false;

            if (Title.Text == "")
            {
                userTitle.Foreground = alert;
                Title.BorderBrush = alert;
                displayValidation = true;
            }

            if (userForenameText.Text == "")
            {
                userForename.Foreground = alert;
                userForenameText.BorderBrush = alert;
                displayValidation = true;
            }

            if (userSurnameText.Text == "")
            {
                userSurname.Foreground = alert;
                userSurnameText.BorderBrush = alert;
                displayValidation = true;
            }

            if (userEmailText.Text == "")
            {
                userEmail.Foreground = alert;
                userEmailText.BorderBrush = alert;
                displayValidation = true;
            }
            else
            {
                if (!client.Check_Email_Not_Exist(userEmailText.Text))
                {
                    userEmail.Foreground = alert;
                    userEmailText.BorderBrush = alert;
                    
                    emailValidation.Visibility = Visibility.Visible;
                }
                else
                {
                    emailValidation.Visibility = Visibility.Hidden;
                }
                      
            }

            if (password1Text.Password == "")
            {
                password1.Foreground = alert;
                password1Text.BorderBrush = alert;
                displayValidation = true;
            }

            if(password2Text.Password == "")
            {
                password2.Foreground = alert;
                password2Text.BorderBrush = alert;
                displayValidation = true;
            }

            if (displayValidation == true)
            {
                fieldsValidation.Visibility = Visibility.Visible;
            }
                           

            if (password1Text.Password != password2Text.Password && password1Text.Password != "")
            {
                password1.Foreground = alert;
                password1Text.BorderBrush =  alert;

                password2.Foreground = alert;
                password2Text.BorderBrush = alert;

                passwordValidation.Visibility = Visibility.Visible;
                return;
            }


            if (Title.Text != "" && userForenameText.Text != "" && userSurnameText.Text != "" && userEmailText.Text != "" && password1Text.Password != "" && password2Text.Password != "" && password1Text.Password == password2Text.Password)
            {

                fieldsValidation.Visibility = Visibility.Hidden;

                if (emailValidation.Visibility == Visibility.Hidden)
                {
                  var  newUser = new TimetablingService.User()
                     {
                         UserTitle = Title.Text,
                         UserForename = userForenameText.Text,
                         UserSurname = userSurnameText.Text,
                         UserEmail = userEmailText.Text,
                         UserType = 1,
                         Password = password2Text.Password,
                         CreateDate = DateTime.Now
                     };

                  client.Register_User(newUser);
                  return;

                }
                
            }
                    
        }

          public void onTitleChange(object sender, RoutedEventArgs e)
            {
                if (Title.SelectedItem != null)
                {
                    userTitle.Foreground = completed;
                    Title.BorderBrush = completed; 
                }
            }

          public void onForenameChange(object sender, RoutedEventArgs e)
            {
                if (userForenameText.Text != "")
                {
                    userForename.Foreground = completed;
                    userForenameText.BorderBrush = completed; 
                }
            }
        public void onSurnameChange(object sender, RoutedEventArgs e)
            {
                if (userSurnameText.Text != "")
                {
                    userSurname.Foreground = completed;
                    userSurnameText.BorderBrush = completed; 
                }
            }

        public void onEmailChange(object sender, RoutedEventArgs e)
            {
                if (userEmailText.Text != "")
                {
                    if (!client.Check_Email_Not_Exist(userEmailText.Text))
                    {
                        userEmail.Foreground = alert;
                        userEmailText.BorderBrush = alert;
                        userEmailText.Foreground = alert;

                        emailValidation.Visibility = Visibility.Visible;
                        return;
                    }
                    else
                    {
                        userEmail.Foreground = completed;
                        userEmailText.BorderBrush = completed;
                        userEmailText.Foreground = new SolidColorBrush(Colors.Black); ;
                        emailValidation.Visibility = Visibility.Hidden;
                        return;
                    }
                }
                
            }

        public void onPassword1Change(object sender, RoutedEventArgs e)
        {
            if (password1Text.Password != "")
            {
                password1.Foreground = completed;
                password1Text.BorderBrush = completed;
            }
        }

        public void onPassword2Change(object sender, RoutedEventArgs e)
        {
            if (password2Text.Password != password1Text.Password)
            {
                password1.Foreground = alert;
                password1Text.BorderBrush = alert;

                password2.Foreground = alert;
                password2Text.BorderBrush = alert;

                passwordValidation.Visibility = Visibility.Visible;

            }
            else
            {
                password1.Foreground = completed;
                password1Text.BorderBrush = completed;

                password2.Foreground = completed;
                password2Text.BorderBrush = completed;
                
                passwordValidation.Visibility = Visibility.Hidden;
            }

        }
    }
}
