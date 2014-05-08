using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using TimetablingClientApplication.TimetablingService;
using TimetablingClientApplication.Views.Events.Pages;

namespace TimetablingClientApplication.Views.Events.Windows
{
    /// <summary>
    /// Interaction logic for CreateEvents.xaml
    /// </summary>
    public partial class CreateEvents
    {
        //Webservice functionality exposed through service reference
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        //Ids maintained for event creation
        private readonly int _userId;
        private int _createdEventId;
        private int _buildingId;
        //Valadation colour to indicate alert
        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        //Boolean value which indicates addition of room or module to event
        private bool _addRoomEnabled;
        private bool _addModuleEnabled;
        //Boolean values which indicate course or building render success
        private bool _courseRendered;
        private bool _buildingRendered;
        //Navigation service passed in from the main window frame
        private readonly NavigationService _navigationService;

        //Event creation window implemented
        public CreateEvents(int userId, NavigationService navigation)
        {   //Page rendered
            InitializeComponent();
            //User Id and navigation maintained
            _userId = userId;
            _navigationService = navigation;
            //Event types returned for event type selection
            var eventTypes = _client.ReturnEventTypes();
            if (eventTypes.Any())
            {   //event types select populated
                foreach (var x in eventTypes)
                {
                    EventTypeSelect.Items.Add(x.TypeName);
                }
                //Event type selected
                EventTypeSelect.Text = eventTypes.First().TypeName;
            }
            //Available times populated into time select
            var timesList = _client.ReturnTimes();
            if (timesList.Any())
            {   //Times select populated
                foreach (var x in timesList)
                {
                    TimeList.Items.Add(x.TimeLiteral);
                }
                //first time selected value
                TimeList.Text = timesList.First().TimeLiteral;
            }
            //repeat types returned
            var repeatsList = _client.ReturnRepeatTypes();
            if (repeatsList.Any())
            {
                //repeat types poulated
                foreach (var x in repeatsList)
                {
                    RepeatSelect.Items.Add(x.RepeatTypeName);
                }
                //first repeat type selected
                RepeatSelect.Text = repeatsList.First().RepeatTypeName;
            }
            //Buildings available returned
            var buildingsList = _client.ReturnBuildings();
            if (buildingsList != null)
            {
                //Buildings select populated
                foreach (var x in buildingsList)
                {
                    BuildingSelect.Items.Add(x.BuildingName);
                }
                //Buildings select item selected
                BuildingSelect.Text = buildingsList.First().BuildingName;
                _buildingId = buildingsList.First().BuildingId;
                //Building rooms populated
                var roomsList = _client.ReturnBuildingRooms(buildingsList.First().BuildingId);
                if (roomsList != null)
                {
                    //rooms list populated
                    foreach (var x in roomsList)
                    {
                        RoomSelect.Items.Add(x.RoomName);
                    }
                    //Room selected
                    RoomSelect.Text = roomsList.First().RoomName;
                    _addRoomEnabled = true;
                }
                else
                {
                    //No rooms in building room select disabled
                    AddRoom.IsChecked = false;
                    AddRoom.IsEnabled = false;
                }
                _buildingRendered = true;
            }
            else
            {
                //No buildings availabel room select disabled
                AddRoom.IsChecked = false;
                AddRoom.IsEnabled = false;
            }

            //Courses available returned
            var coursesList = _client.ReturnCourses();
            if (coursesList != null)
            {
                //Course select populated
                foreach (var x in coursesList)
                {
                    CourseSelect.Items.Add(x.CourseName);
                }
                //Course selected
                CourseSelect.Text = coursesList.First().CourseName;
                //Modules returned for the selcted module
                var modulesList = _client.ReturnCourseModules(coursesList.First().CourseId);
                if (modulesList != null)
                {
                    //Selected module selected
                    ModuleSelect.Text = modulesList.First().ModuleName;
                    _addModuleEnabled = true;
                    foreach (var m in modulesList)
                    {   //Module select items populatedd
                        ModuleSelect.Items.Add(m.ModuleName);
                    }
                }
                else
                {
                    //no modules available for course
                    AddModule.IsChecked = false;
                    AddModule.IsEnabled = false;
                }
                _courseRendered = true;
            }
            else
            {
                //No courses available
                //Course and module select disabled
                AddModule.IsChecked = false;
                AddModule.IsEnabled = false;
            }

            //Duration list populated
            DurationList.Items.Add(15);
            DurationList.Items.Add(30);
            DurationList.Items.Add(60);
            DurationList.Text = "15";
            //Date selected 1 week from todays date
            StartDate.SelectedDate = DateTime.Now.Date.AddDays(7);
        }


