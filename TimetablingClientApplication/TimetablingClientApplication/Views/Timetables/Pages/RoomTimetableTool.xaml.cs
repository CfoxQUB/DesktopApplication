using System;
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
    public partial class RoomTimetableTool
    {

        private int _roomId;
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        private readonly List<Building> _buildings = new List<Building>();
        private readonly List<Room> _rooms = new List<Room>();
        
        private readonly SolidColorBrush _occupied = new SolidColorBrush(Colors.GreenYellow);

        private readonly SolidColorBrush _normal =
            new SolidColorBrush((Color) ColorConverter.ConvertFromString("#FFEAEAEA"));

        private bool _buildingSelected;
        private bool _courseSelected;

        public RoomTimetableTool()
        {
            _buildingSelected = false;
            
            InitializeComponent();

            _buildings.AddRange(_client.ReturnBuildings());
            if (_buildings != null)
            {
                foreach (var x in _buildings)
                {
                    BuildingSelect.Items.Add(x.BuildingName);
                }
                BuildingSelect.SelectedItem = _buildings.First().BuildingName;

                _rooms.AddRange(_client.ReturnBuildingRooms(_buildings.First().BuildingId));

                if (_rooms.Any() && _rooms != null)
                {
                    foreach (var x in _rooms)
                    {
                        RoomSelect.Items.Add(x.RoomName);
                    }
                    RoomSelect.SelectedItem = _rooms.First().RoomName;
                    _roomId = _rooms.First().RoomId;
                }
                else
                {
                    _roomId = 0;
                    RoomSelect.IsEnabled = false;
                }
            }
            else
            {
                _roomId = 0;
                RoomSelect.IsEnabled = false;
                BuildingSelect.IsEnabled = false;
            }

           

            DateSelected.SelectedDate = DateTime.Now;

            if (_roomId != 0 )
            {
                var temp = _client.ReturnWeeksEventsWithFilters(DateTime.Now, _roomId);
                if (temp != null)
                {
                    Populate_Timetable_Display(temp);
                }
            }
        }

        public void Building_Selection_Changed(object sender, RoutedEventArgs e)
        {
            if (_buildingSelected == false)
            {
                _buildingSelected = true;
                return;
            }

            var building = _buildings.SingleOrDefault(x => x.BuildingName == BuildingSelect.SelectedItem.ToString());

            if (building != null)
            {
                BuildingSelect.IsEnabled = true;
                var rooms = _client.ReturnBuildingRooms(building.BuildingId);
                if (rooms != null)
                {
                    RoomSelect.IsEnabled = true;
                    _rooms.Clear();
                    RoomSelect.Items.Clear();
                    
                        foreach (var x in rooms)
                        {
                            RoomSelect.Items.Add(x.RoomName);
                            _rooms.Add(x);
                        }
                        RoomSelect.SelectedItem = _rooms.First().RoomName;
                        _roomId = _rooms.First().RoomId;
                    
                }
                else
                {
                    _roomId = 0;
                    RoomSelect.IsEnabled = false;
                    ClearTimetableOfEvents();
                }
            }
            else
            {
                _roomId = 0;
                BuildingSelect.IsEnabled = false;
                RoomSelect.IsEnabled = false;
                ClearTimetableOfEvents();
            }

            var dateSelected = DateSelected.SelectedDate.GetValueOrDefault();
            if (_roomId != 0)
            {
                var temp = _client.ReturnWeeksEventsWithFilters(dateSelected, _roomId);
                if (temp != null)
                {
                    Populate_Timetable_Display(temp);
                }
            }
        }

        public void Room_Selection_Changed(object sender, RoutedEventArgs e)
        {
            if (_buildingSelected)
            {
                return;
            }

            var room = _rooms.SingleOrDefault(x => x.RoomName == RoomSelect.SelectedItem.ToString());

            if (room == null)
            {
                return;
            }

            _roomId = room.RoomId;

            var dateSelected = DateSelected.SelectedDate.GetValueOrDefault();
            if (_roomId != 0)
            {
                var temp = _client.ReturnWeeksEventsWithFilters(dateSelected, _roomId);
                if (temp != null)
                {
                    Populate_Timetable_Display(temp);
                }
            }
        }

        public void Date_Selection_Changed(object sender, RoutedEventArgs e)
        {
            var dateSelected = DateSelected.SelectedDate.GetValueOrDefault();
            if (_roomId != 0 )
            {
                var temp = _client.ReturnWeeksEventsWithFilters(dateSelected, _roomId);
                if (temp != null)
                {
                    Populate_Timetable_Display(temp);
                }
            }
        }

        public void Populate_Timetable_Display(TimetableEventsListObject timetableObjectList)
        {
            ClearTimetableOfEvents();

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


        public void ReturnDateSelectedWeeksEvents(object sender, EventArgs e)
        {

            ClearTimetableOfEvents();

            var dateSelected = DateSelected.DisplayDate;

            if (_roomId != 0)
            {
                var temp = _client.ReturnWeeksEventsWithFilters(dateSelected, _roomId);
                if (temp != null)
                {
                    Populate_Timetable_Display(temp);
                }
            }
        }

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
