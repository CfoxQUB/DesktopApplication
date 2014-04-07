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
using System.Windows.Shapes;
using TimetablingClientApplication.Views.Events.Pages;
using TimetablingClientApplication.Views.Timetables;
using TimetablingClientApplication.Views.Timetables.Pages;

namespace TimetablingClientApplication.Views.MasterViews
{
    /// <summary>
    /// Interaction logic for MasterView.xaml
    /// </summary>
    public partial class MasterView : Window
    {
        private int _userId = 1;
        public MasterView()
        {
            InitializeComponent();
            //_userId = loginId;
        }

        #region Navigation

        #region MenuBarOptions
        private void MenuItem_NewEvent_Click(object sender, RoutedEventArgs e)
        {
            CreateEvents createEvents = new CreateEvents(_userId);
            createEvents.Show();
        }

        private void MenuItem_EditEvent_Click(object sender, RoutedEventArgs e)
        {
            EditEvents editEvents = new EditEvents(_userId);
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
            Frame.Navigate(new CreateEventsPage(_userId));
        }

        private void Open_Edit_Events_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new EditEventsPage(_userId));
        }

        private void Open_Events_Summary_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new EventPage());
        }

        private void Open_Events_Attendees_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new EventsAttendees());
        }

        private void Open_Timetable_Summary_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new TimetableToolPage());
        }
        
        private void Open_Timetable_Building_Page(object sender, MouseButtonEventArgs e)
        {
            Frame.Navigate(new RoomTimetableTool());
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
