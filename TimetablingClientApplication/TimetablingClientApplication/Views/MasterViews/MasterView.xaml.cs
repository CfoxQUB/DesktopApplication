using System.Windows;
using System.Windows.Input;
using TimetablingClientApplication.Views.Database.Pages;
using TimetablingClientApplication.Views.Events.Pages;
using TimetablingClientApplication.Views.Events.Windows;
using TimetablingClientApplication.Views.Timetables.Pages;

namespace TimetablingClientApplication.Views.MasterViews
{
    /// <summary>
    /// Interaction logic for MasterView.xaml
    /// </summary>
    public partial class MasterView 
    {
        //user Id maintained from login
        private readonly int _userId;

        //Master view initialized
        public MasterView(int loginId)
        {
            //User id mainained and windpw setup
            _userId = loginId;
            InitializeComponent();
        }

        #region Navigation

        #region MenuBarOptions
        
        //from menu bar opens the create events window
        private void MenuItem_NewEvent_Click(object sender, RoutedEventArgs e)
        {
            var createEvents = new CreateEvents(_userId, Frame.NavigationService);
            createEvents.Show();
        }

        //From menu Bar opens the Edit events window
        private void MenuItem_EditEvent_Click(object sender, RoutedEventArgs e)
        {
            var editEvents = new EditEvents(_userId);
            editEvents.Show();
        }

        //Opens timetable tool page which calculates total room usage across the year
        private void Menuitem_TimetableView_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new TimetableToolPage());
        }
        #endregion

        #region Main Navigation

        //OPens the room timetable tool which displays room timetables
        private void Menuitem_WeeklyView_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new RoomTimetableTool());
        }
        
        //Opens crete event page
        private void Open_Event_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new CreateEventsPage(_userId, Frame.NavigationService));
        }

        //Opens edit event page
        private void Open_Edit_Events_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new EditEventsPage(_userId));
        }

        //Opens event summary page
        private void Open_Events_Summary_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new EventPage());
        }

        //Opens timetable sumary page
        private void Open_Timetable_Summary_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new TimetableToolPage());
        }
        
        //Opens timetable building rooms page
        private void Open_Timetable_Building_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new RoomTimetableTool());
        }

        //Opens database room page
        private void Open_Database_Room_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new RoomManagement(_userId));
        }

        //Opens database buildings page
        private void Open_Database_Building_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new BuildingManagement(_userId, Frame.NavigationService));
        }

        //Opens database course page
        private void Open_Timetable_Course_Page(object sender, MouseButtonEventArgs e)
        {

            Frame.Navigate(new CourseManagement(_userId, Frame.NavigationService));
        }

        //Opens database staff page
        private void Open_Timetable_Staff_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new StaffManagement(_userId, Frame.NavigationService));
        }

        //Opens database student page
        private void Open_Timetable_Student_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new StudentManagement(_userId, Frame.NavigationService));
        }

        //Opens database student moduel allocation page
        private void Open_Module_Students_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new ModuleManagementPage());
        }

        //Opens database module allocation page      
        private void Open_Course_Module_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new CourseModuleManagement(_userId, Frame.NavigationService));
        }

        //Opens database staff event page
        private void Open_Event_Staff_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new EventStaffPage());
        }

        //Opens Invitations page
        private void Open_Invites_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Invites(Frame.NavigationService));
        }

        //Opens course timetable page
        private void Open_Course_Timetable_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new CourseTimetableTool());
        }
        
        //Opens reports page
        private void Open_Reports_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new ReportsManagement());
        }

        #endregion

        #endregion


        #region backandforward
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Frame.NavigationService.CanGoBack)
            {
                Frame.NavigationService.GoBack();
            }
          
           
        } 
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (Frame.NavigationService.CanGoForward)
            {
                Frame.NavigationService.GoForward();
            }


        }
        #endregion

        


    }
}
