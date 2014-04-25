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
        private readonly int _userId;
        public MasterView(int loginId)
        {
            _userId = loginId;
            InitializeComponent();
            
        }

        #region Navigation

        #region MenuBarOptions
        private void MenuItem_NewEvent_Click(object sender, RoutedEventArgs e)
        {
            var createEvents = new CreateEvents(_userId);
            createEvents.Show();
        }

        private void MenuItem_EditEvent_Click(object sender, RoutedEventArgs e)
        {
            var editEvents = new EditEvents(_userId);
            editEvents.Show();
        }

        private void Menuitem_TimetableView_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new TimetableToolPage());
        }
        
        private void Menuitem_WeeklyView_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new RoomTimetableTool());
        }

        #endregion

        #region TreeNavigation

        private void Open_Event_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new CreateEventsPage(_userId, Frame.NavigationService));
        }

        private void Open_Edit_Events_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new EditEventsPage(_userId));
        }

        private void Open_Events_Summary_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new EventPage());
        }

        private void Open_Timetable_Summary_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new TimetableToolPage());
        }
        
        private void Open_Timetable_Building_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new RoomTimetableTool());
        }

        private void Open_Database_Room_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new RoomManagement(_userId));
        } 
        
        private void Open_Database_Building_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new BuildingManagement(_userId));
        }

        private void Open_Timetable_Course_Page(object sender, MouseButtonEventArgs e)
        {

            Frame.Navigate(new CourseManagement(_userId, Frame.NavigationService));
        } 
        
        private void Open_Timetable_Staff_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new StaffManagement(_userId, Frame.NavigationService));
        }
        
        private void Open_Timetable_Student_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new StudentManagement(_userId, Frame.NavigationService));
        }

        private void Open_Module_Students_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new ModuleManagementPage());
        } 
        
        private void Open_Course_Module_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new CourseModuleManagement());
        }
        
        private void Open_Event_Staff_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new EventStaffPage());
        }
        
        
        private void Open_Invites_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new Invites(Frame.NavigationService));
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
