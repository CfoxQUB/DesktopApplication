using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// Interaction logic for Timetable.xaml
    /// </summary>
    public partial class Timetable : Window
    {
        TimetablingService.TimetablingServiceClient client = new TimetablingService.TimetablingServiceClient();

        private CultureInfo currentculture = CultureInfo.CurrentCulture;

        private List<TimetablingService.Event> weekEvents = new List<TimetablingService.Event>();

        private int LoggedInUser = new int();

        public Timetable(int UserId)
        {
            InitializeComponent();

            LoggedInUser = UserId;

            var buildingsList = client.ReturnBuildings();

            var RoomsList = client.ReturnBuildingRooms(buildingsList.First().BuildingId);

            foreach (TimetablingService.Building b in buildingsList)
            {
                BuildingSelect.Items.Add(b.BuildingName);
            }

            foreach (TimetablingService.Room r in RoomsList)
            {
                RoomSelect.Items.Add(r.RoomName);
            }

            BuildingSelect.Text = buildingsList.First().BuildingName;
            RoomSelect.Text = RoomsList.First().RoomName;

            var tempEvents = client.ReturnWeeksEvents(DateTime.Now, 1);

            foreach (var e in tempEvents)
            {
                var day = currentculture.Calendar.GetDayOfWeek((DateTime)e.StartDate);
                switch (day.ToString())
                {
                    case "Sunday": SundayEvents.Items.Add(e);
                        break;
                    case "Monday": MondayEvents.Items.Add(e);
                        break;
                    case "Tuesday": TuesdayEvents.Items.Add(e);
                        break;
                    case "Wednesday": WednesdayEvents.Items.Add(e);
                        break;
                    case "Thursday": ThursdayEvents.Items.Add(e);
                        break;
                    case "Friday": FridayEvents.Items.Add(e);
                        break;
                    case "Saturday": SaturdayEvents.Items.Add(e);
                        break;
                }
            }
        }


        #region Navigation
        private void MenuItem_NewEvent_Click(object sender, RoutedEventArgs e)
        {
            CreateEvents createEvents = new CreateEvents(LoggedInUser);
            createEvents.Show();
        }

        private void MenuItem_EditEvent_Click(object sender, RoutedEventArgs e)
        {
            EditEvents editEvents = new EditEvents(LoggedInUser);
            editEvents.Show();
        }
        private void Menuitem_TimetableView_Click(object sender, RoutedEventArgs e)
        {
            Timetable timetableView = new Timetable(LoggedInUser);
            timetableView.Show();
            this.Close();
        }
        #endregion

        private void Datechanged(object sender, RoutedEventArgs e)
        {

            var tempEvents = client.ReturnWeeksEvents((DateTime)DateSelected.SelectedDate, 1);

            if (tempEvents != null)
            {
                foreach (var x in tempEvents)
                {
                    weekEvents.Add((TimetablingService.Event)x);
                }

                PopulateTimetable();
                return;
            }
        }

        public void PopulateTimetable()
        {
            var tempEvents = client.ReturnWeeksEvents((DateTime)DateSelected.SelectedDate, 1);

            if (tempEvents == null)
            {
                return;
            }

            SundayEvents.Items.Clear();
            MondayEvents.Items.Clear();
            TuesdayEvents.Items.Clear();
            WednesdayEvents.Items.Clear();
            ThursdayEvents.Items.Clear();
            FridayEvents.Items.Clear();
            SaturdayEvents.Items.Clear();

            foreach (var e in tempEvents)
            {
                var day = currentculture.Calendar.GetDayOfWeek((DateTime)e.StartDate);
                
                switch (day.ToString())
                {
                    case "Sunday": SundayEvents.Items.Add(e);
                        break;
                    case "Monday": MondayEvents.Items.Add(e);
                        break;
                    case "Tuesday": TuesdayEvents.Items.Add(e);
                        break;
                    case "Wednesday": WednesdayEvents.Items.Add(e);
                        break;
                    case "Thursday": ThursdayEvents.Items.Add(e);
                        break;
                    case "Friday": FridayEvents.Items.Add(e);
                        break;
                    case "Saturday": SaturdayEvents.Items.Add(e);
                        break;
                }
            }


        }
    }
}

