﻿using System;
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
        public static TimetablingService.TimetablingServiceClient Client = new TimetablingService.TimetablingServiceClient();
        int loggedInUserId;
        public CreateEvents(int userId)
        {
            InitializeComponent();
            loggedInUserId = userId;
            var eventTypes = Client.ReturnEventTypes();

            var timesList = Client.ReturnTimes();

            var repeatsList = Client.ReturnRepeatTypes();

            var buildingsList = Client.ReturnBuildings();

            var coursesList = Client.ReturnCourses();

            var modulesList = Client.ReturnCourseModules(coursesList.First().CourseId);

            var roomsList = Client.ReturnBuildingRooms(buildingsList.First().BuildingId);
           
            foreach( var x in eventTypes)
            {
                EventTypeSelect.Items.Add(x.TypeName);
            }

            foreach( var x in timesList)
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
            RoomSelect.Text = roomsList.First().RoomName;
            CourseSelect.Text = coursesList.First().CourseName;
            ModuleSelect.Text = modulesList.First().ModuleName;

            DurationList.Items.Add(15);
            DurationList.Items.Add(30);
            DurationList.Items.Add(60);
            DurationList.Text = "15";
            StartDate.SelectedDate = DateTime.Now.Date.AddDays(7);
          
        }
       

        #region Navigation

        private void MenuItem_NewEvent_Click(object sender, RoutedEventArgs e)
        {
            CreateEvents createEvents = new CreateEvents(loggedInUserId);
            createEvents.Show();
            this.Close();
        }

        private void MenuItem_EditEvent_Click(object sender, RoutedEventArgs e)
        {
            EditEvents editEvents = new EditEvents(loggedInUserId);
            editEvents.Show();
            this.Close();
        }
     
        #endregion

        #region Event Actions

        public void Create_Event(object sender, RoutedEventArgs e)
        {
           Client.CreateEvent(EventTitle.Text, loggedInUserId, EventDescription.Text, EventTypeSelect.SelectedItem.ToString(), RepeatSelect.SelectedItem.ToString(),Convert.ToInt32(DurationList.SelectedValue), Convert.ToDateTime(StartDate.SelectedDate), TimeList.SelectedValue.ToString(), RoomSelect.SelectedValue.ToString(), CourseSelect.SelectedItem.ToString(), ModuleSelect.SelectedItem.ToString());
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
            return;
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

        
    }
}
