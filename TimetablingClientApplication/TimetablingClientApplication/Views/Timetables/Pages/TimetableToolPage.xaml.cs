using System.Collections.Generic;
using System.Linq;
using TimetablingClientApplication.TimetablingService;

namespace TimetablingClientApplication.Views.Timetables.Pages
{
    /// <summary>
    /// Interaction logic for TimetableToolPage.xaml
    /// </summary>
    public partial class TimetableToolPage
    {
        //WEbservice fc=ucntionality exposed through web service
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();
        
        public TimetableToolPage()
        {
            //timetable tool item generated
            var timetableDisplay = _client.ReturnTimetableToolListObject();

            InitializeComponent();
            //Timeslots populated accordig to day
            Populate_Monday_Display(new List<TimetableObject>(timetableDisplay.MondayList), "Mon");
            Populate_Tuesday_Display(new List<TimetableObject>(timetableDisplay.TuesdayList), "Tue");
            Populate_Wednesday_Display(new List<TimetableObject>(timetableDisplay.WednesdayList), "Wed");
            Populate_Thursday_Display(new List<TimetableObject>(timetableDisplay.ThursdayList), "Thur");
            Populate_Friday_Display(new List<TimetableObject>(timetableDisplay.FridayList), "Fri");
            Populate_Saturday_Display(new List<TimetableObject>(timetableDisplay.SaturdayList), "Sat");
            Populate_Sunday_Display(new List<TimetableObject>(timetableDisplay.SundayList), "Sun");
        }

        public void Populate_Monday_Display(List<TimetableObject> timetableObjectList, string dayName)
        {
            if (timetableObjectList.Any())
            {
                var m10 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 10);
                if (m10 != null)
                {
                  Mon10.Content = m10.Percentage + "%";  
                }
                
                var m11 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 11);
                if (m11 != null)
                {
                  Mon11.Content = m11.Percentage + "%";  
                }
                
                var m12 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 12);
                if (m12 != null)
                {
                  Mon12.Content = m12.Percentage + "%";  
                }
                
