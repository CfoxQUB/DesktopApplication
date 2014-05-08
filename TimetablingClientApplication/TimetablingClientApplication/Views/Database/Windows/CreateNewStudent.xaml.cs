using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using TimetablingClientApplication.TimetablingService;
using TimetablingClientApplication.Views.Database.Pages;

namespace TimetablingClientApplication.Views.Database.Windows
{
    /// <summary>
    /// Interaction logic for CreateNewStudent.xaml
    /// </summary>
    public partial class CreateNewStudent
    {
        //Exposing Web service functionality from service reference
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        //Maintain user Id and naviagtion service from main window frame
        private readonly int _userId;
        private readonly NavigationService _navigationService;
        //Observable collection for titles and courses list popoulation
        public ObservableCollection<String> Titles = new ObservableCollection<String>();
        public ObservableCollection<String> Courses = new ObservableCollection<String>();
        //validation colours for setting and resetting validation on fields
        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush _normal = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE3E9EF"));

        //Window initialized
        public CreateNewStudent(int userId, NavigationService navigationService)
        {
            //UserId and Navigation service maintained
            _userId = userId;
            _navigationService = navigationService;
            //Window initialized
            InitializeComponent();

            //Courses returned that are available
            var courses = _client.ReturnCourses();
            if (courses != null)
            {   //courses returned populated into list
                foreach (var c in courses)
                {
                    Courses.Add(c.CourseName);
                }
                Course.ItemsSource = Courses;
            }
            else
            {   //no courses available
                NoCourses.IsOpen = true;
                return;
            }
            //titles added
            Titles.Add("Mr");
            Titles.Add("Mrs");
            Titles.Add("Miss");
            Titles.Add("Ms");
            Title.ItemsSource = Titles;

            //Default page values set
            Forename.Text = "";
            Surname.Text = "";
            Password.Text = "Password";
            Email.Text = "";
            Year.Text = "";
        }

        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// http://www.codegateway.com/2012/03/c-regex-for-email-address.html
        /// Validation on fields to ensure format and content correct
        /// </summary>
        public bool ValidationOnFields()
        {
            //Validation colors reset
            Forename.BorderBrush = _normal;
            Surname.BorderBrush = _normal;
            Password.BorderBrush = _normal;
            Email.BorderBrush = _normal;
            Course.BorderBrush = _normal;
            Title.BorderBrush = _normal;
            //regex for email and number set for comparison
            var regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            var regexNumber = new Regex("^[0-9]+$");
            //regex comparison and number checked and result stored
            Match matchEmail = regexEmail.Match(Email.Text);
            Match matchNumber = regexNumber.Match(Year.Text);

            ValidationMessage.Foreground = _alert;
            //Check to ensure all fields completed
            if (!String.IsNullOrEmpty(Title.Text) && !String.IsNullOrEmpty(Year.Text)&& !String.IsNullOrEmpty(Forename.Text) && !String.IsNullOrEmpty(Email.Text) && !String.IsNullOrEmpty(Surname.Text) && !String.IsNullOrEmpty(Password.Text) && !String.IsNullOrEmpty(Course.Text))
            {
                //Check to ensure email address is valid
                if (!matchEmail.Success)
                {   //Validation failed
                    ValidationMessage.Content = "Email address is invalid.";
                    Email.BorderBrush = _alert;
                    ValidationMessage.Visibility = Visibility.Visible;
                    return false;
                }
                //Check to ensure year number valid
                if (!matchNumber.Success)
                {
                    ValidationMessage.Content = "Year must be a numeric value";
                    Year.BorderBrush = _alert;
                    ValidationMessage.Visibility = Visibility.Visible;
                    return false;
                }

                //check to ensure email address that is now validated is not registered to another staff memeber
                if (_client.CheckStudentExists(Email.Text))
                {
                    ValidationMessage.Visibility = Visibility.Visible;
                    ValidationMessage.Content = "Student creation failed. A Student exists with the same email address.";
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
            Year.BorderBrush = _alert;
            Course.BorderBrush = _alert;
            //Validatino message set and displayed
            ValidationMessage.Foreground = _alert;
            ValidationMessage.Visibility = Visibility.Visible;
            ValidationMessage.Content = "Please ensure all fields are completed.";
            return false;
        }

        //Submission of new student
        public void SubmitNewStudent(object sender, RoutedEventArgs e)
        {
            //Validation checks on fields content anf format
            if (ValidationOnFields())
            {
                //Selcetion of course
                var courseSelected = _client.ReturnCourseIdFromCourseName(Course.Text);
                if (courseSelected != 0)
                {//Course seletion valid
                    var course = _client.ReturnCourseDetail(courseSelected);
                    if (course.CourseId != 0)
                    {//course detail valid
                        var encryptedPassword = _client.Encrypt(Password.Text);
                        //Student submitted to service
                        var result = _client.CreateStudent(Title.Text, Forename.Text, Surname.Text, Email.Text, encryptedPassword,
                            course.CourseId,Convert.ToInt32(Year.Text), _userId);
                        if (result != 0)
                        {//student creation success
                            Success.IsOpen = true;
                        }
                        else
                        {
                            //student creation failed
                            ValidationMessage.Content = "Student Creation Failed. Please contact your System Administrator";
                            ValidationMessage.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }

        //Close Success Popup and redirect
        public void CloseSuccessPopup(object sender, RoutedEventArgs e)
        {
            //Popup closed
            Success.IsOpen = false;
            //Redirect
            _navigationService.Navigate(new StudentManagement(_userId, _navigationService));
            //Window closed
            Close();
        }
        
        public void CloseCoursePopup(object sender, RoutedEventArgs e)
        {
            //Popoup closed
            NoCourses.IsOpen = false;
            //Redirect
            _navigationService.Navigate(new StudentManagement(_userId, _navigationService));
            //Window closed
            Close();
        }
        
     }
    }

