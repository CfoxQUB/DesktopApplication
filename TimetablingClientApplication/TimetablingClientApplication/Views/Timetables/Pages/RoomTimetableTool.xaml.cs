using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

namespace TimetablingClientApplication.Views.Timetables.Pages
{
    /// <summary>
    /// Interaction logic for RoomTimetableTool.xaml
    /// </summary>
    public partial class RoomTimetableTool : Page
    {
        private readonly TimetablingService.TimetablingServiceClient _client = new TimetablingService.TimetablingServiceClient();
        private readonly List<TimetablingService.Building> _buildings = new List<TimetablingService.Building>();
        private readonly List<TimetablingService.Room> _rooms = new List<TimetablingService.Room>();
        private readonly List<TimetablingService.Course> _courses = new List<TimetablingService.Course>();
        private readonly List<TimetablingService.Module> _modules = new List<TimetablingService.Module>();

        private bool _buildingSelected;
        private bool _courseSelected;

        public RoomTimetableTool()
        {
            _buildingSelected = false;
            _courseSelected = false;
            _buildings.AddRange(_client.ReturnBuildings());
            _rooms.AddRange(_client.ReturnBuildingRooms(_buildings.First().BuildingId));
            _courses.AddRange(_client.ReturnCourses());
            _modules.AddRange(_client.ReturnCourseModules(_courses.First().CourseId));
            InitializeComponent();

            foreach (var x in _buildings)
            {
                BuildingSelect.Items.Add(x.BuildingName);
            }

            foreach (var x in _rooms)
            {
                RoomSelect.Items.Add(x.RoomName);
            }
            
            foreach (var x in _courses)
            {
                CourseSelect.Items.Add(x.CourseName);
            }
            
            foreach (var x in _modules)
            {
                ModuleSelect.Items.Add(x.ModuleName);
            }

            BuildingSelect.SelectedItem = _buildings.First().BuildingName;
            CourseSelect.SelectedItem = _courses.First().CourseName;
            ModuleSelect.SelectedItem = _modules.First().ModuleName;
            RoomSelect.SelectedItem = _rooms.First().RoomName;

            
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

        public void Populate_Timetable(List<TimetablingService.Event> eventsList)
        {
            if (eventsList.Any())
            {
                foreach (var e in eventsList)
                {
                    
                   
                } 
            }
            
        }
  }
}
