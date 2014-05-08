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
        //Module id and course Id maintained for module allocations
        private int _moduleId;
        private int _courseId;

        //bolean values used to generate page
        private readonly bool _pageRendered;
        private bool _buildingRendered;
        private bool _noModules;
        //List of students available and thsoe already linked to module
        private readonly ObservableCollection<Student> _availableStudents = new ObservableCollection<Student>(); 
        private readonly ObservableCollection<Student> _selectedStudents = new ObservableCollection<Student>();
        //Course list and module list used to populate select list
        private readonly ObservableCollection<String> _courseList = new ObservableCollection<String>();
        private readonly ObservableCollection<String> _moduleList = new ObservableCollection<String>();
        //Placeholder string for search field
        private const String DefaultSearchString = "Search Students Available . . . ";
        //Webservice referecne used to expose functionality
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        public ModuleManagementPage()
        {
            //boolean values for rendering set
            _pageRendered = false;
            _buildingRendered = false;
            
            InitializeComponent();
            //Default course Id
            _courseId = 0;
            //Courses returned
            var courses = _client.ReturnCourses();
            //Students associate with course returned
            var availableStudents = _client.ReturnCourseStudents(courses.First().CourseId);
            
            if (availableStudents != null)
            {
                foreach (var s in availableStudents)
                { //Availabel students list populated
                    _availableStudents.Add(s);
                }
            }
            AvailableStudents.ItemsSource = _availableStudents;

            if (courses != null)
            {
                _courseId = courses.First().CourseId;

                foreach (var c in courses)
                {  //course List populated
                    _courseList.Add(c.CourseName);
                }
                CourseList.ItemsSource = _courseList;
                CourseList.SelectedItem = _courseList.First();
                //modules of current seleted course returned
                var modules = _client.ReturnCourseModules(courses.First().CourseId);
                
                if (modules != null)
                {
                    _noModules = false;
                    foreach (var m in modules)
                    {   //Modules poulated into list
                        _moduleList.Add(m.ModuleName);
                    }
                    //page defaults setup
                    _moduleId = modules.First().ModuleId;
                    ModuleList.ItemsSource = _moduleList;
                    ModuleList.SelectedItem = _moduleList.First();
                }
                else
                {
                    //No modules, disable features
                    _noModules = true;
                    ModuleList.IsEnabled = false;
                    StudentCount.Content = "N/A";
                }
            }
            else
            {   // No courses disable features
                CourseList.IsEnabled = false;
                ModuleList.IsEnabled = false;
                StudentCount.Content = "N/A";
            }

            //Student module allocations returned
            var selectedStudents = _client.ReturnModuleStudents(_moduleId);
            if (selectedStudents != null)
            {
                foreach (var s in selectedStudents)
                {   //Student module allocations populated
                    _selectedStudents.Add(s);
                }
                SelectedStudents.ItemsSource = _selectedStudents;
            }
            //page default content set
            StudentCount.Content = _selectedStudents.Count();
            _buildingRendered = true;
            _pageRendered = true;
        }

        //removes placeholder text from search field
        public void SeachFieldOnFocus(Object sender, EventArgs e)
        {
            SearchField.Text = "";
        }
        //replaces placeholder text into search field
        public void SeachFieldLoseFocus(Object sender, EventArgs e)
        {
            SearchField.Text = DefaultSearchString;

            var returnedResults = _client.SearchCourseStudentsFunction("", _courseId);
            if (returnedResults == null)
            {   //return 0 students, list cleared
                _availableStudents.Clear();
            }
            else
            {
                //students list repopulated with new results
                _availableStudents.Clear();
                foreach (var r in returnedResults)
                {
                    _availableStudents.Add(r);
                }
            }
        }
        
        //Search field reuslt populated into returned student list
        public void SeachFieldTextChanged(Object sender, EventArgs e)
        {
            var searchString = SearchField.Text;
            if (searchString != DefaultSearchString)
            {
                //Search results returned
                var returnedResults = _client.SearchCourseStudentsFunction(searchString, _courseId);

                if (returnedResults == null)
                {
                    //no results list is cleared
                    _availableStudents.Clear();
                }
                else
                {
                    //Search results populated into list
                    _availableStudents.Clear();
                    foreach (var r in returnedResults)
                    {
                        _availableStudents.Add(r);
                    } 
                }
                
            }
        }

        //Course selection changed, module and students rerendered
        public void CourseSelectionChanged(Object sender, EventArgs e)
        {
            if (_pageRendered)
            {
                _buildingRendered = false;

                var courseSelected = CourseList.SelectedValue.ToString();
                _courseId = _client.ReturnCourseIdFromCourseName(courseSelected);

                //Course modules and students returned
                var returnStudents = _client.ReturnCourseStudents(_courseId);
                var moduleList = _client.ReturnCourseModules(_courseId);
                
                if (returnStudents != null)
                {   //Available Students repopulated
                    _availableStudents.Clear();
                    foreach (var s in returnStudents)
                    {   //Search results returned and populated
                        _availableStudents.Add(s);
                    }
                }

                if (moduleList != null)
                {
                    _noModules = false;
                    //Modules list repopulated
                    _moduleList.Clear();
                    foreach (var s in moduleList)
                    {   
                        _moduleList.Add(s.ModuleName);
                    }
                    ModuleList.SelectedItem = _moduleList.First();
                    
                    //Module students allocations returned
                    var selectStudents = _client.ReturnModuleStudents(moduleList.First().ModuleId);
                    _moduleId = moduleList.First().ModuleId;
                    if (selectStudents != null)
                    {
                        _selectedStudents.Clear();
                        foreach (var s in selectStudents)
                        {  //module students repopoulated
                            _selectedStudents.Add(s);
                        }
                        ModuleList.IsEnabled = true;
                        StudentCount.Content = _selectedStudents.Count();
                    }
                }
                else
                {   //no modules, module select disabled
                    _selectedStudents.Clear();
                    _noModules = true;
                    ModuleList.IsEnabled = false;
                    StudentCount.Content = "N/A";
                }
                _buildingRendered = true;
            }
        }
        
        //Module selection changed
        public void ModuleSelectionChanged(Object sender, EventArgs e)
        {
            if (_pageRendered && _buildingRendered)
            { 
                var moduleName = ModuleList.SelectedValue.ToString();
                //Module list for current course selection made
                var moduleList = _client.ReturnCourseModules(_courseId);
                if (moduleList != null)
                {
                    var selectedModule = moduleList.SingleOrDefault(x => x.ModuleName == moduleName);
                    if (selectedModule != null)
                    {
                        _moduleId = selectedModule.ModuleId;
                        
                        //Module students returned
                        var selectStudents = _client.ReturnModuleStudents(selectedModule.ModuleId);
                        if (selectStudents != null)
                        {
                            _selectedStudents.Clear();
                            foreach (var s in selectStudents)
                            {   //Module students repopulated
                                _selectedStudents.Add(s);
                            }
                        }
                        //Module selection enabled and studnet numbers introduced
                        ModuleList.IsEnabled = true;
                        StudentCount.Content = _selectedStudents.Count();
                    }
                 }
                else
                {   //Module select disabled
                    _selectedStudents.Clear();
                    ModuleList.IsEnabled = false;
                    StudentCount.Content = "N/A";
                }
            }
        }
        
        //add student to module
        public void AddStudentToModule(Object sender, EventArgs e)
        {
            if (!_noModules)
            {
                //Student seelected from availabel list and moved to the selected students list
                var selectedStudent = _availableStudents.SingleOrDefault(x => x == AvailableStudents.SelectedValue);
                if (selectedStudent != null)
                {
                    //Check to ensure student does not already exist in module students
                    var alreadySelected =
                        _selectedStudents.SingleOrDefault(x => x.StudentId == selectedStudent.StudentId);
                    if (alreadySelected == null)
                    {
                        //student added to list
                        _selectedStudents.Add(selectedStudent);
                    }
                }
                //student count updated
                StudentCount.Content = _selectedStudents.Count();
            }
        }
        
        //remove student from selected list
        public void RemoveStudentFromModule(Object sender, EventArgs e)
        {
            //Student selected and removed from selected list
            var selectedStudent = _selectedStudents.SingleOrDefault(x => x == SelectedStudents.SelectedValue);
            if (selectedStudent != null)
            {   //student removed
                _selectedStudents.Remove(selectedStudent);
            }
            //Student count updated
            StudentCount.Content = _selectedStudents.Count();
        }

        //Module student allocation saved
        public void SaveModuleStudents(Object sender, EventArgs e)
        {
            //new student module list created for submission
            var studentModuleList = new List<StudentModule>();

                foreach (var s in _selectedStudents)
                {   //new object created for each student selected
                    var newStudentModule = new StudentModule
                    {
                        StudentId = s.StudentId,
                        ModuleId = _moduleId
                    };
                    studentModuleList.Add(newStudentModule);
                }
              //save changes to module allocation
              _client.AddStudentsToModule(studentModuleList.ToArray(), _moduleId);
         }
    }
}
