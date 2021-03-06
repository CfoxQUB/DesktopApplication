﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TimetablingClientApplication.TimetablingService;
namespace TimetablingClientApplication.Views.Timetables.Pages
{
    /// <summary>
    /// Interaction logic for RoomTimetableTool.xaml
    /// </summary>
    public partial class CourseTimetableTool
    {
        //course Id maintained
        private int _courseId;
        //Webservice functionality exposed through service reference
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        //Courses list used to populate course selector
        private readonly List<Course> _courses = new List<Course>();
        //Timetable event colour
        private readonly SolidColorBrush _occupied = new SolidColorBrush(Colors.GreenYellow);
        private readonly SolidColorBrush _normal = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEAEAEA"));

        public CourseTimetableTool()
        {
            
            InitializeComponent();
            //Course Select Populated    
            var courses = _client.ReturnCourses();
            if (courses != null)
            {
                _courses.AddRange(courses);
                foreach (var x in _courses)
                {
                    CourseSelect.Items.Add(x.CourseName);
                }
                CourseSelect.SelectedItem = _courses.First().CourseName;
                _courseId = _courses.First().CourseId;
            }
            else
            {
                //no courses available timetable display disabled
                CourseSelect.IsEnabled = false;
                DateSelected.IsEnabled = false;
            }
            //todays date selected
            DateSelected.SelectedDate = DateTime.Now;
            //course events details returned
            if ( _courseId != 0)
            {
                var temp = _client.ReturnWeeksEventsForCourses(DateTime.Now, _courseId);
                if (temp != null)
                {
                    //Timetable display populated
                    Populate_Timetable_Display(temp);
                }
            }
        }
        
        //Course selection changed timetable redrawn
        public void Course_Selection_Changed(object sender, RoutedEventArgs e)
        {
            //new course information returned
            var course = _courses.SingleOrDefault(x => x.CourseName == CourseSelect.SelectedItem.ToString());
            if (course != null)
            {
                _courseId = course.CourseId;
            }
            //Date selected
            var dateSelected = DateSelected.SelectedDate.GetValueOrDefault();
            if (_courseId != 0)
            {   //events for course for selected dates week returend
                var temp = _client.ReturnWeeksEventsForCourses(dateSelected, _courseId);
                if (temp != null)
                {
                    //Timetable repopulated
                    Populate_Timetable_Display(temp);
                }
            }
        }
        
        //Date selection changed
        public void Date_Selection_Changed(object sender, RoutedEventArgs e)
        {
            var dateSelected = DateSelected.SelectedDate.GetValueOrDefault();
            if (_courseId != 0)
            {   //Events for selected course with new date returned
                var temp = _client.ReturnWeeksEventsForCourses(dateSelected, _courseId);
                if (temp != null)
                {
                    //Timetable redrawn
                    Populate_Timetable_Display(temp);
                }
            }
        }

