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
    public partial class CreateNewRoom : Window
    {
        private readonly int _userId;

        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush _normal = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE3E9EF"));

        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        public CreateNewRoom(int userId)
        {
            _userId = userId;
            InitializeComponent();

            var buildingsList = _client.ReturnBuildings();
            var roomtypes = _client.ReturnRoomTypes();

            if (buildingsList != null)
            {
                foreach (var b in buildingsList)
                {
                    BuildingSelect.Items.Add(b.BuildingName);
                }
            }
            else
            {
                OpenBuildingPopup();
                return;
            }

            if (roomtypes != null)
            {
                foreach (var r in roomtypes)
                {
                    RoomTypeSelect.Items.Add(r.RoomeTypeDescription);
                }
            }
            else
            {
                OpenRoomTypesPopup();
            }
            
        }

        public bool ValidationOnFields()
        {
            RoomName.BorderBrush = _normal;
            RoomDescription.BorderBrush = _normal;
            BuildingSelect.BorderBrush = _normal;
            RoomTypeSelect.BorderBrush = _normal;
            RoomCapacity.BorderBrush = _normal;

            var roomName = RoomName.Text;
            var roomDescription = RoomDescription.Text;
            var buildingId =_client.ReturnBuildingIdFromBuildingName(BuildingSelect.Text);
            var typeId = _client.ReturnRoomTypeIdFromTypeName(RoomTypeSelect.Text);

            if (_client.CheckRoomExists(roomName))
            {
                ValidationMessage.Foreground = _alert;
                ValidationMessage.Visibility = Visibility.Visible;
                ValidationMessage.Content = "Room creation failed. A room with the same name exists.";
                return false;
            }
            ValidationMessage.Visibility = Visibility.Hidden;
            if (!String.IsNullOrEmpty(roomName) && !String.IsNullOrEmpty(roomDescription) && !String.IsNullOrEmpty(RoomCapacity.Text) && buildingId != 0 && typeId != 0)
            {
              ValidationMessage.Visibility = Visibility.Hidden;
              OpenSuccessPopup();
              return true; 
            }

            RoomName.BorderBrush = _alert;
            RoomDescription.BorderBrush = _alert;
            BuildingSelect.BorderBrush = _alert;
            RoomTypeSelect.BorderBrush = _alert;
            RoomCapacity.BorderBrush = _alert;

            ValidationMessage.Foreground = _alert;
            ValidationMessage.Visibility = Visibility.Visible;
            ValidationMessage.Content = "Please ensure all fields are completed.";
            return false;
        }

        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ValidationOnCapacity(object sender, RoutedEventArgs e)
        {
            var temp = RoomCapacity.Text;

            var regex = new Regex("^[0-9]+$");

            if (!regex.IsMatch(temp))
            {
                ValidationMessage.Foreground = _alert;
                ValidationMessage.Visibility = Visibility.Visible;
                ValidationMessage.Content = "'Capacity' must be a numeric value.";
                return;
            }
            ValidationMessage.Visibility = Visibility.Hidden;
        }

        public void SubmitNewRoom(object sender, RoutedEventArgs e)
        {
            var roomName = RoomName.Text;
            var roomDescription = RoomDescription.Text;
            var buildingId = _client.ReturnBuildingIdFromBuildingName(BuildingSelect.Text);
            var typeId = _client.ReturnRoomTypeIdFromTypeName(RoomTypeSelect.Text);
            var roomCapacity = 0;

            if (!String.IsNullOrEmpty(RoomCapacity.Text))
            {
                roomCapacity = Convert.ToInt32(RoomCapacity.Text);
            }
             
            if (ValidationOnFields())
            {
                _client.CreateNewRoom(buildingId, roomName, roomDescription, roomCapacity, typeId, _userId);
            }
            
        }

        public void OpenSuccessPopup()
        {
            Success.IsOpen = true;
        }
        
        public void CloseSuccessPopup(object sender, RoutedEventArgs e)
        {
            Success.IsOpen = false;
            Close();
        }
        
        public void OpenBuildingPopup()
        {
            NoBuildings.IsOpen = true;
        } 
        
        public void CloseBuildingPopup(object sender, RoutedEventArgs e)
        {
            NoBuildings.IsOpen = false;
            Close();
        } 
        
        public void OpenRoomTypesPopup()
        {
            NoRoomTypes.IsOpen = true;
        }
        
        public void CloseRoomTypesPopup(object sender, RoutedEventArgs e)
        {
            NoRoomTypes.IsOpen = false;
            Close();
        }
    }
}
