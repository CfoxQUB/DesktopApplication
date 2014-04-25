using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TimetablingClientApplication.TimetablingService;

namespace TimetablingClientApplication.Views.Events.Pages
{
    /// <summary>
    /// Interaction logic for ModuleManagementPage.xaml
    /// </summary>
    public partial class ModuleManagementPage 
    {
        private int _moduleId;
        private int _courseId;

        private readonly bool _pageRendered;
        private bool _buildingRendered;
        private bool _noModules;
 
        private readonly ObservableCollection<Student> _availableStudents = new ObservableCollection<Student>(); 
        private readonly ObservableCollection<Student> _selectedStudents = new ObservableCollection<Student>();

        private readonly ObservableCollection<String> _courseList = new ObservableCollection<String>();
        private readonly ObservableCollection<String> _moduleList = new ObservableCollection<String>();

        private const String DefaultSearchString = "Search Students Available . . . ";

        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        public ModuleManagementPage()
        {
            _pageRendered = false;
            _buildingRendered = false;
            
            InitializeComponent();

            _courseId = 0;

            var courses = _client.ReturnCourses();
            var availableStudents = _client.ReturnCourseStudents(courses.First().CourseId);

            _courseId = courses.First().CourseId;

            if (availableStudents != null)
            {
                foreach (var s in availableStudents)
                {
                    _availableStudents.Add(s);
                }
            }
            AvailableStudents.ItemsSource = _availableStudents;

            if (courses != null)
            {
                _courseId = courses.First().CourseId;

                foreach (var c in courses)
                {
                    _courseList.Add(c.CourseName);
                }
                CourseList.ItemsSource = _courseList;
                CourseList.SelectedItem = _courseList.First();

                var modules = _client.ReturnCourseModules(courses.First().CourseId);
                _moduleId = courses.First().CourseId;

                if (modules != null)
                {
                    _noModules = false;
                    foreach (var m in modules)
                    {
                        _moduleList.Add(m.ModuleName);
                    }
                    ModuleList.ItemsSource = _moduleList;
                    ModuleList.SelectedItem = _moduleList.First();
                }
                else
                {
                    _noModules = true;
                    ModuleList.IsEnabled = false;
                    StudentCount.Content = "N/A";
                }
            }
            else
            {
                CourseList.IsEnabled = false;
                ModuleList.IsEnabled = false;
                StudentCount.Content = "N/A";
            }

            var selectedStudents = _client.ReturnModuleStudents(_moduleId);

            if (selectedStudents != null)
            {
                foreach (var s in selectedStudents)
                {
                    _selectedStudents.Add(s);
                }
                SelectedStudents.ItemsSource = _selectedStudents;
            }
            StudentCount.Content = _selectedStudents.Count();
            _buildingRendered = true;
            _pageRendered = true;
        }

        public void SeachFieldOnFocus(Object sender, EventArgs e)
        {
            SearchField.Text = "";
        }
        
        public void SeachFieldLoseFocus(Object sender, EventArgs e)
        {
            SearchField.Text = DefaultSearchString;

            var returnedResults = _client.SearchCourseStudentsFunction("", _courseId);

            if (returnedResults == null)
            {
                _availableStudents.Clear();
            }
            else
            {
                _availableStudents.Clear();

                foreach (var r in returnedResults)
                {
                    _availableStudents.Add(r);
                }
            }
        }
        
        public void SeachFieldTextChanged(Object sender, EventArgs e)
        {
            var searchString = SearchField.Text;

            if (searchString != DefaultSearchString)
            {
                var returnedResults = _client.SearchCourseStudentsFunction(searchString, _courseId);

                if (returnedResults == null)
                {
                    _availableStudents.Clear();
                }
                else
                {
                    _availableStudents.Clear();

                    foreach (var r in returnedResults)
                    {
                        _availableStudents.Add(r);
                    } 
                }
                
            }
        }

