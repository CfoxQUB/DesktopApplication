using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Navigation;
using TimetablingClientApplication.TimetablingService;

namespace TimetablingClientApplication.Views.Events.Pages
{
    /// <summary>
    /// Interaction logic for Invites.xaml
    /// </summary>
    public partial class Invites
    {
        //event and room Id of event selected
        private int _eventId;
        private int _roomId;
        //Navigation service of frame in main window
        private readonly NavigationService _navigationService;
        //Deafult search string (Placeholder)
        private const String DefaultSearchString = "Search Events Available . . .";
        //Webservice reference to expose functionality
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        //Events list for search results
        private readonly ObservableCollection<Event> _eventsList = new ObservableCollection<Event>();
        
        public Invites(NavigationService navigation)
        {
            //navigation service maintained from main window
            _navigationService = navigation;
            //Page initialized
            InitializeComponent();
            //Return events which allow invites only
            var eventsAvailable = _client.ReturnEventsWithRoomsNoModules();
            if (eventsAvailable != null)
            {
                foreach (var e in eventsAvailable)
                {
                    //Events List populated
                    _eventsList.Add(e);
                }
                //Details of first event stored and displayed
                EventName.Content = _eventsList.First().EventTitle;
                _eventId = _eventsList.First().EventId;
                _roomId = _eventsList.First().Room;
                ListedEvents.ItemsSource = _eventsList;
            }

        }
        //Remove placeholder text from search field
        public void SearchFieldOnFocus(Object sender, EventArgs e)
        {
            SearchField.Text = "";
        }
        //Return placeholder to search field and repopulate events
        public void SearchFieldLoseFocus(Object sender, EventArgs e)
        {
            SearchField.Text = DefaultSearchString;

            var eventsAvailable = _client.ReturnEventsWithRoomsNoModules();
            if (eventsAvailable != null)
            {   //events cleared
                _eventsList.Clear();
                foreach (var r in eventsAvailable)
                {  //Events repopulated
                    _eventsList.Add(r);
                }
                //first event reselected
                EventName.Content = _eventsList.First().EventTitle;
                _roomId = _eventsList.First().Room;
                ListedEvents.ItemsSource = _eventsList;
            }
           
        }

        //search function of events with rooms only exposed
        public void SearchFieldTextChanged(Object sender, EventArgs e)
        {
            if (SearchField.Text != DefaultSearchString)
            {
                //appropriate events returned
                var returnedEvents = _client.SearchEventsWithRoomsOnlyFunction(SearchField.Text);
                if (returnedEvents != null)
                {
                    _eventsList.Clear();
                    foreach (var r in returnedEvents)
                    {   //events populated into listed events
                        _eventsList.Add(r);
                    }
                }
                else
                {
                    //No results
                    _eventsList.Clear();
                }
            }
        }
        
        //New event selected from events list
        public void NewEventSelected(Object sender, EventArgs e)
        {
            //New event seelted from list
            var selectedItem = (Event)ListedEvents.SelectedItem;
            //Event details populated into page
            if (selectedItem != null)
            {
                _eventId = selectedItem.EventId;
                _roomId = selectedItem.Room;
                EventName.Content = selectedItem.EventTitle;
            }
        }

        //Navigation to Staff invitations page for this event
        public void StaffInvitesButtonClicked(Object sender, EventArgs e)
        {
            _navigationService.Navigate(new StaffInvites(_eventId, _roomId));
        }
        //Navigation to Student invitations page for this event
        public void StudentInvitesButtonClicked(Object sender, EventArgs e)
        {
            _navigationService.Navigate(new StudentInvites(_eventId, _roomId));
        }

    }
}
