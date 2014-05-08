using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using TimetablingClientApplication.TimetablingService;
using TimetablingClientApplication.Views.Database.Windows;

namespace TimetablingClientApplication.Views.Database.Pages
{
    /// <summary>
    /// Interaction logic for RoomManagement.xaml
    /// </summary>
    public partial class RoomManagement 
    {
        //Timetabling client used to expose webservice functionality
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        //Observable collection used to display rooms of selected building
        private readonly ObservableCollection<Room> _roomCollectionList = new ObservableCollection<Room>();

        //deafaul search string used to reset the placeholder text of the search fieldS
        private const string DefaultSearchString = "Search for Rooms . . . ";

        //Check to see if page has rendered to populate rooms of building
        private readonly bool _pageRendered;

        //User Id passed in from the master page
        private readonly int _userId;

        //Page initialized
        public RoomManagement(int userId)
        {
            //Boolean to determine if the page has been rendered for building select
            _pageRendered = false;
            //Storing the user Id
            _userId = userId;
            //Page created/Initialized
            InitializeComponent();

            //Returning List of buildings
            var buildingsList = _client.ReturnBuildings();

            //Check to ensure buildings exist
            if (buildingsList != null)
            {
                //Populate builings elect and set first value
                foreach (var b in buildingsList)
                {
                    SelectBuilding.Items.Add(b.BuildingName);
                }
                SelectBuilding.SelectedItem = buildingsList.First().BuildingName;
                
                //Return rooms for the selected building
                var rooms = _client.ReturnBuildingRooms(buildingsList.First().BuildingId);           
                
                //check to ensure rooms list is not null or empty
                if (rooms != null)
                {
                    //Population of room select and setting of initial value
                    foreach (var r in rooms)
                    {
                        _roomCollectionList.Add(r);
                    } 
                    RoomList.ItemsSource = _roomCollectionList;

                }
                else
                {
                    //If no rooms disable room select
                    RoomList.IsEnabled =false;
                }
                
                //Setting default page contents
                RoomNameText.Text = "No Selection has been made.";
                RoomDescriptionText.Text = "None";
                RoomTypeText.Text = "None";
                BuildingText.Text = "None";
                EventsText.Text = "None";
                CapacityText.Text = "None";

             
                _pageRendered = true;
                return;
            }
            //Alert opened and Disabled page
            OpenBuildingsAlert();
            DisablePage();
        }

        //Selection of building changes, repopulation of rooms
        public void BuildingSelectionChange(object sender, RoutedEventArgs e)
        {
            //Check to ensure page rendered
            if (_pageRendered)
            {
                //Return building Id from selected building to return rooms later
                var buildingId = _client.ReturnBuildingIdFromBuildingName(SelectBuilding.SelectedItem.ToString());

                //As long as a building Id is returned rooms list returned
                if (buildingId != 0)
                {
                    //Rooms returned from builing Id
                    var noResults = _client.ReturnBuildingRooms(buildingId);

                    //As long as rooms list not null rooms elect populated
                    if (noResults != null)
                    {
                        //setting Item source to rooms list 
                        RoomList.ItemsSource = noResults;
                        EnablePage();
                    }
                    else
                    {
                        //If no rooms exist rooms list is disabled
                        RoomList.IsEnabled = false;
                    }
                }
            }
          }

        //room list item selected
        public void NewRoomSelected(object sender, RoutedEventArgs e)
        {
            //room stored as a local variable
            var selectedEvent = (Room)RoomList.SelectedItem;
            //check to ensure selection is valid
            if (selectedEvent != null)
            {
                //Page contents set to relfect the selected event
                RoomNameText.Text = selectedEvent.RoomName;
                RoomDescriptionText.Text = selectedEvent.RoomDescription;

                //room types returned
                var roomType = _client.ReturnRoomTypes().SingleOrDefault(x => x.RoomTypeId == selectedEvent.RoomType);

                //check to ensure event types returned correctly
                if (roomType != null)
                {
                    RoomTypeText.Text = roomType.RoomeTypeDescription;
                }
                else
                {
                    RoomTypeText.Text = "N/A";
                }

                //Count of rooms events returned
                var eventNumber = _client.ReturnRoomEvents(selectedEvent.RoomId);
                //page content for room events returned and set
                if (eventNumber != null)
                {
                    EventsText.Text = eventNumber.Count().ToString("D");
                }
                else
                {
                    EventsText.Text = "0";
                }
                //Capacity field set
                CapacityText.Text = selectedEvent.Capacity.ToString("D");


                //returning building information
                var building = _client.ReturnRoomBuilding(selectedEvent.RoomId);
                var buildingInfo = _client.ReturnBuildings().SingleOrDefault(x => x.BuildingId == building);

                //Setting page content to builing information
                if (buildingInfo != null)
                {
                    BuildingText.Text = buildingInfo.BuildingNumber + "\r\n" + buildingInfo.AddressLine1 + "\r\n"
                                        + buildingInfo.AddressLine2 + "\r\n" + buildingInfo.City + "\r\n" +
                                        buildingInfo.PostalCode;
                }
            }
        }

