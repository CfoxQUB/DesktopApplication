using System;
using System.Collections.Generic;
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
    /// Interaction logic for CreateNewCourse.xaml
    /// </summary>
    public partial class CreateNewCourse : Window
    {
        private readonly int _userId;

        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush _normal = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE3E9EF"));

        private TimetablingServiceClient _client = new TimetablingServiceClient();
        private NavigationService navigation;

        public CreateNewCourse(int userId, NavigationService navigationService)
        {
            navigation = navigationService;
            _userId = userId;

            InitializeComponent();
        }

        public bool ValidationOnFields()
        {
            var courseName = CourseName.Text;
            var courseDescription = CourseDescription.Text;
            var courseDuration= CourseDuration.Text;
            
            if (_client.CheckCourseExists(courseName))
            {
                ValidationMessage.Content = "Course already exists";
                ValidationMessage.Foreground = _alert;
                ValidationMessage.Visibility = Visibility.Visible;
                return false;
            }

            if (!String.IsNullOrEmpty(courseName) && !String.IsNullOrEmpty(courseDescription) && !String.IsNullOrEmpty(courseDuration) )
            {
                return true;
            }

            CourseName.BorderBrush = _alert;
            CourseDescription.BorderBrush = _alert;
            CourseDuration.BorderBrush = _alert;
            

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
        public void CourseDurationCheck(object sender, RoutedEventArgs e)
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
        public bool CourseDurationValidation()
        {
            var temp = CourseDuration.Text;

            var regex = new Regex("^[0-9]+$");

            if (!regex.IsMatch(temp))
            {
                return false;
            }
            return true;
        } 

        public void SubmitNewCourse(object sender, RoutedEventArgs e)
        {
            if (ValidationOnFields())
            {
                var courseName = CourseName.Text;
                var courseDescription = CourseDescription.Text;
                var courseDuration = Convert.ToInt32(CourseDuration.Text);

                _client.CreateCourse(courseName, courseDescription,_userId, courseDuration);
                Success.IsOpen = true;
            }

        }
        
        public void CloseSuccessPopup(object sender, RoutedEventArgs e)
        {
            Success.IsOpen = false;
            navigation.Navigate(new CourseManagement(_userId, navigation));
            Close();
        }
    }
}