        public void Populate_Timetable_Display(TimetableEventsListObject timetableObjectList)
        {
            //Events currently displayed reset to default colours
            ClearTimetableOfEvents();
            //Monday times list used to repopulate all monday events
            #region Monday
            if (timetableObjectList.MondayList.Any())
            {
                foreach (var e in timetableObjectList.MondayList)
                {
                    if (e.Event != null)
                    {
                        switch (e.Time.TimeId)
                        {
                            case 10:
                                Mon10.Content = e.Event.EventTitle;
                                Mon10.Background = _occupied;
                                break;
                            case 11:
                                Mon11.Content = e.Event.EventTitle;
                                Mon11.Background = _occupied;
                                break;
                            case 12:
                                Mon12.Content = e.Event.EventTitle;
                                Mon12.Background = _occupied;
                                break;
                            case 13:
                                Mon13.Content = e.Event.EventTitle;
                                Mon13.Background = _occupied;
                                break;
                            case 14:
                                Mon14.Content = e.Event.EventTitle;
                                Mon14.Background = _occupied;
                                break;
                            case 15:
                                Mon15.Content = e.Event.EventTitle;
                                Mon15.Background = _occupied;
                                break;
                            case 16:
                                Mon16.Content = e.Event.EventTitle;
                                Mon16.Background = _occupied;
                                break;
                            case 17:
                                Mon17.Content = e.Event.EventTitle;
                                Mon17.Background = _occupied;
                                break;
                        }
                    }
                }
            }

            #endregion
            //Tuesday times list used to repopulate all Tuesday events
            #region Tuesday
            if (timetableObjectList.TuesdayList.Any())
            {
                foreach (var e in timetableObjectList.TuesdayList)
                {
                    if (e.Event != null)
                    {
                        switch (e.Time.TimeId)
                        {
                            case 10:
                                Tue10.Content = e.Event.EventTitle;
                                Tue10.Background = _occupied;
                                break;
                            case 11:
                                Tue11.Content = e.Event.EventTitle;
                                Tue11.Background = _occupied;
                                break;
                            case 12:
                                Tue12.Content = e.Event.EventTitle;
                                Tue12.Background = _occupied;
                                break;
                            case 13:
                                Tue13.Content = e.Event.EventTitle;
                                Tue13.Background = _occupied;
                                break;
                            case 14:
                                Tue14.Content = e.Event.EventTitle;
                                Tue14.Background = _occupied;
                                break;
                            case 15:
                                Tue15.Content = e.Event.EventTitle;
                                Tue15.Background = _occupied;
                                break;
                            case 16:
                                Tue16.Content = e.Event.EventTitle;
                                Tue16.Background = _occupied;
                                break;
                            case 17:
                                Tue17.Content = e.Event.EventTitle;
                                Tue17.Background = _occupied;
                                break;
                        }
                    }
                }
            }
            #endregion
            //Wednesday times list used to repopulate all Wednesday events

            #region Wednesday
            if (timetableObjectList.WednesdayList.Any())
            {
                foreach (var e in timetableObjectList.WednesdayList)
                {
                    if (e.Event != null)
                    {
                        switch (e.Time.TimeId)
                        {
                            case 10:
                                Wed10.Content = e.Event.EventTitle;
                                Wed10.Background = _occupied;
                                break;
                            case 11:
                                Wed11.Content = e.Event.EventTitle;
                                Wed11.Background = _occupied;
                                break;
                            case 12:
                                Wed12.Content = e.Event.EventTitle;
                                Wed12.Background = _occupied;
                                break;
                            case 13:
                                Wed13.Content = e.Event.EventTitle;
                                Wed13.Background = _occupied;
                                break;
                            case 14:
                                Wed14.Content = e.Event.EventTitle;
                                Wed14.Background = _occupied;
                                break;
                            case 15:
                                Wed15.Content = e.Event.EventTitle;
                                Wed15.Background = _occupied;
                                break;
                            case 16:
                                Wed16.Content = e.Event.EventTitle;
                                Wed16.Background = _occupied;
                                break;
                            case 17:
                                Wed17.Content = e.Event.EventTitle;
                                Wed17.Background = _occupied;
                                break;
                        }
                    }
                }
            }
            #endregion
            //Thursday times list used to repopulate all Thursday events
            #region Thursday
            if (timetableObjectList.ThursdayList.Any())
            {
                foreach (var e in timetableObjectList.ThursdayList)
                {
                    if (e.Event != null)
                    {
                        switch (e.Time.TimeId)
                        {
                            case 10:
                                Thur10.Content = e.Event.EventTitle;
                                Thur10.Background = _occupied;
                                break;
                            case 11:
                                Thur11.Content = e.Event.EventTitle;
                                Thur11.Background = _occupied;
                                break;
                            case 12:
                                Thur12.Content = e.Event.EventTitle;
                                Thur12.Background = _occupied;
                                break;
                            case 13:
                                Thur13.Content = e.Event.EventTitle;
                                Thur13.Background = _occupied;
                                break;
                            case 14:
                                Thur14.Content = e.Event.EventTitle;
                                Thur14.Background = _occupied;
                                break;
                            case 15:
                                Thur15.Content = e.Event.EventTitle;
                                Thur15.Background = _occupied;
                                break;
                            case 16:
                                Thur16.Content = e.Event.EventTitle;
                                Thur16.Background = _occupied;
                                break;
                            case 17:
                                Thur17.Content = e.Event.EventTitle;
                                Thur17.Background = _occupied;
                                break;
                        }
                    }
                }
            }
            #endregion
            //Friday times list used to repopulate all Friday events
            #region Friday
            if (timetableObjectList.FridayList.Any())
            {
                foreach (var e in timetableObjectList.FridayList)
                {
                    if (e.Event != null)
                    {
                        switch (e.Time.TimeId)
                        {
                            case 10:
                                Fri10.Content = e.Event.EventTitle;
                                Fri10.Background = _occupied;
                                break;
                            case 11:
                                Fri11.Content = e.Event.EventTitle;
                                Fri11.Background = _occupied;
                                break;
                            case 12:
                                Fri12.Content = e.Event.EventTitle;
                                Fri12.Background = _occupied;
                                break;
                            case 13:
                                Fri13.Content = e.Event.EventTitle;
                                Fri13.Background = _occupied;
                                break;
                            case 14:
                                Fri14.Content = e.Event.EventTitle;
                                Fri14.Background = _occupied;
                                break;
                            case 15:
                                Fri15.Content = e.Event.EventTitle;
                                Fri15.Background = _occupied;
                                break;
                            case 16:
                                Fri16.Content = e.Event.EventTitle;
                                Fri16.Background = _occupied;
                                break;
                            case 17:
                                Fri17.Content = e.Event.EventTitle;
                                Fri17.Background = _occupied;
                                break;
                        }
                    }
                }
            }
            #endregion
            //Saturday times list used to repopulate all Saturday events
            #region Saturday
            if (timetableObjectList.SaturdayList.Any())
            {
                foreach (var e in timetableObjectList.SaturdayList)
                {
                    if (e.Event != null)
                    {
                        switch (e.Time.TimeId)
                        {
                            case 10:
                                Sat10.Content = e.Event.EventTitle;
                                Sat10.Background = _occupied;
                                break;
                            case 11:
                                Sat11.Content = e.Event.EventTitle;
                                Sat11.Background = _occupied;
                                break;
                            case 12:
                                Sat12.Content = e.Event.EventTitle;
                                Sat12.Background = _occupied;
                                break;
                            case 13:
                                Sat13.Content = e.Event.EventTitle;
                                Sat13.Background = _occupied;
                                break;
                            case 14:
                                Sat14.Content = e.Event.EventTitle;
                                Sat14.Background = _occupied;
                                break;
                            case 15:
                                Sat15.Content = e.Event.EventTitle;
                                Sat15.Background = _occupied;
                                break;
                            case 16:
                                Sat16.Content = e.Event.EventTitle;
                                Sat16.Background = _occupied;
                                break;
                            case 17:
                                Sat17.Content = e.Event.EventTitle;
                                Sat17.Background = _occupied;
                                break;
                        }
                    }
                }
            }
            #endregion
            //Sunday times list used to repopulate all Sunday events
            #region Sunday
            if (timetableObjectList.SundayList.Any())
            {
                foreach (var e in timetableObjectList.SundayList)
                {
                    if (e.Event != null)
                    {
                        switch (e.Time.TimeId)
                        {
                            case 10:
                                Sun10.Content = e.Event.EventTitle;
                                Sun10.Background = _occupied;
                                break;
                            case 11:
                                Sun11.Content = e.Event.EventTitle;
                                Sun11.Background = _occupied;
                                break;
                            case 12:
                                Sun12.Content = e.Event.EventTitle;
                                Sun12.Background = _occupied;
                                break;
                            case 13:
                                Sun13.Content = e.Event.EventTitle;
                                Sun13.Background = _occupied;
                                break;
                            case 14:
                                Sun14.Content = e.Event.EventTitle;
                                Sun14.Background = _occupied;
                                break;
                            case 15:
                                Sun15.Content = e.Event.EventTitle;
                                Sun15.Background = _occupied;
                                break;
                            case 16:
                                Sun16.Content = e.Event.EventTitle;
                                Sun16.Background = _occupied;
                                break;
                            case 17:
                                Sun17.Content = e.Event.EventTitle;
                                Sun17.Background = _occupied;
                                break;
                        }
                    }
                }
            }
            #endregion
        }