        //Search field placeholder removed
        private void SearchField_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchRooms.Text = "";
        }

        //Search field placeholder replaced
        private void SearchField_LoseFocus(object sender, RoutedEventArgs e)
        {
            SearchRooms.Text = DefaultSearchString;
        }

        //search results from search field item returned
        private void ReturnSearchResults(object sender, RoutedEventArgs e)
        {
            if (_pageRendered)
            {
                //BuildingId returned
                var buildingId = _client.ReturnBuildingIdFromBuildingName(SelectBuilding.SelectedItem.ToString());
                if (SearchRooms.Text != DefaultSearchString)
                {                  
                    //room search function
                    var results = _client.SearchRoomFunction(buildingId, SearchRooms.Text);
                    //Returned rooms populated into rooms list
                    RoomList.ItemsSource = results;
                }
                
            }
        }

        //confirm delete room popup displayed
        private void DeleteRoomPopup(object sender, RoutedEventArgs e)
        {
            DeleteRoom.IsOpen = true;
        }
        
        //close delete popup action
        private void CloseDeletePopup(object sender, RoutedEventArgs e)
        {
            DeleteRoom.IsOpen = false;
        }

        //confirmation of delete button function
        private void ConfirmDeleteButtonClicked(object sender, RoutedEventArgs e)
        {    
            //room selected
            var selectedRoom = (Room)RoomList.SelectedItem;
            var buildingId = _client.ReturnBuildingIdFromBuildingName(SelectBuilding.SelectedItem.ToString());
            
            //room selected value checked
            if (selectedRoom != null)
            {
                //Delete function of web service
                _client.DeleteRoom(selectedRoom.RoomId);

                //Buildings room list refreshed
                var noResults = _client.ReturnBuildingRooms(buildingId);
                RoomList.ItemsSource = noResults;
            }
            //Popup closed and default page content reset
            DeleteRoom.IsOpen = false;
            RoomNameText.Text = "No Selection has been made.";
            RoomDescriptionText.Text = "None";
            RoomTypeText.Text = "None";
            BuildingText.Text = "None";
            EventsText.Text = "None";
            CapacityText.Text = "None";
        }

        //disable page action
        public void DisablePage()
        {
            SearchRooms.IsEnabled = false;
            RoomList.IsEnabled = false;
            EditRoomButton.IsEnabled = false;
            DeleteRoomButton.IsEnabled = false;
        }
        
        //enable page features
        public void EnablePage()
        {
            SearchRooms.IsEnabled = true;
            RoomList.IsEnabled = true;
            EditRoomButton.IsEnabled = true;
            DeleteRoomButton.IsEnabled = true;
        }

        //Add new room window opened
        public void AddNewRoom(object sender, RoutedEventArgs e)
        {
            //new window created and opened
            var newRoom = new CreateNewRoom(_userId);
            newRoom.Show();
        }

        //Edit room wondow opened with selected room id passed in
        public void EditRoom(object sender, RoutedEventArgs e)
        {
            //room selected
            var roomId = (Room) RoomList.SelectedItem;
            if (roomId != null)
            {
                //new window cretaded and opened
                var newRoom = new EditRoom(_userId, roomId.RoomId);
                newRoom.Show();
            }
            
        }

        //No builings alert opened
        public void OpenBuildingsAlert()
        {
            NoBuildings.IsOpen = true;
        }
        //close buildings alert button function
        public void CloseBuildingsAlert(object sender, RoutedEventArgs e)
        {
            NoBuildings.IsOpen = false;
        }


    }
}
