using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using TimetablingClientApplication.TimetablingService;
using TimetablingClientApplication.Views.Database.Pages;

namespace TimetablingClientApplication.Views.Database.Windows
{
    /// <summary>
    /// Interaction logic for AddNewBuilding.xaml
    /// </summary>
    public partial class CreateNewModule
    {
        //User id and navigation service of frame maintained for redirect and module creation
        private readonly int _userId;
        private readonly NavigationService _navigation;
        //Validation error color aplaied to validation message text when error occurs
        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        //Webservice functionality exposed through service reference
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        //Window initailaized
        public CreateNewModule(int userId, NavigationService navigation)
        {
            //user Id and navigatino service maintained
            _userId = userId;
            _navigation = navigation;
            //Window opened
            InitializeComponent();
        }

        //validation on fields to ensure values valid
        public bool ValidationOnFields()
        {
            //Text fields values selceted
            var moduleName = ModuleName.Text;
            var moduleDesc = Description.Text;

            //check to ensure module does not already exist
            if (_client.CheckModuleExists(moduleName))
            {   //Validation message populated to relfect error
                ValidationMessage.Content = "Module already exists";
                ValidationMessage.Foreground = _alert;
                ValidationMessage.Visibility = Visibility.Visible;
                return false;
            }

            //Validation on field contents
            if (!String.IsNullOrEmpty(moduleName) && !String.IsNullOrEmpty(moduleDesc))
            {   //validation passed
                return true;
            }
            //Validation failed
            ModuleName.BorderBrush = _alert;
            Description.BorderBrush = _alert;
            //Validation message reflects error
            ValidationMessage.Content = "Save changes failed. Please ensure all the fields are completed.";
            ValidationMessage.Foreground = _alert;
            ValidationMessage.Visibility = Visibility.Visible;

            return false;
        }
      
        //Submission on new module to database
        public void SubmitNewModule(object sender, RoutedEventArgs e)
        {
            //validation on fields check
            if (ValidationOnFields())
            {
                //Validation passed and fields values selected
                var moduleName = ModuleName.Text;
                var moduleDesc = Description.Text;
                //Module creation result maintained
                var result = _client.CreateModule(moduleName, moduleDesc, 0, _userId);
                if (result != 0)
                {
                    Success.IsOpen = true;
                }
                else
                {
                    //Error in database
                    ValidationMessage.Content = "Save module failed. Please contact your system administrator";
                    ValidationMessage.Foreground = _alert;
                    ValidationMessage.Visibility = Visibility.Visible; 
                }
            }

        }
        //success popup close function with redirect 
        public void CloseSuccessPopup(object sender, EventArgs e)
        {
            //close popup
            Success.IsOpen = false;
            //_navigation redirect
            _navigation.Navigate(new CourseModuleManagement(_userId, _navigation));
            //Close window
            Close();
        }
    }
}
