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
using System.Windows.Shapes;

namespace TimetablingClientApplication.Views.MasterViews
{
    /// <summary>
    /// Interaction logic for MasterView.xaml
    /// </summary>
    public partial class MasterView : Window
    {
        private int _userId = 1;
        public MasterView()
        {
            InitializeComponent();
            //_userId = loginId;
        }

        #region Navigation
        private void MenuItem_NewEvent_Click(object sender, RoutedEventArgs e)
        {
            CreateEvents createEvents = new CreateEvents(_userId);
            createEvents.Show();
        }

        private void MenuItem_EditEvent_Click(object sender, RoutedEventArgs e)
        {
            EditEvents editEvents = new EditEvents(_userId);
            editEvents.Show();
        }

        private void Menuitem_TimetableView_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new TimetablePage());
        }
        #endregion
    }
}
