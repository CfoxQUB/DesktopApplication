using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using TimetablingClientApplication.TimetablingService;
using TimetablingClientApplication.Views.Database.Pages;

namespace TimetablingClientApplication.Views.Database.Windows
{
    /// <summary>
    /// Interaction logic for EditStudent.xaml
    /// </summary>
    public partial class EditStudent
    {
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        private readonly NavigationService _navigationService;

        private readonly int _userId;
        
        public ObservableCollection<String> Titles = new ObservableCollection<String>();

        public ObservableCollection<String> Courses = new ObservableCollection<String>();
        
        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);

        private readonly SolidColorBrush _normal = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE3E9EF"));

        private readonly Student _student = new Student();

        public EditStudent(int userId, int studentId, NavigationService navigationService)
        {
            _userId = userId;

            _navigationService = navigationService;

            _student = _client.ReturnStudentDetail(studentId);

            var courses = _client.ReturnCourses();

            InitializeComponent();

            Titles.Add("Mr");
            Titles.Add("Mrs");
            Titles.Add("Miss");
            Titles.Add("Ms");
            Titles.Add("Dr");

            Title.ItemsSource = Titles;

            ValidationMessage.Foreground = _alert;

            if (courses != null)
            {
                Title.SelectedItem = Titles.SingleOrDefault(x => x == _student.StudentTitle);
                Forename.Text = _student.StudentForename;
                Surname.Text = _student.StudentSurname;
                Email.Text = _student.StudentEmail;
                Year.Text = _student.Year.ToString("D");

                foreach (var c in courses)
                {
                    Courses.Add(c.CourseName);
                }

                var selectedCourse = courses.SingleOrDefault(x => x.CourseId == _student.Course);
                Course.ItemsSource = Courses;

                if (selectedCourse != null)
                {
                    Course.Text = selectedCourse.CourseName;
                }

                return;
            }

            Title.SelectedItem = "No Selection";
            Forename.Text = "N/A";
            Surname.Text = "N/A";
            Email.Text = "N/A";
            Course.SelectedItem = "N/A";

        }

        /// <summary>
        /// http://www.codegateway.com/2012/03/c-regex-for-email-address.html
        /// </summary>
        /// <returns></returns>
        public bool ValidationOnFields()
        {
            Title.BorderBrush = _normal;
            Forename.BorderBrush = _normal;
            Surname.BorderBrush = _normal;
            Email.BorderBrush = _normal;
            Title.BorderBrush = _normal;
            Course.BorderBrush = _normal;
            Year.BorderBrush = _normal;

            var regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            var regexNumber = new Regex("^[0-9]+$");

            Match matchEmail = regexEmail.Match(Email.Text);
            Match matchNumber = regexNumber.Match(Year.Text);


            if (!String.IsNullOrEmpty(Forename.Text) && !String.IsNullOrEmpty(Surname.Text) && !String.IsNullOrEmpty(Email.Text) )
            {
                if (!CheckPassword())
                {
                    return false;
                }

                if (!matchEmail.Success)
                {
                    ValidationMessage.Content = "Email address is invalid";
                    ValidationMessage.Visibility = Visibility.Visible;
                    return false;
                }

                if (!matchNumber.Success)
                {
                    ValidationMessage.Content = "Year must be a numeric value;";
                    ValidationMessage.Visibility = Visibility.Visible;
                    return false;
                }

                ValidationMessage.Visibility = Visibility.Hidden;
                return true;
            }

            ValidationMessage.Content = "Save Changes failed. Please ensure all fields are completed";
            Title.BorderBrush = _alert;
            Forename.BorderBrush = _alert;
            Surname.BorderBrush = _alert;
            Email.BorderBrush = _alert;
            Course.BorderBrush = _alert;

            return false;
        }

        public void SaveStudent(object sender, RoutedEventArgs e)
        {
            var password = _student.Password;

            if (ValidationOnFields())
            {
                if (CheckPassword())
                {
                    password = _client.Encrypt(Password.Text);
                }

                var courseSelected = _client.ReturnCourseIdFromCourseName(Course.Text);
                if (courseSelected != 0)
                {
                    var course = _client.ReturnCourseDetail(courseSelected);
                    if (course.CourseId != 0)
                    {
                        var result = _client.EditStudent(_student.StudentId, Title.Text, Forename.Text, Surname.Text, Email.Text, password,
                            course.CourseId, Convert.ToInt32(Year.Text));
                        if (result)
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

        public bool CheckPassword()
        {
            ValidationMessage.Visibility = Visibility.Hidden;
            Password.BorderBrush = _normal;
            ConfirmPassword.BorderBrush = _normal;

            if (String.IsNullOrEmpty(ConfirmPassword.Text) && String.IsNullOrEmpty(Password.Text))
            {
                return true;
            }

            if (Password.Text == ConfirmPassword.Text && !String.IsNullOrEmpty(Password.Text))
            {
                return true;
            }

            ValidationMessage.Content = "Passwords cannot be saved until both fields match.";
            Password.BorderBrush = _alert;
            ConfirmPassword.BorderBrush = _alert;
            ValidationMessage.Visibility = Visibility.Visible;
            return false;
        }


    }
}
