using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TimetablingClientApplication.TimetablingService;

namespace TimetablingClientApplication.Views.Events.Pages
{
    /// <summary>
    /// Interaction logic for EventPage.xaml
    /// </summary>
    public partial class EventPage 
    {
        //Webservice functionality exposed through service reference
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        //observable collection used to display events associated with a particular room
        private readonly ObservableCollection<Event> _eventsList = new ObservableCollection<Event>();
        //Boolean value used to indicate building select is rendered
        private bool _buildingSelected;
     
        public EventPage()
        {
            InitializeComponent();
            //rendering of buildings List
            _buildingSelected = false;
            var buildingsList = _client.ReturnBuildings();
            if (buildingsList != null)
            {
                foreach (var b in buildingsList)
                {
                    //Popuation of buildings List
                    BuildingList.Items.Add(b.BuildingName);
                }
                BuildingList.Text = buildingsList.First().BuildingName;
                //Rendering of rooms associated with building
                var roomsList = _client.ReturnBuildingRooms(buildingsList.First().BuildingId);
                if (roomsList != null)
                {
                    foreach (var r in roomsList)
                    {   //Population of room select item
                        RoomList.Items.Add(r.RoomName);
                    }
                    RoomList.Text = roomsList.First().RoomName;
                    //Rendering of events List for room events
                    var eventList = _client.ReturnRoomEvents(roomsList.First().RoomId);
                    if (eventList != null)
                    {
                        //Population of events list
                        foreach (Event e in eventList)
                        {
                            _eventsList.Add(e);
                        }
                        EventCount.Content = eventList.Count().ToString("D");
                    }
                    ListedEvents.ItemsSource = _eventsList;
                }
                else
                {
                    //No rooms
                    RoomList.IsEnabled = false;
                }
            }
            else
            {
                //No buildings or rooms so filters disabled
                RoomList.IsEnabled = false;
                BuildingList.IsEnabled = false;
            }

       }
        
        //Building selection changed, update rooms and events
        public void Building_Selection_Changed(object sender, RoutedEventArgs e)
        {
            if (!_buildingSelected)
            {
                return;
            }
            //building selected information
            var tempName = BuildingList.SelectedItem;
            var buildingId = _client.ReturnBuildingIdFromBuildingName(tempName.ToString());
            
            //Rooms list of building returned
            var roomList = _client.ReturnBuildingRooms(buildingId);
            RoomList.Items.Clear();
            if (roomList != null)
            {
                //rooms List enabled and populated
                RoomList.IsEnabled = true;
                foreach (var x in roomList)
                {
                    RoomList.Items.Add(x.RoomName);
                }
                RoomList.Text = roomList.First().RoomName;

                //Events list of room populated
                var events = _client.ReturnRoomEvents(roomList.First().RoomId);
                _eventsList.Clear();
                if (events != null)
                {   //Events added to the events List
                    foreach (Event s in events)
                    {
                        _eventsList.Add(s);
                    }
                    EventCount.Content = events.Count().ToString("D");
                    ListedEvents.ItemsSource = _eventsList;
                }
            }
            else
            {
                //rooms list disabled
                RoomList.IsEnabled = false;
                EventCount.Content = "0";
                _eventsList.Clear();
            }
            _buildingSelected = true;
      
        }

        //room secltion changed update events list
        public void Room_Selection_Changed(object sender, RoutedEventArgs e)
        {
            if (!_buildingSelected)
            {
                _buildingSelected = true;
                return;
            }

            //Building In formation selected
            var tempName = BuildingList.SelectedItem;
            var buildingId = _client.ReturnBuildingIdFromBuildingName(tempName.ToString());
            //Room information checked if valid
            if (RoomList.SelectedItem != null)
            {
                //Room inforamtion returned
                var roomId = _client.ReturnRoomId(buildingId, RoomList.SelectedItem.ToString());
                var events = _client.ReturnRoomEvents(roomId);

                //EventsLsit cleard for repopoulation
                _eventsList.Clear();
                if (events != null)
                {
                    //Events list repopulated
                    foreach (Event s in events)
                    {
                        _eventsList.Add(s);
                    }
                    EventCount.Content = events.Count().ToString("D");
                    ListedEvents.ItemsSource = _eventsList;
                }
                else
                {
                    //Event count reset
                    EventCount.Content = "0";
                }
            }
        }
    
    }
}
