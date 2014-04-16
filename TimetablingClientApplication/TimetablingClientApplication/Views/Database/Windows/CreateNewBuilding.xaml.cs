﻿using System;
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
    /// Interaction logic for AddNewBuilding.xaml
    /// </summary>
    public partial class CreateNewBuilding : Window
    {
        private readonly int _userId;

        private readonly SolidColorBrush _alert = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush _normal = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE3E9EF"));

        private TimetablingServiceClient _client = new TimetablingServiceClient();

        public CreateNewBuilding(int userId)
        {
            _userId = userId;

            InitializeComponent();
        }

        public bool ValidationOnFields()
        {
            var buildingName = BuildingName.Text;
            var buildingNumber = BuildingNumber.Text;
            var addressLine1 = AddressLine1.Text;
            var addressLine2 = AddressLine2.Text;
            var city = City.Text;
            var postcode = Postcode.Text;

            if (_client.CheckBuildingExists(buildingName))
            {
                ValidationMessage.Content = "Building already exists";
                ValidationMessage.Foreground = _alert;
                ValidationMessage.Visibility = Visibility.Visible;
                return false;
            }

            if (!String.IsNullOrEmpty(buildingName) && !String.IsNullOrEmpty(buildingNumber) && !String.IsNullOrEmpty(addressLine1) && !String.IsNullOrEmpty(addressLine2) && !String.IsNullOrEmpty(city) && !String.IsNullOrEmpty(postcode) && BuildingNumberCheck())
            {
                return true;
            }

            BuildingName.BorderBrush = _alert;
            BuildingNumber.BorderBrush = _alert;
            AddressLine1.BorderBrush = _alert;
            AddressLine2.BorderBrush = _alert;
            City.BorderBrush = _alert;
            Postcode.BorderBrush = _alert;

            ValidationMessage.Content = "Save changes failed. Please ensure all the fields are completed.";
            ValidationMessage.Foreground = _alert;
            ValidationMessage.Visibility = Visibility.Visible;

            return false;
        }
      
        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ValidationOnBuildingNumber(object sender, RoutedEventArgs e)
        {
            var temp = BuildingNumber.Text;

            var regex = new Regex("^[0-9]+$");

            if (!regex.IsMatch(temp))
            {
                ValidationMessage.Content = "'Building Number' must be a numeric value.";
                ValidationMessage.Foreground = _alert;
                ValidationMessage.Visibility = Visibility.Visible;
            }
            ValidationMessage.Visibility = Visibility.Hidden;
        } 
        
        /// <summary>
        /// http://stackoverflow.com/questions/273141/regex-for-numbers-only
        /// </summary>
        public bool BuildingNumberCheck()
        {
            var temp = BuildingNumber.Text;

            var regex = new Regex("^[0-9]+$");

            if (!regex.IsMatch(temp))
            {
                return false;
            }
            
            return true;
        } 

        public void SubmitNewBuilding(object sender, RoutedEventArgs e)
        {
            if (ValidationOnFields())
            {
                var buildingName = BuildingName.Text;
                var buildingNumber = Convert.ToInt32(BuildingNumber.Text);
                var addressLine1 = AddressLine1.Text;
                var addressLine2 = AddressLine2.Text;
                var city = City.Text;
                var postcode = Postcode.Text;

                _client.CreateNewBuilding(buildingName, buildingNumber, addressLine1, addressLine2, postcode, city, _userId);
            }

        }
    }
}