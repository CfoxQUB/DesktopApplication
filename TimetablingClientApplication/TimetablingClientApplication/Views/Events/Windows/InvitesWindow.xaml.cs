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

namespace TimetablingClientApplication.Views.Events.Windows
{
    /// <summary>
    /// Interaction logic for InvitesWindow.xaml
    /// </summary>
    public partial class InvitesWindow : Window
    {
        private readonly int _eventId;
        public InvitesWindow(int eventId)
        {
            InitializeComponent();
            _eventId = eventId;
        }
    }
}
