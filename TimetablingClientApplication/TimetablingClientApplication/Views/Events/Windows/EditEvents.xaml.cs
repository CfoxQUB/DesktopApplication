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
using TimetablingClientApplication;
using TimetablingClientApplication.TimetablingService;

namespace TimetablingClientApplication
{
    /// <summary>
    /// Interaction logic for EditEvents.xaml
    /// </summary>
    public partial class EditEvents : Window
    {
    
        private TimetablingServiceClient _client = new TimetablingServiceClient();
        private readonly string _defaultSearchString = "Enter Search Details here . . . .";
        private int _editedEventId;
        private readonly int _loggedInUserId;
        private readonly List<Time> _timesList = new List<Time>();
        private readonly List<RepeatType> _repeatsList = new List<RepeatType>();
        private readonly List<EventType> _eventTypes = new List<EventType>();
        private readonly List<Building> _buildingsList = new List<Building>();
        private readonly List<Course> _courseList = new List<Course>();

        private bool _buildingSelected;
        private bool _courseSelected;
        
        public ObservableCollection<Event> EventsList = new ObservableCollection<Event>();
        public EditEvents(int userId)
        {
            _loggedInUserId = userId;

            InitializeComponent();
            

            var events = new List<Event>();
            var temp = _client.ReturnEvents();

            if (temp != null)
            {
                events.AddRange(temp);
            }

            if (events.Any())
            {
                EventsinList(events);
                
            }
            else
            {
                NoEventsInList();
                NoEvents.IsOpen = true;
            }

        }

        private void SearchField_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchField.Text = "";
        }

        private void SearchField_LoseFocus(object sender, RoutedEventArgs e)
        {
            SearchField.Text = _defaultSearchString;
        }

        private void ReturnSearchResults(object sender, RoutedEventArgs e)
        {
            if (SearchField.Text != _defaultSearchString)
            {
                var results = _client.SearchFunction(SearchFilter.SelectedItem.ToString(), SearchField.Text);

                ListedEvents.ItemsSource = results;
                return;
            }
            
            if (ListedEvents.ItemsSource == null)
            {
                var noResults = _client.ReturnEvents();
                ListedEvents.ItemsSource = noResults;
            }
            
        }


        public void NewEventSelected(object sender, RoutedEventArgs e)
        {
            if (ListedEvents.SelectedItem == null)
            {
                return;
            }
            
            _buildingSelected = false;
            _courseSelected = false;
            
            var selectedEvent = (Event)ListedEvents.SelectedItem;
            _editedEventId = selectedEvent.EventId;
            var modulesList = _client.ReturnCourseModules(selectedEvent.Course);

            var eventBuilding = _client.ReturnRoomBuilding(selectedEvent.Room);
            var roomsList = _client.ReturnBuildingRooms(eventBuilding);

            RoomSelect.Items.Clear();
            foreach (var x in roomsList)
            {
                RoomSelect.Items.Add(x.RoomName);
            }

            ModuleSelect.Items.Clear();
            foreach (var x in modulesList)
            {
                ModuleSelect.Items.Add(x.ModuleName);
            }

            EventTitle.Text = selectedEvent.EventTitle;
            EventDescription.Text = selectedEvent.EventDescription;
            StartDate.SelectedDate = selectedEvent.StartDate;

            DurationList.SelectedItem = selectedEvent.Duration;
            TimeList.SelectedItem = _timesList.SingleOrDefault(x => x.TimeId == selectedEvent.Time).TimeLiteral;
            RepeatSelect.SelectedItem = _repeatsList.SingleOrDefault(x => x.RepeatTypeId == selectedEvent.Repeats).RepeatTypeName;
            EventTypeSelect.SelectedItem = _eventTypes.SingleOrDefault(x => x.TypeId == selectedEvent.EventType).TypeName;
            BuildingSelect.SelectedItem = _buildingsList.SingleOrDefault(x => x.BuildingId == eventBuilding).BuildingName;
            CourseSelect.SelectedItem = _courseList.SingleOrDefault(x => x.CourseId == selectedEvent.Course).CourseName;
            ModuleSelect.SelectedItem = modulesList.SingleOrDefault(x => x.ModuleId == selectedEvent.Module).ModuleName;
            RoomSelect.SelectedItem = roomsList.SingleOrDefault(x => x.RoomId == selectedEvent.Room).RoomName;

            _buildingSelected = true;
            _courseSelected = true;
        }

