using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using TimetablingClientApplication.TimetablingService;


namespace TimetablingClientApplication.Views.Events.Pages
{
    /// <summary>
    /// Interaction logic for CreateEventsPage.xaml
    /// </summary>
    public partial class CreateEventsPage
    {
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        private readonly int _userId;
        private int _createdEventId;
        private int _buildingId;

        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);

        private bool _addRoomEnabled;
        private bool _addModuleEnabled;

        private bool _courseRendered;
        private bool _buildingRendered;

        private readonly NavigationService _navigationService; 

        public CreateEventsPage(int userId, NavigationService navigation)
        {
            InitializeComponent();
            _userId = userId;
            _navigationService = navigation;
            var eventTypes = _client.ReturnEventTypes();

            var timesList = _client.ReturnTimes();

            var repeatsList = _client.ReturnRepeatTypes();

            var buildingsList = _client.ReturnBuildings();

            var coursesList = _client.ReturnCourses();

            var modulesList = _client.ReturnCourseModules(coursesList.First().CourseId);

            var roomsList = _client.ReturnBuildingRooms(buildingsList.First().BuildingId);

            foreach (var x in eventTypes)
            {
                EventTypeSelect.Items.Add(x.TypeName);
            }

            foreach (var x in timesList)
            {
                TimeList.Items.Add(x.TimeLiteral);
            }

            foreach (var x in repeatsList)
            {
                RepeatSelect.Items.Add(x.RepeatTypeName);
            }

            foreach (var x in buildingsList)
            {
                BuildingSelect.Items.Add(x.BuildingName);
            }

            foreach (var x in coursesList)
            {
                CourseSelect.Items.Add(x.CourseName);
            }

            foreach (var x in roomsList)
            {
                RoomSelect.Items.Add(x.RoomName);
            }

            EventTypeSelect.Text = eventTypes.First().TypeName;
            TimeList.Text = timesList.First().TimeLiteral;
            RepeatSelect.Text = repeatsList.First().RepeatTypeName;
            BuildingSelect.Text = buildingsList.First().BuildingName;
            _buildingId = buildingsList.First().BuildingId;
            RoomSelect.Text = roomsList.First().RoomName;
            CourseSelect.Text = coursesList.First().CourseName;
            ModuleSelect.Text = modulesList.First().ModuleName;
            
            DurationList.Items.Add(15);
            DurationList.Items.Add(30);
            DurationList.Items.Add(60);
            DurationList.Text = "15";

            StartDate.SelectedDate = DateTime.Now.Date.AddDays(7);

            _addRoomEnabled = true;
            _addModuleEnabled = true;

            _courseRendered = true;
        }


        #region Event Actions

        public void Create_Event(object sender, RoutedEventArgs e)
        {
            if (CheckTitleAndDescription())
            {
                var room = "0";

                var course = "0";
                var module = "0";

                if (_addRoomEnabled)
                {
                    room = RoomSelect.Text;
                    
                }
                
                if (_addModuleEnabled)
                {
                    course = CourseSelect.Text;
                    module =  ModuleSelect.Text;
                }

                var students = Convert.ToInt32(ModuleStudents.Content.ToString()); 
                var capacity = Convert.ToInt32(RoomCapacity.Content.ToString());

                if (_addModuleEnabled && _addRoomEnabled )
                {
                    if (students <= capacity)
                    {
                        _createdEventId = _client.CreateEvent(EventTitle.Text, _userId, EventDescription.Text,
                            EventTypeSelect.SelectedItem.ToString(), RepeatSelect.SelectedItem.ToString(),
                            Convert.ToInt32(DurationList.SelectedValue), Convert.ToDateTime(StartDate.SelectedDate),
                            TimeList.SelectedValue.ToString(), room,
                            course, module);
                    }
                    else
                    {
                        CapacityAttendee.IsOpen = true;
                    }
                }
                else
                {
                    _createdEventId = _client.CreateEvent(EventTitle.Text, _userId, EventDescription.Text,
                            EventTypeSelect.SelectedItem.ToString(), RepeatSelect.SelectedItem.ToString(),
                            Convert.ToInt32(DurationList.SelectedValue), Convert.ToDateTime(StartDate.SelectedDate),
                            TimeList.SelectedValue.ToString(), room,
                            course, module);
                }
                
                if (_createdEventId != 0)
                {
                    Success.IsOpen = true;
                }
            }
        }

