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
    /// Interaction logic for CreateNewRoom.xaml
    /// </summary>
    public partial class CreateNewRoom : Window
    {
        private int _userId;

        private TimetablingServiceClient _client = new TimetablingServiceClient();

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
                BuildingSelect.IsEnabled = false;
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
                RoomTypeSelect.IsEnabled = false;
            }
            
        }

        public bool ValidationOnFields()
        {
            var roomName = RoomName.Text;
            var roomDescription = RoomDescription.Text;
            var buildingId =_client.ReturnBuildingIdFromBuildingName(BuildingSelect.Text);
            var typeId = _client.ReturnRoomTypeIdFromTypeName(RoomTypeSelect.Text);

            if (_client.CheckRoomExists(roomName))
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
        
        public void SubmitNewRoom(object sender, RoutedEventArgs e)
        {
            var roomName = RoomName.Text;
            var roomDescription = RoomDescription.Text;
            var buildingId = _client.ReturnBuildingIdFromBuildingName(BuildingSelect.Text);
            var typeId = _client.ReturnRoomTypeIdFromTypeName(RoomTypeSelect.Text);
            var roomCapacity = Convert.ToInt32(RoomCapacity.Text);

            if (ValidationOnFields())
            {
                _client.CreateNewRoom(buildingId, roomName, roomDescription, roomCapacity, typeId, _userId);
            }
            
        }
    }
}