        public void Building_Selection_Changed(object sender, RoutedEventArgs e)
        {
            if (_buildingSelected == false)
            {
                _buildingSelected = true;
                return;
            }

            var tempName = BuildingSelect.SelectedItem;
            var buildingId = _client.ReturnBuildingIdFromBuildingName(tempName.ToString());
            var roomList = _client.ReturnBuildingRooms(buildingId);
            RoomSelect.Items.Clear();

            foreach (var x in roomList)
            {
                RoomSelect.Items.Add(x.RoomName);
            }
            RoomSelect.Text = roomList.First().RoomName;

        }

        public void Course_Selection_Changed(object sender, RoutedEventArgs e)
        {
            if (_courseSelected == false)
            {
                _courseSelected = true;
                return;
            }

            var tempName = CourseSelect.SelectedItem;
            var courseId = _client.ReturnCourseIdFromCourseName(tempName.ToString());
            var moduleList = _client.ReturnCourseModules(courseId);
            ModuleSelect.Items.Clear();

            foreach (var x in moduleList)
            {
                ModuleSelect.Items.Add(x.ModuleName);
            }
            ModuleSelect.Text = moduleList.First().ModuleName;
        }

        private void ListedEvents_DeleteEvent(object sender, MouseButtonEventArgs e)
        {
            DeleteEvent.IsOpen = true;
        }

        public void Save_Event_Changes(object sender, RoutedEventArgs e)
        {
            if (CheckForNonValues())
            {
                if (_client.EditEvent(_editedEventId, _loggedInUserId, EventTitle.Text, EventDescription.Text,
                    EventTypeSelect.SelectedItem.ToString(), RepeatSelect.SelectedItem.ToString(),
                    Convert.ToInt32(DurationList.SelectedValue), Convert.ToDateTime(StartDate.SelectedDate),
                    TimeList.SelectedValue.ToString(), RoomSelect.SelectedValue.ToString(),
                    CourseSelect.SelectedItem.ToString(), ModuleSelect.SelectedItem.ToString()))
                {
                    var refreshResults = _client.ReturnEvents();
                    ListedEvents.ItemsSource = refreshResults;
                    ListedEvents.SelectedItem = refreshResults.SingleOrDefault(x => x.EventId == _editedEventId);
                }
            }
        }

        public void NoEventsInList()
        {
            SearchField.IsEnabled = false;
            EventTitle.IsEnabled = false;
            EventDescription.IsEnabled = false;
            StartDate.IsEnabled = false;
            TimeList.IsEnabled = false;
            DurationList.IsEnabled = false;
            EventTypeSelect.IsEnabled = false;
            CourseSelect.IsEnabled = false;
            ModuleSelect.IsEnabled = false;
            BuildingSelect.IsEnabled = false;
            RoomSelect.IsEnabled = false;
            ListedEvents.IsEnabled = false;
            SearchFilter.IsEnabled = false;
            SubmitChangesButton.IsEnabled = false;
        }

