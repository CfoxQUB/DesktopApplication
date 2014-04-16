using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimetablingClientApplication.TimetablingService;
using TimetablingClientApplication.Views.Database.Windows;

namespace TimetablingClientApplication.Views.Database.Pages
{
    /// <summary>
    /// Interaction logic for BuildingManagement.xaml
    /// </summary>
    public partial class BuildingManagement : Page
    {

        private TimetablingServiceClient _client = new TimetablingServiceClient();

        private readonly ObservableCollection<Building> _buildingCollectionList = new ObservableCollection<Building>();

        private readonly string _defaultSearchString = "Search for Buildings . . . ";

        private bool pageRendered = false;

        private int _userId;

        public BuildingManagement(int userId)
        {
            _userId = userId;
            InitializeComponent();
            var buildingsList = _client.ReturnBuildings();

            if (buildingsList != null)
            {
                foreach (var b in buildingsList)
                {
                    _buildingCollectionList.Add(b);
                }
            }

            BuildingList.ItemsSource = _buildingCollectionList;
            BuildingNameText.Text = "No selection made.";
            BuildingNumber.Text = "No selection made";
            EventsText.Text = "None";
        }

        public void NewBuildingSelected(object sender, RoutedEventArgs e)
        {
            var selectedBuilding = (Building) BuildingList.SelectedItem;
            if (selectedBuilding != null)
            {
                BuildingNameText.Text = selectedBuilding.BuildingName;
                
                var eventNumber = _client.ReturnBuildingEvents(selectedBuilding.BuildingId);

                if (eventNumber != null)
                {
                    EventsText.Text = eventNumber.Count().ToString();
                }
                else
                {
                    EventsText.Text = "0";
                }

                BuildingNumber.Text = selectedBuilding.BuildingNumber.ToString();
                AddressLine1.Text = selectedBuilding.AddressLine1;
                AddressLine2.Text = selectedBuilding.AddressLine2;
                City.Text = selectedBuilding.City;
                Postcode.Text = selectedBuilding.PostalCode;

            }
        }

        private void SearchField_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchRooms.Text = "";
        }

        private void SearchField_LoseFocus(object sender, RoutedEventArgs e)
        {
            SearchRooms.Text = _defaultSearchString;
        }

        private void ReturnSearchResults(object sender, RoutedEventArgs e)
        {
            if (pageRendered)
            {
                if (SearchRooms.Text != _defaultSearchString)
                {
                    var results = _client.SearchBuildingFunction(SearchRooms.Text);

                    BuildingList.ItemsSource = results;
                }

            }
        }

        private void DeleteBuildingPopup(object sender, RoutedEventArgs e)
        {
            DeleteBuilding.IsOpen = true;
        }

        private void CloseDeletePopup(object sender, RoutedEventArgs e)
        {
            DeleteBuilding.IsOpen = false;
        }

        private void ConfirmDeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            var selectedBuilding = (Building) BuildingList.SelectedItem;
            var buildingId = _client.ReturnBuildingIdFromBuildingName(BuildingList.SelectedItem.ToString());
            if (selectedBuilding != null)
            {
                _client.DeleteBuilding(selectedBuilding.BuildingId);

                var noResults = _client.ReturnBuildings();

                BuildingList.ItemsSource = noResults;

                BuildingNameText.Text = "No selection made.";
                BuildingNumber.Text = "No selection made";
                AddressLine1.Text = "";
                AddressLine2.Text = "";
                Postcode.Text = "";
                City.Text = "";
                EventsText.Text = "None";
            }

            DeleteBuilding.IsOpen = false;
        }


        public void DisablePage()
        {
            SearchRooms.IsEnabled = false;
            BuildingList.IsEnabled = false;
            EditBuildingButton.IsEnabled = false;
            DeleteBuildingButton.IsEnabled = false;
        }

        public void EnablePage()
        {
            SearchRooms.IsEnabled = true;
            BuildingList.IsEnabled = true;
            EditBuildingButton.IsEnabled = true;
            DeleteBuildingButton.IsEnabled = true;
        }

        public void AddNewBuilding(object sender, RoutedEventArgs e)
        {
            var building = new CreateNewBuilding(_userId);
            building.Show();
        }

        public void EditBuilding(object sender, RoutedEventArgs e)
        {
            var buildingId = (Building) BuildingList.SelectedItem;
            if (buildingId != null)
            {
                var editbuilding = new EditBuilding(_userId, buildingId.BuildingId);
                editbuilding.Show();
            }
        }
    }
}
