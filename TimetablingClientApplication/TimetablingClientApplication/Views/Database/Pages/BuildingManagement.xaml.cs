using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TimetablingClientApplication.TimetablingService;
using TimetablingClientApplication.Views.Database.Windows;

namespace TimetablingClientApplication.Views.Database.Pages
{
    /// <summary>
    /// Interaction logic for BuildingManagement.xaml
    /// </summary>
    public partial class BuildingManagement 
    {
        //Timetabling Client used to expose Webservice functionality
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        //Observable list used to display database content in page
        private readonly ObservableCollection<Building> _buildingCollectionList = new ObservableCollection<Building>();

        //Contstant strig value used to compare and reset search field placeholder 
        private const String DefaultSearchString = "Search for Buildings . . . ";

        //boolean check to determine if page has been rendered for room list population
        private readonly bool _pageRendered;

        //User Id passed from master page
        private readonly int _userId;

        public BuildingManagement(int userId)
        {
            _userId = userId;
            _pageRendered = false;
            InitializeComponent();

            //Population of the building list of returned data from database
            var buildingsList = _client.ReturnBuildings();
            if (buildingsList != null)
            {
                foreach (var b in buildingsList)
                {
                    _buildingCollectionList.Add(b);
                }
            }

            //Setting default values on page as no building is yet selected
            BuildingList.ItemsSource = _buildingCollectionList;
            BuildingNameText.Text = "No selection made.";
            BuildingNumber.Text = "No selection made";
            EventsText.Text = "None";
        }

        
        // When building selected in the builings list
        // the page content will reflect this buildings
        // information. 
        public void NewBuildingSelected(object sender, RoutedEventArgs e)
        {
            var selectedBuilding = (Building) BuildingList.SelectedItem;
            if (selectedBuilding == null)
            {
                return;
            }

            BuildingNameText.Text = selectedBuilding.BuildingName;
                
                //Returns the buildings events
                var eventNumber = _client.ReturnBuildingEvents(selectedBuilding.BuildingId);

                if (eventNumber != null)
                {
                    EventsText.Text = eventNumber.Count().ToString("D");
                }
                else
                {
                    EventsText.Text = "0";
                }
                //Populates page with selected buildings information
                BuildingNumber.Text = selectedBuilding.BuildingNumber.ToString("D");
                AddressLine1.Text = selectedBuilding.AddressLine1;
                AddressLine2.Text = selectedBuilding.AddressLine2;
                City.Text = selectedBuilding.City;
                Postcode.Text = selectedBuilding.PostalCode;

        }

        //Removes placeholder and replaces with blank text.
        private void SearchField_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchRooms.Text = "";
        }

        //Restores placeholder with the default search string value
        private void SearchField_LoseFocus(object sender, RoutedEventArgs e)
        {
            SearchRooms.Text = DefaultSearchString;
        }

        //Populates the buildings list with the results which relate to the 
        //search value passed in.
        private void ReturnSearchResults(object sender, RoutedEventArgs e)
        {
            if (_pageRendered)
            {
                if (SearchRooms.Text != DefaultSearchString)
                {
                    var results = _client.SearchBuildingFunction(SearchRooms.Text);

                    BuildingList.ItemsSource = results;
                }

            }
        }

        //Opens the delete popup for confimation of the deleting of the builing
        //selected.
        private void DeleteBuildingPopup(object sender, RoutedEventArgs e)
        {
            DeleteBuilding.IsOpen = true;
        }

        //Closes delete Popup
        private void CloseDeletePopup(object sender, RoutedEventArgs e)
        {
            DeleteBuilding.IsOpen = false;
        }

        //Confirm deletion of the building
        //calls the method in the webservice by passing in builing Id
        private void ConfirmDeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            var selectedBuilding = (Building) BuildingList.SelectedItem;

            if (selectedBuilding != null)
            {
                //Deletion of builing through web service
                _client.DeleteBuilding(selectedBuilding.BuildingId);

                //Refreshing of the buildings list in page
                var noResults = _client.ReturnBuildings();

                BuildingList.ItemsSource = noResults;

                //Resets the page content to reflect building deleted
                BuildingNameText.Text = "No selection made.";
                BuildingNumber.Text = "No selection made";
                AddressLine1.Text = "";
                AddressLine2.Text = "";
                Postcode.Text = "";
                City.Text = "";
                EventsText.Text = "None";
            }
            //Close popup
            DeleteBuilding.IsOpen = false;
        }

        //Opens new window for the creation of a new building
        public void AddNewBuilding(object sender, RoutedEventArgs e)
        {
            var building = new CreateNewBuilding(_userId);
            building.Show();
        }

        //Opens new window for the editing of selected building
        public void EditBuilding(object sender, RoutedEventArgs e)
        {
            var buildingId = (Building) BuildingList.SelectedItem;
            if (buildingId != null)
            {
                var editbuilding = new EditBuilding(_userId, buildingId.BuildingId);
                editbuilding.Show();
            }
        }
    }
}
