using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TimetablingClientApplication.TimetablingService;

namespace TimetablingClientApplication.Views.Events.Pages
{
    /// <summary>
    /// Interaction logic for EditEvents.xaml
    /// </summary>
    public partial class EditEventsPage
    {
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        private const String DefaultSearchString = "Enter Search Details here . . . .";
        private int _editedEventId;
        private readonly int _loggedInUserId;
        private int _selectedEventId;

        private readonly List<Building> _buildingsList = new List<Building>();
        private readonly List<Room> _roomsList = new List<Room>();
        private readonly List<Course> _courseList = new List<Course>();
        private readonly List<Module> _moduleList = new List<Module>();

        private bool _buildingSelected;
        private bool _courseSelected;

        private bool _roomEnabled;
        private bool _moduleEnabled;

        private readonly ObservableCollection<Event> _eventsList = new ObservableCollection<Event>();

        public EditEventsPage(int userId)
        {
            _loggedInUserId = userId;

            InitializeComponent();

            var events = new List<Event>();
            events.AddRange(_client.ReturnEvents());

            if (events.Any())
            {
                foreach (var e in events)
                {
                    _eventsList.Add(e);
                }
            }

            ListedEvents.ItemsSource = _eventsList;

            _courseList.AddRange(_client.ReturnCourses());
            _moduleList.AddRange(_client.ReturnCourseModules(_courseList.First().CourseId));

            _buildingsList.AddRange(_client.ReturnBuildings());
            _roomsList.AddRange(_client.ReturnBuildingRooms(_buildingsList.First().BuildingId));

            RenderCourses(_courseList, 0);
            RenderModules(_moduleList, 0);
            RenderBuildings(_buildingsList, 0);
            RenderRooms(_roomsList, 0);
            RenderDurations(15);

            RenderTimes(0);
            RenderEventTypes(0);

            SearchFilter.Items.Add("Event Title");
            SearchFilter.Items.Add("Event Description");
            SearchFilter.SelectedItem = "Event Title";

            SearchField.Text = DefaultSearchString;

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
            AddRoom.IsEnabled = false;
            AddModule.IsEnabled = false;
            RoomSelect.IsEnabled = false;
            StatusList.IsEnabled = false;
            SubmitChangesButton.IsEnabled = false;
            SaveStatus.IsEnabled = false;

            StatusList.Items.Add("Confirmed");
            StatusList.Items.Add("Denied");
            StatusList.Items.Add("Pending");
            StatusList.Items.Add("New");
            StatusList.Text = "New";

            _buildingSelected = false;
            _courseSelected = false;

            _roomEnabled = true;
            _moduleEnabled = true;
        }

        private void SearchField_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchField.Text = "";
        }

        private void SearchField_LoseFocus(object sender, RoutedEventArgs e)
        {
            SearchField.Text = DefaultSearchString;
        }

