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
        private int _roomId;
        private readonly NavigationService _navigationService;
        private const String DefaultSearchString = "Search Events Available . . .";
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        private readonly ObservableCollection<Event> _eventsList = new ObservableCollection<Event>();
        private int _eventId;

        public Invites(NavigationService navigation)
        {
            _navigationService = navigation;

            InitializeComponent();

            var eventsAvailable = _client.ReturnEventsWithRoomsNoModules();
            if (eventsAvailable != null)
            {
                foreach (var e in eventsAvailable)
                {
                    _eventsList.Add(e);
                }

                EventName.Content = _eventsList.First().EventTitle;
                _eventId = _eventsList.First().EventId;
                _roomId = _eventsList.First().Room;
                ListedEvents.ItemsSource = _eventsList;
            }

        }

        public void SearchFieldOnFocus(Object sender, EventArgs e)
        {
            SearchField.Text = "";
        }

        public void SearchFieldLoseFocus(Object sender, EventArgs e)
        {
            SearchField.Text = DefaultSearchString;

            var eventsAvailable = _client.ReturnEventsWithRoomsNoModules();
            if (eventsAvailable != null)
            {
                _eventsList.Clear();
                foreach (var r in eventsAvailable)
                {
                    _eventsList.Add(r);
                }

                EventName.Content = _eventsList.First().EventTitle;
                _roomId = _eventsList.First().Room;
                ListedEvents.ItemsSource = _eventsList;
            }
           
        }

        public void SearchFieldTextChanged(Object sender, EventArgs e)
        {
            if (SearchField.Text != DefaultSearchString)
            {
                var returnedEvents = _client.SearchEventsWithRoomsOnlyFunction(SearchField.Text);

                if (returnedEvents != null)
                {
                    _eventsList.Clear();
                    foreach (var r in returnedEvents)
                    {
                        _eventsList.Add(r);
                    }
                }
                else
                {
                    _eventsList.Clear();
                }
            }
        }
        
        public void NewEventSelected(Object sender, EventArgs e)
        {
            var selectedItem = (Event)ListedEvents.SelectedItem;

            if (selectedItem != null)
            {
                _eventId = selectedItem.EventId;
                _roomId = selectedItem.Room;
                EventName.Content = selectedItem.EventTitle;
            }
        }

        public void StaffInvitesButtonClicked(Object sender, EventArgs e)
        {
            _navigationService.Navigate(new StaffInvites(_eventId, _roomId));
        }
        
        public void StudentInvitesButtonClicked(Object sender, EventArgs e)
        {
            _navigationService.Navigate(new StudentInvites(_eventId, _roomId));
        }

    }
}
