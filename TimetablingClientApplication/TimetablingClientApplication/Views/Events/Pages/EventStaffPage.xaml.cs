using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TimetablingClientApplication.TimetablingService;

namespace TimetablingClientApplication.Views.Events.Pages
{
    /// <summary>
    /// Interaction logic for EventStaffPage.xaml
    /// </summary>
    public partial class EventStaffPage 
    {
        private readonly ObservableCollection<Event> _allEvents = new ObservableCollection<Event>();
        private readonly ObservableCollection<Staff> _courseStaff = new ObservableCollection<Staff>();

        private int _eventId;
        private int _staffId;

        private const String DefaultSearchString = "Search Events Available . . ."; 

        private readonly TimetablingServiceClient _client =  new TimetablingServiceClient();

        public EventStaffPage()
        {
            _eventId = 0;
            _staffId = 0;

            InitializeComponent();

            var returnEvents = _client.ReturnEventsWithModules();

            var returnCourseStaff =  new List<Staff>();

            StaffAllocated.Content = "No Staff Member Allocated";
            EventSelected.Content = "No Event Selected";
            if (returnEvents != null)
            {
                foreach (var e in returnEvents)
                {
                    _allEvents.Add(e);
                }
                _eventId = returnEvents.First().EventId;
                
                var eventStaff = _client.ReturnEventStaff(_eventId);
                
                EventSelected.Content = returnEvents.First().EventTitle;
                if (eventStaff != null)
                {
                    _staffId = eventStaff.StaffId;
                    StaffAllocated.Content = eventStaff.StaffTitle + " " + eventStaff.StaffForename + " " +
                                             eventStaff.StaffSurname;
                }
                
                returnCourseStaff.AddRange(_client.ReturnCourseStaff(returnEvents.First().Course));

                foreach (var m in returnCourseStaff)
                {
                    _courseStaff.Add(m);
                }
            }

            ListedEvents.ItemsSource = _allEvents;
            ListedStaff.ItemsSource = _courseStaff;
        }

        public void SearchFieldOnFocus(Object sender, EventArgs e)
        {
            SearchField.Text = "";
        }
        
        public void SearchFieldLoseFocus(Object sender, EventArgs e)
        {
            SearchField.Text = DefaultSearchString;
        }
        
        public void SearchFieldTextChanged(Object sender, EventArgs e)
        {
            if (SearchField.Text != DefaultSearchString)
            {
              var returnedEvents = _client.SearchEventsWithModulesFunction(SearchField.Text);

                if (returnedEvents != null)
                {
                    _allEvents.Clear();

                    foreach (var d in returnedEvents)
                    {
                        _allEvents.Add(d);
                    }
                }
            }
        }

        public void NewEventSelected(Object sender, EventArgs e)
        {
            var selectedEvent = (Event)ListedEvents.SelectedItem;
            var returnCourseStaff = new List<Staff>();
            if (selectedEvent != null)
            {
                _eventId = selectedEvent.EventId;
                EventSelected.Content = selectedEvent.EventTitle;

                var eventStaff = _client.ReturnEventStaff(selectedEvent.EventId);

                if (eventStaff != null)
                {
                    StaffAllocated.Content = eventStaff.StaffTitle + " " + eventStaff.StaffForename + " " +
                                             eventStaff.StaffSurname;
                }
                else
                {
                    StaffAllocated.Content = "No Staff Member Allocated";
                }

                returnCourseStaff.AddRange(_client.ReturnCourseStaff(selectedEvent.Course));

                _courseStaff.Clear();
                foreach (var m in returnCourseStaff)
                {
                    _courseStaff.Add(m);
                }
            }
        } 
        
        
        public void NewStaffMemeberAllocated(Object sender, EventArgs e)
        {
            var selectedStaff = (Staff)ListedStaff.SelectedItem;

            if (selectedStaff != null)
            {
                _staffId = selectedStaff.StaffId;
                StaffAllocated.Content = selectedStaff.StaffTitle + " " + selectedStaff.StaffForename + " " +
                                             selectedStaff.StaffSurname;
            }
            else
            {
                StaffAllocated.Content = "No Staff Member Allocated";
            }
        }

        public void SaveStaffAllocation(Object sender, EventArgs e)
        {
            if (_staffId != 0 && _eventId != 0)
            {
                _client.StaffEvent(_eventId, _staffId);
                Line1.Text = "Staff Allocation Suceeded";
                StaffAllocation.IsOpen = true;
            }
            else
            {
                Line1.Text = "Staff Allocation Failed";
                StaffAllocation.IsOpen = true;
            }
        }
    }
}
