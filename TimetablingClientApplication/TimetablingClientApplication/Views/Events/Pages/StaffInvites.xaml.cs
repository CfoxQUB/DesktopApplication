using System;
using System.Collections.ObjectModel;
using System.Linq;
using TimetablingClientApplication.TimetablingService;

namespace TimetablingClientApplication.Views.Events.Pages
{
    /// <summary>
    /// Interaction logic for StaffInvites.xaml
    /// </summary>
    public partial class StaffInvites
    {
        //Room item that is selected for capacity
        private readonly Room _eventRoom;
        //Event id and numeration of staff for event
        private readonly int _eventId;
        private int _eventStudents;
        //available space in room by adding current invites to capacity
        private readonly int _availableSpace;
        //string used as placeholder text
        private const String DefaultSearchString = "Search Staff Available . . .";
        //Webservice functionality exposed through reference
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        //Staff lists of both available and invited staff
        private readonly ObservableCollection<Staff> _availableStaff = new ObservableCollection<Staff>();
        private readonly ObservableCollection<Staff> _invitedStaff = new ObservableCollection<Staff>();

        public StaffInvites(int eventId, int roomId)
        {
            InitializeComponent();
            //event Id and room details maintained as reference
            _eventId = eventId;
            _eventRoom = _client.ReturnRoomDetail(roomId);
            //staff returned for invitation purposes
            var returnAllStaff = _client.ReturnStaff();
            //Invite already associated returned
            var returnEventInvites = _client.ReturnEventsStaffInvites(eventId);
            //student invites if any returned to calculate room capacity
            var students =  _client.ReturnEventsStudentInvites(eventId);

            if (students != null)
            {  //Students invited calculated
                _eventStudents = students.Count();
            }
            else
            {  //default is 0
                _eventStudents = 0;
            }
            
            //staff available
            if (returnAllStaff != null)
            {   //if staff available populate available staff
                foreach (var i in returnAllStaff)
                {
                    _availableStaff.Add(i);
                }
            }
            ListedStaff.ItemsSource = _availableStaff;

            if (returnEventInvites != null && returnAllStaff != null)
            {   //As long as invites available
                foreach (var i in returnEventInvites)
                {   //Invitations available added to invitations list
                    _invitedStaff.Add(returnAllStaff.SingleOrDefault(x=>x.StaffId == i.StaffId));
                }
            }
            //Page information setup
            InvitesNumber.Content = _invitedStaff.Count();
            ListedSelectedStaff.ItemsSource = _invitedStaff;
            RoomCapacity.Content = _eventRoom.Capacity;
            //Available space calculated
            _availableSpace = _eventRoom.Capacity - (_eventStudents + _invitedStaff.Count);
            AvailableSpace.Content = _availableSpace;

            //if available space is negative it is displayed as 0
            if (_availableSpace < 0)
            {
                AvailableSpace.Content = "0";
            }
        }

        //Search field placeholder removes on focus
        public void SearchFieldOnFocus(Object sender, EventArgs e)
        {
            SearchAvailableStaff.Text = "";
        }
        //Search field placeholder replaced when focus lost
        public void SearchFieldLoseFocus(Object sender, EventArgs e)
        {
            SearchAvailableStaff.Text = DefaultSearchString;
        }

        //Search field content changed and results returned
        public void SearchFieldTextChanged(Object sender, EventArgs e)
        {
            if (SearchAvailableStaff.Text != DefaultSearchString)
            {   
                //retured staff members from search function
                var returnedStaff = _client.SearchStaffFunction(SearchAvailableStaff.Text);
                if (returnedStaff != null)
                {   //available staff cleared
                    _availableStaff.Clear();
                    foreach (var r in returnedStaff)
                    {   //availabel staff repopulated
                        _availableStaff.Add(r);
                    }
                }
                else
                {
                    //No results returned
                    _availableStaff.Clear();
                }
            }
        }

        //Staff invitation added
        public void AddStaffInvite(Object sender, EventArgs e)
        {
            //staffmemeber selected
            var selectedStaff = (Staff)ListedStaff.SelectedItem;
            if (selectedStaff != null && !_invitedStaff.Contains(selectedStaff))
            {   //Invited staff list updated
                _invitedStaff.Add(selectedStaff);
            }
            //Capacity availability updated
            var availableSpace = _eventRoom.Capacity - _invitedStaff.Count;
            AvailableSpace.Content = availableSpace;
            if (availableSpace < 0)
            {
                AvailableSpace.Content = "0";
            }
            InvitesNumber.Content = _invitedStaff.Count();
        }

        //Removal of staff invitation
        public void RemoveStaffInvite(Object sender, EventArgs e)
        {
            //Staff member selected
            var selectedStaff = (Staff)ListedSelectedStaff.SelectedItem;
            if (selectedStaff != null)
            {   //Invited staff removed
                _invitedStaff.Remove(selectedStaff);
            }
            //capacity updated
            var availableSpace = _eventRoom.Capacity - _invitedStaff.Count;
            AvailableSpace.Content = availableSpace;
            //available space updated to reflect changes
            if (availableSpace < 0)
            {
                AvailableSpace.Content = "0";
            }

            InvitesNumber.Content = _invitedStaff.Count();
        }
        
        //Save staff invitations according to invitations list
        public void SaveStaffInvites(Object sender, EventArgs e)
        {
            //as long as capacity not exceeded and invitations arent 0
            if (_invitedStaff.Count <= _availableSpace && _invitedStaff.Count > 0)
            {
                //Staff invites updated
                _client.AddStaffInvitesToEvent(_invitedStaff.ToArray(), _eventId);
            }
        }
    }
}
