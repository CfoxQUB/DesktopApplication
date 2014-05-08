using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using TimetablingClientApplication.TimetablingService;


namespace TimetablingClientApplication.Views.Database.Windows
{
    /// <summary>
    /// Interaction logic for EditCourse.xaml
    /// </summary>
    public partial class EditCourse 
    {   
        //User Id and course Id maintained for changes saved
        private readonly int _userId;
        private readonly int _courseId;
        //Alert colour to relfect errors on page
        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        //Webservice functionality exposed 
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        //Window Initialised
        public EditCourse(int userId, int courseId)
        {
            //User Id and course Id maintained for changes saved
            _userId = userId;
            _courseId = courseId;
            //Window initialized
            InitializeComponent();
            //Return details of course details
            var course = _client.ReturnCourseDetail(courseId);
            if (course != null)
            {
                CourseName.Text = course.CourseName;
                CourseDescription.Text = course.CourseDescription;
                CourseDuration.Text = course.Duration.ToString("D");
            }
        }

        //Validation on field values in page
        public bool ValidationOnFields()
        {
            //Course values selected for validation
            var courseName = CourseName.Text;
            var courseDescription = CourseDescription.Text;
            var courseDuration = CourseName.Text;
            //check to ensure details of course are valid and in correct format
            if (!String.IsNullOrEmpty(courseName) && !String.IsNullOrEmpty(courseDescription) && !String.IsNullOrEmpty(courseDuration) && CourseDurationCheck())
            {
                //check to ensure match is success
                if (!CourseDurationCheck())
                {   //Building Number is invalid and validation message set to relfect this
                    ValidationMessage.Content = "Course Duration must be a numeric value";
                    ValidationMessage.Visibility = Visibility.Visible;
                    return false;
                }
                //Validation passed
                return true;
            }
            //Validation alert on fields
            CourseName.BorderBrush = _alert;
            CourseDescription.BorderBrush = _alert;
            CourseName.BorderBrush = _alert;
            //Validation failed and error message to reflect details
            ValidationMessage.Content = "Save changes failed. Please ensure all the fields are completed.";
            ValidationMessage.Foreground = _alert;
            ValidationMessage.Visibility = Visibility.Visible;

            return false;
        }

        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// Validation of course number to ensure numeric value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ValidationOnCourseDuration(object sender, RoutedEventArgs e)
        {   //Duration value selected
            var temp = CourseDuration.Text;
            //new regex expression created for comparison
            var regex = new Regex("^[0-9]+$");
            //check against the value against the regex value
            if (!regex.IsMatch(temp))
            {
                //Validation failed
                ValidationMessage.Content = "'Duration' must be a numeric value.";
                ValidationMessage.Foreground = _alert;
                ValidationMessage.Visibility = Visibility.Visible;
                return;
            }
            //Valiadation success
            ValidationMessage.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// Validation of course number to ensure numeric value
        /// </summary>
        public bool CourseDurationCheck()
        {   //Duration value selected
            var temp = CourseDuration.Text;
            //new regex expression created for comparison
            var regex = new Regex("^[0-9]+$");
            //check against the value against the regex value
            if (!regex.IsMatch(temp))
            {
                //Validation failed
                return false;
            }
            //Valiadation success
            return true;
        } 

        //changes in course selcted submitted for saving
        public void SaveCourse(object sender, RoutedEventArgs e)
        {
            //validatin on fields content for format and null values
            if (ValidationOnFields())
            {
                //validation passed
                var courseName = CourseName.Text;
                var courseDescription = CourseDescription.Text;
                var courseDuration = Convert.ToInt32(CourseName.Text);

                var result = _client.EditCourse(_courseId, courseName, courseDuration, courseDescription, _userId);
                if (result)
                {   //Save changes successful
                    Success.IsOpen = true;
                }
                else
                {
                    //Saving changes failed
                    ValidationMessage.Content = "Save Changes Failed please contact your System Administratator";
                    ValidationMessage.Foreground = _alert;
                    ValidationMessage.Visibility = Visibility.Visible;
                }
            }
        }
        //Close Success popup
        public void CloseSuccessPopup(object sender, RoutedEventArgs e)
        {
            //close Popup
            Success.IsOpen = false;
            //close Winodw
            Close();
        }
    }
}
