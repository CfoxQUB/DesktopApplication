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
        private readonly Room _eventRoom;
        private readonly int _eventStaff;
        private readonly int _eventId;
        private readonly int _availableSpace;
        private const String DefaultSearchString = "Search Students Available . . .";

        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        private readonly ObservableCollection<Student> _availableStudents = new ObservableCollection<Student>();
        private readonly ObservableCollection<Student> _invitedStudents = new ObservableCollection<Student>();

        public StudentInvites(int eventId, int roomId)
        {
            InitializeComponent();
            _eventId = eventId;
            _eventRoom = _client.ReturnRoomDetail(roomId);
            _eventStaff = 0;
            var returnAllStudents = _client.ReturnStudents();

            var returnEventInvites = _client.ReturnEventsStudentInvites(eventId);
            var staff =  _client.ReturnEventsStaffInvites(eventId);

            if (staff != null)
            {
                _eventStaff = staff.Count();
            }
            
            if (returnAllStudents != null)
            {
                foreach (var i in returnAllStudents)
                {
                    _availableStudents.Add(i);
                }
            }

            ListedStudents.ItemsSource = _availableStudents;
            if (returnEventInvites != null && returnAllStudents != null)
            {
                foreach (var i in returnEventInvites)
                {
                    _invitedStudents.Add(returnAllStudents.SingleOrDefault(x=>x.StudentId == i.StudentId));
                }
            }
            InvitesNumber.Content = _invitedStudents.Count();
            ListedSelectedStudent.ItemsSource = _invitedStudents;
            RoomCapacity.Content = _eventRoom.Capacity;

            _availableSpace = _eventRoom.Capacity - (_eventStaff + _invitedStudents.Count);
            AvailableSpace.Content = _availableSpace;

            if (_availableSpace < 0)
            {
                AvailableSpace.Content = "0";
            }
        }

        public void SearchFieldOnFocus(Object sender, EventArgs e)
        {
            SearchAvailableStudents.Text = "";
        }

        public void SearchFieldLoseFocus(Object sender, EventArgs e)
        {
            SearchAvailableStudents.Text = DefaultSearchString;
        }

        public void SearchFieldTextChanged(Object sender, EventArgs e)
        {
            if (SearchAvailableStudents.Text != DefaultSearchString)
            {
                var returnedStudents = _client.SearchStudentFunction(SearchAvailableStudents.Text);

                if (returnedStudents != null)
                {
                    _availableStudents.Clear();
                    foreach (var r in returnedStudents)
                    {
                        _availableStudents.Add(r);
                    }
                }
                else
                {
                    _availableStudents.Clear();
                }
            }
        }

        public void AddStudentInvite(Object sender, EventArgs e)
        {
            var selectedStudent = (Student)ListedStudents.SelectedItem;

            if (selectedStudent != null && !_invitedStudents.Contains(selectedStudent))
            {
                _invitedStudents.Add(selectedStudent);
            }

            var availableSpace = _eventRoom.Capacity - _invitedStudents.Count;
            AvailableSpace.Content = availableSpace;

            if (availableSpace < 0)
            {
                AvailableSpace.Content = "0";
            }

            InvitesNumber.Content = _invitedStudents.Count();
        }

        public void RemoveStudentInvite(Object sender, EventArgs e)
        {
            var selectedStudent = (Student)ListedSelectedStudent.SelectedItem;

            if (selectedStudent != null)
            {
                _invitedStudents.Remove(selectedStudent);
            }

            var availableSpace = _eventRoom.Capacity - _invitedStudents.Count;
            AvailableSpace.Content = availableSpace;

            if (availableSpace < 0)
            {
                AvailableSpace.Content = "0";
            }

            InvitesNumber.Content = _invitedStudents.Count();
        }
        
        
        public void SaveStudentInvites(Object sender, EventArgs e)
        {
            if (_invitedStudents.Count <= _availableSpace && _invitedStudents.Count > 0)
            {
                _client.AddStudentInvitesToEvent(_invitedStudents.ToArray(), _eventId);
            }
        }
    }
}