        public void EventsinList(List<Event> events)
        {
            #region Populate Page

            foreach (Event e in events)
            {
                EventsList.Add(e);
            }
            
            ListedEvents.ItemsSource = EventsList;

            var firstEvent = events.First();
            _editedEventId = firstEvent.EventId;

            var eventBuilding = _client.ReturnRoomBuilding(firstEvent.Room);
            var roomsList = _client.ReturnBuildingRooms(eventBuilding);
            var modulesList = _client.ReturnCourseModules(firstEvent.Course);

            foreach (var x in roomsList)
            {
                RoomSelect.Items.Add(x.RoomName);
            }

            foreach (var x in modulesList)
            {
                ModuleSelect.Items.Add(x.ModuleName);
            }

            EventTitle.Text = firstEvent.EventTitle;
            EventDescription.Text = firstEvent.EventDescription;
            StartDate.SelectedDate = firstEvent.StartDate;

            _timesList.AddRange(_client.ReturnTimes());
            _repeatsList.AddRange(_client.ReturnRepeatTypes());
            _eventTypes.AddRange(_client.ReturnEventTypes());
            _buildingsList.AddRange(_client.ReturnBuildings());
            _courseList.AddRange(_client.ReturnCourses());

            if (_timesList.Any())
            {
                foreach (var x in _timesList)
                {
                    TimeList.Items.Add(x.TimeLiteral);
                }
            }
            else
            {
                TimeList.Items.Add("N/A");
            }
           
            if (_repeatsList.Any())
            {
                foreach (var x in _repeatsList)
                {
                    RepeatSelect.Items.Add(x.RepeatTypeName);
                }
            }
            else
            {
                RepeatSelect.Items.Add("N/A");
            }
            
            if (_eventTypes.Any())
            {
                foreach (var x in _eventTypes)
                {
                    EventTypeSelect.Items.Add(x.TypeName);
                }
            }
            else
            {
                EventTypeSelect.Items.Add("N/A");
            }
            
            if (_courseList.Any())
            {
                foreach (var x in _courseList)
                {
                    CourseSelect.Items.Add(x.CourseName);
                }
            }
            else
            {
                CourseSelect.Items.Add("N/A");
            }
            
            if (_buildingsList.Any())
            {
                foreach (var x in _buildingsList)
                {
                    BuildingSelect.Items.Add(x.BuildingName);
                }
            }
            else
            {
                BuildingSelect.Items.Add("N/A");
            }

            DurationList.Items.Add(15);
            DurationList.Items.Add(30);
            DurationList.Items.Add(60);

            SearchFilter.Items.Add("Event Title");
            SearchFilter.Items.Add("Event Description");
            SearchFilter.SelectedItem = "Event Title";
            SearchField.Text = _defaultSearchString;

            #endregion

            DurationList.SelectedItem = firstEvent.Duration;

            #region Select Events properties in Dropdowns

            var timeListItem = _timesList.SingleOrDefault(x => x.TimeId == firstEvent.Time);
            var repeatSelectItem = _repeatsList.SingleOrDefault(x => x.RepeatTypeId == firstEvent.Repeats);
            var eventTypeSelectItem = _eventTypes.SingleOrDefault(x => x.TypeId == firstEvent.EventType);
            var buildingSelectItem = _buildingsList.SingleOrDefault(x => x.BuildingId == eventBuilding);
            var courseSelectItem = _courseList.SingleOrDefault(x => x.CourseId == firstEvent.Course);
            var moduleSelectItem = modulesList.SingleOrDefault(x => x.ModuleId == firstEvent.Module);
            var roomSelectItem = roomsList.SingleOrDefault(x => x.RoomId == firstEvent.Room);

            if (timeListItem != null)
            {
                TimeList.SelectedItem = timeListItem.TimeLiteral;
            }
            else
            {
                TimeList.SelectedIndex = 0;
            }

            if (repeatSelectItem != null)
            {
                RepeatSelect.SelectedItem = repeatSelectItem.RepeatTypeName;
            }
            else
            {
                RepeatSelect.SelectedIndex = 0;
            }

            if (eventTypeSelectItem != null)
            {
                EventTypeSelect.SelectedItem = eventTypeSelectItem.TypeName;
            }
            else
            {
                EventTypeSelect.SelectedIndex = 0;
            }

            if (buildingSelectItem != null)
            {
                BuildingSelect.SelectedItem = buildingSelectItem.BuildingName;
            }
            else
            {
                BuildingSelect.SelectedIndex = 0;
            }


            if (courseSelectItem != null)
            {
                CourseSelect.SelectedItem = courseSelectItem.CourseName;
            }
            else
            {
                CourseSelect.SelectedIndex = 0;
            }


            if (moduleSelectItem != null)
            {
                ModuleSelect.SelectedItem = moduleSelectItem.ModuleName;
            }
            else
            {
                ModuleSelect.SelectedIndex = 0;
            }
            
            if (roomSelectItem != null)
            {
                RoomSelect.SelectedItem = roomSelectItem.RoomName;
            }
            else
            {
                RoomSelect.SelectedIndex = 0;
            }
            #endregion

        }

        public bool CheckForNonValues()
        {
            if (EventTypeSelect.SelectedItem.ToString() != "N/A" && RepeatSelect.SelectedItem.ToString() != "N/A" && TimeList.SelectedValue.ToString() != "N/A" && RoomSelect.SelectedValue.ToString() != "N/A" && CourseSelect.SelectedItem.ToString() != "N/A" && ModuleSelect.SelectedItem.ToString()  != "N/A")
            {
                return true;
            }

            return false;
        }

        public void CloseNoEventsPopup(object sender, RoutedEventArgs e)
        {
            NoEvents.IsOpen = false;
        }

        public void DeleteEventButtonClicked(object sender, RoutedEventArgs e)
        {
            var deletedEvent = (Event)ListedEvents.SelectedItem;
            if (deletedEvent != null)
            {
                if (_client.DeleteEvent(deletedEvent.EventId))
                {
                    var refreshResults = new List<Event>();
                    var temp = _client.ReturnEvents();

                    if (temp != null)
                    {
                        refreshResults.AddRange(temp);
                        ListedEvents.ItemsSource = refreshResults;
                        ListedEvents.SelectedItem = refreshResults.First();
                        return;
                    }
                    ListedEvents.ItemsSource = refreshResults;
                    NoEventsInList();
                }

            }
            DeleteEvent.IsOpen = false;
        }

        public void CloseDeleteEventPopup(object sender, RoutedEventArgs e)
        {
            DeleteEvent.IsOpen = false;
        }
    }
}
