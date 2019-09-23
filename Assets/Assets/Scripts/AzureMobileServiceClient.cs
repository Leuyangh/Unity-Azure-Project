using System.Net;
using System.Net.Http;
using Microsoft.WindowsAzure.MobileServices;

public static class AzureMobileServiceClient
{
    // Unity versions below 2018.2 must use the http:// endpoint here, not the https:// endpoint.
    private const string backendUrl = "https://leuyanghtestapp.azurewebsites.net";
    private static MobileServiceClient client;

    public static MobileServiceClient Client
    {
        get
        {
            if (client == null)
            {
#if UNITY_ANDROID
                // Android builds fail at runtime due to missing GZip support, so build a handler that uses Deflate for Android
                var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate };
                client = new MobileServiceClient(backendUrl, handler);
#else
                client = new MobileServiceClient(backendUrl);
#endif
            }

            return client;
        }
    }
}
