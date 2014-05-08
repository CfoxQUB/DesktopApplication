using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using TimetablingClientApplication.TimetablingService;
using TimetablingClientApplication.Views.Database.Pages;

namespace TimetablingClientApplication.Views.Database.Windows
{
    /// <summary>
    /// Interaction logic for CreateNewCourse.xaml
    /// </summary>
    public partial class CreateNewCourse 
    {
        //User Id maintained for course creation
        private readonly int _userId;
        //alert to indicate errors in validation message
        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        //webservice functionality exposed through this reference
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        //Navigation service of frmae in main window to redirect from window
        private readonly NavigationService _navigation;

        //Window initailized
        public CreateNewCourse(int userId, NavigationService navigationService)
        {   //user Id and navigatino service maintained
            _navigation = navigationService;
            _userId = userId;
            //Window initailized
            InitializeComponent();
        }

        //Validatin on fields which must pass or course creation
        public bool ValidationOnFields()
        {
            //text field details are selected
            var courseName = CourseName.Text;
            var courseDescription = CourseDescription.Text;
            var courseDuration= CourseDuration.Text;
            
            //Chech to ensure no duplicate course exists in the database
            if (_client.CheckCourseExists(courseName))
            {//If course already exists the validation is set to reflect it
                ValidationMessage.Content = "Course already exists";
                ValidationMessage.Foreground = _alert;
                ValidationMessage.Visibility = Visibility.Visible;
                return false;
            }
            //check to ensure fields follow the correct format and are not empty
            if (!String.IsNullOrEmpty(courseName) && !String.IsNullOrEmpty(courseDescription) && !String.IsNullOrEmpty(courseDuration) )
            {
                if (!CourseDurationValidation())
                {
                    //Duration is invalid and validation message set to relfect this
                    ValidationMessage.Content = "Course Duration must be a numeric value.";
                    ValidationMessage.Visibility = Visibility.Visible;
                    return false;
                }
                //validation passes
                return true;
            }
            //validation failed and fileds highlighted to reflect this
            CourseName.BorderBrush = _alert;
            CourseDescription.BorderBrush = _alert;
            CourseDuration.BorderBrush = _alert;
            
            //Validation message torelfect error taht has occured
            ValidationMessage.Content = "Course Creation failed. Please ensure all the fields are completed.";
            ValidationMessage.Foreground = _alert;
            ValidationMessage.Visibility = Visibility.Visible;

            return false;
        }
      
        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// check to ensure that the course duration is a numeric value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CourseDurationCheck(object sender, RoutedEventArgs e)
        {   //Course Duration value is selected
            var temp = CourseDuration.Text;
            //New regex is generated for comparison
            var regex = new Regex("^[0-9]+$");
            //Check against selcted value
            if (!regex.IsMatch(temp))
            {
                //Validaiont on field failed validatino message populated to relfect this
                ValidationMessage.Content = "'Duration' must be a numeric value.";
                ValidationMessage.Foreground = _alert;
                ValidationMessage.Visibility = Visibility.Visible;
                return;
            }
            //validation on field success
            ValidationMessage.Visibility = Visibility.Hidden;
        } 
        
        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// </summary>
        public bool CourseDurationValidation()
        {   //Course Duration value is selected
            var temp = CourseDuration.Text;
            //New regex is generated for comparison
            var regex = new Regex("^[0-9]+$");
            //Check against selcted value
            if (!regex.IsMatch(temp))
            {//Validation on fieled value failed
                return false;
            }
            //Validation on fieled value failed
            return true;
        } 

        public void SubmitNewCourse(object sender, RoutedEventArgs e)
        {
            //Validaiton on all fields values
            if (ValidationOnFields())
            {
                //validation passes so vlaues are selected
                var courseName = CourseName.Text;
                var courseDescription = CourseDescription.Text;
                var courseDuration = Convert.ToInt32(CourseDuration.Text);
                //_client passed information for course creation

                var result = _client.CreateCourse(courseName, courseDescription, _userId, courseDuration);
                if (result != 0)
                {
                    Success.IsOpen = true;
                }
                else
                {
                    //Error in database
                    ValidationMessage.Content = "Save Course failed. Please contact your system administrator";
                    ValidationMessage.Foreground = _alert;
                    ValidationMessage.Visibility = Visibility.Visible;
                }
            }
         }
        
        //Close success popup on successful course creation
        public void CloseSuccessPopup(object sender, RoutedEventArgs e)
        {
            //close popup
            Success.IsOpen = false;
            //NavigationService redirect to frame course management which is refreshed
            _navigation.Navigate(new CourseManagement(_userId, _navigation));
            Close();
        }
    }
}
