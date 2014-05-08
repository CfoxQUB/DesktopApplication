using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using TimetablingClientApplication.TimetablingService;
using TimetablingClientApplication.Views.Database.Pages;

namespace TimetablingClientApplication.Views.Database.Windows
{
    /// <summary>
    /// Interaction logic for EditStudent.xaml
    /// </summary>
    public partial class EditStudent
    {
        //Web service functionality exposed through service reference
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        //User id and navigation service maintained
        private readonly int _userId;
        private readonly NavigationService _navigationService;
        //Titles and courses listed
        public ObservableCollection<String> Titles = new ObservableCollection<String>();
        public ObservableCollection<String> Courses = new ObservableCollection<String>();
        //Validation colors to indicate errors and reset of such
        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush _normal = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE3E9EF"));
        //student onject to be edited
        private readonly Student _student = new Student();

        //Window created
        public EditStudent(int userId, int studentId, NavigationService navigationService)
        {
            //User Id for editing and navigatino service for redirect
            _userId = userId;
            _navigationService = navigationService;
            InitializeComponent();

            //titkles list created
            Titles.Add("Mr");
            Titles.Add("Mrs");
            Titles.Add("Miss");
            Titles.Add("Ms");
            Titles.Add("Dr");
            Title.ItemsSource = Titles;

            //Student details returned
            var student = _client.ReturnStudentDetail(studentId);
            if (student != null)
            {
                _student = student;
                //set information on page of selected student
                Title.SelectedItem = Titles.SingleOrDefault(x => x == _student.StudentTitle);
                Forename.Text = _student.StudentForename;
                Surname.Text = _student.StudentSurname;
                Email.Text = _student.StudentEmail;
                Year.Text = _student.Year.ToString("D");
            }
            //courses list returned
            var courses = _client.ReturnCourses();
            //validation message colour set
            ValidationMessage.Foreground = _alert;
            //course select list population
            if (courses != null)
            {
                //Courses select populated
                foreach (var c in courses)
                {
                    Courses.Add(c.CourseName);
                }
                //student selected course
                var selectedCourse = courses.SingleOrDefault(x => x.CourseId == _student.Course);
                Course.ItemsSource = Courses;
                if (selectedCourse != null)
                {   //setting slected course toreflect student
                    Course.Text = selectedCourse.CourseName;
                }

                return;
            }
            //defaults if student selceted isnt valid
            Title.SelectedItem = "No Selection";
            Forename.Text = "N/A";
            Surname.Text = "N/A";
            Email.Text = "N/A";
            Course.SelectedItem = "N/A";

        }

        /// <summary>
        /// http://www.codegateway.com/2012/03/c-regex-for-email-address.html
        /// Validation on field format and content
        /// </summary>
        /// <returns></returns>
        public bool ValidationOnFields()
        {
            //Validation colours reset
            Title.BorderBrush = _normal;
            Forename.BorderBrush = _normal;
            Surname.BorderBrush = _normal;
            Email.BorderBrush = _normal;
            Title.BorderBrush = _normal;
            Course.BorderBrush = _normal;
            Year.BorderBrush = _normal;
            //Reges for comparison against email and number
            var regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            var regexNumber = new Regex("^[0-9]+$");
            //comparison of fieldss against email and number
            Match matchEmail = regexEmail.Match(Email.Text);
            Match matchNumber = regexNumber.Match(Year.Text);

            //chcek for field contents are not null
            if (!String.IsNullOrEmpty(Forename.Text) && !String.IsNullOrEmpty(Surname.Text) && !String.IsNullOrEmpty(Email.Text) )
            {
                //check passsword fields are valid
                if (!CheckPassword())
                {
                    return false;
                }
                //Email address format check
                if (!matchEmail.Success)
                {//email address invalid
                    ValidationMessage.Content = "Email address is invalid";
                    ValidationMessage.Visibility = Visibility.Visible;
                    return false;
                }
                //Yeaer field validation
                if (!matchNumber.Success)
                {   //validation on year field failed
                    ValidationMessage.Content = "Year must be a numeric value;";
                    ValidationMessage.Visibility = Visibility.Visible;
                    return false;
                }

                ValidationMessage.Visibility = Visibility.Hidden;
                return true;
            }
            //Validation failed
            ValidationMessage.Content = "Save Changes failed. Please ensure all fields are completed";
            Title.BorderBrush = _alert;
            Forename.BorderBrush = _alert;
            Surname.BorderBrush = _alert;
            Email.BorderBrush = _alert;
            Course.BorderBrush = _alert;

            return false;
        }

        //Student changes submitted
        public void SaveStudent(object sender, RoutedEventArgs e)
        {
            //Password maintained
            var password = _student.Password;
            //Validation on fields values
            if (ValidationOnFields())
            {
                //Check password values
                if (CheckPassword())
                {   //encryption of new password
                    password = _client.Encrypt(Password.Text);
                }
                //Course selection changed
                var courseSelected = _client.ReturnCourseIdFromCourseName(Course.Text);
                if (courseSelected != 0)
                {   //course details returned
                    var course = _client.ReturnCourseDetail(courseSelected);
                    if (course.CourseId != 0)
                    {
                        //changes submitted to service
                        var result = _client.EditStudent(_student.StudentId, Title.Text, Forename.Text, Surname.Text, Email.Text, password,
                            course.CourseId, Convert.ToInt32(Year.Text));
                        if (result)
                        {
                            Success.IsOpen = true;
                        }
                        else
                        {
                            ValidationMessage.Content = "Save Changes failed. Please contact your Administrator";
                            ValidationMessage.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }

        public void CloseSuccessPopup(object sender, RoutedEventArgs e)
        {
            Success.IsOpen = false;
            _navigationService.Navigate(new StudentManagement(_userId, _navigationService));
            Close();
        }

        //Check to ensure if password is changed the confirm password matches
        //if passwords are empty old password maintained
        public bool CheckPassword()
        {
            //validation reset
            ValidationMessage.Visibility = Visibility.Hidden;
            Password.BorderBrush = _normal;
            ConfirmPassword.BorderBrush = _normal;

            //Check to ensure passwords are empty
            if (String.IsNullOrEmpty(ConfirmPassword.Text) && String.IsNullOrEmpty(Password.Text))
            {
                return true;
            }
            //if password fields are valid check to ensure they match
            if (Password.Text == ConfirmPassword.Text && !String.IsNullOrEmpty(Password.Text))
            {
                return true;
            }
            //Validation failed
            ValidationMessage.Content = "Passwords cannot be saved until both fields match.";
            Password.BorderBrush = _alert;
            ConfirmPassword.BorderBrush = _alert;
            ValidationMessage.Visibility = Visibility.Visible;
            return false;
        }


    }
}
