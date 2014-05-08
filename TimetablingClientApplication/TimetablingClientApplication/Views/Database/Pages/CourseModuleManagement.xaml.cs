using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Navigation;
using TimetablingClientApplication.TimetablingService;
using TimetablingClientApplication.Views.Database.Windows;

namespace TimetablingClientApplication.Views.Database.Pages
{
    /// <summary>
    /// Interaction logic for CourseModuleManagement.xaml
    /// </summary>
    public partial class CourseModuleManagement 
    {
        //Selceted course Id maintained for submission of new modules
        private int _courseId;

        //Page rendered boolean for population of modules
        private readonly bool _pageRendered;

        //Observable collections bound to lists on page to display courses modules and availabale modules
        private readonly ObservableCollection<Module> _availableModules = new ObservableCollection<Module>(); 
        private readonly ObservableCollection<Module> _selectedModules = new ObservableCollection<Module>();

        private int _userId;
        private NavigationService _navigation;

        //Course list added to the dropdown to select course
        private readonly ObservableCollection<String> _courseList = new ObservableCollection<String>();
        
        //Constant string used to reset search field placeholder
        private const String DefaultSearchString = "Search Modules Available . . . ";

        //Timetabling client used to expose web service functionality
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        public CourseModuleManagement(int userId, NavigationService navigation)
        {
            _pageRendered = false;
            _userId = userId;
            _navigation = navigation;

            InitializeComponent();

            //default course Id 0 does not exists in database
            _courseId = 0;

            //returning full list of courses and modules
            var courses = _client.ReturnCourses();
            var availableModules = _client.ReturnModules();

            //population of modules available for addition to course
            if (availableModules != null)
            {
                foreach (var m in availableModules)
                {
                    _availableModules.Add(m);
                }
            }
            AvailableModules.ItemsSource = _availableModules;
            SelectedModules.ItemsSource = _selectedModules;

            //Population of courses list seen on page
            if (courses != null)
            {
                _courseId = courses.First().CourseId;

                foreach (var c in courses)
                {
                    _courseList.Add(c.CourseName);
                }
                CourseList.ItemsSource = _courseList;
                CourseList.SelectedItem = _courseList.First();

                //Population of modules that appear in the selected courses modules
                var modules = _client.ReturnCourseModules(courses.First().CourseId);
                if (modules != null)
                {
                    foreach (var m in modules)
                    {
                        _selectedModules.Add(m);
                    }
                 }
                ModuleCount.Content = _selectedModules.Count();
                _pageRendered = true;
            }
        }

        //Removes placeholder text in search field
        public void SeachFieldOnFocus(Object sender, EventArgs e)
        {
            SearchField.Text = "";
        }
        
        //Replaces placeholder in search field and repopulates 
        // the default values for modules available
        public void SeachFieldLoseFocus(Object sender, EventArgs e)
        {
            SearchField.Text = DefaultSearchString;

            var returnedResults = _client.SearchModulesFunction("");

            if (returnedResults == null)
            {
                _availableModules.Clear();
            }
            else
            {
                //Reset the modules that are available
                _availableModules.Clear();
                foreach (var r in returnedResults)
                {
                    _availableModules.Add(r);
                }
            }
        }
        
        //returns the modules that match the value passed into the search field
        public void SeachFieldTextChanged(Object sender, EventArgs e)
        {
            var searchString = SearchField.Text;

            if (searchString != DefaultSearchString)
            {
                //Modules returned and populated into page
                var returnedResults = _client.SearchModulesFunction(searchString);
                if (returnedResults == null)
                {
                    //No results
                    _availableModules.Clear();
                }
                else
                {
                    //Results returned
                    _availableModules.Clear();
                    foreach (var r in returnedResults)
                    {
                        _availableModules.Add(r);
                    } 
                }
                
            }
        }

        //Repopuates the modules that are associated with a course
        //depending on the selection made in the course selection
        public void CourseSelectionChanged(Object sender, EventArgs e)
        {
            if (_pageRendered)
            {
                //details of the course selected returned
                var courseSelected = CourseList.SelectedValue.ToString();
                _courseId = _client.ReturnCourseIdFromCourseName(courseSelected);

                //Return modules of selected course
                var moduleList = _client.ReturnCourseModules(_courseId);
                if (moduleList != null)
                {
                    //course modules populated
                    _selectedModules.Clear();
                    foreach (var s in moduleList)
                    {
                        _selectedModules.Add(s);
                    }

                }
                //Module count updated
                ModuleCount.Content = _selectedModules.Count();
            }
        }
        
        //Addition of modules that are availabel to the course that has been selected
        public void AddModuleToCourse(Object sender, EventArgs e)
        {
            var selectedModule = _availableModules.SingleOrDefault(x => x == AvailableModules.SelectedValue);

            if (selectedModule != null)
            {
                //Check to ensure module has not already been added
                var alreadySelected = _selectedModules.SingleOrDefault(x => x.ModuleId == selectedModule.ModuleId);
                
                //If modules does not already exist it is added to the course modules
                if (alreadySelected == null)
                {
                    _selectedModules.Add(selectedModule);   
                }
             }
            //Module count updated
            ModuleCount.Content = _selectedModules.Count();
        }

        //Module removed from the selected course
        public void RemoveModuleFromCourse(Object sender, EventArgs e)
        {
            //Selected module
            var selectedModule = _selectedModules.SingleOrDefault(x => x == SelectedModules.SelectedValue);

            //So long as the module exists in the selected list removed from course module list
            if (selectedModule != null)
            {
                _selectedModules.Remove(selectedModule);
            }
            //module count updated
            ModuleCount.Content = _selectedModules.Count();
        }

        //Save function which passes across the modules associated with
        //the course to the appropriate webservice method
        public void SaveCourseModules(Object sender, EventArgs e)
        {
            var courseModuleList = new List<Module>();
              //creation of module list passed to teh webservice
              foreach (var c in _selectedModules)
                {
                    courseModuleList.Add(c);
                }
            
            //Modules saved to associated course
            _client.AddModulesToCourse(courseModuleList.ToArray(), _courseId);
         }
        
        //Opens new window to create a new module
        public void CreateNewModule(Object sender, EventArgs e)
        {
            var newModule = new CreateNewModule(_userId, _navigation);
            newModule.Show();
        }

    }
}

