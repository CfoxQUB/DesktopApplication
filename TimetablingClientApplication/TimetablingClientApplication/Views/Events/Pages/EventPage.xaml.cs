using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace TimetablingClientApplication.Views.Events.Pages
{
    /// <summary>
    /// Interaction logic for EventPage.xaml
    /// </summary>
    public partial class EventPage : Page
    {
        private TimetablingService.TimetablingServiceClient client = new TimetablingService.TimetablingServiceClient();

        private ObservableCollection<TimetablingService.Event> EventsList = new ObservableCollection<TimetablingService.Event>();
        private bool _buildingSelected;
        //private int LoggedInUserId;

        public EventPage()
        {
            InitializeComponent();
            _buildingSelected = false;
            //LoggedInUserId = userId;
            var eventList = client.ReturnEvents();
            var buildingsList = client.ReturnBuildings();

            var roomsList = client.ReturnBuildingRooms(buildingsList.First().BuildingId);

            if (eventList != null)
            {
                foreach (TimetablingService.Event e in eventList)
                {
                    EventsList.Add(e);
                }
            }

            foreach (TimetablingService.Building b in buildingsList)
            {
                BuildingList.Items.Add(b.BuildingName);
            }

            foreach (TimetablingService.Room r in roomsList)
            {
                RoomList.Items.Add(r.RoomName);
            }
            
            BuildingList.Text = buildingsList.First().BuildingName;
            RoomList.Text = roomsList.First().RoomName;
            
           ListedEvents.ItemsSource = EventsList;
           
        }
        
        public void Building_Selection_Changed(object sender, RoutedEventArgs e)
        {
            _buildingSelected = false;
            var tempName = BuildingList.SelectedItem;
            var buildingId = client.ReturnBuildingIdFromBuildingName(tempName.ToString());
            var roomList = client.ReturnBuildingRooms(buildingId);

            RoomList.Items.Clear();         
            
            foreach (var x in roomList)
            {
                RoomList.Items.Add(x.RoomName);
            }
            RoomList.Text = roomList.First().RoomName;
            var events = client.ReturnRoomEvents(roomList.First().RoomId);
            EventsList.Clear();
 
            foreach (TimetablingService.Event s in events)
            {
                EventsList.Add(s);
            }

            ListedEvents.ItemsSource = EventsList;
            _buildingSelected = true;
      
        }

        public void Room_Selection_Changed(object sender, RoutedEventArgs e)
        {
            if (_buildingSelected == false)
            {
                _buildingSelected = true;
                return;
            }

            var tempName = BuildingList.SelectedItem;
            var buildingId = client.ReturnBuildingIdFromBuildingName(tempName.ToString());

            var tempRoom = RoomList.SelectedItem;
            var roomId = client.ReturnRoomId(buildingId, RoomList.SelectedItem.ToString());
            var events = client.ReturnRoomEvents(roomId);
     
            EventsList.Clear();

            foreach (TimetablingService.Event s in events)
            {
                EventsList.Add(s);
            }

            ListedEvents.ItemsSource = EventsList;
           
        }
    
    }
}
