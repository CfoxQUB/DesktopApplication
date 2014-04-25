using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Navigation;
using TimetablingClientApplication.TimetablingService;
using TimetablingClientApplication.Views.Database.Windows;

namespace TimetablingClientApplication.Views.Database.Pages
{
    /// <summary>
    /// Interaction logic for StaffTableManagement.xaml
    /// </summary>
    public partial class StaffManagement
    {
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        private readonly ObservableCollection<Staff> _staffCollectionList = new ObservableCollection<Staff>();

        private const string DefaultSearchString = "Search for Staff . . . ";

        private readonly NavigationService _navigationService;
        
        private readonly int _userId;

        public StaffManagement(int userId, NavigationService navigationService)
        {
            _userId = userId;
            _navigationService = navigationService;

            InitializeComponent();

            var staffList = _client.ReturnStaff();

            if (staffList != null)
            {
                foreach (var s in staffList)
                {
                    _staffCollectionList.Add(s);
                }
            }

            StaffList.ItemsSource = _staffCollectionList;
            Title.Text = "No selection made.";
            Forename.Text = "None";
            Surname.Text = "None";
            Email.Text = "None";
            Course.Text = "None";

        }

        private void SearchField_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchStaff.Text = "";
        }

        private void SearchField_LoseFocus(object sender, RoutedEventArgs e)
        {
            SearchStaff.Text = DefaultSearchString;
        }

        private void ReturnSearchResults(object sender, RoutedEventArgs e)
        {
           
            if (SearchStaff.Text != DefaultSearchString)
                {
                    var results = _client.SearchStaffFunction(SearchStaff.Text);
                    StaffList.ItemsSource = results;
                }
        }

        private void DeleteStaffPopup(object sender, RoutedEventArgs e)
        {
            DeleteStaff.IsOpen = true;
        }
        
        private void CloseDeletePopup(object sender, RoutedEventArgs e)
        {
            DeleteStaff.IsOpen = false;
        }

        private void ConfirmDeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            var selectedRoom = (Staff)StaffList.SelectedItem;
            
            if (selectedRoom != null)
            {
                _client.DeleteStaff(selectedRoom.StaffId);

                var noResults = _client.ReturnStaff();

                StaffList.ItemsSource = noResults;
            }
            DeleteStaff.IsOpen = false;
            Title.Text = "No selection made.";
            Forename.Text = "None";
            Surname.Text = "None";
            Email.Text = "None";
            Course.Text = "None";
        }

        public void AddNewStaff(object sender, RoutedEventArgs e)
        {
            var newStaff = new CreateNewStaff(_userId, _navigationService);
            newStaff.Show();
        }

        public void EditStaff(object sender, RoutedEventArgs e)
        {
            var staffId = (Staff)StaffList.SelectedItem;
            if (staffId != null)
            {
                var editStaff = new EditStaff(_userId, staffId.StaffId, _navigationService);
                editStaff.Show();
            }

        }

        public void NewStaffSelected(object sender, RoutedEventArgs e)
        {
            var selectedStaff = (Staff)StaffList.SelectedItem;
            if (selectedStaff != null)
            {
                Title.Text = selectedStaff.StaffTitle;
                Forename.Text = selectedStaff.StaffForename;
                Surname.Text = selectedStaff.StaffSurname;
                Email.Text = selectedStaff.StaffEmail;

                var courseInfo = _client.ReturnCourseDetail(selectedStaff.Course);

                if (courseInfo != null && selectedStaff.Course != 0)
                {
                    Course.Text = courseInfo.CourseName;
                    return;
                }
                Course.Text = "None";
            }
        }
     }
}
