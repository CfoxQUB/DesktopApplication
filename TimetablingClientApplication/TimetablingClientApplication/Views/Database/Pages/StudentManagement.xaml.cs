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
        //Web service client used to expose web service functionality
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        //List of students used to poulate the students listed view and search results
        private readonly ObservableCollection<Student> _studentCollectionList = new ObservableCollection<Student>();

        //Default searhc string used as palceholder and comparater
        private const string DefaultSearchString = "Search for Students . . . ";

        //Navigation service used to refresh and navigate the frame of the master window
        private readonly NavigationService _navigationService;
        
        //Storde user id
        private readonly int _userId;

        public StudentManagement(int userId, NavigationService navigationService)
        {
            //User Id and navigation service stored
            _userId = userId;
            _navigationService = navigationService;

            //Page Initialized
            InitializeComponent();

            //Students returned from database
            var studentList = _client.ReturnStudents();

            //Check to ensure students exist
            if (studentList != null)
            {
                //Population of students list
                foreach (var s in studentList)
                {
                    _studentCollectionList.Add(s);
                }
            }
            StudentList.ItemsSource = _studentCollectionList;
            //Default page information set
            Title.Text = "No selection made.";
            Forename.Text = "None";
            Surname.Text = "None";
            Email.Text = "None";
            Course.Text = "None";
            Year.Text = "None";

        }
        //Placeholder removed from search field
        private void SearchField_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchStudent.Text = "";
        }

        //Placeholder reset in search field
        private void SearchField_LoseFocus(object sender, RoutedEventArgs e)
        {
            SearchStudent.Text = DefaultSearchString;
        }

        //Search results returned from database
        private void ReturnSearchResults(object sender, RoutedEventArgs e)
        {
            //Search field text selected
            if (SearchStudent.Text != DefaultSearchString)
                {
                //results returned
                    var results = _client.SearchStudentFunction(SearchStudent.Text);
                //results displayed    
                StudentList.ItemsSource = results;
                }
        }
        //delete student popup displayed
        private void DeleteStudentPopup(object sender, RoutedEventArgs e)
        {
            DeleteStudent.IsOpen = true;
        }
        
        //Close delete student popup function
        private void CloseDeletePopup(object sender, RoutedEventArgs e)
        {
            DeleteStudent.IsOpen = false;
        }

        //Confirmdelete button within popup function
        private void ConfirmDeleteButtonClicked(object sender, RoutedEventArgs e)
        {   //student selected from list
            var selectedStudent = (Student)StudentList.SelectedItem;
            //Check to ensure student selction valid
            if (selectedStudent != null)
            {
                //Deletion of student and students list refreshed
                _client.DeleteStudent(selectedStudent.StudentId);
                var noResults = _client.ReturnStudents();
                StudentList.ItemsSource = noResults;
            }
            //Popup closed and default page contents refreshed
            DeleteStudent.IsOpen = false;
            Title.Text = "No selection made.";
            Forename.Text = "None";
            Surname.Text = "None";
            Email.Text = "None";
            Course.Text = "None";
            Year.Text = "None";
        }

        //Add student window opened
        public void AddNewStudent(object sender, RoutedEventArgs e)
        {
            var newStudent = new CreateNewStudent(_userId, _navigationService);
            newStudent.Show();
        }

        //Edit student window opened with selected student
        public void EditStudent(object sender, RoutedEventArgs e)
        {
            //student selected
            var studentId = (Student)StudentList.SelectedItem;
            if (studentId != null)
            {
                var editStudent = new EditStudent(_userId, studentId.StudentId, _navigationService);
                editStudent.Show();
            }

        }

        //Selection of studnet in list changed
        public void NewStudentSelected(object sender, RoutedEventArgs e)
        {
            //student slection changed
            var selectedStudent = (Student)StudentList.SelectedItem;
            if (selectedStudent != null)
            {
                //Display details of student refreshed
                Title.Text = selectedStudent.StudentTitle;
                Forename.Text = selectedStudent.StudentForename;
                Surname.Text = selectedStudent.StudentSurname;
                Email.Text = selectedStudent.StudentEmail;
                Year.Text = selectedStudent.Year.ToString("D");

                //course information returned and displayed
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
