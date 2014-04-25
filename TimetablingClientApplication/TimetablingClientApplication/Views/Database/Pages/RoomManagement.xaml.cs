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
            _pageRendered = false;

            _userId = userId;
            InitializeComponent();
            var buildingsList = _client.ReturnBuildings();
            var rooms = _client.ReturnBuildingRooms(buildingsList.First().BuildingId);

            if (buildingsList != null)
            {

                foreach (var b in buildingsList)
                {
                    SelectBuilding.Items.Add(b.BuildingName);
                }
            
                if (rooms != null)
                {
                    foreach (var r in rooms)
                    {
                        _roomCollectionList.Add(r);
                    } 
                }
                
                RoomNameText.Text = "No Selection has been made.";
                RoomDescriptionText.Text = "None";
                RoomTypeText.Text = "None";
                BuildingText.Text = "None";
                EventsText.Text = "None";
                CapacityText.Text = "None";

                RoomList.ItemsSource = _roomCollectionList;
                SelectBuilding.SelectedItem = buildingsList.First().BuildingName;
                _pageRendered = true;
                return;
            }
            OpenBuildingsAlert();
            DisablePage();
        }

        public void BuildingSelectionChange(object sender, RoutedEventArgs e)
        {
            if (_pageRendered)
            {
                var buildingId = _client.ReturnBuildingIdFromBuildingName(SelectBuilding.SelectedItem.ToString());

                var noResults = _client.ReturnBuildingRooms(buildingId);

                RoomList.ItemsSource = noResults;

                if (noResults != null)
                {
                   EnablePage();
                }
                else
                {
                    DisablePage();
                }
            }
        }

        public void NewRoomSelected(object sender, RoutedEventArgs e)
        {
            var selectedEvent = (Room)RoomList.SelectedItem;
            if (selectedEvent != null)
            {
                RoomNameText.Text = selectedEvent.RoomName;
                RoomDescriptionText.Text = selectedEvent.RoomDescription;

                var roomType = _client.ReturnRoomTypes().SingleOrDefault(x => x.RoomTypeId == selectedEvent.RoomType);

                if (roomType != null)
                {
                    RoomTypeText.Text = roomType.RoomeTypeDescription;
                }
                else
                {
                    RoomTypeText.Text = "N/A";
                }

                var eventNumber = _client.ReturnRoomEvents(selectedEvent.RoomId);

                if (eventNumber != null)
                {
                    EventsText.Text = eventNumber.Count().ToString("D");
                }
                else
                {
                    EventsText.Text = "0";
                }

                CapacityText.Text = selectedEvent.Capacity.ToString("D");

                var building = _client.ReturnRoomBuilding(selectedEvent.RoomId);
                var buildingInfo = _client.ReturnBuildings().SingleOrDefault(x => x.BuildingId == building);

                if (buildingInfo != null)
                {
                    BuildingText.Text = buildingInfo.BuildingNumber + "\r\n" + buildingInfo.AddressLine1 + "\r\n"
                                        + buildingInfo.AddressLine2 + "\r\n" + buildingInfo.City + "\r\n" +
                                        buildingInfo.PostalCode;
                }
            }
        }

        private void SearchField_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchRooms.Text = "";
        }

        private void SearchField_LoseFocus(object sender, RoutedEventArgs e)
        {
            SearchRooms.Text = DefaultSearchString;
        }

        private void ReturnSearchResults(object sender, RoutedEventArgs e)
        {
            if (_pageRendered)
            {
                var buildingId = _client.ReturnBuildingIdFromBuildingName(SelectBuilding.SelectedItem.ToString());
                if (SearchRooms.Text != DefaultSearchString)
                {                  
                    var results = _client.SearchRoomFunction(buildingId, SearchRooms.Text);
                    
                    RoomList.ItemsSource = results;
                }
                
            }
        }

        private void DeleteRoomPopup(object sender, RoutedEventArgs e)
        {
            DeleteRoom.IsOpen = true;
        }
        
        private void CloseDeletePopup(object sender, RoutedEventArgs e)
        {
            DeleteRoom.IsOpen = false;
        }

        private void ConfirmDeleteButtonClicked(object sender, RoutedEventArgs e)
        {    
            var selectedRoom = (Room)RoomList.SelectedItem;
            var buildingId = _client.ReturnBuildingIdFromBuildingName(SelectBuilding.SelectedItem.ToString());
            
            if (selectedRoom != null)
            {
                _client.DeleteRoom(selectedRoom.RoomId);

                var noResults = _client.ReturnBuildingRooms(buildingId);

                RoomList.ItemsSource = noResults;
            }

            DeleteRoom.IsOpen = false;
            RoomNameText.Text = "No Selection has been made.";
            RoomDescriptionText.Text = "None";
            RoomTypeText.Text = "None";
            BuildingText.Text = "None";
            EventsText.Text = "None";
            CapacityText.Text = "None";
        }


        public void DisablePage()
        {
            SearchRooms.IsEnabled = false;
            RoomList.IsEnabled = false;
            EditRoomButton.IsEnabled = false;
            DeleteRoomButton.IsEnabled = false;
        }
        
        public void EnablePage()
        {
            SearchRooms.IsEnabled = true;
            RoomList.IsEnabled = true;
            EditRoomButton.IsEnabled = true;
            DeleteRoomButton.IsEnabled = true;
        }

        public void AddNewRoom(object sender, RoutedEventArgs e)
        {
            var newRoom = new CreateNewRoom(_userId);
            newRoom.Show();
        }

        public void EditRoom(object sender, RoutedEventArgs e)
        {
            var roomId = (Room) RoomList.SelectedItem;
            if (roomId != null)
            {
                var newRoom = new EditRoom(_userId, roomId.RoomId);
                newRoom.Show();
            }
            
        }

        public void OpenBuildingsAlert()
        {
            NoBuildings.IsOpen = true;
        }
        
        public void CloseBuildingsAlert(object sender, RoutedEventArgs e)
        {
            NoBuildings.IsOpen = false;
        }


    }
}
