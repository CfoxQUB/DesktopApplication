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
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        private readonly NavigationService _navigationService;

        private readonly int _userId;

        public ObservableCollection<String> Titles = new ObservableCollection<String>();

        public ObservableCollection<String> Courses = new ObservableCollection<String>();

        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);

        private readonly SolidColorBrush _normal = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE3E9EF"));

        public CreateNewStudent(int userId, NavigationService navigationService)
        {
            _userId = userId;

            _navigationService = navigationService;

            InitializeComponent();

            var courses = _client.ReturnCourses();

            if (courses != null)
            {
                foreach (var c in courses)
                {
                    Courses.Add(c.CourseName);
                }
            }

            Course.ItemsSource = Courses;

            Titles.Add("Mr");
            Titles.Add("Mrs");
            Titles.Add("Miss");
            Titles.Add("Ms");

            Title.ItemsSource = Titles;
            Forename.Text = "";
            Surname.Text = "";
            Password.Text = "Password";
            Email.Text = "";
            Year.Text = "";
        }

        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// http://www.codegateway.com/2012/03/c-regex-for-email-address.html
        /// </summary>
        public bool ValidationOnFields()
        {
            Forename.BorderBrush = _normal;
            Surname.BorderBrush = _normal;
            Password.BorderBrush = _normal;
            Email.BorderBrush = _normal;
            Course.BorderBrush = _normal;
            Title.BorderBrush = _normal;

            var regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            var regexNumber = new Regex("^[0-9]+$");

            Match matchEmail = regexEmail.Match(Email.Text);
            Match matchNumber = regexNumber.Match(Year.Text);

            ValidationMessage.Foreground = _alert;

            if (!String.IsNullOrEmpty(Title.Text) && !String.IsNullOrEmpty(Year.Text)&& !String.IsNullOrEmpty(Forename.Text) && !String.IsNullOrEmpty(Email.Text) && !String.IsNullOrEmpty(Surname.Text) && !String.IsNullOrEmpty(Password.Text) && !String.IsNullOrEmpty(Course.Text))
            {
                if (!matchEmail.Success)
                {
                    ValidationMessage.Content = "Email address is invalid.";
                    Email.BorderBrush = _alert;
                    ValidationMessage.Visibility = Visibility.Visible;
                    return false;
                }
                
                if (!matchNumber.Success)
                {
                    ValidationMessage.Content = "Year must be a numeric value";
                    Year.BorderBrush = _alert;
                    ValidationMessage.Visibility = Visibility.Visible;
                    return false;
                }

                if (_client.CheckStudentExists(Email.Text))
                {
                    ValidationMessage.Visibility = Visibility.Visible;
                    ValidationMessage.Content = "Student creation failed. A Student exists with the same email address.";
                    return false;
                }

                ValidationMessage.Visibility = Visibility.Hidden;
                return true;
            }

            Forename.BorderBrush = _alert;
            Surname.BorderBrush = _alert;
            Title.BorderBrush = _alert;
            Password.BorderBrush = _alert;
            Email.BorderBrush = _alert;
            Year.BorderBrush = _alert;
            Course.BorderBrush = _alert;

            ValidationMessage.Foreground = _alert;
            ValidationMessage.Visibility = Visibility.Visible;
            ValidationMessage.Content = "Please ensure all fields are completed.";
            return false;
        }

        
        public void SubmitNewStudent(object sender, RoutedEventArgs e)
        {
            if (ValidationOnFields())
            {
                var courseSelected = _client.ReturnCourseIdFromCourseName(Course.Text);
                if (courseSelected != 0)
                {
                    var course = _client.ReturnCourseDetail(courseSelected);
                    if (course.CourseId != 0)
                    {
                        var encryptedPassword = _client.Encrypt(Password.Text);
                        var result = _client.CreateStudent(Title.Text, Forename.Text, Surname.Text, Email.Text, encryptedPassword,
                            course.CourseId,Convert.ToInt32(Year.Text), _userId);

                        if (result != 0)
                        {
                            OpenSuccessPopup();
                            _navigationService.Navigate(new StudentManagement(_userId, _navigationService));
                        }
                    }
                }
            }
        }

        public void OpenSuccessPopup()
        {
            Success.IsOpen = true;
        }

        public void CloseSuccessPopup(object sender, RoutedEventArgs e)
        {
            Success.IsOpen = false;
            Close();
        }
        
     }
    }

