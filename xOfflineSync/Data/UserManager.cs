/*
 * To add Offline Sync Support:
 *  1) Add the NuGet package Microsoft.Azure.Mobile.Client.SQLiteStore (and dependencies) to all client projects
 *  2) Uncomment the #define OFFLINE_SYNC_ENABLED
 *
 * For more information, see: http://go.microsoft.com/fwlink/?LinkId=620342
 */
#define OFFLINE_SYNC_ENABLED

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
#endif

namespace xOfflineSync
{
    public partial class UserManager
    {
        static UserManager defaultInstance = new UserManager();
        MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<Users> userTable;
#else
        IMobileServiceTable<User> userTable;
#endif

        const string offlineDbPath = @"localstore.db";

        private UserManager()
        {
            this.client = new MobileServiceClient(Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(offlineDbPath);
            store.DefineTable<Users>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.userTable = client.GetSyncTable<Users>();
#else
            this.userTable = client.GetTable<User>();
#endif
        }

        public static UserManager DefaultManager
        {
            get
            {
                return defaultInstance;
            }
            private set
            {
                defaultInstance = value;
            }
        }

        public MobileServiceClient CurrentClient
        {
            get { return client; }
        }

        public bool IsOfflineEnabled
        {
            get { return userTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Users>; }
        }

        public async Task<ObservableCollection<Users>> GetUsersAsync(bool syncItems = false)
        {
            try
            {
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
                IEnumerable<Users> items = await userTable
                    .Where(user => !user.Done)
                    .ToEnumerableAsync();

                return new ObservableCollection<Users>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine("Invalid sync operation: {0}", new[] { msioe.Message });
            }
            catch (Exception e)
            {
                Debug.WriteLine("Sync error: {0}", new[] { e.Message });
                throw e;
            }
            return null;
        }

        public async Task SaveTaskAsync(Users item)
        {
            try
            {
                if (item.Id == null)
                {
                    await userTable.InsertAsync(item);
                }
                else
                {
                    await userTable.UpdateAsync(item);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Save error: {0}", new[] { e.Message });
            }
        }

#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                try
                {
                    await this.client.SyncContext.PushAsync();
                }
                catch
                {
                    Debug.WriteLine("Failed on PushAsync");
                    throw;
                }
                try
                {
                    await this.userTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allUsers",
                    this.userTable.CreateQuery());
                }
                catch
                {
                    Debug.WriteLine("Failed on PullAsync");
                    throw;
                }
                
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)
                {
                    syncErrors = exc.PushResult.Errors;
                    Debug.WriteLine("Error: " + syncErrors);
                    
                }
                Debug.WriteLine("Failed on MobileServicePushFailedException");
                throw;
            }

            // Simple error/conflict handling. A real application would handle the various errors like network conditions,
            // server conflicts and others via the IMobileServiceSyncHandler.
            if (syncErrors != null)
            {
                foreach (var error in syncErrors)
                {
                    if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
                    {
                        //Update failed, reverting to server's copy.
                        await error.CancelAndUpdateItemAsync(error.Result);
                    }
                    else
                    {
                        // Discard local change.
                        await error.CancelAndDiscardItemAsync();
                    }

                    Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
                }
            }
        }
#endif
    }
}