        public void CourseSelectionChanged(Object sender, EventArgs e)
        {
            if (_pageRendered)
            {
                _buildingRendered = false;

                var courseSelected = CourseList.SelectedValue.ToString();

                _courseId = _client.ReturnCourseIdFromCourseName(courseSelected);

                var returnStudents = _client.ReturnCourseStudents(_courseId);
                var moduleList = _client.ReturnCourseModules(_courseId);
                
                if (returnStudents != null)
                {
                    _availableStudents.Clear();
                    foreach (var s in returnStudents)
                    {
                        _availableStudents.Add(s);
                    }
                }

                if (moduleList != null)
                {
                    _noModules = false;
                    _moduleList.Clear();
                    foreach (var s in moduleList)
                    {
                        _moduleList.Add(s.ModuleName);
                    }
                    ModuleList.SelectedItem = _moduleList.First();
                    
                    var selectStudents = _client.ReturnModuleStudents(moduleList.First().ModuleId);
                    _moduleId = moduleList.First().ModuleId;
                    if (selectStudents != null)
                    {
                        _selectedStudents.Clear();
                        foreach (var s in selectStudents)
                        {
                            _selectedStudents.Add(s);
                        }
                        ModuleList.IsEnabled = true;
                        StudentCount.Content = _selectedStudents.Count();
                    }
                }
                else
                {
                    _selectedStudents.Clear();
                    _noModules = true;
                    ModuleList.IsEnabled = false;
                    StudentCount.Content = "N/A";
                }
                _buildingRendered = true;
            }
        }
        
        public void ModuleSelectionChanged(Object sender, EventArgs e)
        {
            if (_pageRendered && _buildingRendered)
            { 
                var moduleName = ModuleList.SelectedValue.ToString();

                var moduleList = _client.ReturnCourseModules(_courseId);
                if (moduleList != null)
                {
                    var selectedModule = moduleList.SingleOrDefault(x => x.ModuleName == moduleName);

                    if (selectedModule != null)
                    {
                        _moduleId = selectedModule.ModuleId;
                        var selectStudents = _client.ReturnModuleStudents(selectedModule.ModuleId);

                        if (selectStudents != null)
                        {
                            _selectedStudents.Clear();
                            foreach (var s in selectStudents)
                            {
                                _selectedStudents.Add(s);
                            }
                        }
                        ModuleList.IsEnabled = true;
                        StudentCount.Content = _selectedStudents.Count();
                    }
                 }
                else
                {
                    _selectedStudents.Clear();
                    ModuleList.IsEnabled = false;
                    StudentCount.Content = "N/A";
                }
            }
        }
        
        public void AddStudentToModule(Object sender, EventArgs e)
        {
            if (!_noModules)
            {
                var selectedStudent = _availableStudents.SingleOrDefault(x => x == AvailableStudents.SelectedValue);

                if (selectedStudent != null)
                {
                    var alreadySelected =
                        _selectedStudents.SingleOrDefault(x => x.StudentId == selectedStudent.StudentId);
                    if (alreadySelected == null)
                    {
                        _selectedStudents.Add(selectedStudent);
                    }

                }

                StudentCount.Content = _selectedStudents.Count();
            }
        }
        
        public void RemoveStudentFromModule(Object sender, EventArgs e)
        {
            var selectedStudent = _selectedStudents.SingleOrDefault(x => x == SelectedStudents.SelectedValue);

            if (selectedStudent != null)
            {
                _selectedStudents.Remove(selectedStudent);
            }

            StudentCount.Content = _selectedStudents.Count();
        }

        public void SaveModuleStudents(Object sender, EventArgs e)
        {
            var studentModuleList = new List<StudentModule>();

                foreach (var s in _selectedStudents)
                {
                    var newStudentModule = new StudentModule
                    {
                        StudentId = s.StudentId,
                        ModuleId = _moduleId
                    };
                    studentModuleList.Add(newStudentModule);
                }

              _client.AddStudentsToModule(studentModuleList.ToArray(), _moduleId);
              
            
        }

    }
}
