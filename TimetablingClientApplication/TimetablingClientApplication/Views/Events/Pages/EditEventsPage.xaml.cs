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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TimetablingClientApplication.Views.Events.Pages
{
    /// <summary>
    /// Interaction logic for EditEventsPage.xaml
    /// </summary>
    public partial class EditEventsPage : Page
    {
        TimetablingService.TimetablingServiceClient client = new TimetablingService.TimetablingServiceClient();
        private string defaultSearchString = "Enter Search Details here . . . .";
        private int EditedEventId = new int();
        private int LoggedInUserId = new int();
        private List<TimetablingService.Time> TimesList = new List<TimetablingService.Time>();
        private List<TimetablingService.RepeatType> RepeatsList = new List<TimetablingService.RepeatType>();
        private List<TimetablingService.EventType> EventTypes = new List<TimetablingService.EventType>();
        private List<TimetablingService.Building> BuildingsList = new List<TimetablingService.Building>();
        private List<TimetablingService.Course> CourseList = new List<TimetablingService.Course>();

        private bool BuildingSelected = false;
        private bool CourseSelected = false;
        
        public ObservableCollection<TimetablingService.Event> eventsList = new ObservableCollection<TimetablingService.Event>();
        public EditEventsPage(int UserId)
        {
            InitializeComponent();
            LoggedInUserId = UserId;
            var events = client.ReturnEvents();
            var firstEvent = events.First();
            EditedEventId = firstEvent.EventId;
            TimesList.AddRange(client.ReturnTimes());
            RepeatsList.AddRange(client.ReturnRepeatTypes());
            EventTypes.AddRange(client.ReturnEventTypes());
            BuildingsList.AddRange(client.ReturnBuildings());
            CourseList.AddRange(client.ReturnCourses());
            var ModulesList = client.ReturnCourseModules((int)firstEvent.Course);
            
            var eventBuilding = client.ReturnRoomBuilding((int)firstEvent.Room);
            var roomsList = client.ReturnBuildingRooms(eventBuilding);

            foreach (TimetablingService.Event e in events)
            {
                eventsList.Add(e);
            }

            foreach (var x in TimesList)
            {
                TimeList.Items.Add(x.TimeLiteral);
            }

            foreach (var x in RepeatsList)
            {
                RepeatSelect.Items.Add(x.RepeatTypeName);
            }

            foreach (var x in EventTypes)
            {
                EventTypeSelect.Items.Add(x.TypeName);
            }

            foreach (var x in CourseList)
            {
                CourseSelect.Items.Add(x.CourseName);
            }

            foreach (var x in roomsList)
            {
                RoomSelect.Items.Add(x.RoomName);
            }

            foreach (var x in ModulesList)
            {
                ModuleSelect.Items.Add(x.ModuleName);
            }

            foreach (var x in BuildingsList)
            {
                BuildingSelect.Items.Add(x.BuildingName);
            }
           ListedEvents.ItemsSource = eventsList;
           

           EventTitle.Text = firstEvent.EventTitle;
           EventDescription.Text = firstEvent.EventDescription;
           StartDate.SelectedDate = firstEvent.StartDate;
           
           DurationList.Items.Add(15);
           DurationList.Items.Add(30);
           DurationList.Items.Add(60);

           SearchFilter.Items.Add("Event Title");
           SearchFilter.Items.Add("Event Description");
           SearchFilter.SelectedItem = "Event Title";
           SearchField.Text = defaultSearchString;

           DurationList.SelectedItem = firstEvent.Duration;
           TimeList.SelectedItem = TimesList.SingleOrDefault(x=>x.TimeId == firstEvent.Time).TimeLiteral;
           RepeatSelect.SelectedItem = RepeatsList.SingleOrDefault(x => x.RepeatTypeId == firstEvent.Repeats).RepeatTypeName;
           EventTypeSelect.SelectedItem = EventTypes.SingleOrDefault(x => x.TypeId == firstEvent.EventType).TypeName;
           BuildingSelect.SelectedItem = BuildingsList.SingleOrDefault(x => x.BuildingId == eventBuilding).BuildingName;
           CourseSelect.SelectedItem = CourseList.SingleOrDefault(x => x.CourseId == firstEvent.Course).CourseName;
           ModuleSelect.SelectedItem = ModulesList.SingleOrDefault(x => x.ModuleId == firstEvent.Module).ModuleName;
           RoomSelect.SelectedItem = roomsList.SingleOrDefault(x => x.RoomId == firstEvent.Room).RoomName;
        }

        private void SearchField_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchField.Text = "";
        }

        private void SearchField_LoseFocus(object sender, RoutedEventArgs e)
        {
            SearchField.Text = defaultSearchString;
        }

        private void ReturnSearchResults(object sender, RoutedEventArgs e)
        {
            if (SearchField.Text != defaultSearchString)
            {
                var results = client.SearchFunction(SearchFilter.SelectedItem.ToString(), SearchField.Text);

                ListedEvents.ItemsSource = results;
                return;
            }
            
            if (ListedEvents.ItemsSource == null)
            {
                var noResults = client.ReturnEvents();
                ListedEvents.ItemsSource = noResults;
            }
            
        }


        public void NewEventSelected(object sender, RoutedEventArgs e)
        {
            if (ListedEvents.SelectedItem == null)
            {
                return;
            }
            
            BuildingSelected = false;
            CourseSelected = false;
            
            var selectedEvent = (TimetablingService.Event)ListedEvents.SelectedItem;
            EditedEventId = selectedEvent.EventId;
            var ModulesList = client.ReturnCourseModules((int)selectedEvent.Course);

            var EventBuilding = client.ReturnRoomBuilding((int)selectedEvent.Room);
            var RoomsList = client.ReturnBuildingRooms(EventBuilding);

            RoomSelect.Items.Clear();
            foreach (var x in RoomsList)
            {
                RoomSelect.Items.Add(x.RoomName);
            }

            ModuleSelect.Items.Clear();
            foreach (var x in ModulesList)
            {
                ModuleSelect.Items.Add(x.ModuleName);
            }

            EventTitle.Text = selectedEvent.EventTitle;
            EventDescription.Text = selectedEvent.EventDescription;
            StartDate.SelectedDate = selectedEvent.StartDate;

            DurationList.SelectedItem = selectedEvent.Duration;
            TimeList.SelectedItem = TimesList.SingleOrDefault(x => x.TimeId == selectedEvent.Time).TimeLiteral;
            RepeatSelect.SelectedItem = RepeatsList.SingleOrDefault(x => x.RepeatTypeId == selectedEvent.Repeats).RepeatTypeName;
            EventTypeSelect.SelectedItem = EventTypes.SingleOrDefault(x => x.TypeId == selectedEvent.EventType).TypeName;
            BuildingSelect.SelectedItem = BuildingsList.SingleOrDefault(x => x.BuildingId == EventBuilding).BuildingName;
            CourseSelect.SelectedItem = CourseList.SingleOrDefault(x => x.CourseId == selectedEvent.Course).CourseName;
            ModuleSelect.SelectedItem = ModulesList.SingleOrDefault(x => x.ModuleId == selectedEvent.Module).ModuleName;
            RoomSelect.SelectedItem = RoomsList.SingleOrDefault(x => x.RoomId == selectedEvent.Room).RoomName;

            BuildingSelected = true;
            CourseSelected = true;
        }

        public void Building_Selection_Changed(object sender, RoutedEventArgs e)
        {
            if (BuildingSelected == false)
            {
                BuildingSelected = true;
                return;
            }

            var tempName = BuildingSelect.SelectedItem;
            var buildingId = client.ReturnBuildingIdFromBuildingName(tempName.ToString());
            var roomList = client.ReturnBuildingRooms(buildingId);
            RoomSelect.Items.Clear();

            foreach (var x in roomList)
            {
                RoomSelect.Items.Add(x.RoomName);
            }
            RoomSelect.Text = roomList.First().RoomName;
            return;
        }

        public void Course_Selection_Changed(object sender, RoutedEventArgs e)
        {
            if (CourseSelected == false)
            {
                CourseSelected = true;
                return;
            }

            var tempName = CourseSelect.SelectedItem;
            var CourseId = client.ReturnCourseIdFromCourseName(tempName.ToString());
            var moduleList = client.ReturnCourseModules(CourseId);
            ModuleSelect.Items.Clear();

            foreach (var x in moduleList)
            {
                ModuleSelect.Items.Add(x.ModuleName);
            }
            ModuleSelect.Text = moduleList.First().ModuleName;
            return;
        }

        private void ListedEvents_DeleteEvent(object sender, MouseButtonEventArgs e)
        {
            var deletedEvent = (TimetablingService.Event)ListedEvents.SelectedItem;
            if (deletedEvent != null)
            {
                client.DeleteEvent(deletedEvent.EventId);

                var refreshResults = client.ReturnEvents();
                ListedEvents.ItemsSource = refreshResults;

                ListedEvents.SelectedItem = refreshResults.First();
            }
        }

        public void Save_Event_Changes(object sender, RoutedEventArgs e)
        {

            if (client.EditEvent(EditedEventId, LoggedInUserId, EventTitle.Text, EventDescription.Text, EventTypeSelect.SelectedItem.ToString(), RepeatSelect.SelectedItem.ToString(), Convert.ToInt32(DurationList.SelectedValue), Convert.ToDateTime(StartDate.SelectedDate), TimeList.SelectedValue.ToString(), RoomSelect.SelectedValue.ToString(), CourseSelect.SelectedItem.ToString(), ModuleSelect.SelectedItem.ToString()))
            {
                var refreshResults = client.ReturnEvents();
                ListedEvents.ItemsSource = refreshResults;
                ListedEvents.SelectedItem = refreshResults.SingleOrDefault(x=>x.EventId == EditedEventId);
                return;
            }
               
        }
    }
}
