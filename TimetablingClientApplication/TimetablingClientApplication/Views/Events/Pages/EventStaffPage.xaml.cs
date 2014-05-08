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
        //Observable collections for display of staff and events
        private readonly ObservableCollection<Event> _allEvents = new ObservableCollection<Event>();
        private readonly ObservableCollection<Staff> _courseStaff = new ObservableCollection<Staff>();

        //Event and staff Ids maintaied for allocation
        private int _eventId;
        private int _staffId;
        //Default search string placeholder text
        private const String DefaultSearchString = "Search Events Available . . ."; 
        //Webservice functionality exposed through service reference
        private readonly TimetablingServiceClient _client =  new TimetablingServiceClient();

        //Page Initialized
        public EventStaffPage()
        {
            //Default Ids set for staff and event
            _eventId = 0;
            _staffId = 0;

            InitializeComponent();
            //Events list returned
            var returnEvents = _client.ReturnEventsWithModules();
            //Course staff list for event associated course
            var returnCourseStaff =  new List<Staff>();
            //Default page values
            StaffAllocated.Content = "No Staff Member Allocated";
            EventSelected.Content = "No Event Selected";
            if (returnEvents != null)
            {   //Events list population
                foreach (var e in returnEvents)
                {
                    _allEvents.Add(e);
                }
                _eventId = returnEvents.First().EventId;
                
                //Event staff returned
                var eventStaff = _client.ReturnEventStaff(_eventId);
                EventSelected.Content = returnEvents.First().EventTitle;
                if (eventStaff != null)
                {   //selected staff member displayed
                    _staffId = eventStaff.StaffId;
                    StaffAllocated.Content = eventStaff.StaffTitle + " " + eventStaff.StaffForename + " " +
                                             eventStaff.StaffSurname;
                }
                //Course staff members returned
                returnCourseStaff.AddRange(_client.ReturnCourseStaff(returnEvents.First().Course));

                foreach (var m in returnCourseStaff)
                {
                    //Staff List populated
                    _courseStaff.Add(m);
                }
            }
            //itemsources set
            ListedEvents.ItemsSource = _allEvents;
            ListedStaff.ItemsSource = _courseStaff;
        }

        //ON focus method removes placeholder in search field
        public void SearchFieldOnFocus(Object sender, EventArgs e)
        {
            SearchField.Text = "";
        }
        
        //Reste palceholder text in search field
        public void SearchFieldLoseFocus(Object sender, EventArgs e)
        {
            SearchField.Text = DefaultSearchString;
        }
        
        //Search Events results returned
        public void SearchFieldTextChanged(Object sender, EventArgs e)
        {
            if (SearchField.Text != DefaultSearchString)
            {
                //search results returned
              var returnedEvents = _client.SearchEventsWithModulesFunction(SearchField.Text);

                if (returnedEvents != null)
                {
                    //Populate events in page
                    _allEvents.Clear();

                    foreach (var d in returnedEvents)
                    {
                        _allEvents.Add(d);
                    }
                }
            }
        }

        //New event selection, population of staff and selected staff
        public void NewEventSelected(Object sender, EventArgs e)
        {
            var selectedEvent = (Event)ListedEvents.SelectedItem;
            var returnCourseStaff = new List<Staff>();
            if (selectedEvent != null)
            {
                _eventId = selectedEvent.EventId;
                EventSelected.Content = selectedEvent.EventTitle;
                //Allocated staff returned
                var eventStaff = _client.ReturnEventStaff(selectedEvent.EventId);

                if (eventStaff != null)
                {
                    //allocated staff details displayed
                    StaffAllocated.Content = eventStaff.StaffTitle + " " + eventStaff.StaffForename + " " +
                                             eventStaff.StaffSurname;
                }
                else
                {
                    //No allocation
                    StaffAllocated.Content = "No Staff Member Allocated";
                }
                //staff list for course returned
                returnCourseStaff.AddRange(_client.ReturnCourseStaff(selectedEvent.Course));

                _courseStaff.Clear();
                foreach (var m in returnCourseStaff)
                {
                    //Staff from course populated
                    _courseStaff.Add(m);
                }
            }
        } 
        
        //save new staff memebr allocation
        public void NewStaffMemeberAllocated(Object sender, EventArgs e)
        {
            //staff selected
            var selectedStaff = (Staff)ListedStaff.SelectedItem;

            if (selectedStaff != null)
            {
                //staff allocated 
                _staffId = selectedStaff.StaffId;
                StaffAllocated.Content = selectedStaff.StaffTitle + " " + selectedStaff.StaffForename + " " +
                                             selectedStaff.StaffSurname;
            }
            else
            {
                StaffAllocated.Content = "No Staff Member Allocated";
            }
        }

        //save new staff allocation
        public void SaveStaffAllocation(Object sender, EventArgs e)
        {
            if (_staffId != 0 && _eventId != 0)
            {   //Staff allocation success
                _client.StaffEvent(_eventId, _staffId);
                Line1.Text = "Staff Allocation Suceeded";
                StaffAllocation.IsOpen = true;
            }
            else
            {
                //Staff allocation failed
                Line1.Text = "Staff Allocation Failed";
                StaffAllocation.IsOpen = true;
            }
        }
    }
}
