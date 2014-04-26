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
        //timtableing client used to expose functinality of webservice
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        //Observable collection bound to the staff list to display staff in database
        private readonly ObservableCollection<Staff> _staffCollectionList = new ObservableCollection<Staff>();

        //Default search string used for placeholder in search field
        private const string DefaultSearchString = "Search for Staff . . . ";

        //Navigation service used to update the view upon staff actions
        private readonly NavigationService _navigationService;
        
        //User Id user currently logged in
        private readonly int _userId;

        public StaffManagement(int userId, NavigationService navigationService)
        {
            //initialization of maintained data
            _userId = userId;
            _navigationService = navigationService;

            //Page rendered
            InitializeComponent();

            //Staff List returned from web service and added to listed display
            var staffList = _client.ReturnStaff();
            if (staffList != null)
            {
                foreach (var s in staffList)
                {
                    _staffCollectionList.Add(s);
                }
            }
            //Binding of  observable collection to page list
            StaffList.ItemsSource = _staffCollectionList;
            //Setting default display items
            Title.Text = "No selection made.";
            Forename.Text = "None";
            Surname.Text = "None";
            Email.Text = "None";
            Course.Text = "None";

        }

        //removes the placeholder in searhc field
        private void SearchField_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchStaff.Text = "";
        }

        //replaces the placeholder in searhc field
        private void SearchField_LoseFocus(object sender, RoutedEventArgs e)
        {
            SearchStaff.Text = DefaultSearchString;
        }

        //retursn the search results from web service by passing in search item
        private void ReturnSearchResults(object sender, RoutedEventArgs e)
        {
            //Check to ensure search item not the same as the default search string
           if (SearchStaff.Text != DefaultSearchString)
                {
                    //results returned and bound to the StaffList list view
                    var results = _client.SearchStaffFunction(SearchStaff.Text);
                    StaffList.ItemsSource = results;
                }
        }

        // opens the Delete staff confirm popup
        private void DeleteStaffPopup(object sender, RoutedEventArgs e)
        {
            DeleteStaff.IsOpen = true;
        }
        
        //Close the Delete staff Popup
        private void CloseDeletePopup(object sender, RoutedEventArgs e)
        {
            DeleteStaff.IsOpen = false;
        }

        //Deletion of staff member button (Confirms delete)
        private void ConfirmDeleteButtonClicked(object sender, RoutedEventArgs e)
        {   //Selected staff member
            var selectedStaff = (Staff)StaffList.SelectedItem;
            //check staff memebr is not null value
            if (selectedStaff != null)
            {
                //Deletion of staff member by Ws
                _client.DeleteStaff(selectedStaff.StaffId);

                //staff members list returned to reflect deletion
                var refreshList = _client.ReturnStaff();
                StaffList.ItemsSource = refreshList;
            }
            //Closing of popup
            DeleteStaff.IsOpen = false;
            //page default display values reset
            Title.Text = "No selection made.";
            Forename.Text = "None";
            Surname.Text = "None";
            Email.Text = "None";
            Course.Text = "None";
        }

        //Add staff window opened
        public void AddNewStaff(object sender, RoutedEventArgs e)
        {
            var newStaff = new CreateNewStaff(_userId, _navigationService);
            newStaff.Show();
        }

        //Edit staff window opened staff id passed through to return in detail
        public void EditStaff(object sender, RoutedEventArgs e)
        {   //Staff memebr selected
            var staffId = (Staff)StaffList.SelectedItem;
            if (staffId != null)
            {
                //Edit staff window opened
                var editStaff = new EditStaff(_userId, staffId.StaffId, _navigationService);
                editStaff.Show();
            }

        }

        //staff member selection changed in listed staff
        public void NewStaffSelected(object sender, RoutedEventArgs e)
        {
            var selectedStaff = (Staff)StaffList.SelectedItem;
            if (selectedStaff != null)
            {
                //display information associated with the staff member
                Title.Text = selectedStaff.StaffTitle;
                Forename.Text = selectedStaff.StaffForename;
                Surname.Text = selectedStaff.StaffSurname;
                Email.Text = selectedStaff.StaffEmail;

                //return course information
                var courseInfo = _client.ReturnCourseDetail(selectedStaff.Course);
                //course info displayed
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
