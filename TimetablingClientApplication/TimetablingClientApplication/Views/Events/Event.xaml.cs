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

namespace TimetablingClientApplication
{
    /// <summary>
    /// Interaction logic for Event.xaml
    /// </summary>
    public partial class Event : Window
    {
        TimetablingService.TimetablingServiceClient client = new TimetablingService.TimetablingServiceClient();

        public ObservableCollection<TimetablingService.Event> eventsList = new ObservableCollection<TimetablingService.Event>();
        public bool buildingSelected = false;
        int LoggedInUserId = new int();

        public Event(int UserId)
        {
            InitializeComponent();
            LoggedInUserId = UserId;
            var EventList = client.ReturnEvents();
            var buildingsList = client.ReturnBuildings();

            var RoomsList = client.ReturnBuildingRooms(buildingsList.First().BuildingId);

            if (EventList != null)
            {
                foreach (TimetablingService.Event e in EventList)
                {
                    eventsList.Add(e);
                }
            }

            foreach (TimetablingService.Building b in buildingsList)
            {
                BuildingList.Items.Add(b.BuildingName);
            }

            foreach (TimetablingService.Room r in RoomsList)
            {
                RoomList.Items.Add(r.RoomName);
            }
            
            BuildingList.Text = buildingsList.First().BuildingName;
            RoomList.Text = RoomsList.First().RoomName;
            
           ListedEvents.ItemsSource = eventsList;
           
        }
        

        #region Navigation
        private void MenuItem_NewEvent_Click(object sender, RoutedEventArgs e)
        {
            CreateEvents createEvents = new CreateEvents(LoggedInUserId);
            createEvents.Show();
        }

        private void MenuItem_EditEvent_Click(object sender, RoutedEventArgs e)
        {
            EditEvents editEvents = new EditEvents(LoggedInUserId);
            editEvents.Show();
        }
 
        private void Menuitem_TimetableView_Click(object sender, RoutedEventArgs e)
        {
            Timetable timetableView = new Timetable(LoggedInUserId);
            timetableView.Show();
            this.Close();
        }
        #endregion


        public void Building_Selection_Changed(object sender, RoutedEventArgs e)
        {
            buildingSelected = false;
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
            eventsList.Clear();
 
            foreach (TimetablingService.Event s in events)
            {
                eventsList.Add(s);
            }

            ListedEvents.ItemsSource = eventsList;
            buildingSelected = true;
            return;
        }

        public void Room_Selection_Changed(object sender, RoutedEventArgs e)
        {
            if (buildingSelected == false)
            {
                buildingSelected = true;
                return;
            }

            var tempName = BuildingList.SelectedItem;
            var buildingId = client.ReturnBuildingIdFromBuildingName(tempName.ToString());

            var tempRoom = RoomList.SelectedItem;
            var roomId = client.ReturnRoomId(buildingId, RoomList.SelectedItem.ToString());
            var events = client.ReturnRoomEvents(roomId);
     
            eventsList.Clear();

            foreach (TimetablingService.Event s in events)
            {
                eventsList.Add(s);
                
            }

            ListedEvents.ItemsSource = eventsList;
            return;
        }
    }
}
