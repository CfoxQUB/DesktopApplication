using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using TimetablingClientApplication.TimetablingService;

namespace TimetablingClientApplication.Views.Database.Windows
{
    /// <summary>
    /// Interaction logic for CreateNewRoom.xaml
    /// </summary>
    public partial class CreateNewRoom 
    {
        //User Id maintained for room creation
        private readonly int _userId;
        //Clourse used for validation and reset of validation errors
        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush _normal = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE3E9EF"));
        //Web service functionality exposed through reference
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        //window initialized
        public CreateNewRoom(int userId)
        {
            //user Id maintained for room creation
            _userId = userId;
            //window created
            InitializeComponent();
            //Buildings list and room types list craeted
            var buildingsList = _client.ReturnBuildings();
            var roomtypes = _client.ReturnRoomTypes();

            //Check for buildings list is available
            if (buildingsList != null)
            {
                //Buildings list populated
                foreach (var b in buildingsList)
                {
                    BuildingSelect.Items.Add(b.BuildingName);
                }
            }
            else
            {
                //No buildings exist error popup opened
                NoBuildings.IsOpen = true;
                return;
            }

            //room types list check
            if (roomtypes != null)
            {
                //room types list poulated
                foreach (var r in roomtypes)
                {
                    RoomTypeSelect.Items.Add(r.RoomeTypeDescription);
                }
            }
            else
            {
                //Room types error popup opened
                NoRoomTypes.IsOpen = true;
            }
            
        }

        //Validation on fields to ensure fileds contents are valid
        public bool ValidationOnFields()
        {
            //fields rest to normal colour
            RoomName.BorderBrush = _normal;
            RoomDescription.BorderBrush = _normal;
            BuildingSelect.BorderBrush = _normal;
            RoomTypeSelect.BorderBrush = _normal;
            RoomCapacity.BorderBrush = _normal;

            //fields values selected for checking
            var roomName = RoomName.Text;
            var roomDescription = RoomDescription.Text;
            var roomCapacity = RoomCapacity.Text;
            var buildingId =_client.ReturnBuildingIdFromBuildingName(BuildingSelect.Text);
            var typeId = _client.ReturnRoomTypeIdFromTypeName(RoomTypeSelect.Text);

            //Check to ensure room with same name does not exist
            if (_client.CheckRoomExists(roomName))
            {
                //Validation alert that room already exists
                ValidationMessage.Foreground = _alert;
                ValidationMessage.Visibility = Visibility.Visible;
                ValidationMessage.Content = "Room creation failed. A room with the same name exists.";
                return false;
            }
            ValidationMessage.Visibility = Visibility.Hidden;
            if (!String.IsNullOrEmpty(roomName) && !String.IsNullOrEmpty(roomDescription) && !String.IsNullOrEmpty(roomCapacity) && buildingId != 0 && typeId != 0)
            {
                if (!ValidationOnCapacity())
                {
                    //Duration is invalid and validation message set to relfect this
                    ValidationMessage.Content = "Capacity must be a numeric value.";
                    ValidationMessage.Visibility = Visibility.Visible;
                    return false;
                }
              //validation on fields passed
              return true; 
            }
            //Validation on fields failed alert colour added to borders to indicate errors
            RoomName.BorderBrush = _alert;
            RoomDescription.BorderBrush = _alert;
            BuildingSelect.BorderBrush = _alert;
            RoomTypeSelect.BorderBrush = _alert;
            RoomCapacity.BorderBrush = _alert;
            //Validation message set and displayed
            ValidationMessage.Foreground = _alert;
            ValidationMessage.Visibility = Visibility.Visible;
            ValidationMessage.Content = "Please ensure all fields are completed.";
            return false;
        }

        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// Validation on capacity field for numeric value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ValidationOnCapacity(object sender, RoutedEventArgs e)
        {
            //room capacity value selected
            var temp = RoomCapacity.Text;
            //Regex for comparison selected
            var regex = new Regex("^[0-9]+$");

            //Comparison of field value against the regex created
            if (!regex.IsMatch(temp))
            {
                //validation failed alerts and message displayed
                ValidationMessage.Foreground = _alert;
                ValidationMessage.Visibility = Visibility.Visible;
                ValidationMessage.Content = "'Capacity' must be a numeric value.";
                return;
            }
            ValidationMessage.Visibility = Visibility.Hidden;
        }
        
        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// Validation on capacity field for numeric value
        /// </summary>
        public bool ValidationOnCapacity()
        {
            //room capacity value selected
            var temp = RoomCapacity.Text;
            //Regex for comparison selected
            var regex = new Regex("^[0-9]+$");

            //Comparison of field value against the regex created
            if (!regex.IsMatch(temp))
            {
                //Validation failed
                return false;
            }
            //Validation success
            return true;
        }

        //submission of new room values for validation adn creation
        public void SubmitNewRoom(object sender, RoutedEventArgs e)
        {
            if (ValidationOnFields())
            {
                //Validation passed
                var roomName = RoomName.Text;
                var roomDescription = RoomDescription.Text;
                var buildingId = _client.ReturnBuildingIdFromBuildingName(BuildingSelect.Text);
                var typeId = _client.ReturnRoomTypeIdFromTypeName(RoomTypeSelect.Text);
                var roomCapacity = 0;
                //capacity converted to integer
                if (!String.IsNullOrEmpty(RoomCapacity.Text))
                {
                    roomCapacity = Convert.ToInt32(RoomCapacity.Text);
                }
                //room submitted for creatino
                var result = _client.CreateNewRoom(buildingId, roomName, roomDescription, roomCapacity, typeId, _userId);
                if (result != 0)
                {
                    //roomcreatino successful
                    Success.IsOpen = true;
                }
                else
                {//room creation failed
                    ValidationMessage.Content = "Save Room failed. Please contact your system administrator";
                    ValidationMessage.Foreground = _alert;
                    ValidationMessage.Visibility = Visibility.Visible;
                }
            }
            
        }

        //close success popup function
        public void CloseSuccessPopup(object sender, RoutedEventArgs e)
        {
            Success.IsOpen = false;
            Close();
        }
        
        //close no building popup function
        public void CloseBuildingPopup(object sender, RoutedEventArgs e)
        {
            //Popup closed
            NoBuildings.IsOpen = false;
            //window closed
            Close();
        }

        //close no room types popup function
        public void CloseRoomTypesPopup(object sender, RoutedEventArgs e)
        {
            //Popup closed
            NoRoomTypes.IsOpen = false;
            //window closed
            Close();
        }
    }
}