        //Events returned for new selected date
        public void ReturnDateSelectedWeeksEvents(object sender, EventArgs e)
        {
            //Timetable disaply cleared
            ClearTimetableOfEvents();
            //events returned and timetable redrawn
            var dateSelected = DateSelected.DisplayDate;
            if (_courseId != 0)
            {
                var temp = _client.ReturnWeeksEventsForCourses(dateSelected, _courseId);
                if (temp != null)
                {
                    Populate_Timetable_Display(temp);
                }
            }
        }

        //Timetable fields cleared
        public void ClearTimetableOfEvents()
        {
            #region Clear Monday
            Mon10.Content = "";
            Mon11.Content = "";
            Mon12.Content = "";
            Mon13.Content = "";
            Mon14.Content = "";
            Mon15.Content = "";
            Mon16.Content = "";
            Mon17.Content = "";
            
            Mon10.Background = _normal;
            Mon11.Background = _normal;
            Mon12.Background = _normal;
            Mon13.Background = _normal;
            Mon14.Background = _normal;
            Mon15.Background = _normal;
            Mon16.Background = _normal;
            Mon17.Background = _normal;
            #endregion

            #region Clear Tuesday
            Tue10.Content = "";
            Tue11.Content = "";
            Tue12.Content = "";
            Tue13.Content = "";
            Tue14.Content = "";
            Tue15.Content = "";
            Tue16.Content = "";
            Tue17.Content = "";

            Tue10.Background = _normal;
            Tue11.Background = _normal;
            Tue12.Background = _normal;
            Tue13.Background = _normal;
            Tue14.Background = _normal;
            Tue15.Background = _normal;
            Tue16.Background = _normal;
            Tue17.Background = _normal;
            #endregion

            #region Clear Wednesday
            Wed10.Content = "";
            Wed11.Content = "";
            Wed12.Content = "";
            Wed13.Content = "";
            Wed14.Content = "";
            Wed15.Content = "";
            Wed16.Content = "";
            Wed17.Content = "";

            Wed10.Background = _normal;
            Wed11.Background = _normal;
            Wed12.Background = _normal;
            Wed13.Background = _normal;
            Wed14.Background = _normal;
            Wed15.Background = _normal;
            Wed16.Background = _normal;
            Wed17.Background = _normal;
            #endregion

            #region Clear Thursday
            Thur10.Content = "";
            Thur11.Content = "";
            Thur12.Content = "";
            Thur13.Content = "";
            Thur14.Content = "";
            Thur15.Content = "";
            Thur16.Content = "";
            Thur17.Content = "";

            Thur10.Background = _normal;
            Thur11.Background = _normal;
            Thur12.Background = _normal;
            Thur13.Background = _normal;
            Thur14.Background = _normal;
            Thur15.Background = _normal;
            Thur16.Background = _normal;
            Thur17.Background = _normal;
            #endregion
            
            #region Clear Friday
            Fri10.Content = "";
            Fri11.Content = "";
            Fri12.Content = "";
            Fri13.Content = "";
            Fri14.Content = "";
            Fri15.Content = "";
            Fri16.Content = "";
            Fri17.Content = "";

            Fri10.Background = _normal;
            Fri11.Background = _normal;
            Fri12.Background = _normal;
            Fri13.Background = _normal;
            Fri14.Background = _normal;
            Fri15.Background = _normal;
            Fri16.Background = _normal;
            Fri17.Background = _normal;
            #endregion
            
            #region Clear Saturday
            Sat10.Content = "";
            Sat11.Content = "";
            Sat12.Content = "";
            Sat13.Content = "";
            Sat14.Content = "";
            Sat15.Content = "";
            Sat16.Content = "";
            Sat17.Content = "";
            
            Sat10.Background = _normal;
            Sat11.Background = _normal;
            Sat12.Background = _normal;
            Sat13.Background = _normal;
            Sat14.Background = _normal;
            Sat15.Background = _normal;
            Sat16.Background = _normal;
            Sat17.Background = _normal;
            #endregion
            
            #region Clear Sunday
            Sun10.Content = "";
            Sun11.Content = "";
            Sun12.Content = "";
            Sun13.Content = "";
            Sun14.Content = "";
            Sun15.Content = "";
            Sun16.Content = "";
            Sun17.Content = "";

            Sun10.Background = _normal;
            Sun11.Background = _normal;
            Sun12.Background = _normal;
            Sun13.Background = _normal;
            Sun14.Background = _normal;
            Sun15.Background = _normal;
            Sun16.Background = _normal;
            Sun17.Background = _normal;
            #endregion
        }

   }
}
