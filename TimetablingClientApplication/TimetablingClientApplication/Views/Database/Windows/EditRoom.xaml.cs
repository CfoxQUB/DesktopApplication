using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using TimetablingClientApplication.TimetablingService;

namespace TimetablingClientApplication.Views.Database.Windows
{
    /// <summary>
    /// Interaction logic for EditRoom.xaml
    /// </summary>
    public partial class EditRoom 
    {
        //User Id and roomId maintained for editing purposes
        private readonly int _userId;
        private readonly int _roomId;

        //colours used to relfect errors in page and reset of validation
        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush _normal = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE3E9EF"));
        //Webservice functionality exposed through service reference
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        //Window created
        public EditRoom(int userId, int roomId)
        {
            //User Id and roomId maintained for editing purposes
            _userId = userId;
            _roomId = roomId;
            //Window initialzed
            InitializeComponent();
            //room details returned from web service
            var room = _client.ReturnRoomDetail(roomId);
            if (room != null)
            {   //room details populated into page
                RoomName.Text = room.RoomName;
                RoomDescription.Text = room.RoomDescription;
                RoomCapacity.Text = room.Capacity.ToString("D");
                RoomTypeSelect.Text = room.RoomName;

                var buildingsList = _client.ReturnBuildings();
                var roomtypes = _client.ReturnRoomTypes();
                //buildings select populated
                if (buildingsList != null)
                {
                    foreach (var b in buildingsList)
                    {
                        BuildingSelect.Items.Add(b.BuildingName);
                    }

                    var selectedbuilding = buildingsList.SingleOrDefault(x=>x.BuildingId == room.Building);
                    if (selectedbuilding != null)
                    {
                        BuildingSelect.SelectedItem = selectedbuilding.BuildingName;
                    }
                    else
                    {
                        NoBuildings.IsOpen = true;
                        return;
                    }
                    
                }
                else
                {
                    //Popup opened
                    NoBuildings.IsOpen = true;
                    return;
                }
                //room types populated
                if (roomtypes != null)
                {
                    foreach (var r in roomtypes)
                    {
                        RoomTypeSelect.Items.Add(r.RoomeTypeDescription);
                    }

                    var selectedType = roomtypes.SingleOrDefault(x => x.RoomTypeId == room.RoomType);

                    if (selectedType != null)
                    {
                        RoomTypeSelect.SelectedItem = selectedType.RoomeTypeDescription;
                        return;
                    }
                    NoRoomTypes.IsOpen = true;
                }
                else
                {
                    NoRoomTypes.IsOpen = true;
                }
            }

        }

        //validation on page fields
        public bool ValidationOnFields()
        {
            //Validation colors reset
            RoomName.BorderBrush = _normal;
            RoomDescription.BorderBrush = _normal;
            BuildingSelect.BorderBrush = _normal;
            RoomTypeSelect.BorderBrush = _normal;
            RoomCapacity.BorderBrush = _normal;

            //values selected from window
            var roomName = RoomName.Text;
            var roomDescription = RoomDescription.Text;
            var buildingId = _client.ReturnBuildingIdFromBuildingName(BuildingSelect.Text);
            var typeId = _client.ReturnRoomTypeIdFromTypeName(RoomTypeSelect.Text);

            //Check to ensure values in fields are of correct format and not null
            if (!String.IsNullOrEmpty(roomName) && !String.IsNullOrEmpty(roomDescription) && ValidationOnCapacity() && buildingId != 0 && typeId != 0)
            {
                if (!ValidationOnCapacity())
                {
                    ValidationMessage.Content = "'Capacity' must be a numeric value.";
                    ValidationMessage.Visibility = Visibility.Visible;
                    return false;
                }
                //validation passed
                ValidationMessage.Visibility = Visibility.Hidden;
                return true;
            }
            //validation failed
            //Alert set to all fields to reflect error
            RoomName.BorderBrush = _alert;
            RoomDescription.BorderBrush = _alert;
            BuildingSelect.BorderBrush = _alert;
            RoomTypeSelect.BorderBrush = _alert;
            RoomCapacity.BorderBrush = _alert;
            //Validation error set to indicate error
            ValidationMessage.Foreground = _alert;
            ValidationMessage.Visibility = Visibility.Visible;
            ValidationMessage.Content = "Please ensure all fields are completed.";
            return false;
        }

        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// Check to ensure capacity value is a numeric value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ValidationOnCapacity(object sender, RoutedEventArgs e)
        {   //capacity value selected
            var temp = RoomCapacity.Text;
            //new regex expression cretde for comparison
            var regex = new Regex("^[0-9]+$");
            //comparison to value and regex expression
            if (!regex.IsMatch(temp))
            {//validation failed
                ValidationMessage.Content = "'Capacity' must be a numeric value.";
                ValidationMessage.Visibility = Visibility.Visible;
            }
            //Validation passed
            ValidationMessage.Visibility = Visibility.Hidden;
        }
        
        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// Check to ensure capacity value is a numeric value
        /// </summary>
        public bool ValidationOnCapacity()
        {   //capacity value selected
            var temp = RoomCapacity.Text;
            //new regex expression cretde for comparison
            var regex = new Regex("^[0-9]+$");
            //comparison to value and regex expression
            if (!regex.IsMatch(temp))
            {//validation failed
                return false;
            }
            //Validation passed
            return true;
        } 
        
        //Changes passed to the service for saving
        public void SaveRoomChanges(object sender, RoutedEventArgs e)
        {   
            //Validation on fields values
            if (ValidationOnFields())
            {
                //Values in page selected
                var roomName = RoomName.Text;
                var roomDescription = RoomDescription.Text;
                var buildingId = _client.ReturnBuildingIdFromBuildingName(BuildingSelect.Text);
                var typeId = _client.ReturnRoomTypeIdFromTypeName(RoomTypeSelect.Text);
                var roomCapacity = Convert.ToInt32(RoomCapacity.Text);

                var result = _client.EditRoom(_roomId, buildingId, roomName, roomDescription, roomCapacity, typeId, _userId);
                if (result)
                {   //Save changes successful
                    Success.IsOpen = true;
                }
                else
                {
                    //Saving changes failed
                    ValidationMessage.Content = "Save Changes Failed please contact your System Administratator";
                    ValidationMessage.Foreground = _alert;
                    ValidationMessage.Visibility = Visibility.Visible;
                }
            
            }
            
        }
        
        //Close success Popup function
        public void CloseSuccessPopup(object sender, RoutedEventArgs e)
        {   //Popup closed
            Success.IsOpen = false;
            //window closed
            Close();
        }

        //Close Building Popup function
        public void CloseBuildingPopup(object sender, RoutedEventArgs e)
        {   //popup closed
            NoBuildings.IsOpen = false;
            //window closed
            Close();
        }

        //Close success Room Types function
        public void CloseRoomTypesPopup(object sender, RoutedEventArgs e)
        {   //popup closed 
            NoRoomTypes.IsOpen = false;
            //window closed
            Close();
        }
    }
}
