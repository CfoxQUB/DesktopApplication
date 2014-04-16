using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Collections.ObjectModel;
using TimetablingClientApplication.TimetablingService;
using TimetablingClientApplication.Views.Database.Windows;

namespace TimetablingClientApplication.Views.Database.Pages
{
    /// <summary>
    /// Interaction logic for RoomManagement.xaml
    /// </summary>
    public partial class RoomManagement : Page
    {
        private TimetablingServiceClient _client = new TimetablingServiceClient();

        private readonly ObservableCollection<Room> _roomCollectionList = new ObservableCollection<Room>();

        private readonly string _defaultSearchString = "Search for Rooms . . . ";

        private bool pageRendered = false;

        private int _userId;

        public RoomManagement(int userId)
        {
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
                pageRendered = true;
                return;
            }
            OpenBuildingsAlert();
            DisablePage();
        }

        public void BuildingSelectionChange(object sender, RoutedEventArgs e)
        {
            if (pageRendered)
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
                    EventsText.Text = eventNumber.Count().ToString();
                }
                else
                {
                    EventsText.Text = "0";
                }

                CapacityText.Text = selectedEvent.Capacity.ToString();

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
            SearchRooms.Text = _defaultSearchString;
        }

        private void ReturnSearchResults(object sender, RoutedEventArgs e)
        {
            if (pageRendered)
            {
                var buildingId = _client.ReturnBuildingIdFromBuildingName(SelectBuilding.SelectedItem.ToString());
                if (SearchRooms.Text != _defaultSearchString)
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