        #region Event Actions

        //Event creation method
        public void Create_Event(object sender, RoutedEventArgs e)
        {
            //Check values of title and description not null
            if (CheckTitleAndDescription())
            {
                //Default values for event creation
                var room = "0";
                var course = "0";
                var module = "0";
                var students = 0;
                var capacity = 0;

                //add room is enabled
                if (_addRoomEnabled && RoomSelect.IsEnabled)
                {
                    //room select enabled so room selectd
                    room = RoomSelect.Text;
                    //capacity saved 
                    capacity = Convert.ToInt32(RoomCapacity.Content.ToString());
                }

                //Add module is enabled
                if (_addModuleEnabled && ModuleSelect.IsEnabled)
                {   //Course and module details selected
                    course = CourseSelect.Text;
                    module = ModuleSelect.Text;
                    students = Convert.ToInt32(ModuleStudents.Content.ToString());
                }

                //If module and room both selected check module attendees do not exceed the room capacity
                if (_addModuleEnabled && _addRoomEnabled && RoomSelect.IsEnabled && ModuleSelect.IsEnabled)
                {
                    if (students <= capacity)
                    {
                        //event details passed to create event methods of web service
                        _createdEventId = _client.CreateEvent(EventTitle.Text, _userId, EventDescription.Text,
                            EventTypeSelect.SelectedItem.ToString(), RepeatSelect.SelectedItem.ToString(),
                            Convert.ToInt32(DurationList.SelectedValue), Convert.ToDateTime(StartDate.SelectedDate),
                            TimeList.SelectedValue.ToString(), room,
                            course, module);
                    }
                    else
                    {
                        //Capacity is exceeded
                        CapacityAttendee.IsOpen = true;
                    }
                }
                else
                {
                    //If module or room enabled event created with one or the other
                    _createdEventId = _client.CreateEvent(EventTitle.Text, _userId, EventDescription.Text,
                            EventTypeSelect.SelectedItem.ToString(), RepeatSelect.SelectedItem.ToString(),
                            Convert.ToInt32(DurationList.SelectedValue), Convert.ToDateTime(StartDate.SelectedDate),
                            TimeList.SelectedValue.ToString(), room,
                            course, module);
                }

                if (_createdEventId != 0)
                {
                    //successful event creation
                    Success.IsOpen = true;
                }
                else
                {
                    //event creation failed
                    ValidationMessage.Text = "Event Creation Failed. Maximum Met.";
                    ValidationMessage.Visibility = Visibility.Visible;

                }
            }
        }

        //Building Selection Changed
        public void Building_Selection_Changed(object sender, RoutedEventArgs e)
        {   //Check to ensure page rendered
            _buildingRendered = false;
            //Building Id returned of selected building from selected list
            _buildingId = _client.ReturnBuildingIdFromBuildingName(BuildingSelect.SelectedItem.ToString());
            if (_buildingId != 0)
            {
                //rooms list returned from building selected
                var roomList = _client.ReturnBuildingRooms(_buildingId);
                //Previous buildings rooms removed
                RoomSelect.Items.Clear();
                if (roomList != null)
                {   //New rooms added to room select list
                    foreach (var x in roomList)
                    {
                        RoomSelect.Items.Add(x.RoomName);
                    }
                    //Room selected
                    RoomSelect.Text = roomList.First().RoomName;
                    //Capacity of Room updated
                    RoomCapacity.Content = roomList.First().Capacity.ToString("D");
                    RoomSelect.IsEnabled = true;
                }
                else
                {
                    //No rooms available
                    RoomCapacity.Content = "N/A";
                    RoomSelect.IsEnabled = false;
                }
                _buildingRendered = true;
            }
        }

        //room selection changed
        public void Room_Selection_Changed(object sender, RoutedEventArgs e)
        {
            //Building select list must be rendered before rooms updated
            if (_buildingRendered)
            {   //Room selected from list
                var tempName = RoomSelect.SelectedItem;
                var roomId = _client.ReturnRoomId(_buildingId, tempName.ToString());
                //room details returned
                var roomDetails = _client.ReturnRoomDetail(roomId);
                if (roomDetails != null)
                {   //room capacity updated
                    RoomCapacity.Content = roomDetails.Capacity;
                }
            }
        }

