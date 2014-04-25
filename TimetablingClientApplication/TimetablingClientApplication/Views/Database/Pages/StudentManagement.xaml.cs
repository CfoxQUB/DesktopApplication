using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Navigation;
using TimetablingClientApplication.TimetablingService;
using TimetablingClientApplication.Views.Database.Windows;

namespace TimetablingClientApplication.Views.Database.Pages
{
    /// <summary>
    /// Interaction logic for StudentManagement.xaml
    /// </summary>
    public partial class StudentManagement 
    {
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        private readonly ObservableCollection<Student> _studentCollectionList = new ObservableCollection<Student>();

        private const string DefaultSearchString = "Search for Students . . . ";

        private readonly NavigationService _navigationService;
        
        private readonly int _userId;

        public StudentManagement(int userId, NavigationService navigationService)
        {
            _userId = userId;
            _navigationService = navigationService;

            InitializeComponent();

            var studentList = _client.ReturnStudents();

            if (studentList != null)
            {
                foreach (var s in studentList)
                {
                    _studentCollectionList.Add(s);
                }
            }

            StudentList.ItemsSource = _studentCollectionList;
            Title.Text = "No selection made.";
            Forename.Text = "None";
            Surname.Text = "None";
            Email.Text = "None";
            Course.Text = "None";
            Year.Text = "None";

        }

        private void SearchField_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchStudent.Text = "";
        }

        private void SearchField_LoseFocus(object sender, RoutedEventArgs e)
        {
            SearchStudent.Text = DefaultSearchString;
        }

        private void ReturnSearchResults(object sender, RoutedEventArgs e)
        {

            if (SearchStudent.Text != DefaultSearchString)
                {
                    var results = _client.SearchStudentFunction(SearchStudent.Text);
                    StudentList.ItemsSource = results;
                }
        }

        private void DeleteStudentPopup(object sender, RoutedEventArgs e)
        {
            DeleteStudent.IsOpen = true;
        }
        
        private void CloseDeletePopup(object sender, RoutedEventArgs e)
        {
            DeleteStudent.IsOpen = false;
        }

        private void ConfirmDeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            var selectedStudent = (Student)StudentList.SelectedItem;

            if (selectedStudent != null)
            {
                _client.DeleteStudent(selectedStudent.StudentId);

                var noResults = _client.ReturnStudents();

                StudentList.ItemsSource = noResults;
            }
            DeleteStudent.IsOpen = false;
            Title.Text = "No selection made.";
            Forename.Text = "None";
            Surname.Text = "None";
            Email.Text = "None";
            Course.Text = "None";
            Year.Text = "None";
        }

        public void AddNewStudent(object sender, RoutedEventArgs e)
        {
            var newStudent = new CreateNewStudent(_userId, _navigationService);
            newStudent.Show();
        }

        public void EditStudent(object sender, RoutedEventArgs e)
        {
            var studentId = (Student)StudentList.SelectedItem;
            if (studentId != null)
            {
                var editStudent = new EditStudent(_userId, studentId.StudentId, _navigationService);
                editStudent.Show();
            }

        }

        public void NewStudentSelected(object sender, RoutedEventArgs e)
        {
            var selectedStudent = (Student)StudentList.SelectedItem;
            if (selectedStudent != null)
            {
                Title.Text = selectedStudent.StudentTitle;
                Forename.Text = selectedStudent.StudentForename;
                Surname.Text = selectedStudent.StudentSurname;
                Email.Text = selectedStudent.StudentEmail;
                Year.Text = selectedStudent.Year.ToString("D");

                var studentInfo = _client.ReturnCourseDetail(selectedStudent.Course);
                Course.Text = "None";

                if (selectedStudent.Course != 0)
                {
                    Course.Text = studentInfo.CourseName;
                }
                Year.Text = selectedStudent.Year.ToString("D");
            }
        }
    }
}
