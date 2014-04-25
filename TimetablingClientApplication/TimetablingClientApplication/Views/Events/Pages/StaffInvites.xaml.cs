using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using TimetablingClientApplication.TimetablingService;

namespace TimetablingClientApplication.Views.Events.Pages
{
    /// <summary>
    /// Interaction logic for StaffInvites.xaml
    /// </summary>
    public partial class StaffInvites
    {
        private readonly Room _eventRoom;
        private readonly int _eventStudents;
        private readonly int _eventId;
        private readonly int _availableSpace;
        private const String DefaultSearchString = "Search Staff Available . . .";

        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        private readonly ObservableCollection<Staff> _availableStaff = new ObservableCollection<Staff>();
        private readonly ObservableCollection<Staff> _invitedStaff = new ObservableCollection<Staff>();

        public StaffInvites(int eventId, int roomId)
        {
            InitializeComponent();
            _eventId = eventId;
            _eventRoom = _client.ReturnRoomDetail(roomId);
            _eventStudents = 0;
            var returnAllStaff = _client.ReturnStaff();

            var returnEventInvites = _client.ReturnEventsStaffInvites(eventId);
            var students =  _client.ReturnEventsStudentInvites(eventId);

            if (students != null)
            {
                _eventStudents = students.Count();
            }
            
            if (returnAllStaff != null)
            {
                foreach (var i in returnAllStaff)
                {
                    _availableStaff.Add(i);
                }
            }

            ListedStaff.ItemsSource = _availableStaff;
            if (returnEventInvites != null && returnAllStaff != null)
            {
                foreach (var i in returnEventInvites)
                {
                    _invitedStaff.Add(returnAllStaff.SingleOrDefault(x=>x.StaffId == i.StaffId));
                }
            }
            InvitesNumber.Content = _invitedStaff.Count();
            ListedSelectedStaff.ItemsSource = _invitedStaff;
            RoomCapacity.Content = _eventRoom.Capacity;

            _availableSpace = _eventRoom.Capacity - (_eventStudents + _invitedStaff.Count);
            AvailableSpace.Content = _availableSpace;

            if (_availableSpace < 0)
            {
                AvailableSpace.Content = "0";
            }
        }

        public void SearchFieldOnFocus(Object sender, EventArgs e)
        {
            SearchAvailableStaff.Text = "";
        }

        public void SearchFieldLoseFocus(Object sender, EventArgs e)
        {
            SearchAvailableStaff.Text = DefaultSearchString;
        }

        public void SearchFieldTextChanged(Object sender, EventArgs e)
        {
            if (SearchAvailableStaff.Text != DefaultSearchString)
            {
                var returnedStaff = _client.SearchStaffFunction(SearchAvailableStaff.Text);

                if (returnedStaff != null)
                {
                    _availableStaff.Clear();
                    foreach (var r in returnedStaff)
                    {
                        _availableStaff.Add(r);
                    }
                }
                else
                {
                    _availableStaff.Clear();
                }
            }
        }

        public void AddStaffInvite(Object sender, EventArgs e)
        {
            var selectedStaff = (Staff)ListedStaff.SelectedItem;

            if (selectedStaff != null && !_invitedStaff.Contains(selectedStaff))
            {
                _invitedStaff.Add(selectedStaff);
            }

            var availableSpace = _eventRoom.Capacity - _invitedStaff.Count;
            AvailableSpace.Content = availableSpace;

            if (availableSpace < 0)
            {
                AvailableSpace.Content = "0";
            }

            InvitesNumber.Content = _invitedStaff.Count();
        }

        public void RemoveStaffInvite(Object sender, EventArgs e)
        {
            var selectedStaff = (Staff)ListedSelectedStaff.SelectedItem;

            if (selectedStaff != null)
            {
                _invitedStaff.Remove(selectedStaff);
            }

            var availableSpace = _eventRoom.Capacity - _invitedStaff.Count;
            AvailableSpace.Content = availableSpace;

            if (availableSpace < 0)
            {
                AvailableSpace.Content = "0";
            }

            InvitesNumber.Content = _invitedStaff.Count();
        }
        
        
        public void SaveStaffInvites(Object sender, EventArgs e)
        {
            if (_invitedStaff.Count <= _availableSpace && _invitedStaff.Count > 0)
            {
                _client.AddStaffInvitesToEvent(_invitedStaff.ToArray(), _eventId);
            }
        }
    }
}