        private void ReturnSearchResults(object sender, RoutedEventArgs e)
        {
            if (SearchField.Text != DefaultSearchString)
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

            EventTitle.IsEnabled = true;
            EventDescription.IsEnabled = true;
            StartDate.IsEnabled = true;
            TimeList.IsEnabled = true;
            DurationList.IsEnabled = true;
            EventTypeSelect.IsEnabled = true;
            CourseSelect.IsEnabled = true;
            ModuleSelect.IsEnabled = true;
            BuildingSelect.IsEnabled = true;
            RoomSelect.IsEnabled = true;
            AddRoom.IsEnabled = true;
            AddModule.IsEnabled = true;
            RoomSelect.IsEnabled = true;
            StatusList.IsEnabled = true;
            SubmitChangesButton.IsEnabled = true;
            SaveStatus.IsEnabled = true;

            _courseList.Clear();
            _moduleList.Clear();
            _buildingsList.Clear();
            _roomsList.Clear();

            var selectedEvent = (Event)ListedEvents.SelectedItem;
            _selectedEventId = selectedEvent.EventId;
            EventTitle.Text = selectedEvent.EventTitle;
            EventDescription.Text = selectedEvent.EventDescription;
            StartDate.SelectedDate = selectedEvent.StartDate;
            StatusList.SelectedItem = selectedEvent.Status;

            _editedEventId = selectedEvent.EventId;

            _courseList.AddRange(_client.ReturnCourses());

            if (selectedEvent.Course != 0)
            {
                var modules = _client.ReturnCourseModules(selectedEvent.Course);
                if (modules != null)
                {
                    _moduleList.AddRange(modules);
                }
            }
            else
            {
                _moduleList.AddRange(_client.ReturnCourseModules(_courseList.First().CourseId));
            }

            _buildingsList.AddRange(_client.ReturnBuildings());

            var roomBuilding = 0;
            var roomId = 0;

            if (selectedEvent.Room != 0)
            {
                roomBuilding = _client.ReturnRoomDetail(selectedEvent.Room).Building;
                roomId = selectedEvent.Room;
                var rooms = _client.ReturnBuildingRooms(roomBuilding);

                if (rooms != null)
                {
                    _roomsList.AddRange(rooms);
                }

            }
            else
            {
                _roomsList.AddRange(_client.ReturnBuildingRooms(_buildingsList.First().BuildingId));
            }


            RenderCourses(_courseList, selectedEvent.Course);
            RenderModules(_moduleList, selectedEvent.Module);
            RenderBuildings(_buildingsList, roomBuilding);
            RenderRooms(_roomsList, roomId);
            RenderTimes(selectedEvent.Time);
            RenderEventTypes(selectedEvent.EventType);
            RenderDurations(selectedEvent.Duration);


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
            RoomSelect.IsEnabled = true;
            if (roomList != null)
            {
                foreach (var x in roomList)
                {
                    RoomSelect.Items.Add(x.RoomName);
                }
                RoomSelect.Text = roomList.First().RoomName;
                RoomCapacity.Content = roomList.First().Capacity.ToString("D");
            }
            else
            {
                RoomSelect.Items.Add("None");
                RoomSelect.Text = "None";
                RoomCapacity.Content = "None";
            }

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

            ModuleSelect.IsEnabled = true;
            if (moduleList != null)
            {
                foreach (var x in moduleList)
                {
                    ModuleSelect.Items.Add(x.ModuleName);
                }
                ModuleSelect.Text = moduleList.First().ModuleName;
                var students = _client.ReturnModuleStudentsNumbers(moduleList.First().ModuleId);
                ModuleStudents.Content = students.ToString("D");
            }
            else
            {
                ModuleSelect.Items.Add("None");
                ModuleSelect.Text = "None";
                ModuleStudents.Content = "None";
            }
        }

        public void Module_Selection_Changed(object sender, RoutedEventArgs e)
        {
            if (_moduleEnabled)
            {
                var moduleName = ModuleSelect.SelectedValue.ToString();
                var moduleId = _client.ReturnModuleIdFromModuleName(moduleName);
                var moduleStudents = _client.ReturnModuleStudentsNumbers(moduleId);
                ModuleStudents.Content = moduleStudents.ToString("D");
            }
        }

        private void ListedEvents_DeleteEvent(object sender, MouseButtonEventArgs e)
        {
            DeleteEvent.IsOpen = true;
        }

        public void Save_Event_Changes(object sender, RoutedEventArgs e)
        {
            var course = "0";
            var module = "0";
            var room = "0";

            if (_roomEnabled)
            {
                room = RoomSelect.SelectedValue.ToString();
                if (room == "None")
                {
                    room = "0";
                }
            }

            if (_moduleEnabled)
            {
                course = CourseSelect.SelectedItem.ToString();
                module = ModuleSelect.SelectedItem.ToString();

                if (module == "None")
                {
                    module = "0";
                }
            }

            if (_client.EditEvent(_editedEventId, _loggedInUserId, EventTitle.Text, EventDescription.Text,
                EventTypeSelect.SelectedItem.ToString(), Convert.ToInt32(DurationList.SelectedValue),
                Convert.ToDateTime(StartDate.SelectedDate),
                TimeList.SelectedValue.ToString(), room,
                course, module))
            {
                var refreshResults = _client.ReturnEvents();
                ListedEvents.ItemsSource = refreshResults;
                ListedEvents.SelectedItem = refreshResults.SingleOrDefault(x => x.EventId == _editedEventId);
            }

        }

