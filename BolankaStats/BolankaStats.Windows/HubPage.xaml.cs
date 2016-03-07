using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BolankaStats.Data;
using BolankaStats.Common;
using BolankaStats.DataModel;
using System.Collections;

// The Universal Hub Application project template is documented at http://go.microsoft.com/fwlink/?LinkID=391955

namespace BolankaStats
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private StatsClient client = new StatsClient();

        /// <summary>
        /// Gets the NavigationHelper used to aid in navigation and process lifetime management.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the DefaultViewModel. This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public HubPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-4");
            this.DefaultViewModel["Section3Items"] = sampleDataGroup;
            var entrances = await client.GetEntrances();
            this.DefaultViewModel["Entrances"] = entrances;

            List<EntrancesByDay> entracesPerDay = new List<EntrancesByDay>();
            var saturday = await client.GetEntrancesByDay("Saturday");
            var sunday = await client.GetEntrancesByDay("Sunday");
            entracesPerDay.Add(new EntrancesByDay("Tuesday", saturday));
            entracesPerDay.Add(new EntrancesByDay("Wednsday", saturday));
            entracesPerDay.Add(new EntrancesByDay("Thursday", saturday));
            entracesPerDay.Add(new EntrancesByDay("Friday", saturday));
            entracesPerDay.Add(new EntrancesByDay("Saturday", saturday));
            entracesPerDay.Add(new EntrancesByDay("Sunday", sunday));
            System.Diagnostics.Debug.WriteLine("entracesPerDay");
            System.Diagnostics.Debug.WriteLine("Saturday "+ saturday);
            System.Diagnostics.Debug.WriteLine(entracesPerDay);
            this.DefaultViewModel["EntrancesPerDay"] = entracesPerDay;
            ((TimePicker)CommonHelper.FindChildControl<TimePicker>(this, "EntranceTimePicker")).Time=DateTime.Now.TimeOfDay;
        }

        /// <summary>
        /// Invoked when a HubSection header is clicked.
        /// </summary>
        /// <param name="sender">The Hub that contains the HubSection whose header was clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubSection section = e.Section;
            var group = section.DataContext;
            this.Frame.Navigate(typeof(SectionPage), ((SampleDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        /// <param name="sender">The GridView or ListView
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemPage), itemId);
        }
        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// and <see cref="Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void Button_Click(object sender, RoutedEventArgs e)
        {            
            DateTime date = ((DatePicker)CommonHelper.FindChildControl<DatePicker>(this,"EntranceDatePicker")).Date.Date.AddHours(((TimePicker)CommonHelper.FindChildControl<TimePicker>(this,"EntranceTimePicker")).Time.Hours);
            int people = Int32.Parse(((TextBox)CommonHelper.FindChildControl<TextBox>(this, "NumberOfPeople")).Text);
            Entrance entrance = new Entrance(date, people, ((Button)sender).Name == "InButton" ? true : false);
            StatsClient client = new StatsClient();
            await client.PostEntrance(entrance);
            ((Grid)CommonHelper.FindChildControl<Grid>(this, "AddForm")).Visibility = Visibility.Collapsed;
            ((StackPanel)CommonHelper.FindChildControl<StackPanel>(this, "PostAdd")).Visibility = Visibility.Visible;
            var entrances = await client.GetEntrances();
            this.DefaultViewModel["Entrances"] = entrances;
            List < EntrancesByDay > entracesPerDay = new List<EntrancesByDay>();
            var saturday = await client.GetEntrancesByDay("saturday");
            var sunday = await client.GetEntrancesByDay("sunday");
            entracesPerDay.Add(new EntrancesByDay("saturday", saturday));
            entracesPerDay.Add(new EntrancesByDay("sunday", sunday));
            System.Diagnostics.Debug.WriteLine("entracesPerDay");
            System.Diagnostics.Debug.WriteLine(entracesPerDay);
            this.DefaultViewModel["EntrancesPerDay"] = entracesPerDay;
            this.UpdateLayout();
        }    

        private void AddNext_Click(object sender, RoutedEventArgs e)
        {
            ((Grid)CommonHelper.FindChildControl<Grid>(this, "AddForm")).Visibility = Visibility.Visible;
            ((StackPanel)CommonHelper.FindChildControl<StackPanel>(this, "PostAdd")).Visibility = Visibility.Collapsed;
        }
    }
}
