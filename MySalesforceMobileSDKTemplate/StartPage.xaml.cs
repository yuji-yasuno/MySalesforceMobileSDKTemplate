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

namespace MySalesforceMobileSDKTemplate
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
            
            MySFRestAPI api = MySFRestAPI.getInstance();
            api.apiVersion = apiVersion;
            if (api.coordinator == null) {
                api.coordinator = new MySFOAuthCoordinator(clientId, redirectUrl, scope);
            }
            api.coordinator.onCompletedAuthorization += coordinator_onCompletedAuthorization;
            api.coordinator.onFailedAuthorization += coordinator_onFailedAuthorization;
            api.coordinator.onCanceledAuthorization += coordinator_onCanceledAuthorization;
            api.coordinator.onCompletedRefresh += coordinator_onCompletedRefresh;
            api.coordinator.onFailedRefresh += coordinator_onFailedRefresh;
            api.coordinator.onRequestFailedRefresh += coordinator_onRequestFailedRefresh;
            api.coordinator.onCompletedRevokeToken += coordinator_onCompletedRevokeToken;
            api.coordinator.onRequestFailedRevokeToken += coordinator_onRequestFailedRevokeToken;
            api.coordinator.onFailedRevokeToken += coordinator_onFailedRevokeToken;

            if (!isNeedOAuthenticate)
            {
                api.coordinator.credentials.accessToken = share.storedData.accessToken;
                api.coordinator.credentials.refreshToken = share.storedData.refreshToken;
                api.coordinator.credentials.instanceUrl = new Uri(share.storedData.instanceUrl);
                api.coordinator.credentials.domain = api.coordinator.credentials.instanceUrl.Authority;
                api.coordinator.refresh();
            }
            else 
            {
                api.coordinator.authenticate();
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
            MySFRestAPI api = MySFRestAPI.getInstance();
            api.coordinator.onCompletedAuthorization -= coordinator_onCompletedAuthorization;
            api.coordinator.onFailedAuthorization -= coordinator_onFailedAuthorization;
            api.coordinator.onCanceledAuthorization -= coordinator_onCanceledAuthorization;
            api.coordinator.onCompletedRefresh -= coordinator_onCompletedRefresh;
            api.coordinator.onFailedRefresh -= coordinator_onFailedRefresh;
            api.coordinator.onRequestFailedRefresh -= coordinator_onRequestFailedRefresh;
            api.coordinator.onCompletedRevokeToken -= coordinator_onCompletedRevokeToken;
            api.coordinator.onRequestFailedRevokeToken -= coordinator_onRequestFailedRevokeToken;
            api.coordinator.onFailedRevokeToken -= coordinator_onFailedRevokeToken;

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
            share.storedData.accessToken = api.coordinator.credentials.accessToken;
            share.storedData.refreshToken = api.coordinator.credentials.refreshToken;
            share.storedData.instanceUrl = api.coordinator.credentials.instanceUrl.AbsoluteUri;
            serializer.WriteObject(stream, share.storedData);
            await stream.FlushAsync();
        }
    }
}
