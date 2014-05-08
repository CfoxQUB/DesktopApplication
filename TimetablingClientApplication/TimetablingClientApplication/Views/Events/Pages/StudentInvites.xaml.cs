using System;
using System.Collections.ObjectModel;
using System.Linq;
using TimetablingClientApplication.TimetablingService;

namespace TimetablingClientApplication.Views.Events.Pages
{
    /// <summary>
    /// Interaction logic for StudentInvites.xaml
    /// </summary>
    public partial class StudentInvites
    {
        //Ids required for updating events and staff
        private readonly Room _eventRoom;
        private readonly int _eventStaff;
        private readonly int _eventId;
        private readonly int _availableSpace;
        //Placeholder text for search field
        private const String DefaultSearchString = "Search Students Available . . .";
        //Webservice functionality exposed through reference
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        //List of available and invited students
        private readonly ObservableCollection<Student> _availableStudents = new ObservableCollection<Student>();
        private readonly ObservableCollection<Student> _invitedStudents = new ObservableCollection<Student>();

        public StudentInvites(int eventId, int roomId)
        {
            InitializeComponent();
            //Event, Staff, and room information maintained
            _eventId = eventId;
            _eventRoom = _client.ReturnRoomDetail(roomId);
            _eventStaff = 0;
            
            //return staff invitations for event
            var staff =  _client.ReturnEventsStaffInvites(eventId);
            if (staff != null)
            {
                //count current invitations
                _eventStaff = staff.Count();
            }
            //Return students to be available
            var returnAllStudents = _client.ReturnStudents();
            if (returnAllStudents != null)
            {   //populate available students lists
                foreach (var i in returnAllStudents)
                {
                    _availableStudents.Add(i);
                }
            }
            ListedStudents.ItemsSource = _availableStudents;
            //return student event Invitations for current event
            var returnEventInvites = _client.ReturnEventsStudentInvites(eventId);
            if (returnEventInvites != null && returnAllStudents != null)
            {
                foreach (var i in returnEventInvites)
                {   //Populate Invitations into page
                    _invitedStudents.Add(returnAllStudents.SingleOrDefault(x=>x.StudentId == i.StudentId));
                }
            }
            //Update invitations number and capacity info
            InvitesNumber.Content = _invitedStudents.Count();
            ListedSelectedStudent.ItemsSource = _invitedStudents;
            RoomCapacity.Content = _eventRoom.Capacity;
            //capacity calculated
            _availableSpace = _eventRoom.Capacity - (_eventStaff + _invitedStudents.Count);
            AvailableSpace.Content = _availableSpace;
            //available space calcualted
            if (_availableSpace < 0)
            {
                AvailableSpace.Content = "0";
            }
        }

        //Placeholder removed from search field
        public void SearchFieldOnFocus(Object sender, EventArgs e)
        {
            SearchAvailableStudents.Text = "";
        }

        //Placeholder replaced when search field lost focus
        public void SearchFieldLoseFocus(Object sender, EventArgs e)
        {
            SearchAvailableStudents.Text = DefaultSearchString;
        }

        //Search results returned according to the search field string
        public void SearchFieldTextChanged(Object sender, EventArgs e)
        {
            if (SearchAvailableStudents.Text != DefaultSearchString)
            {
                //search results returned
                var returnedStudents = _client.SearchStudentFunction(SearchAvailableStudents.Text);
                if (returnedStudents != null)
                {   
                    _availableStudents.Clear();
                    foreach (var r in returnedStudents)
                    {   //results populated
                        _availableStudents.Add(r);
                    }
                }
                else
                {
                    //No results
                    _availableStudents.Clear();
                }
            }
        }

        //Add staff Invite added to invitations list 
        public void AddStudentInvite(Object sender, EventArgs e)
        {
            //Student selected from available students
            var selectedStudent = (Student)ListedStudents.SelectedItem;
            if (selectedStudent != null && !_invitedStudents.Contains(selectedStudent))
            {
                //Student added to the invitation
                _invitedStudents.Add(selectedStudent);
            }

            //calculate available space and display
            var availableSpace = _eventRoom.Capacity - _invitedStudents.Count;
            AvailableSpace.Content = availableSpace;
            if (availableSpace < 0)
            {
                AvailableSpace.Content = "0";
            }
            //Display current invites
            InvitesNumber.Content = _invitedStudents.Count();
        }

        //Remove student invitation
        public void RemoveStudentInvite(Object sender, EventArgs e)
        {
            //Select student invite to be removed
            var selectedStudent = (Student)ListedSelectedStudent.SelectedItem;
            //remove selected student
            if (selectedStudent != null)
            {
                _invitedStudents.Remove(selectedStudent);
            }
            //recalculate available space and display
            var availableSpace = _eventRoom.Capacity - _invitedStudents.Count;
            AvailableSpace.Content = availableSpace;
            if (availableSpace < 0)
            {
                AvailableSpace.Content = "0";
            }
            //recalculate total invited students
            InvitesNumber.Content = _invitedStudents.Count();
        }
        
        //save listsed student invites
        public void SaveStudentInvites(Object sender, EventArgs e)
        {
            //as long as capacity not exceed invites saved 
            if (_invitedStudents.Count <= _availableSpace && _invitedStudents.Count > 0)
            {
                _client.AddStudentInvitesToEvent(_invitedStudents.ToArray(), _eventId);
            }
        }
    }
}