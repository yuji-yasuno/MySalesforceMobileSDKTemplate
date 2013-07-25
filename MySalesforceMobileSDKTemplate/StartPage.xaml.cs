using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using System.Runtime.Serialization;
using MySalesforceMobileSDK;

namespace $safeprojectname$
{
    public sealed partial class StartPage : Page
    {
        public StartPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            ResourceLoader loader = new ResourceLoader();
            String clientId = loader.GetString("client_id");
            String redirectUrl = loader.GetString("redirect_uri");
            String scope = loader.GetString("scope");
            String apiVersion = loader.GetString("api");
            String oauthTokenFile = loader.GetString("oauth_data_filename");

            Boolean isNeedOAuthenticate = false;
            StorageFile file = null;
            try
            {
                file = await ApplicationData.Current.LocalFolder.GetFileAsync(oauthTokenFile);
            }
            catch (FileNotFoundException ex) 
            {
                isNeedOAuthenticate = true;
            }

            MySFSharedData share = MySFSharedData.getInstance();
            if (file != null) {
                var stream = await file.OpenStreamForReadAsync();
                var serializer = new DataContractSerializer(typeof(MySFOAuthStoredData));
                share.storedData = (MySFOAuthStoredData)serializer.ReadObject(stream);
                if (share.storedData.accessToken.Length == 0 || share.storedData.refreshToken.Length == 0 || share.storedData.instanceUrl.Length == 0) isNeedOAuthenticate = true;
            }
            
            MySFRestAPI api = new MySFRestAPI();
            MySFRestAPI.apiVersion = apiVersion;
            if (MySFRestAPI.coordinator == null) {
                MySFRestAPI.coordinator = new MySFOAuthCoordinator(clientId, redirectUrl, scope);
            }
            MySFRestAPI.coordinator.onCompletedAuthorization += coordinator_onCompletedAuthorization;
            MySFRestAPI.coordinator.onFailedAuthorization += coordinator_onFailedAuthorization;
            MySFRestAPI.coordinator.onCanceledAuthorization += coordinator_onCanceledAuthorization;
            MySFRestAPI.coordinator.onCompletedRefresh += coordinator_onCompletedRefresh;
            MySFRestAPI.coordinator.onFailedRefresh += coordinator_onFailedRefresh;
            MySFRestAPI.coordinator.onRequestFailedRefresh += coordinator_onRequestFailedRefresh;
            MySFRestAPI.coordinator.onCompletedRevokeToken += coordinator_onCompletedRevokeToken;
            MySFRestAPI.coordinator.onRequestFailedRevokeToken += coordinator_onRequestFailedRevokeToken;
            MySFRestAPI.coordinator.onFailedRevokeToken += coordinator_onFailedRevokeToken;

            if (!isNeedOAuthenticate)
            {
                MySFRestAPI.coordinator.credentials.accessToken = share.storedData.accessToken;
                MySFRestAPI.coordinator.credentials.refreshToken = share.storedData.refreshToken;
                MySFRestAPI.coordinator.credentials.instanceUrl = new Uri(share.storedData.instanceUrl);
                MySFRestAPI.coordinator.credentials.domain = MySFRestAPI.coordinator.credentials.instanceUrl.Authority;
                MySFRestAPI.coordinator.refresh();
            }
            else 
            {
                MySFRestAPI.coordinator.authenticate();
            }
        }

        private void coordinator_onCompletedAuthorization(Object sender, MySFOAuthEventArgs args)
        {
            prepareBeforeMove();
            this.appframe.Navigate(typeof(MainPage));
        }
        private void coordinator_onFailedAuthorization(Object sender, MySFOAuthEventArgs args)
        {
            
        }
        private void coordinator_onCanceledAuthorization(Object sender, MySFOAuthEventArgs args)
        {
            
        }
        private void coordinator_onCompletedRefresh(Object sender, MySFOAuthEventArgs args)
        {
            prepareBeforeMove();
            this.appframe.Navigate(typeof(MainPage));
        }
        private void coordinator_onFailedRefresh(Object sender, MySFOAuthEventArgs args)
        {
            
        }
        private void coordinator_onRequestFailedRefresh(Object sender, MySFOAuthEventArgs args)
        {
            
        }
        private void coordinator_onCompletedRevokeToken(Object sender, MySFOAuthEventArgs args)
        {
            
        }
        private void coordinator_onFailedRevokeToken(Object sender, MySFOAuthEventArgs args)
        {
            
        }
        private void coordinator_onRequestFailedRevokeToken(Object sender, MySFOAuthEventArgs args)
        {
            
        }

        private async void prepareBeforeMove() 
        {
            ResourceLoader loader = new ResourceLoader();
            String oauthTokenFile = loader.GetString("oauth_data_filename");

            StorageFile file = null;
            Boolean notFound = false;
            try
            {
                file = await ApplicationData.Current.LocalFolder.GetFileAsync(oauthTokenFile);
            }
            catch (FileNotFoundException ex)
            {
                notFound = true;
            }
            if (notFound)
            {
                file = await ApplicationData.Current.LocalFolder.CreateFileAsync(oauthTokenFile, CreationCollisionOption.ReplaceExisting);
            }

            var stream = await file.OpenStreamForWriteAsync();
            var serializer = new DataContractSerializer(typeof(MySFOAuthStoredData));
            MySFSharedData share = MySFSharedData.getInstance();
            if (share.storedData == null) share.storedData = new MySFOAuthStoredData();
            share.storedData.accessToken = MySFRestAPI.coordinator.credentials.accessToken;
            share.storedData.refreshToken = MySFRestAPI.coordinator.credentials.refreshToken;
            share.storedData.instanceUrl = MySFRestAPI.coordinator.credentials.instanceUrl.AbsoluteUri;
            serializer.WriteObject(stream, share.storedData);
            await stream.FlushAsync();
        }
    }
}
