﻿using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;

namespace xOfflineSync
{
    public partial class UserList : ContentPage
    {
        UserManager manager;

        public UserList()
        {
            InitializeComponent();

            manager = UserManager.DefaultManager;
            if (Device.RuntimePlatform == Device.UWP)
            {
                var refreshButton = new Button
                {
                    Text = "Refresh",
                    HeightRequest = 30
                };
                refreshButton.Clicked += OnRefreshItems;
                buttonsPanel.Children.Add(refreshButton);

                

                if (manager.IsOfflineEnabled)
                {
                    var syncButton = new Button
                    {
                        Text = "Sync items",
                        HeightRequest = 30
                    };
                    syncButton.Clicked += OnSyncItems;
                    buttonsPanel.Children.Add(syncButton);
                }
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Set syncItems to true in order to synchronize the data on startup when running in offline mode
            await RefreshItems(true, syncItems: true);
        }

        // Data methods
        async Task AddItem(Users item)
        {
            await manager.SaveTaskAsync(item);
            userList.ItemsSource = await manager.GetUsersAsync();
        }

        async Task CompleteItem(Users item)
        {
            item.Done = true;
            await manager.SaveTaskAsync(item);
            userList.ItemsSource = await manager.GetUsersAsync();
        }

        public async void OnAdd(object sender, EventArgs e)
        {
            var user = new Users {
                FirstName   = firstName.Text,
                LastName    = lastName.Text
            };
            await AddItem(user);

            firstName.Text = string.Empty;
            firstName.Unfocus();

            lastName.Text = string.Empty;
            lastName.Unfocus();
        }

        // Event handlers
        public async void OnSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var user = e.SelectedItem as Users;
            if (Device.RuntimePlatform != Device.iOS && user != null)
            {
                // Not iOS - the swipe-to-delete is discoverable there
                if (Device.RuntimePlatform == Device.Android)
                {
                    await DisplayAlert(user.Name, "Press-and-hold to remove User " + user.FirstName + " " + user.LastName, "Okaly Dokaly!");
                }
                else
                {
                    // Windows, not all platforms support the Context Actions yet
                    if (await DisplayAlert("Delete User?", "Do you wish to delete " + user.FirstName + " " + user.LastName + "?", "Bye, Felicia", "Cancel"))
                    {
                        await CompleteItem(user);
                    }
                }
            }

            // prevents background getting highlighted
            userList.SelectedItem = null;
        }

        // http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/listview/#context
        public async void OnComplete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var user = mi.CommandParameter as Users;
            await CompleteItem(user);
        }

        // http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/listview/#pulltorefresh
        public async void OnRefresh(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();


            var list = (ListView)sender;
            Exception error = null;

            try
            {
                await RefreshItems(false, true);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                list.EndRefresh();       
            }

            if (error != null)
            {
                await DisplayAlert("Refresh Error", "Couldn't refresh data (" + error.Message + ")", "OK");
            }
        }

        public async void OnSyncItems(object sender, EventArgs e)
        {
            await RefreshItems(true, true);
        }

        public async void OnRefreshItems(object sender, EventArgs e)
        {
  
            await RefreshItems(true, false);
            
            Debug.WriteLine(userList);

        }


        private async Task RefreshItems(bool showActivityIndicator, bool syncItems)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                try
                {
                    userList.ItemsSource = await manager.GetUsersAsync(syncItems);
                }
                catch
                {
                    await DisplayAlert("Refresh timeout", "Connection issue\nReconnect to network to enable sync\n(Can still save data locally)", "OK");
                }
            }
        }

        private class ActivityIndicatorScope : IDisposable
        {
            private readonly bool showIndicator;
            private readonly ActivityIndicator indicator;
            private Task indicatorDelay;

            public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
            {
                this.indicator = indicator;
                this.showIndicator = showIndicator;

                if (showIndicator)
                {
                    indicatorDelay = Task.Delay(2000);
                    SetIndicatorActivity(true);
                }
                else
                {
                    indicatorDelay = Task.FromResult(0);
                }
            }

            private void SetIndicatorActivity(bool isActive)
            {
                this.indicator.IsVisible = isActive;
                this.indicator.IsRunning = isActive;
            }

            public void Dispose()
            {
                if (showIndicator)
                {
                    indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }
    }


}