        public void Save_Event_Status(object sender, RoutedEventArgs e)
        {
            if (_selectedEventId != 0)
            {
                var status = StatusList.Text;
                if (_roomEnabled && _moduleEnabled)
                {
                    _client.ChangeEventStatus(status, _selectedEventId);

                    var events = new List<Event>();
                    events.AddRange(_client.ReturnEvents());
                    _eventsList.Clear();
                    if (events.Any())
                    {
                        foreach (var i in events)
                        {
                            _eventsList.Add(i);
                        }
                    }

                    ListedEvents.ItemsSource = _eventsList;
                    ListedEvents.SelectedItem = _eventsList.SingleOrDefault(x => x.EventId == _selectedEventId);

                }
                else
                {
                    EventStatus.IsOpen = true;
                }
            }
        }

        public void DisableFeatures()
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

        public void RenderCourses(List<Course> courseList, int courseId)
        {
            if (courseList.Count > 0)
            {
                CourseSelect.IsEnabled = true;
                CourseSelect.Items.Clear();

                foreach (var c in courseList)
                {
                    CourseSelect.Items.Add(c.CourseName);
                }
                var courseDetails = courseList.SingleOrDefault(x => x.CourseId == courseId);

                if (courseDetails != null)
                {
                    CourseSelect.SelectedItem = courseDetails.CourseName;
                    AddModule.IsChecked = true;
                }
                else
                {
                    CourseSelect.SelectedItem = courseList.First().CourseName;
                    AddModule.IsChecked = false;
                    CourseSelect.IsEnabled = false;
                    _moduleEnabled = false;
                }
            }
            else
            {
                CourseSelect.IsEnabled = false;
            }
        }

        public void RenderModules(List<Module> courseModules, int moduleId)
        {
            if (courseModules.Count > 0)
            {
                ModuleSelect.IsEnabled = true;
                ModuleSelect.Items.Clear();

                foreach (var c in courseModules)
                {
                    ModuleSelect.Items.Add(c.ModuleName);
                }

                var moduleDetails = courseModules.SingleOrDefault(x => x.ModuleId == moduleId);

                if (moduleDetails != null)
                {
                    ModuleSelect.SelectedItem = moduleDetails.ModuleName;
                    AddModule.IsChecked = true;

                }
                else
                {
                    ModuleSelect.SelectedItem = courseModules.First().ModuleName;
                    AddModule.IsChecked = false;
                    ModuleSelect.IsEnabled = false;
                    _moduleEnabled = false;
                }
            }
            else
            {
                ModuleSelect.IsEnabled = false;
            }
        }

        public void RenderBuildings(List<Building> buildingsList, int buildingId)
        {
            if (buildingsList.Count > 0)
            {
                BuildingSelect.IsEnabled = true;
                BuildingSelect.Items.Clear();
                foreach (var b in buildingsList)
                {
                    BuildingSelect.Items.Add(b.BuildingName);
                }

                var buildingDetails = buildingsList.SingleOrDefault(x => x.BuildingId == buildingId);

                if (buildingDetails != null)
                {
                    BuildingSelect.SelectedItem = buildingDetails.BuildingName;
                    AddRoom.IsChecked = true;
                }
                else
                {
                    BuildingSelect.SelectedItem = buildingsList.First().BuildingName;
                    AddRoom.IsChecked = false;
                    BuildingSelect.IsEnabled = false;
                    _roomEnabled = false;
                }
            }
            else
            {
                BuildingSelect.IsEnabled = false;
            }
        }

        public void RenderRooms(List<Room> buildingsRooms, int roomId)
        {
            if (buildingsRooms.Count > 0)
            {
                RoomSelect.IsEnabled = true;
                RoomSelect.Items.Clear();
                foreach (var b in buildingsRooms)
                {
                    RoomSelect.Items.Add(b.RoomName);
                }

                var roomsDetails = buildingsRooms.SingleOrDefault(x => x.RoomId == roomId);

                if (roomsDetails != null)
                {
                    RoomSelect.SelectedItem = roomsDetails.RoomName;
                    AddRoom.IsChecked = true;
                }
                else
                {
                    RoomSelect.SelectedItem = buildingsRooms.First().RoomName;
                    AddRoom.IsChecked = false;
                    RoomSelect.IsEnabled = false;
                    _roomEnabled = false;
                }
            }
            else
            {
                RoomSelect.IsEnabled = false;
            }
        }

