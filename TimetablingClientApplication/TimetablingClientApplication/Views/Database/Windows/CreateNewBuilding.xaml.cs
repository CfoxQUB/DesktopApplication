using System;
using System.Text.RegularExpressions;
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
    public partial class CreateNewBuilding 
    {
        //User Id maintained for building creation
        private readonly int _userId;
        //Validation colour to indicate error
        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        //Webservice functionality exposed through service reference
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        //Navigation service of frmae in main window to redirect from window
        private readonly NavigationService _navigation;

        //window initialized
        public CreateNewBuilding(int userId, NavigationService navigationService)
        {
            //user Id and navigatino service maintained
            _navigation = navigationService;
            _userId = userId;
            //window created
            InitializeComponent();
        }

        //Validation on the fields in order for building creation
        public bool ValidationOnFields()
        {
            var buildingName = BuildingName.Text;
            var buildingNumber = BuildingNumber.Text;
            var addressLine1 = AddressLine1.Text;
            var addressLine2 = AddressLine2.Text;
            var city = City.Text;
            var postcode = Postcode.Text;
            //Check to see if building exists with same name
            if (_client.CheckBuildingExists(buildingName))
            {
                //validaiton message to indicate building already exists
                ValidationMessage.Content = "Building already exists";
                ValidationMessage.Foreground = _alert;
                ValidationMessage.Visibility = Visibility.Visible;
                return false;
            }
            //as long as all fields are completed validaiton passes
            if (!String.IsNullOrEmpty(buildingName) && !String.IsNullOrEmpty(buildingNumber) && !String.IsNullOrEmpty(addressLine1) && !String.IsNullOrEmpty(addressLine2) && !String.IsNullOrEmpty(city) && !String.IsNullOrEmpty(postcode) && !String.IsNullOrEmpty(buildingNumber) )
            {
                if (!BuildingNumberCheck())
                {
                    //Building number is invalid and validation message set to relfect this
                    ValidationMessage.Content = "Building number must be a numeric value.";
                    ValidationMessage.Visibility = Visibility.Visible;
                    return false;
                }
                return true;
            }
            //If validation does not pass all the fields are set to _alert colour to indicate error
            BuildingName.BorderBrush = _alert;
            BuildingNumber.BorderBrush = _alert;
            AddressLine1.BorderBrush = _alert;
            AddressLine2.BorderBrush = _alert;
            City.BorderBrush = _alert;
            Postcode.BorderBrush = _alert;
            //validation message displayed
            ValidationMessage.Content = "Building Creation failed. Please ensure all the fields are completed.";
            ValidationMessage.Foreground = _alert;
            ValidationMessage.Visibility = Visibility.Visible;

            return false;
        }
      
        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// Validatino on building number which must be a numeric value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ValidationOnBuildingNumber(object sender, RoutedEventArgs e)
        {
            //building number text field seleted
            var temp = BuildingNumber.Text;
            //new regex expression created for comparison
            var regex = new Regex("^[0-9]+$");

            //check against the regex that has been created
            if (!regex.IsMatch(temp))
            {
                //if regex comparison fails the validation reflects failure
                ValidationMessage.Content = "'Building Number' must be a numeric value.";
                ValidationMessage.Foreground = _alert;
                ValidationMessage.Visibility = Visibility.Visible;
            }
            ValidationMessage.Visibility = Visibility.Hidden;
        } 
        
        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// Check used in the validation method rather than on change method used above
        /// </summary>
        public bool BuildingNumberCheck()
        {   //building number text field seleted
            var temp = BuildingNumber.Text;
            //new regex expression created for comparison
            var regex = new Regex("^[0-9]+$");
            //check against the regex that has been created
            if (!regex.IsMatch(temp))
            {
                //comparison failed
                return false;
            }
            //comparison valid
            return true;
        } 

        //Creation of new building on click function
        public void SubmitNewBuilding(object sender, RoutedEventArgs e)
        {
            //Validation on fields required for creation of building
            if (ValidationOnFields())
            {
                //validation passed the fileds values are selected
                var buildingName = BuildingName.Text;
                var buildingNumber = Convert.ToInt32(BuildingNumber.Text);
                var addressLine1 = AddressLine1.Text;
                var addressLine2 = AddressLine2.Text;
                var city = City.Text;
                var postcode = Postcode.Text;
                //Module creation result maintained
                var result = _client.CreateNewBuilding(buildingName, buildingNumber, addressLine1, addressLine2, postcode, city, _userId);
                if (result != 0)
                {
                    Success.IsOpen = true;
                }
                else
                {
                    ValidationMessage.Content = "Save Building failed. Please contact your system administrator";
                    ValidationMessage.Foreground = _alert;
                    ValidationMessage.Visibility = Visibility.Visible;
                }
            }

        }

        //Close success popup on successful building creation
        public void CloseSuccessPopup(object sender, RoutedEventArgs e)
        {
            //close popup
            Success.IsOpen = false;
            //NavigationService redirect to frame course management which is refreshed
            _navigation.Navigate(new BuildingManagement(_userId, _navigation));
            Close();
        }
    }
}
