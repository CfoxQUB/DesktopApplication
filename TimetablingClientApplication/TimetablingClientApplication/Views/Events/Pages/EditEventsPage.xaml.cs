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
        //Webservice functionality used to expose functionality 
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        //Default search string
        private const String DefaultSearchString = "Enter Search Details here . . . .";
        //Id used to edit event
        private int _editedEventId;
        private readonly int _loggedInUserId;
        private int _selectedEventId;
        //List used to populate and maintain seletion of event values
        private readonly List<Building> _buildingsList = new List<Building>();
        private readonly List<Room> _roomsList = new List<Room>();
        private readonly List<Course> _courseList = new List<Course>();
        private readonly List<Module> _moduleList = new List<Module>();
        //Bool values used for building and course rendering
        private bool _buildingSelected;
        private bool _courseSelected;
        //Room and module enabled boolean values
        private bool _roomEnabled;
        private bool _moduleEnabled;
        //events list display item
        private readonly ObservableCollection<Event> _eventsList = new ObservableCollection<Event>();

        //Page created
        public EditEventsPage(int userId)
        {
            //User Id maintained for editing of event
            _loggedInUserId = userId;
            //Page created
            InitializeComponent();
            //Events returned from Database
            var events = _client.ReturnEvents();
            if (events != null)
            {   //populatino of events in page
                foreach (var e in events)
                {
                    _eventsList.Add(e);
                }
                ListedEvents.ItemsSource = _eventsList;
            }

            //available courses returned
            var courseList = _client.ReturnCourses();
            if (courseList != null)
            {   //courses seletion list populated
                _courseList.AddRange(courseList);
                RenderCourses(_courseList, 0);
                //modules for selected course returned
                var moduleList = _client.ReturnCourseModules(_courseList.First().CourseId);
                if (moduleList != null)
                {   //Modules list populated
                    _moduleList.AddRange(moduleList);
                    RenderModules(_moduleList, 0);
                }
            }
            //Buildings select returned
            var buildingsList = _client.ReturnBuildings();
            if (buildingsList != null)
            {
                //Building list seltion poulated
                _buildingsList.AddRange(buildingsList);
                RenderBuildings(_buildingsList, 0);
                //rooms return for building selected
                var roomsList = _client.ReturnBuildingRooms(_buildingsList.First().BuildingId);
                if (roomsList != null)
                {   //rooms list populated
                    _roomsList.AddRange(roomsList);
                    RenderRooms(_roomsList, 0);
                }
            }
            //Times, event types and duration rendered
            RenderDurations(15);
            RenderTimes(0);
            RenderEventTypes(0);
            //Search filter options added
            SearchFilter.Items.Add("Event Title");
            SearchFilter.Items.Add("Event Description");
            SearchFilter.SelectedItem = "Event Title";
            //Placeholder text set
            SearchField.Text = DefaultSearchString;
            //Enableing features on page
            EnablingFeatures(false);
            SaveStatus.IsEnabled = false;
            //Status' available populated 
            StatusList.Items.Add("Confirmed");
            StatusList.Items.Add("Denied");
            StatusList.Items.Add("Pending");
            StatusList.Items.Add("New");
            StatusList.Text = "New";
            //Course and buildings boolean set
            _buildingSelected = false;
            _courseSelected = false;
            //room and module addition enabled
            _roomEnabled = true;
            _moduleEnabled = true;
        }

        //Removes placeholder when the search field has focus
        private void SearchField_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchField.Text = "";
        }

        //Replaces placeholder whe search field loses focus
        private void SearchField_LoseFocus(object sender, RoutedEventArgs e)
        {
            SearchField.Text = DefaultSearchString;
        }

        //return events that match the search field which is entered
        private void ReturnSearchResults(object sender, RoutedEventArgs e)
        {
            //check to ensure string entered is valid
            if (SearchField.Text != DefaultSearchString)
            {
                //results of search function populated
                var results = _client.SearchFunction(SearchFilter.SelectedItem.ToString(), SearchField.Text);
                ListedEvents.ItemsSource = results;
                return;
            }
            //Default events returned as not results returned
            if (ListedEvents.ItemsSource == null)
            {
                var noResults = _client.ReturnEvents();
                ListedEvents.ItemsSource = noResults;
            }

        }

        //Poulate edit page fields with selected event details
        public void NewEventSelected(object sender, RoutedEventArgs e)
        {
            if (ListedEvents.SelectedItem == null)
            {
                return;
            }

            _buildingSelected = false;
            _courseSelected = false;

            EnablingFeatures(true);
            SaveStatus.IsEnabled = true;
            //clear current values in page
            _courseList.Clear();
            _moduleList.Clear();
            _buildingsList.Clear();
            _roomsList.Clear();
            //return details of event that is selected
            var selectedEvent = (Event) ListedEvents.SelectedItem;
            if (selectedEvent != null)
            {
                //Populate basic information of event
                _selectedEventId = selectedEvent.EventId;
                EventTitle.Text = selectedEvent.EventTitle;
                EventDescription.Text = selectedEvent.EventDescription;
                StartDate.SelectedDate = selectedEvent.StartDate;
                StatusList.SelectedItem = selectedEvent.Status;
                //Id of event maintained for submission of changes\
                _editedEventId = selectedEvent.EventId;
                //Course list refreshed to match events course
                var courseList = _client.ReturnCourses();
                if (courseList != null)
                {
                    //refresh course select
                    _courseList.Clear();
                    _courseList.AddRange(courseList);
                    RenderCourses(_courseList, selectedEvent.Course);
                    if (selectedEvent.Course != 0)
                    {   // modules of seleted course returned
                        var modules = _client.ReturnCourseModules(selectedEvent.Course);
                        if (modules != null)
                        {
                            _moduleList.AddRange(modules);
                        }
                        RenderModules(_moduleList, selectedEvent.Module);
                    }
                    else
                    {  //No modules available
                        ModuleSelect.IsEnabled = false;
                    }
                }
                else
                {   //No courses, course and module select disabled
                    AddModule.IsChecked = false;
                    AddModule.IsEnabled = false;
                    CourseSelect.IsEnabled = false;
                    ModuleSelect.IsEnabled = false;
                }
                //Building information returned
                var buildingsList = _client.ReturnBuildings();
                var roomBuilding = 0;
                
                if (buildingsList != null)
                {   //Building information and select list refreshed
                    _buildingsList.Clear();
                    _buildingsList.AddRange(buildingsList);
                    //Buildings list refreshed
                    RenderBuildings(_buildingsList, roomBuilding);
                    if (selectedEvent.Room != 0)
                    {
                        //building rooms refreshed to reflect current selction
                        roomBuilding = _client.ReturnRoomDetail(selectedEvent.Room).Building;
                        var rooms = _client.ReturnBuildingRooms(roomBuilding);

                        if (rooms != null)
                        {   //rooms List refreshed
                            _roomsList.Clear();
                            _roomsList.AddRange(rooms);
                            RenderRooms(_roomsList, selectedEvent.Room);
                        }
                    }
                    else
                    {
                        //Room select disabled due to no rooms
                        RoomSelect.IsEnabled = false;
                    }
                }
                else
                {
                    //Room and building select disabled due to no rooms or buildings available
                    AddRoom.IsChecked = false;
                    AddRoom.IsEnabled = false;
                    BuildingSelect.IsEnabled = false;
                    RoomSelect.IsEnabled = false;
                }

                //times, event tyoes and durations refreshed
                RenderTimes(selectedEvent.Time);
                RenderEventTypes(selectedEvent.EventType);
                RenderDurations(selectedEvent.Duration);

                _buildingSelected = true;
                _courseSelected = true;
            }
        }

        //Selction of builing changed
        public void Building_Selection_Changed(object sender, RoutedEventArgs e)
        {
            if (_buildingSelected == false)
            {
                _buildingSelected = true;
                return;
            }
            //building details returned
            var buildingId = _client.ReturnBuildingIdFromBuildingName(BuildingSelect.SelectedItem.ToString());
            if (buildingId != 0)
            {
                //rooms of building refreshed to reflect current selection
                var roomList = _client.ReturnBuildingRooms(buildingId);
                if (roomList != null)
                {
                    //Rooms list cleared and repopulated
                    RoomSelect.Items.Clear();
                    RoomSelect.IsEnabled = true;
                    foreach (var x in roomList)
                    {
                        RoomSelect.Items.Add(x.RoomName);
                    }
                    //event room selected and content updated
                    RoomSelect.Text = roomList.First().RoomName;
                    RoomCapacity.Content = roomList.First().Capacity.ToString("D");
                }
                else
                {
                    //room select disabled
                    RoomSelect.IsEnabled = false;
                }
            }
            else
            {
                //Room and building select disabled
                AddRoom.IsChecked = false;
                AddRoom.IsEnabled = false;
                BuildingSelect.IsEnabled = false;
                RoomSelect.IsEnabled = false;
            }
        }

        //Course selection changed
        public void Course_Selection_Changed(object sender, RoutedEventArgs e)
        {
            if (_courseSelected == false)
            {
                _courseSelected = true;
                return;
            }
            //Course information returned
            var courseId = _client.ReturnCourseIdFromCourseName(CourseSelect.SelectedItem.ToString());

            if (courseId != 0)
            {
                //Modules list of course selected returned and repopulated
                var moduleList = _client.ReturnCourseModules(courseId);
                if (moduleList != null)
                {
                    //Modules repopulated
                    ModuleSelect.Items.Clear();
                    ModuleSelect.IsEnabled = true;
                    foreach (var x in moduleList)
                    {
                        ModuleSelect.Items.Add(x.ModuleName);
                    }
                    //Module information associated with event selected
                    ModuleSelect.Text = moduleList.First().ModuleName;
                    var students = _client.ReturnModuleStudentsNumbers(moduleList.First().ModuleId);
                    ModuleStudents.Content = students.ToString("D");
                }
                else
                {
                    //No modules exist so select is disabled
                    ModuleSelect.IsEnabled = false;
                }
            }
            else
            {
                //No courses or modules both disabled
                CourseSelect.IsEnabled = false;
                ModuleSelect.IsEnabled = false;
            }
        }

        public void Module_Selection_Changed(object sender, RoutedEventArgs e)
        {
            if (_moduleEnabled)
            {
                //Updates the selection of module that has been made
                var moduleName = ModuleSelect.SelectedValue.ToString();
                var moduleId = _client.ReturnModuleIdFromModuleName(moduleName);
                var moduleStudents = _client.ReturnModuleStudentsNumbers(moduleId);
                ModuleStudents.Content = moduleStudents.ToString("D");
            }
        }

        private void ListedEvents_DeleteEvent(object sender, MouseButtonEventArgs e)
        {
            //Confirm delete event is opened
            DeleteEvent.IsOpen = true;
        }

        //changes made to event submitted
        public void Save_Event_Changes(object sender, RoutedEventArgs e)
        {
            //default course, module and room values
            var course = "0";
            var module = "0";
            var room = "0";
            //check to see if room is enabled
            if (RoomSelect.IsEnabled)
            {
                //room selected
                room = RoomSelect.SelectedValue.ToString();
            }
            //Check to see if module is enabled
            if (ModuleSelect.IsEnabled)
            {   //Course and module information selected
                course = CourseSelect.SelectedItem.ToString();
                module = ModuleSelect.SelectedItem.ToString();
            }

            //edited event submitted to service
            if (_client.EditEvent(_editedEventId, _loggedInUserId, EventTitle.Text, EventDescription.Text,
                EventTypeSelect.SelectedItem.ToString(), Convert.ToInt32(DurationList.SelectedValue),
                Convert.ToDateTime(StartDate.SelectedDate),
                TimeList.SelectedValue.ToString(), room,
                course, module))
            {
                //refresh of page contents to relfect edited event
                var refreshResults = _client.ReturnEvents();
                ListedEvents.ItemsSource = refreshResults;
                ListedEvents.SelectedItem = refreshResults.SingleOrDefault(x => x.EventId == _editedEventId);
            }
           
        }

        //Save change made to event status
        public void Save_Event_Status(object sender, RoutedEventArgs e)
        {
            if (_selectedEventId != 0)
            {
                //status selected and submitted to service
                var status = StatusList.Text;
                var result = _client.ChangeEventStatus(status, _selectedEventId);
                //result of change event status
                switch (result)
                {
                    case "success":
                        EventStatus.IsOpen = true;
                        break;
                    case "module":
                        ConfirmedEventExists.IsOpen = true;
                        FailedText1.Text = "An event is already Confirmed with the same";
                        FailedText2.Text = "Module exists";
                        break;
                    case "course":
                        ConfirmedEventExists.IsOpen = true;
                        FailedText1.Text = "An event is already Confirmed with the same";
                        FailedText2.Text = "course for this time";
                        break;
                    case "room":
                        ConfirmedEventExists.IsOpen = true;
                        FailedText1.Text = "An event is already Confirmed with the same";
                        FailedText2.Text = "room at this time exists";
                        break;
                    case "both":
                        ConfirmedEventExists.IsOpen = true;
                        FailedText1.Text = "An event is already Confirmed with the same";
                        FailedText2.Text = "Time and Module exists";
                        break;
                    case "failed":
                        ConfirmedEventExists.IsOpen = true;
                        FailedText1.Text = "Event status change failed for unknown reasons";
                        FailedText2.Text = "its is possible event no longer exists.";
                        break;
                }
                //Update events list
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
        }
        //render course selection in page
        public void RenderCourses(List<Course> courseList, int courseId)
        {
            if (courseList.Count > 0)
            {
                //course selction enabled and courses populated
                CourseSelect.IsEnabled = true;
                CourseSelect.Items.Clear();
                foreach (var c in courseList)
                {
                    CourseSelect.Items.Add(c.CourseName);
                }
                //Course details returned and updated in page
                var courseDetails = courseList.SingleOrDefault(x => x.CourseId == courseId);
                if (courseDetails != null)
                {
                    CourseSelect.SelectedItem = courseDetails.CourseName;
                    AddModule.IsChecked = true;
                }
                else
                {
                    //Defaults course selection if no course already selected
                    CourseSelect.SelectedItem = courseList.First().CourseName;
                    AddModule.IsChecked = false;
                    CourseSelect.IsEnabled = false;
                    _moduleEnabled = false;
                }
            }
            else
            {
                //No courses  feature disabled
                CourseSelect.IsEnabled = false;
            }
        }

        //render Module selection in page
        public void RenderModules(List<Module> courseModules, int moduleId)
        {
            if (courseModules.Count > 0)
            {
                //Ad module feature enabled
                ModuleSelect.IsEnabled = true;
                ModuleSelect.Items.Clear();
                //Modules select populated
                foreach (var c in courseModules)
                {
                    ModuleSelect.Items.Add(c.ModuleName);
                }
                //Selected module details selected
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
                //no modules, feature disabled
                ModuleSelect.IsEnabled = false;
            }
        }

        //Render buildings select feature
        public void RenderBuildings(List<Building> buildingsList, int buildingId)
        {
            if (buildingsList.Count > 0)
            {
                //Building select enabled 
                BuildingSelect.IsEnabled = true;
                BuildingSelect.Items.Clear();
                foreach (var b in buildingsList)
                {
                    //buildings list populated
                    BuildingSelect.Items.Add(b.BuildingName);
                }

                //Building details updated
                var buildingDetails = buildingsList.SingleOrDefault(x => x.BuildingId == buildingId);
                if (buildingDetails != null)
                {
                    //Currently selected building set
                    BuildingSelect.SelectedItem = buildingDetails.BuildingName;
                    AddRoom.IsChecked = true;
                }
                else
                {
                    //building default values set if no building selected
                    BuildingSelect.SelectedItem = buildingsList.First().BuildingName;
                    AddRoom.IsChecked = false;
                    BuildingSelect.IsEnabled = false;
                    _roomEnabled = false;
                }
            }
            else
            {
                //No buildings , feature disabled
                BuildingSelect.IsEnabled = false;
            }
        }

        //Render rooms of building selected in the building selector
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

        //render times available
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

        //render event types available
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

        //render durations availabel
        public void RenderDurations(int duration)
        {
            DurationList.Items.Clear();
            DurationList.Items.Add(15);
            DurationList.Items.Add(30);
            DurationList.Items.Add(60);
            DurationList.SelectedItem = duration;
        }

        //close No events popup function
        public void CloseNoEventsPopup(object sender, RoutedEventArgs e)
        {
            NoEvents.IsOpen = false;
        }

        //clsoe event status change fail popup
        public void StatusChangeFailed(object sender, RoutedEventArgs e)
        {
            EventStatus.IsOpen = false;
        }
        
        //close conflict popup
        public void EventAlreadyConfirmedClose(object sender, RoutedEventArgs e)
        {
            ConfirmedEventExists.IsOpen = false;
        }

        //Deletion of Selected Event
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
                    EnablingFeatures(false);
                }
                DeleteEvent.IsOpen = false;
            }
            DeleteEvent.IsOpen = false;
        }

        //Close delete event popup
        public void CloseDeleteEventPopup(object sender, RoutedEventArgs e)
        {
            DeleteEvent.IsOpen = false;
        }

        //Enabling of module selcetion
        private void AddModule_OnChecked(object sender, RoutedEventArgs e)
        {
            AddModule.IsChecked = true;
            CourseSelect.IsEnabled = true;
            ModuleSelect.IsEnabled = true;
            StudentsLabel.Visibility = Visibility.Visible;
            ModuleStudents.Visibility = Visibility.Visible;
            _moduleEnabled = true;
        }

        //disabling of module selection
        private void AddModule_Unchecked(object sender, RoutedEventArgs e)
        {
            AddModule.IsChecked = false;
            CourseSelect.IsEnabled = false;
            ModuleSelect.IsEnabled = false;
            StudentsLabel.Visibility = Visibility.Hidden;
            ModuleStudents.Visibility = Visibility.Hidden;
            _moduleEnabled = false;
        }

        //enabling of room selection
        private void AddRoom_OnChecked(object sender, RoutedEventArgs e)
        {
            AddRoom.IsChecked = true;
            RoomSelect.IsEnabled = true;
            BuildingSelect.IsEnabled = true;
            CapacityLabel.Visibility = Visibility.Visible;
            RoomCapacity.Visibility = Visibility.Visible;
            _roomEnabled = true;
        }

        //disabling of room selection
        private void AddRoom_Unchecked(object sender, RoutedEventArgs e)
        {
            AddRoom.IsChecked = false;
            RoomSelect.IsEnabled = false;
            BuildingSelect.IsEnabled = false;
            CapacityLabel.Visibility = Visibility.Hidden;
            RoomCapacity.Visibility = Visibility.Hidden;
            _roomEnabled = false;
        }

        //enable of all peage featured or disable
        public void EnablingFeatures(bool check)
        {
            EventTitle.IsEnabled = check;
            EventDescription.IsEnabled = check;
            StartDate.IsEnabled = check;
            TimeList.IsEnabled = check;
            DurationList.IsEnabled = check;
            EventTypeSelect.IsEnabled = check;
            CourseSelect.IsEnabled = check;
            ModuleSelect.IsEnabled = check;
            BuildingSelect.IsEnabled = check;
            RoomSelect.IsEnabled = check;
            AddRoom.IsEnabled = check;
            AddModule.IsEnabled = check;
            RoomSelect.IsEnabled = check;
            StatusList.IsEnabled = check;
            SubmitChangesButton.IsEnabled = check;
        }
    }
}