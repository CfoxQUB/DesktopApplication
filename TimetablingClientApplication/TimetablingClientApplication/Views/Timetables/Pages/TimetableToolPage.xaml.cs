using System;
using System.Collections.Generic;
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

namespace TimetablingClientApplication.Views.Timetables
{
    /// <summary>
    /// Interaction logic for TimetableToolPage.xaml
    /// </summary>
    public partial class TimetableToolPage : Page
    {
        
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        private readonly TimetableDisplayListObject  _timetableDisplay = new TimetableDisplayListObject(); 
        
        public TimetableToolPage()
        {
            _timetableDisplay = _client.ReturnTimetableDisplayListObject();
            
            InitializeComponent();

            Populate_Monday_Display(new List<TimetableObject>(_timetableDisplay.MondayList), "Mon");
            Populate_Tuesday_Display(new List<TimetableObject>(_timetableDisplay.TuesdayList), "Tue");
            Populate_Wednesday_Display(new List<TimetableObject>(_timetableDisplay.WednesdayList), "Wed");
            Populate_Thursday_Display(new List<TimetableObject>(_timetableDisplay.ThursdayList), "Thur");
            Populate_Friday_Display(new List<TimetableObject>(_timetableDisplay.FridayList), "Fri");
            Populate_Saturday_Display(new List<TimetableObject>(_timetableDisplay.SaturdayList), "Sat");
            Populate_Sunday_Display(new List<TimetableObject>(_timetableDisplay.SundayList), "Sun");
        }

        public void Populate_Monday_Display(List<TimetableObject> timetableObjectList, string dayName)
        {
            Mon10.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 10).Percentage + "%";
            Mon11.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 11).Percentage + "%";
            Mon12.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 12).Percentage + "%";
            Mon13.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 13).Percentage + "%";
            Mon14.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 14).Percentage + "%";
            Mon15.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 15).Percentage + "%";
            Mon16.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 16).Percentage + "%";
            Mon17.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 17).Percentage + "%";
        } 
        
        public void Populate_Tuesday_Display(List<TimetableObject> timetableObjectList, string dayName)
        {

            Tue10.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 10).Percentage + "%";
            Tue11.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 11).Percentage + "%";
            Tue12.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 12).Percentage + "%";
            Tue13.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 13).Percentage + "%";
            Tue14.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 14).Percentage + "%";
            Tue15.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 15).Percentage + "%";
            Tue16.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 16).Percentage + "%";
            Tue17.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 17).Percentage + "%";
        }
        
        public void Populate_Wednesday_Display(List<TimetableObject> timetableObjectList, string dayName)
        {

            Wed10.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 10).Percentage + "%";
            Wed11.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 11).Percentage + "%";
            Wed12.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 12).Percentage + "%";
            Wed13.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 13).Percentage + "%";
            Wed14.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 14).Percentage + "%";
            Wed15.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 15).Percentage + "%";
            Wed16.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 16).Percentage + "%";
            Wed17.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 17).Percentage + "%";
        }

        public void Populate_Thursday_Display(List<TimetableObject> timetableObjectList, string dayName)
        {

            Thur10.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 10).Percentage + "%";
            Thur11.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 11).Percentage + "%";
            Thur12.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 12).Percentage + "%";
            Thur13.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 13).Percentage + "%";
            Thur14.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 14).Percentage + "%";
            Thur15.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 15).Percentage + "%";
            Thur16.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 16).Percentage + "%";
            Thur17.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 17).Percentage + "%";
        }

        public void Populate_Friday_Display(List<TimetableObject> timetableObjectList, string dayName)
        {

            Fri10.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 10).Percentage + "%";
            Fri11.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 11).Percentage + "%";
            Fri12.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 12).Percentage + "%";
            Fri13.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 13).Percentage + "%";
            Fri14.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 14).Percentage + "%";
            Fri15.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 15).Percentage + "%";
            Fri16.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 16).Percentage + "%";
            Fri17.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 17).Percentage + "%";
        }


        public void Populate_Saturday_Display(List<TimetableObject> timetableObjectList, string dayName)
        {

            Sat10.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 10).Percentage + "%";
            Sat11.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 11).Percentage + "%";
            Sat12.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 12).Percentage + "%";
            Sat13.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 13).Percentage + "%";
            Sat14.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 14).Percentage + "%";
            Sat15.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 15).Percentage + "%";
            Sat16.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 16).Percentage + "%";
            Sat17.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 17).Percentage + "%";
        }

        public void Populate_Sunday_Display(List<TimetableObject> timetableObjectList, string dayName)
        {

            Sun10.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 10).Percentage + "%";
            Sun11.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 11).Percentage + "%";
            Sun12.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 12).Percentage + "%";
            Sun13.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 13).Percentage + "%";
            Sun14.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 14).Percentage + "%";
            Sun15.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 15).Percentage + "%";
            Sun16.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 16).Percentage + "%";
            Sun17.Content = timetableObjectList.SingleOrDefault(x => x.Timeslot == 17).Percentage + "%";
        }
    }
}