                var m13 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 13);
                if (m13 != null)
                {
                  Mon13.Content = m13.Percentage + "%";  
                }
                
                var m14 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 14);
                if (m14 != null)
                {
                  Mon14.Content = m14.Percentage + "%";  
                }
                
                var m15 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 15);
                if (m15 != null)
                {
                  Mon15.Content = m15.Percentage + "%";  
                }
                
                var m16 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 16);
                if (m16 != null)
                {
                  Mon16.Content = m16.Percentage + "%";  
                }
                
                var m17 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 17);
                if (m17 != null)
                {
                  Mon17.Content = m17.Percentage + "%";  
                }
                
             }
            
        } 
        
        public void Populate_Tuesday_Display(List<TimetableObject> timetableObjectList, string dayName)
        {
            if (timetableObjectList.Any())
            {
                var t10 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 10);
                if (t10 != null)
                {
                    Tue10.Content = t10.Percentage + "%";
                }

                var t11 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 11);
                if (t11 != null)
                {
                    Tue11.Content = t11.Percentage + "%";
                }

                var t12 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 12);
                if (t12 != null)
                {
                    Tue12.Content = t12.Percentage + "%";
                }

                var t13 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 13);
                if (t13 != null)
                {
                    Tue13.Content = t13.Percentage + "%";
                }

                var t14 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 14);
                if (t14 != null)
                {
                    Tue14.Content = t14.Percentage + "%";
                }

                var t15 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 15);
                if (t15 != null)
                {
                    Tue15.Content = t15.Percentage + "%";
                }

                var t16 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 16);
                if (t16 != null)
                {
                    Tue16.Content = t16.Percentage + "%";
                }

                var t17 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 17);
                if (t17 != null)
                {
                    Tue17.Content = t17.Percentage + "%";
                }

            }
        }
        
        public void Populate_Wednesday_Display(List<TimetableObject> timetableObjectList, string dayName)
        {
            if (timetableObjectList.Any())
            {
                var w10 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 10);
                if (w10 != null)
                {
                    Wed10.Content = w10.Percentage + "%";
                }

                var w11 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 11);
                if (w11 != null)
                {
                    Wed11.Content = w11.Percentage + "%";
                }

                var w12 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 12);
                if (w12 != null)
                {
                    Wed12.Content = w12.Percentage + "%";
                }

                var w13 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 13);
                if (w13 != null)
                {
                    Wed13.Content = w13.Percentage + "%";
                }

                var w14 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 14);
                if (w14 != null)
                {
                    Wed14.Content = w14.Percentage + "%";
                }

                var w15 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 15);
                if (w15 != null)
                {
                    Wed15.Content = w15.Percentage + "%";
                }

                var w16 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 16);
                if (w16 != null)
                {
                    Wed16.Content = w16.Percentage + "%";
                }

                var w17 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 17);
                if (w17 != null)
                {
                    Wed17.Content = w17.Percentage + "%";
                }

            }
        }

        public void Populate_Thursday_Display(List<TimetableObject> timetableObjectList, string dayName)
        {
            if (timetableObjectList.Any())
            {
                var th10 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 10);
                if (th10 != null)
                {
                    Thur10.Content = th10.Percentage + "%";
                }

                var th11 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 11);
                if (th11 != null)
                {
                    Thur11.Content = th11.Percentage + "%";
                }

                var th12 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 12);
                if (th12 != null)
                {
                    Thur12.Content = th12.Percentage + "%";
                }

                var th13 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 13);
                if (th13 != null)
                {
                    Thur13.Content = th13.Percentage + "%";
                }

                var th14 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 14);
                if (th14 != null)
                {
                    Thur14.Content = th14.Percentage + "%";
                }

                var th15 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 15);
                if (th15 != null)
                {
                    Thur15.Content = th15.Percentage + "%";
                }

                var th16 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 16);
                if (th16 != null)
                {
                    Thur16.Content = th16.Percentage + "%";
                }

                var th17 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 17);
                if (th17 != null)
                {
                    Thur17.Content = th17.Percentage + "%";
                }

            }
        }

        public void Populate_Friday_Display(List<TimetableObject> timetableObjectList, string dayName)
        {
            if (timetableObjectList.Any())
            {
                var fr10 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 10);
                if (fr10 != null)
                {
                    Fri10.Content = fr10.Percentage + "%";
                }

                var fr11 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 11);
                if (fr11 != null)
                {
                    Fri11.Content = fr11.Percentage + "%";
                }

                var fr12 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 12);
                if (fr12 != null)
                {
                    Fri12.Content = fr12.Percentage + "%";
                }

                var fr13 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 13);
                if (fr13 != null)
                {
                    Fri13.Content = fr13.Percentage + "%";
                }

                var fr14 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 14);
                if (fr14 != null)
                {
                    Fri14.Content = fr14.Percentage + "%";
                }

                var fr15 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 15);
                if (fr15 != null)
                {
                    Fri15.Content = fr15.Percentage + "%";
                }

                var fr16 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 16);
                if (fr16 != null)
                {
                    Fri16.Content = fr16.Percentage + "%";
                }

                var fr17 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 17);
                if (fr17 != null)
                {
                    Fri17.Content = fr17.Percentage + "%";
                }

            }
        }

        public void Populate_Saturday_Display(List<TimetableObject> timetableObjectList, string dayName)
        {
            if (timetableObjectList.Any())
            {
                var sa10 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 10);
                if (sa10 != null)
                {
                    Sat10.Content = sa10.Percentage + "%";
                }

                var sa11 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 11);
                if (sa11 != null)
                {
                    Sat11.Content = sa11.Percentage + "%";
                }

                var sa12 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 12);
                if (sa12 != null)
                {
                    Sat12.Content = sa12.Percentage + "%";
                }

                var sa13 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 13);
                if (sa13 != null)
                {
                    Sat13.Content = sa13.Percentage + "%";
                }

                var sa14 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 14);
                if (sa14 != null)
                {
                    Sat14.Content = sa14.Percentage + "%";
                }

                var sa15 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 15);
                if (sa15 != null)
                {
                    Sat15.Content = sa15.Percentage + "%";
                }

                var sa16 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 16);
                if (sa16 != null)
                {
                    Sat16.Content = sa16.Percentage + "%";
                }

                var sa17 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 17);
                if (sa17 != null)
                {
                    Sat17.Content = sa17.Percentage + "%";
                }

            }
        }

        public void Populate_Sunday_Display(List<TimetableObject> timetableObjectList, string dayName)
        {
            if (timetableObjectList.Any())
            {
                var su10 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 10);
                if (su10 != null)
                {
                    Sun10.Content = su10.Percentage + "%";
                }

                var su11 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 11);
                if (su11 != null)
                {
                    Sun11.Content = su11.Percentage + "%";
                }

                var su12 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 12);
                if (su12 != null)
                {
                    Sun12.Content = su12.Percentage + "%";
                }

                var su13 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 13);
                if (su13 != null)
                {
                    Sun13.Content =su13.Percentage + "%";
                }

                var su14 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 14);
                if (su14 != null)
                {
                    Sun14.Content = su14.Percentage + "%";
                }

                var su15 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 15);
                if (su15 != null)
                {
                    Sun15.Content = su15.Percentage + "%";
                }

                var su16 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 16);
                if (su16 != null)
                {
                    Sun16.Content = su16.Percentage + "%";
                }

                var su17 = timetableObjectList.SingleOrDefault(x => x.Timeslot == 17);
                if (su17 != null)
                {
                    Sun17.Content = su17.Percentage + "%";
                }

            }
        }
    }
}
