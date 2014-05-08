using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Navigation;
using TimetablingClientApplication.TimetablingService;
using System.Windows;
using TimetablingClientApplication.Views.Database.Pages;

namespace TimetablingClientApplication.Views.Database.Windows
{
    /// <summary>
    /// Interaction logic for CreateNewStaff.xaml
    /// </summary>
    public partial class CreateNewStaff 
    {
        //webservice functionality exposed through service reference
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        //User Id and navigation service of main window frame maintained
        private readonly NavigationService _navigationService;
        private readonly int _userId;

        //Observble collections used to populate titles and courses
        public ObservableCollection<String> Titles = new ObservableCollection<String>();
        public ObservableCollection<String> Courses = new ObservableCollection<String>();

        //validation colours
        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush _normal = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE3E9EF"));

        //Window initialized
        public CreateNewStaff(int userId, NavigationService navigationService)
        {
            //User Id and navigation service of main window frame maintained
            _userId = userId;
            _navigationService = navigationService;
            //window initialized
            InitializeComponent();
            //Course available returned
            var courses = _client.ReturnCourses();
            //If courses are availabel course select populated
            if (courses != null)
            {
                foreach (var c in courses)
                {
                    Courses.Add(c.CourseName);
                }
                Course.ItemsSource = Courses;
            }
            else
            {
                //No course popup opend and redirect on close
                NoCourses.IsOpen = true;
                return;
            }

            //Titles added
            Titles.Add("Mr");
            Titles.Add("Mrs");
            Titles.Add("Miss");
            Titles.Add("Ms");
            Title.ItemsSource = Titles;
            
            //Default values added to text fields in window
            Forename.Text = "";
            Surname.Text = "";
            Password.Text = "Password";
            Email.Text = "";
        }

        /// <summary>
        /// http://www.codegateway.com/2012/03/c-regex-for-email-address.html
        /// Validation on email address and other fields to ensure correct values passed through
        /// </summary>
        /// <returns></returns>
        public bool ValidationOnFields()
        {
            //Valdation colour of fileds is reset
            Forename.BorderBrush = _normal;
            Surname.BorderBrush = _normal;
            Password.BorderBrush = _normal;
            Email.BorderBrush = _normal;
            Course.BorderBrush = _normal;
            Title.BorderBrush = _normal;
            //new regex created for comparison to emaila ddress
            var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            //Check against email address entered
            Match match = regex.Match(Email.Text.Trim());

            ValidationMessage.Foreground = _alert;
            //Check to ensure fields are completed
            if (!String.IsNullOrEmpty(Title.Text) && !String.IsNullOrEmpty(Forename.Text) && !String.IsNullOrEmpty(Email.Text) && !String.IsNullOrEmpty(Surname.Text) && !String.IsNullOrEmpty(Password.Text) && !String.IsNullOrEmpty(Course.Text))
            {
                //check against regex result to ensure email address valid
                if (!match.Success)
                {//Vlaidation message set to reflect incorrect email
                    ValidationMessage.Content = "Email address is invalid.";
                    Email.BorderBrush = _alert;
                    ValidationMessage.Visibility = Visibility.Visible;
                    return false;
                }
                //Check to emsure email address is not already assigned to another staff memeber
                if (_client.CheckStaffExists(Email.Text.Trim()))
                {//Vlaidation message set to reflect existing email
                    ValidationMessage.Visibility = Visibility.Visible;
                    ValidationMessage.Content = "Staff creation failed. A Staff member exists with the same email address.";
                    return false;
                }
                //Validation passed
                ValidationMessage.Visibility = Visibility.Hidden;
                return true;
            }
            //Validation failed
            Forename.BorderBrush = _alert;
            Surname.BorderBrush = _alert;
            Title.BorderBrush = _alert;
            Password.BorderBrush = _alert;
            Email.BorderBrush = _alert;
            Course.BorderBrush = _alert;
            //Validation alert set
            ValidationMessage.Foreground = _alert;
            ValidationMessage.Visibility = Visibility.Visible;
            ValidationMessage.Content = "Please ensure all fields are completed.";
            return false;
        }

        //Submission of new staff memeber from values in page
        public void SubmitNewStaff(object sender, RoutedEventArgs e)
        {   //Validation check
            if (ValidationOnFields())
            {//validation passed
                var courseSelected = _client.ReturnCourseIdFromCourseName(Course.Text);
                //Course selection validated
                if (courseSelected != 0)
                {
                    //Coursed detail returned
                    var course = _client.ReturnCourseDetail(courseSelected);
                    if (course.CourseId != 0)
                    {
                        //Password encryped
                        var encryptedPassword = _client.Encrypt(Password.Text);
                        //staff memebr created
                        var result = _client.CreateStaff(Title.Text, Forename.Text, Surname.Text, Email.Text, encryptedPassword,
                            course.CourseId, _userId);
                        if (result != 0)
                        {//creation success
                            Success.IsOpen = true;
                        }
                        else
                        {
                            //staff creation failed
                            ValidationMessage.Visibility = Visibility.Visible;
                            ValidationMessage.Content = "Staff Creation failed. Please contact you system Administrator";
                        }
                    }
                }
            }
        }

        //Close Success popup function and redirect
        public void CloseSuccessPopup(object sender, RoutedEventArgs e)
        {
            //Popup closed
            Success.IsOpen = false;
            //Redirect 
            _navigationService.Navigate(new StaffManagement(_userId, _navigationService));
            //close window
            Close();
        }

        //Close  Course popup
        public void CloseCoursePopup(object sender, RoutedEventArgs e)
        {  
            //Close Popup
            NoCourses.IsOpen = false;
            //Redirect
            _navigationService.Navigate(new StaffManagement(_userId, _navigationService));
            //close window
            Close();
        }

        
     }
}