        //Course selction changed  updateds courses and modules selection
        public void Course_Selection_Changed(object sender, RoutedEventArgs e)
        {
            _courseRendered = false;
            //Course selected details selected and returned
            var tempName = CourseSelect.SelectedItem;
            var courseId = _client.ReturnCourseIdFromCourseName(tempName.ToString());
            //Module list of seleted course updated
            var moduleList = _client.ReturnCourseModules(courseId);
            //Previouse modules removed
            ModuleSelect.Items.Clear();
            if (moduleList != null)
            {
                //Course modules updated
                foreach (var x in moduleList)
                {
                    ModuleSelect.Items.Add(x.ModuleName);
                }
                //Selected Item initialised
                ModuleSelect.Text = moduleList.First().ModuleName;
                //Module students numbers returned for comparison to capacity
                var moduleStudents = _client.ReturnModuleStudentsNumbers(moduleList.First().ModuleId);
                ModuleStudents.Content = moduleStudents.ToString("D");
                ModuleSelect.IsEnabled = true;
            }
            else
            {
                //No modules available
                ModuleStudents.Content = "N/A";
                ModuleSelect.IsEnabled = false;
            }

            _courseRendered = true;
        }

        //Module selection changed
        public void Module_Selection_Changed(object sender, RoutedEventArgs e)
        {   //Course must be rendered before modules updated
            if (_courseRendered)
            {
                //selected module details maintined and returned
                var moduleName = ModuleSelect.SelectedValue.ToString();
                var moduleId = _client.ReturnModuleIdFromModuleName(moduleName);
                //Module students numbers returned and updated
                var moduleStudents = _client.ReturnModuleStudentsNumbers(moduleId);
                ModuleStudents.Content = moduleStudents.ToString("D");
            }
        }

        #endregion

        //Check to ensure event Title and Description not null
        public bool CheckTitleAndDescription()
        {
            //Validation Messagerest
            ValidationMessage.Visibility = Visibility.Hidden;

            if (String.IsNullOrEmpty(EventTitle.Text) || String.IsNullOrEmpty(EventDescription.Text))
            {
                //Validation on fields
                Title.Foreground = _alert;
                EventTitle.BorderBrush = _alert;
                Description.Foreground = _alert;
                EventDescription.BorderBrush = _alert;
                ValidationMessage.Text = "Title and Description must be completed.";
                ValidationMessage.Visibility = Visibility.Visible;
                return false;
            }

            return true;
        }

        //Close capacity popup which indicates Capacity exceeded
        public void CapacityAttendeeClicked(object sender, RoutedEventArgs e)
        {
            //close popup
            CapacityAttendee.IsOpen = false;
        }

        //Event success popup redraws the page
        public void CloseEventSuccessPopup(object sender, RoutedEventArgs e)
        {
            _navigationService.Navigate(new CreateEventsPage(_userId, _navigationService));
        }

        //Module addition enabled
        private void AddModule_OnChecked(object sender, RoutedEventArgs e)
        {
            //checks and enables the course selection and module selection
            AddModule.IsChecked = true;
            CourseSelect.IsEnabled = true;
            ModuleSelect.IsEnabled = true;
            _addModuleEnabled = true;
            //Shows label for student and module information
            StudentsLabel.Visibility = Visibility.Visible;
            ModuleStudents.Visibility = Visibility.Visible;
        }

        //Module addition disabled
        private void AddModule_Unchecked(object sender, RoutedEventArgs e)
        {
            //checks and disables the course selection and module selection
            AddModule.IsChecked = false;
            CourseSelect.IsEnabled = false;
            ModuleSelect.IsEnabled = false;
            _addModuleEnabled = false;
            //hides label for student and module information
            StudentsLabel.Visibility = Visibility.Hidden;
            ModuleStudents.Visibility = Visibility.Hidden;
        }

        //Room addition enabled
        private void AddRoom_OnChecked(object sender, RoutedEventArgs e)
        {
            //checks and enables the room selection 
            AddRoom.IsChecked = true;
            RoomSelect.IsEnabled = true;
            BuildingSelect.IsEnabled = true;
            _addRoomEnabled = true;
            //Shows label for room and capacity information
            CapacityLabel.Visibility = Visibility.Visible;
            RoomCapacity.Visibility = Visibility.Visible;
        }

        //Room addition disabled
        private void AddRoom_Unchecked(object sender, RoutedEventArgs e)
        {
            //checks and disables the room selection 
            AddRoom.IsChecked = false;
            RoomSelect.IsEnabled = false;
            BuildingSelect.IsEnabled = false;
            _addRoomEnabled = false;
            //Hides label for room and capacity information
            CapacityLabel.Visibility = Visibility.Hidden;
            RoomCapacity.Visibility = Visibility.Hidden;
        }
    }


}