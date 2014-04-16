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
        private readonly int _userId;
        private readonly int _courseId;

        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        public EditCourse(int userId, int courseId)
        {
            _userId = userId;
            _courseId = courseId;
            InitializeComponent();
            var course = _client.ReturnCourseDetail(courseId);

            if (course != null)
            {
                CourseName.Text = course.CourseName;
                CourseDescription.Text = course.CourseDescription;
                CourseDuration.Text = course.Duration.ToString("0");
            }
        }

        public bool ValidationOnFields()
        {
            var courseName = CourseName.Text;
            var courseDescription = CourseDescription.Text;
            var courseDuration = CourseName.Text;

            if (!String.IsNullOrEmpty(courseName) && !String.IsNullOrEmpty(courseDescription) && !String.IsNullOrEmpty(courseDuration) && CourseDurationCheck())
            {
                return true;
            }
            
            ValidationMessage.Content = "Save changes failed. Please ensure all the fields are completed.";
            ValidationMessage.Foreground = _alert;
            ValidationMessage.Visibility = Visibility.Visible;

            return false;
        }

        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ValidationOnCourseDuration(object sender, RoutedEventArgs e)
        {
            var temp = CourseDuration.Text;

            var regex = new Regex("^[0-9]+$");

            if (!regex.IsMatch(temp))
            {
                ValidationMessage.Content = "'Duration' must be a numeric value.";
                ValidationMessage.Foreground = _alert;
                ValidationMessage.Visibility = Visibility.Visible;
                return;
            }
            ValidationMessage.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// </summary>
        public bool CourseDurationCheck()
        {
            var temp = CourseDuration.Text;

            var regex = new Regex("^[0-9]+$");

            if (!regex.IsMatch(temp))
            {
                return false;
            }

            return true;
        } 

        public void SaveCourse(object sender, RoutedEventArgs e)
        {
            if (ValidationOnFields())
            {
                var courseName = CourseName.Text;
                var courseDescription = CourseDescription.Text;
                var courseDuration = Convert.ToInt32(CourseName.Text);

                _client.EditCourse(_courseId, courseName, courseDuration, courseDescription, _userId);
                Success.IsOpen = true;
            }
        }

        public void CloseSuccessPopup(object sender, RoutedEventArgs e)
        {
            Success.IsOpen = false;
            Close();
        }
    }
}
