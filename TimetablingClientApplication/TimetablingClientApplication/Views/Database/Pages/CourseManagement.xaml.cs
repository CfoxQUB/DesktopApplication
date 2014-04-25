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
        //Timetabling client used to expose the functionality of the web service
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        //Observable collection used to populate the course list on the page
        private readonly ObservableCollection<Course> _courseCollectionList = new ObservableCollection<Course>();

        //Default search string value used as placeholder
        private const string DefaultSearchString = "Search for Courses . . . ";

        //frame navigation service passed in from the master window
        private readonly NavigationService _navigationService;
        
        //User id passed in from the master window
        private readonly int _userId;

        public CourseManagement(int userId, NavigationService navigationService)
        {
            _navigationService = navigationService;

            _userId = userId;

            InitializeComponent();

            //return courses from  web service to populate the page
            var courseList = _client.ReturnCourses();
            if (courseList != null)
            {
                foreach (var b in courseList)
                {
                    _courseCollectionList.Add(b);
                }
            }

            //popuation of the fields used to displa course information
            CourseList.ItemsSource = _courseCollectionList;
            CourseNameText.Text = "No selection made.";
            CourseDescription.Text = "None";
            CourseDuration.Text = "None";
        }

        //New course selectied from the list
        //Repopulates the information on the page to reflect selected 
        //Course.
        public void NewCourseSelected(object sender, RoutedEventArgs e)
        {
            //returns selected course in list
            var selectedCourse = (Course) CourseList.SelectedItem;
            if (selectedCourse != null)
            {
                //Populates th field on the page to display course information
                CourseNameText.Text = selectedCourse.CourseName;
                CourseDescription.Text = selectedCourse.CourseDescription;
                CourseDuration.Text = selectedCourse.Duration.ToString("N");
                return;
            }
            //Default values if selected course is null
            CourseNameText.Text = "No selection made.";
            CourseDescription.Text = "None";
            CourseDuration.Text = "None";
        }

        //On focus of search field the placeholder is removed and empty
        //string is added in
        private void SearchField_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchCourses.Text = "";
        }

        //focus of search field lost, placeholder text is replaced
        private void SearchField_LoseFocus(object sender, RoutedEventArgs e)
        {
            SearchCourses.Text = DefaultSearchString;
        }

        //Search string that is enteredd in to the search field is passed
        //to appropriate method of web service
        private void ReturnSearchResults(object sender, RoutedEventArgs e)
        {
            if (SearchCourses.Text != DefaultSearchString)
                {
                    var results = _client.SearchCourseFunction(SearchCourses.Text);

                    CourseList.ItemsSource = results;
                }
         }

        //Delete popup opened to confirm deletion of the selction made
        private void DeleteCoursePopup(object sender, RoutedEventArgs e)
        {
            DeleteCourse.IsOpen = true;
        }

        //Delete popup closed
        private void CloseDeletePopup(object sender, RoutedEventArgs e)
        {
            DeleteCourse.IsOpen = false;
        }

        //Confirmation of delete, webservice functinality for this delete exposed
        private void ConfirmDeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            var selectedCourse = (Course) CourseList.SelectedItem;
            
            if (selectedCourse != null)
            {
                //deletion of course, Id passed to WS
                _client.DeleteCourse(selectedCourse.CourseId);

                //Course list refreshed
                var noResults = _client.ReturnCourses();
                CourseList.ItemsSource = noResults;

                //Defaults page details repopulated
                CourseNameText.Text = "No selection made.";
                CourseDescription.Text = "None";
                CourseDuration.Text = "None";
            }
            //Delete popup closed
            DeleteCourse.IsOpen = false;
        }
        
        //New course window opened
        public void AddNewCourse(object sender, RoutedEventArgs e)
        {
            var building = new CreateNewCourse(_userId, _navigationService);
            building.Show();
        }

        //Edit currently selected course window opened
        public void EditCourse(object sender, RoutedEventArgs e)
        {
            var courseId = (Course) CourseList.SelectedItem;
            if (courseId != null)
            {
                //If selection isnt null window opened
                var editbuilding = new EditCourse(_userId, courseId.CourseId);
                editbuilding.Show();
            }
        }

        
    }
}
