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
using TimetablingClientApplication.Views.MasterViews;

namespace TimetablingClientApplication.Views.Events.Pages
{
    /// <summary>
    /// Interaction logic for CreateEventsPage.xaml
    /// </summary>
    public partial class CreateEventsPage : Page
    {
        public static TimetablingService.TimetablingServiceClient Client = new TimetablingService.TimetablingServiceClient();

        private readonly int _loggedInUserId;
        private int _createdEventId;
        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush _normal = new SolidColorBrush(Colors.Black);

        private readonly bool _addRoomEnabled = false;
        private readonly bool _addModuleEnabled = false;
        
        
        private readonly String _building;
        private readonly String _course;
        private readonly String _time;
        private readonly String _type;
        private readonly String _duration;
        private readonly String _repeat;

        public CreateEventsPage(int userId)
        {
            InitializeComponent();
            _loggedInUserId = userId;
            var eventTypes = Client.ReturnEventTypes();

            var timesList = Client.ReturnTimes();

            var repeatsList = Client.ReturnRepeatTypes();

            var buildingsList = Client.ReturnBuildings();

            var coursesList = Client.ReturnCourses();

            var modulesList = Client.ReturnCourseModules(coursesList.First().CourseId);

            var roomsList = Client.ReturnBuildingRooms(buildingsList.First().BuildingId);

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
            _type = EventTypeSelect.Text;

            TimeList.Text = timesList.First().TimeLiteral;
            _time = TimeList.Text;
            RepeatSelect.Text = repeatsList.First().RepeatTypeName;
            _repeat = RepeatSelect.Text;
            BuildingSelect.Text = buildingsList.First().BuildingName;
            _building = BuildingSelect.Text;
            RoomSelect.Text = roomsList.First().RoomName;
            CourseSelect.Text = coursesList.First().CourseName;
            _course = CourseSelect.Text;
            ModuleSelect.Text = modulesList.First().ModuleName;
            
            DurationList.Items.Add(15);
            DurationList.Items.Add(30);
            DurationList.Items.Add(60);
            DurationList.Text = "15";
            _duration = "15";

            StartDate.SelectedDate = DateTime.Now.Date.AddDays(7);

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

                
                   _createdEventId = Client.CreateEvent(EventTitle.Text, _loggedInUserId, EventDescription.Text,
                        EventTypeSelect.SelectedItem.ToString(), RepeatSelect.SelectedItem.ToString(),
                        Convert.ToInt32(DurationList.SelectedValue), Convert.ToDateTime(StartDate.SelectedDate),
                        TimeList.SelectedValue.ToString(), room,
                        course, module);
                

                if (_createdEventId != 0)
                {
                    Success.IsOpen = true;
                }
            }
        }

        public void Building_Selection_Changed(object sender, RoutedEventArgs e)
        {
            var tempName = BuildingSelect.SelectedItem;
            var buildingId = Client.ReturnBuildingIdFromBuildingName(tempName.ToString());
            var roomList = Client.ReturnBuildingRooms(buildingId);
            RoomSelect.Items.Clear();

            foreach (var x in roomList)
            {
                RoomSelect.Items.Add(x.RoomName);
            }
            RoomSelect.Text = roomList.First().RoomName;
        }

        public void Course_Selection_Changed(object sender, RoutedEventArgs e)
        {
            var tempName = CourseSelect.SelectedItem;
            var courseId = Client.ReturnCourseIdFromCourseName(tempName.ToString());
            var moduleList = Client.ReturnCourseModules(courseId);
            ModuleSelect.Items.Clear();

            foreach (var x in moduleList)
            {
                ModuleSelect.Items.Add(x.ModuleName);
            }
            ModuleSelect.Text = moduleList.First().ModuleName;

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

        public void InvitesButtonClicked(object sender, RoutedEventArgs e)
        {
            
        }

        public void ResetCreateEventPopup(object sender, RoutedEventArgs e)
        {
            Title.Foreground = _normal;
            EventTitle.BorderBrush = _normal;
            Description.Foreground = _normal;
            EventDescription.BorderBrush = _normal;
            ValidationMessage.Visibility = Visibility.Hidden;

            ReRenderPage();
        }

       
        public void ReRenderPage()
        {

            EventDescription.Text = "";
            EventTitle.Text = "";

            BuildingSelect.Text = _building;
            TimeList.Text = _time;
            RepeatSelect.Text = _repeat;
            CourseSelect.Text = _course;
            
            DurationList.Text = "15";

            Success.IsOpen = false;
        }

        private void AddModule_OnChecked(object sender, RoutedEventArgs e)
        {
            AddModule.IsChecked = true;
            CourseSelect.IsEnabled = true;
            ModuleSelect.IsEnabled = true;
        } 

        private void AddModule_Unchecked(object sender, RoutedEventArgs e)
        {
            AddModule.IsChecked = false;
            CourseSelect.IsEnabled = false;
            ModuleSelect.IsEnabled = false;
        } 

        private void AddRoom_OnChecked(object sender, RoutedEventArgs e)
        {
            AddRoom.IsChecked = true;
            RoomSelect.IsEnabled = true;
            BuildingSelect.IsEnabled = true;
        } 

        private void AddRoom_Unchecked(object sender, RoutedEventArgs e)
        {
            AddRoom.IsChecked = false;
            RoomSelect.IsEnabled = false;
            BuildingSelect.IsEnabled = false;
        }
    }

}
