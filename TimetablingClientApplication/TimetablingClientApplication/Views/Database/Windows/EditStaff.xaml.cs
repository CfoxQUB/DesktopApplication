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
    /// Interaction logic for EditStaff.xaml
    /// </summary>
    public partial class EditStaff
    {
        //Web service functionality exposed through service reference
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        // user Id and navigation service maintained
        private readonly int _userId;
        private readonly NavigationService _navigationService;
        //Obseravble collection used to populate the Titles and Courses
        public ObservableCollection<String> Titles = new ObservableCollection<String>();
        public ObservableCollection<String> Courses = new ObservableCollection<String>();
        //Staff member that is being edited
        private readonly Staff _staff = new Staff(); 
        //Validation colors applied to fields to indicate errors and reset of validation
        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush _normal = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE3E9EF"));

        //Window created
        public EditStaff(int userId, int staffId, NavigationService navigationService)
        {
            //User id and navigation service maintained 
            _userId = userId;
            _navigationService = navigationService;

            InitializeComponent();
            //Titles selection setup
            Titles.Add("Mr");
            Titles.Add("Mrs");
            Titles.Add("Miss");
            Titles.Add("Ms");
            Titles.Add("Dr");
            Title.ItemsSource = Titles;
            //Staff member details returned
            var staff = _client.ReturnStaffDetail(staffId);
            if (staff != null)
            {
                _staff = staff;
                //staff details populated into available fields
                Title.SelectedItem = Titles.SingleOrDefault(x => x == _staff.StaffTitle);
                Forename.Text = _staff.StaffForename;
                Surname.Text = _staff.StaffSurname;
                Email.Text = _staff.StaffEmail;
            }
            //Validation message colour set 
            ValidationMessage.Foreground = _alert;
            //Courses available returned
            var courses = _client.ReturnCourses();
            if (courses != null)
            {
                //Courses available populated into course select
                foreach (var c in courses)
                {
                    Courses.Add(c.CourseName);
                }
                //selected course set to equal that to staff members allocated course
                var selectedCourse = courses.SingleOrDefault(x => x.CourseId == _staff.Course);
                Course.ItemsSource = Courses;
                if (selectedCourse != null)
                {   //Select display item that equals course in course list
                    Course.Text = selectedCourse.CourseName;
                }
                
                return;
            }
            //Deafault page values if staff member selected isnt valid
            Title.SelectedItem = "No Selection";
            Forename.Text = "N/A";
            Surname.Text = "N/A";
            Email.Text = "N/A";
            Course.SelectedItem = "N/A";

        }

        /// <summary>
        /// http://www.codegateway.com/2012/03/c-regex-for-email-address.html
        /// Validation to ensure the values on the page are of correct value and format
        /// </summary>
        /// <returns></returns>
        public bool ValidationOnFields()
        {
            //Field colours reset
            Title.BorderBrush = _normal;
            Forename.BorderBrush = _normal;
            Surname.BorderBrush = _normal;
            Email.BorderBrush = _normal;
            Title.BorderBrush = _normal;
            Course.BorderBrush = _normal;
            //new regex expression used for comparison against email address field
            var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            //comparison made against email field format
            Match match = regex.Match(Email.Text);

            //Check to ensure email address values are not null
            if (!String.IsNullOrEmpty(Forename.Text) && !String.IsNullOrEmpty(Surname.Text) && !String.IsNullOrEmpty(Email.Text))
            {
                //check to ensure match is success
                if (!CheckPassword())
                {
                    //Passwords invalid
                    ValidationMessage.Content = "Passwords cannot be saved until both fields match.";
                    Password.BorderBrush = _alert;
                    ConfirmPassword.BorderBrush = _alert;
                    ValidationMessage.Visibility = Visibility.Visible;
                    return false;
                }
                
                //check to ensure match is success
                if (!match.Success)
                {   //Email address is invalid and validation message set to relfect this
                    ValidationMessage.Content = "Email address is invalid";
                    ValidationMessage.Visibility = Visibility.Visible;
                    return false;
                }
                //Validation passed
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

        //Save staff changes to database
        public void SaveStaff(object sender, RoutedEventArgs e)
        {
            //Selection of current staff member password
            var password = _staff.Password;
            
            //Validation on fields format and content
            if (ValidationOnFields())
            {
                //Chec password field validation
                if (CheckPassword())
                {
                    //Encryption of password field
                    password = _client.Encrypt(Password.Text);
                }
                //Course selected
                var courseSelected = _client.ReturnCourseIdFromCourseName(Course.Text);
                if (courseSelected != 0)
                {
                    //detail of course returned
                    var course = _client.ReturnCourseDetail(courseSelected);
                    if (course.CourseId != 0)
                    {
                        //staff member changes submitted
                        var result = _client.EditStaff(_staff.StaffId,Title.Text, Forename.Text, Surname.Text, Email.Text, password,
                            course.CourseId);
                        if (result)
                        {   //savechanges success
                            Success.IsOpen = true;
                        }
                    }
                }
            }
        }

       //Close popup and window
        public void CloseSuccessPopup(object sender, RoutedEventArgs e)
        {   //Popup closed
            Success.IsOpen = false;
            //Redirect
            _navigationService.Navigate(new StaffManagement(_userId, _navigationService));
            //window closed
            Close();
        }

        //check password fields both match and are not null
        public bool CheckPassword()
        {   //Validation message reset
            ValidationMessage.Visibility = Visibility.Hidden;
            Password.BorderBrush = _normal;
            ConfirmPassword.BorderBrush = _normal;
            //Check to see if password fields are empty
            if (String.IsNullOrEmpty(ConfirmPassword.Text) && String.IsNullOrEmpty(Password.Text))
            {
                return true;
            }
            //Check to see if password fields are not null and match
            if (Password.Text == ConfirmPassword.Text && !String.IsNullOrEmpty(Password.Text))
            {
                return true;
            }
            
            return false;
        }

       
     }
}
