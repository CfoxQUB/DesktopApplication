using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using TimetablingClientApplication.TimetablingService;

namespace TimetablingClientApplication.Views.Database.Windows
{
    /// <summary>
    /// Interaction logic for EditBuilding.xaml
    /// </summary>
    public partial class EditBuilding 
    {
        //UserId and building Id maintained for editing
        private readonly int _userId;
        private readonly int _buildingId;
        //Colour for errors on text fields
        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        //Webservice functionality exposed
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        //Window initialized
        public EditBuilding(int userId, int buildingId)
        {
            //User Id and building Id maintained
            _userId = userId;
            _buildingId = buildingId;
            //Window Initialized
            InitializeComponent();
            //Building detail returned
            var building = _client.ReturnBuildingDetail(buildingId);
            if (building != null)
            {   //Building information poulated into page
                BuildingName.Text = building.BuildingName;
                AddressLine1.Text = building.AddressLine1;
                AddressLine2.Text = building.AddressLine2;
                BuildingNumber.Text = building.BuildingNumber.ToString("D");
                City.Text = building.City;
                Postcode.Text = building.PostalCode;
            }

        }

        //validation on page fields
        public bool ValidationOnFields()
        {
            //Text field values selected
            var buildingName = BuildingName.Text;
            var buildingNumber = BuildingNumber.Text;
            var addressLine1 = AddressLine1.Text;
            var addressLine2 = AddressLine2.Text;
            var city = City.Text;
            var postcode = Postcode.Text;
            //Checks amade to ensure field contents are valid , not null and in correct format
            if (!String.IsNullOrEmpty(buildingName) && !String.IsNullOrEmpty(buildingNumber) && !String.IsNullOrEmpty(addressLine1) && !String.IsNullOrEmpty(addressLine2) && !String.IsNullOrEmpty(city) && !String.IsNullOrEmpty(postcode) && BuildingNumberCheck())
            {
                //check to ensure match is success
                if (!BuildingNumberCheck())
                {   //Building Number is invalid and validation message set to relfect this
                    ValidationMessage.Content = "Building Number must be a numeric value";
                    ValidationMessage.Visibility = Visibility.Visible;
                    return false;
                }
                //validation passed
                return true;
            }
            //Validation failed
            BuildingName.BorderBrush = _alert;
            BuildingNumber.BorderBrush = _alert;
            AddressLine1.BorderBrush = _alert;
            AddressLine2.BorderBrush = _alert;
            City.BorderBrush = _alert;
            Postcode.BorderBrush = _alert;
            //Validation message updated
            ValidationMessage.Content = "Save changes failed. Empty Fields present";
            ValidationMessage.Foreground = _alert;
            ValidationMessage.Visibility = Visibility.Visible;

            return false;
        }

        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// Validation of building number to ensure numerice field (for field on change)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ValidationOnBuildingNumber(object sender, RoutedEventArgs e)
        {   //Building number value selected
            var temp = BuildingNumber.Text;
            //regex expression for comparison created
            var regex = new Regex("^[0-9]+$");
            //check of value against regex expression
            if (!regex.IsMatch(temp))
            {
                //Validation on building number failed
                ValidationMessage.Content = "'Building Number' must be a numeric value.";
                ValidationMessage.Foreground = _alert;
                ValidationMessage.Visibility = Visibility.Visible;
            }
            //Validation passed
            ValidationMessage.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// Validation of building number to ensure numerice field
        /// </summary>
        public bool BuildingNumberCheck()
        {   //Building number value selected
            var temp = BuildingNumber.Text;
            //regex expression for comparison created
            var regex = new Regex("^[0-9]+$");
            //check of value against regex expression
            if (!regex.IsMatch(temp))
            {
                //Validation failed
                return false;
            }
            //Validation passed
            return true;
        } 

        //Building changes submitted to servcie
        public void SaveBuilding(object sender, RoutedEventArgs e)
        {
            //validation on field values
            if (ValidationOnFields())
            {   //validation passed
                //Values of building selected
                var buildingName = BuildingName.Text;
                var buildingNumber = Convert.ToInt32(BuildingNumber.Text);
                var addressLine1 = AddressLine1.Text;
                var addressLine2 = AddressLine2.Text;
                var city = City.Text;
                var postcode = Postcode.Text;

                var result = _client.EditBuilding(_buildingId, buildingName, buildingNumber, addressLine1, addressLine2, city, postcode, _userId);
                if (result)
                {   //Changes saved
                    Success.IsOpen = true;
                }
                else
                {
                    //Validation on building changes failed
                    ValidationMessage.Content = "Save Changes failed. Please contact System Administrator";
                    ValidationMessage.Foreground = _alert;
                    ValidationMessage.Visibility = Visibility.Visible;
                }
            }
            
        }
        //Close success Popup
        public void CloseSuccessPopup(object sender, RoutedEventArgs e)
        {   //Close Popop
            Success.IsOpen = false;
            //Clsoe Window
            Close();
        }
    }
}
