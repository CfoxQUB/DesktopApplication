using System.Collections.ObjectModel;
using System.Windows.Navigation;
using TimetablingClientApplication.TimetablingService;

namespace TimetablingClientApplication.Views.Database.Pages
{
    /// <summary>
    /// Interaction logic for StaffTableManagement.xaml
    /// </summary>
    public partial class StaffTableManagement
    {
        private readonly TimetablingServiceClient _client = new TimetablingServiceClient();

        private readonly ObservableCollection<Staff> _staffCollectionList = new ObservableCollection<Staff>();

        private const string DefaultSearchString = "Search for Courses . . . ";

        private readonly NavigationService _navigationService;
        
        private readonly int _userId;

        public StaffTableManagement(int userId, NavigationService navigationService)
        {
            _userId = userId;
            _navigationService = navigationService;

            InitializeComponent();
        }
    }
}
