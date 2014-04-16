using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TimetablingClientApplication.TimetablingService;

namespace TimetablingClientApplication.Views.Database.Windows
{
    /// <summary>
    /// Interaction logic for EditRoom.xaml
    /// </summary>
    public partial class EditRoom : Window
    {
        private readonly int _userId;
        private readonly int _roomId;

        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush _normal = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE3E9EF"));

        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        public EditRoom(int userId, int roomId)
        {
            _userId = userId;
            _roomId = roomId;
            InitializeComponent();
            var room = _client.ReturnRoomDetail(roomId);

            if (room != null)
            {
                RoomName.Text = room.RoomName;
                RoomDescription.Text = room.RoomDescription;
                RoomCapacity.Text = room.Capacity.ToString();
                RoomTypeSelect.Text = room.RoomName;

                var buildingsList = _client.ReturnBuildings();
                var roomtypes = _client.ReturnRoomTypes();

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
                        OpenBuildingPopup();
                        return;
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

                    var selectedType = roomtypes.SingleOrDefault(x => x.RoomTypeId == room.RoomType);

                    if (selectedType != null)
                    {
                        RoomTypeSelect.SelectedItem = selectedType.RoomeTypeDescription;
                        return;
                    }
                    OpenRoomTypesPopup();
                }
                else
                {
                    OpenRoomTypesPopup();
                }
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
            var buildingId = _client.ReturnBuildingIdFromBuildingName(BuildingSelect.Text);
            var typeId = _client.ReturnRoomTypeIdFromTypeName(RoomTypeSelect.Text);

            
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
                ValidationMessage.Content = "'Capacity' must be a numeric value.";
                ValidationMessage.Visibility = Visibility.Visible;
            }
        } 
        
        public void SaveRoomChanges(object sender, RoutedEventArgs e)
        {
            var roomName = RoomName.Text;
            var roomDescription = RoomDescription.Text;
            var buildingId = _client.ReturnBuildingIdFromBuildingName(BuildingSelect.Text);
            var typeId = _client.ReturnRoomTypeIdFromTypeName(RoomTypeSelect.Text);
            var roomCapacity = Convert.ToInt32(RoomCapacity.Text);

            if (ValidationOnFields())
            {
                _client.EditRoom(_roomId, buildingId, roomName, roomDescription, roomCapacity, typeId, _userId);
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
