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

        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        private readonly NavigationService _navigationService;

        private readonly int _userId;

        public ObservableCollection<String> Titles = new ObservableCollection<String>();

        public ObservableCollection<String> Courses = new ObservableCollection<String>();

        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);

        private readonly SolidColorBrush _normal = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE3E9EF"));

        public CreateNewStaff(int userId, NavigationService navigationService)
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
        }

        public bool ValidationOnFields()
        {
            Forename.BorderBrush = _normal;
            Surname.BorderBrush = _normal;
            Password.BorderBrush = _normal;
            Email.BorderBrush = _normal;
            Course.BorderBrush = _normal;
            Title.BorderBrush = _normal;

            var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(Email.Text);

            ValidationMessage.Foreground = _alert;

            if (!String.IsNullOrEmpty(Title.Text) && !String.IsNullOrEmpty(Forename.Text) && !String.IsNullOrEmpty(Email.Text) && !String.IsNullOrEmpty(Surname.Text) && !String.IsNullOrEmpty(Password.Text) && !String.IsNullOrEmpty(Course.Text))
            {
                if (!match.Success)
                {
                    ValidationMessage.Content = "Email address is invalid.";
                    Email.BorderBrush = _alert;
                    ValidationMessage.Visibility = Visibility.Visible;
                    return false;
                }

                if (_client.CheckStaffExists(Email.Text))
                {
                    ValidationMessage.Visibility = Visibility.Visible;
                    ValidationMessage.Content = "Staff creation failed. A Staff member exists with the same email address.";
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
            Course.BorderBrush = _alert;

            ValidationMessage.Foreground = _alert;
            ValidationMessage.Visibility = Visibility.Visible;
            ValidationMessage.Content = "Please ensure all fields are completed.";
            return false;
        }

        
        public void SubmitNewStaff(object sender, RoutedEventArgs e)
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
                        var result = _client.CreateStaff(Title.Text, Forename.Text, Surname.Text, Email.Text, encryptedPassword,
                            course.CourseId, _userId);
                        if (result != 0)
                        {
                            OpenSuccessPopup();
                            _navigationService.Navigate(new StaffManagement(_userId, _navigationService));
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

        public void OpenCoursePopup()
        {
            NoCourses.IsOpen = true;
        }

        public void CloseCoursePopup(object sender, RoutedEventArgs e)
        {
            NoCourses.IsOpen = false;
            Close();
        }

        
     }
}