        public void RenderTimes(int timeId)
        {
            var timeList = _client.ReturnTimes();
            TimeList.Items.Clear();
            if (timeList.Any())
            {
                TimeList.IsEnabled = true;
                foreach (var t in timeList)
                {
                    TimeList.Items.Add(t.TimeLiteral);
                }

                var timeDetails = timeList.SingleOrDefault(x => x.TimeId == timeId);

                if (timeDetails != null)
                {
                    TimeList.SelectedItem = timeDetails.TimeLiteral;
                }
                else
                {
                    RoomSelect.SelectedItem = timeList.First().TimeLiteral;
                }
            }
            else
            {
                TimeList.IsEnabled = false;
            }
        }

        public void RenderEventTypes(int typeId)
        {
            var typeList = _client.ReturnEventTypes();
            if (typeList.Any())
            {
                EventTypeSelect.IsEnabled = true;
                EventTypeSelect.Items.Clear();
                foreach (var t in typeList)
                {
                    EventTypeSelect.Items.Add(t.TypeName);
                }
                EventTypeSelect.SelectedItem = typeList.First().TypeName;

                var typeDetails = typeList.SingleOrDefault(x => x.TypeId == typeId);

                if (typeDetails != null)
                {
                    EventTypeSelect.SelectedItem = typeDetails.TypeName;
                }
                else
                {
                    EventTypeSelect.SelectedItem = typeList.First().TypeName;
                }
            }
            else
            {
                EventTypeSelect.IsEnabled = false;
            }
        }

        public void RenderDurations(int duration)
        {
            DurationList.Items.Clear();

            DurationList.Items.Add(15);
            DurationList.Items.Add(30);
            DurationList.Items.Add(60);

            DurationList.SelectedItem = duration;
        }

        public void CloseNoEventsPopup(object sender, RoutedEventArgs e)
        {
            NoEvents.IsOpen = false;
        }

        public void StatusChangeFailed(object sender, RoutedEventArgs e)
        {
            EventStatus.IsOpen = false;
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
                        DeleteEvent.IsOpen = false;
                        return;
                    }
                    ListedEvents.ItemsSource = refreshResults;
                    DeleteEvent.IsOpen = false;
                    DisableFeatures();
                }
                DeleteEvent.IsOpen = false;
            }
            DeleteEvent.IsOpen = false;
        }

        public void CloseDeleteEventPopup(object sender, RoutedEventArgs e)
        {
            DeleteEvent.IsOpen = false;
        }

        private void AddModule_OnChecked(object sender, RoutedEventArgs e)
        {
            AddModule.IsChecked = true;
            CourseSelect.IsEnabled = true;
            ModuleSelect.IsEnabled = true;
            StudentsLabel.Visibility = Visibility.Visible;
            ModuleStudents.Visibility = Visibility.Visible;
            _moduleEnabled = true;
        }

        private void AddModule_Unchecked(object sender, RoutedEventArgs e)
        {
            AddModule.IsChecked = false;
            CourseSelect.IsEnabled = false;
            ModuleSelect.IsEnabled = false;
            StudentsLabel.Visibility = Visibility.Hidden;
            ModuleStudents.Visibility = Visibility.Hidden;
            _moduleEnabled = false;
        }

        private void AddRoom_OnChecked(object sender, RoutedEventArgs e)
        {
            AddRoom.IsChecked = true;
            RoomSelect.IsEnabled = true;
            BuildingSelect.IsEnabled = true;
            CapacityLabel.Visibility = Visibility.Visible;
            RoomCapacity.Visibility = Visibility.Visible;
            _roomEnabled = true;
        }

        private void AddRoom_Unchecked(object sender, RoutedEventArgs e)
        {
            AddRoom.IsChecked = false;
            RoomSelect.IsEnabled = false;
            BuildingSelect.IsEnabled = false;
            CapacityLabel.Visibility = Visibility.Hidden;
            RoomCapacity.Visibility = Visibility.Hidden;
            _roomEnabled = false;
        }
    }
}