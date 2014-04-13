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

        private TimetablingServiceClient _client = new TimetablingServiceClient();

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

                    BuildingSelect.SelectedItem = buildingsList.SingleOrDefault(x=>x.BuildingId == room.Building).BuildingName;
                }
                else
                {
                    BuildingSelect.IsEnabled = false;
                }

                if (roomtypes != null)
                {
                    foreach (var r in roomtypes)
                    {
                        RoomTypeSelect.Items.Add(r.RoomeTypeDescription);
                    }
                    RoomTypeSelect.SelectedItem = roomtypes.SingleOrDefault(x => x.RoomTypeId == room.RoomType).RoomeTypeDescription;
                }
                else
                {
                    RoomTypeSelect.IsEnabled = false;
                }
            }

        }

        public bool ValidationOnFields()
        {
            var roomName = RoomName.Text;
            var roomDescription = RoomDescription.Text;
            var buildingId =_client.ReturnBuildingIdFromBuildingName(BuildingSelect.Text);
            var typeId = _client.ReturnRoomTypeIdFromTypeName(RoomTypeSelect.Text);

            if (!_client.CheckRoomExists(roomName))
            {
                return false;
            }

            if (!String.IsNullOrEmpty(roomName) && !String.IsNullOrEmpty(roomDescription))
            {
                if (buildingId != 0 && typeId != 0)
                {
                    return true;
                }
                return false;
            }
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
    }
}