        public void Building_Selection_Changed(object sender, RoutedEventArgs e)
        {
            _buildingRendered = false;
            var tempName = BuildingSelect.SelectedItem;
            var buildingId = _client.ReturnBuildingIdFromBuildingName(tempName.ToString());
            _buildingId = buildingId;
            var roomList = _client.ReturnBuildingRooms(buildingId);
            RoomSelect.Items.Clear();

            foreach (var x in roomList)
            {
                RoomSelect.Items.Add(x.RoomName);
            }
            RoomSelect.Text = roomList.First().RoomName;
            RoomCapacity.Content = roomList.First().Capacity.ToString("D");
            _buildingRendered = true;
        }
        
        public void Room_Selection_Changed(object sender, RoutedEventArgs e)
        {
            if (_buildingRendered)
            {
                var tempName = RoomSelect.SelectedItem;
                var roomId = _client.ReturnRoomId(_buildingId, tempName.ToString());

                var roomDetails = _client.ReturnRoomDetail(roomId);

                if (roomDetails != null)
                {
                    RoomCapacity.Content = roomDetails.Capacity;
                }
            }
        }

        public void Course_Selection_Changed(object sender, RoutedEventArgs e)
        {
            _courseRendered = false;

            var tempName = CourseSelect.SelectedItem;
            var courseId = _client.ReturnCourseIdFromCourseName(tempName.ToString());
            var moduleList = _client.ReturnCourseModules(courseId);
            ModuleSelect.Items.Clear();

            if (moduleList != null)
            {

                foreach (var x in moduleList)
                {
                    ModuleSelect.Items.Add(x.ModuleName);
                }
                ModuleSelect.Text = moduleList.First().ModuleName;

                var moduleStudents = _client.ReturnModuleStudentsNumbers(moduleList.First().ModuleId);
                ModuleStudents.Content = moduleStudents.ToString("D");
            }
            else
            {
                ModuleSelect.Items.Add("No modules");
                ModuleSelect.Text = "No modules";
            }

            _courseRendered = true;
        }

        public void Module_Selection_Changed(object sender, RoutedEventArgs e)
        {
            if (_courseRendered)
            {
                var moduleName = ModuleSelect.SelectedValue.ToString();
                var moduleId = _client.ReturnModuleIdFromModuleName(moduleName);
                var moduleStudents = _client.ReturnModuleStudentsNumbers(moduleId);
                ModuleStudents.Content = moduleStudents.ToString("D");
            }
        }

        #endregion

        public bool CheckTitleAndDescription()
        {
            ValidationMessage.Visibility = Visibility.Hidden;

            if (String.IsNullOrEmpty(EventTitle.Text) || String.IsNullOrEmpty(EventDescription.Text))
            {
                Title.Foreground = _alert;
                EventTitle.BorderBrush = _alert;
                Description.Foreground = _alert;
                EventDescription.BorderBrush = _alert;
                ValidationMessage.Visibility = Visibility.Visible;
                return false;
            }

            return true;
        }

        public void CapacityAttendeeClicked(object sender, RoutedEventArgs e)
        {
            CapacityAttendee.IsOpen = false;
        } 
        
        public void CloseEventSuccessPopup(object sender, RoutedEventArgs e)
        {
            _navigationService.Navigate(new CreateEventsPage(_userId, _navigationService));
        }

        private void AddModule_OnChecked(object sender, RoutedEventArgs e)
        {
            AddModule.IsChecked = true;
            CourseSelect.IsEnabled = true;
            ModuleSelect.IsEnabled = true;
            _addModuleEnabled = true;
            StudentsLabel.Visibility = Visibility.Visible;
            ModuleStudents.Visibility = Visibility.Visible;
        } 

        private void AddModule_Unchecked(object sender, RoutedEventArgs e)
        {
            AddModule.IsChecked = false;
            CourseSelect.IsEnabled = false;
            ModuleSelect.IsEnabled = false;
            _addModuleEnabled = false;
            StudentsLabel.Visibility = Visibility.Hidden;
            ModuleStudents.Visibility = Visibility.Hidden;
        } 

        private void AddRoom_OnChecked(object sender, RoutedEventArgs e)
        {
            AddRoom.IsChecked = true;
            RoomSelect.IsEnabled = true;
            BuildingSelect.IsEnabled = true;
            _addRoomEnabled = true;
            CapacityLabel.Visibility = Visibility.Visible;
            RoomCapacity.Visibility = Visibility.Visible;
        } 

        private void AddRoom_Unchecked(object sender, RoutedEventArgs e)
        {
            AddRoom.IsChecked = false;
            RoomSelect.IsEnabled = false;
            BuildingSelect.IsEnabled = false;
            _addRoomEnabled = false;
            CapacityLabel.Visibility = Visibility.Hidden;
            RoomCapacity.Visibility = Visibility.Hidden;
        }
    }

}
