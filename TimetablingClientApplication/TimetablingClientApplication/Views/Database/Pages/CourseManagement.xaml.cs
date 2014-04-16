using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Navigation;
using TimetablingClientApplication.TimetablingService;
using TimetablingClientApplication.Views.Database.Windows;

namespace TimetablingClientApplication.Views.Database.Pages
{
    /// <summary>
    /// Interaction logic for CourseManagement.xaml
    /// </summary>
    public partial class CourseManagement 
    {
        
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        private readonly ObservableCollection<Course> _courseCollectionList = new ObservableCollection<Course>();

        private const string DefaultSearchString = "Search for Courses . . . ";

        private readonly NavigationService _navigationService;
        private readonly int _userId;

        public CourseManagement(int userId, NavigationService navigationService)
        {
            _navigationService = navigationService;

            _userId = userId;

            InitializeComponent();

            var courseList = _client.ReturnCourses();

            if (courseList != null)
            {
                foreach (var b in courseList)
                {
                    _courseCollectionList.Add(b);
                }
            }

            CourseList.ItemsSource = _courseCollectionList;
            CourseNameText.Text = "No selection made.";
            CourseDescription.Text = "None";
            CourseDuration.Text = "None";
        }

        public void NewCourseSelected(object sender, RoutedEventArgs e)
        {
            var selectedCourse = (Course) CourseList.SelectedItem;
            if (selectedCourse != null)
            {
                CourseNameText.Text = selectedCourse.CourseName;
                CourseDescription.Text = selectedCourse.CourseDescription;
                CourseDuration.Text = selectedCourse.Duration.ToString("N");
                return;
            }

            CourseNameText.Text = "No selection made.";
            CourseDescription.Text = "None";
            CourseDuration.Text = "None";
        }

        private void SearchField_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchCourses.Text = "";
        }

        private void SearchField_LoseFocus(object sender, RoutedEventArgs e)
        {
            SearchCourses.Text = DefaultSearchString;
        }

        private void ReturnSearchResults(object sender, RoutedEventArgs e)
        {
            if (SearchCourses.Text != DefaultSearchString)
                {
                    var results = _client.SearchCourseFunction(SearchCourses.Text);

                    CourseList.ItemsSource = results;
                }
         }

        private void DeleteCoursePopup(object sender, RoutedEventArgs e)
        {
            DeleteCourse.IsOpen = true;
        }

        private void CloseDeletePopup(object sender, RoutedEventArgs e)
        {
            DeleteCourse.IsOpen = false;
        }

        private void ConfirmDeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            var selectedCourse = (Course) CourseList.SelectedItem;
            
            if (selectedCourse != null)
            {
                _client.DeleteCourse(selectedCourse.CourseId);

                var noResults = _client.ReturnCourses();

                CourseList.ItemsSource = noResults;

                CourseNameText.Text = "No selection made.";
                CourseDescription.Text = "None";
                CourseDuration.Text = "None";
            }

            DeleteCourse.IsOpen = false;
        }
        
        public void DisablePage()
        {
            SearchCourses.IsEnabled = false;
            CourseList.IsEnabled = false;
            EditCourseButton.IsEnabled = false;
            DeleteCourseButton.IsEnabled = false;
        }

        public void EnablePage()
        {
            SearchCourses.IsEnabled = true;
            CourseList.IsEnabled = true;
            EditCourseButton.IsEnabled = true;
            DeleteCourseButton.IsEnabled = true;
        }

        public void AddNewCourse(object sender, RoutedEventArgs e)
        {
            var building = new CreateNewCourse(_userId, _navigationService);
            building.Show();
        }

        public void EditCourse(object sender, RoutedEventArgs e)
        {
            var courseId = (Course) CourseList.SelectedItem;
            if (courseId != null)
            {
                var editbuilding = new EditCourse(_userId, courseId.CourseId);
                editbuilding.Show();
            }
        }

        
    }
}
