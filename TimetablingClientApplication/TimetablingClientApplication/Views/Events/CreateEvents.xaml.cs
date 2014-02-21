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
    /// Interaction logic for CreateEvents.xaml
    /// </summary>
    public partial class CreateEvents : Window
    {
        public static TimetablingService.TimetablingServiceClient client = new TimetablingService.TimetablingServiceClient();
        int LoggedInUserId = new int();
        public CreateEvents(int UserId)
        {
            InitializeComponent();
            LoggedInUserId = UserId;
            var EventTypes = client.ReturnEventTypes();

            var TimesList = client.ReturnTimes();

            var RepeatsList = client.ReturnRepeatTypes();

            var BuildingsList = client.ReturnBuildings();

            var CoursesList = client.ReturnCourses();

            var ModulesList = client.ReturnCourseModules(CoursesList.First().CourseId);

            var RoomsList = client.ReturnBuildingRooms(BuildingsList.First().BuildingId);
           
            foreach( var x in EventTypes)
            {
                EventTypeSelect.Items.Add(x.TypeName);
            }

            foreach( var x in TimesList)
            {
                TimeList.Items.Add(x.TimeLiteral);
            }

            foreach (var x in RepeatsList)
            {
                RepeatSelect.Items.Add(x.RepeatTypeName);
            }

            foreach (var x in BuildingsList)
            {
                BuildingSelect.Items.Add(x.BuildingName);
            }

            foreach (var x in CoursesList)
            {
                CourseSelect.Items.Add(x.CourseName);
            }

            foreach (var x in RoomsList)
            {
                RoomSelect.Items.Add(x.RoomName);
            }

            EventTypeSelect.Text = EventTypes.First().TypeName;
            TimeList.Text = TimesList.First().TimeLiteral;
            RepeatSelect.Text = RepeatsList.First().RepeatTypeName;
            BuildingSelect.Text = BuildingsList.First().BuildingName;
            RoomSelect.Text = RoomsList.First().RoomName;
            CourseSelect.Text = CoursesList.First().CourseName;
            ModuleSelect.Text = ModulesList.First().ModuleName;

            this.DurationList.Items.Add(15);
            this.DurationList.Items.Add(30);
            this.DurationList.Items.Add(60);
            DurationList.Text = "15";
            StartDate.SelectedDate = DateTime.Now.Date.AddDays(7);
          
        }
       

        #region Navigation

        private void MenuItem_NewEvent_Click(object sender, RoutedEventArgs e)
        {
            CreateEvents createEvents = new CreateEvents(LoggedInUserId);
            createEvents.Show();
            this.Close();
        }

        private void MenuItem_EditEvent_Click(object sender, RoutedEventArgs e)
        {
            EditEvents editEvents = new EditEvents(LoggedInUserId);
            editEvents.Show();
            this.Close();
        }
     
        #endregion

        #region Event Actions

        public void Create_Event(object sender, RoutedEventArgs e)
        {
           client.CreateEvent(EventTitle.Text, LoggedInUserId, EventDescription.Text, EventTypeSelect.SelectedItem.ToString(), RepeatSelect.SelectedItem.ToString(),Convert.ToInt32(DurationList.SelectedValue), Convert.ToDateTime(StartDate.SelectedDate), TimeList.SelectedValue.ToString(), RoomSelect.SelectedValue.ToString(), CourseSelect.SelectedItem.ToString(), ModuleSelect.SelectedItem.ToString());
        }

        public void Building_Selection_Changed(object sender, RoutedEventArgs e)
        {
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

        #endregion

        
    }
}
