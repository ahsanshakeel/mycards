using System.Net.Http.Headers;
using Microsoft.Graph;
using MyCard.Web.Security;

namespace MyCard.Web.Security
{
    public class SDKHelper
    {

        // Get an authenticated Microsoft Graph Service client.
        public static GraphServiceClient GetAuthenticatedClient()
        {
            GraphServiceClient graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        string accessToken = await MyCardAuthProvider.Instance.GetUserAccessTokenAsync();

                        // Append the access token to the request.
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                        // This header has been added to identify our sample in the Microsoft Graph service. If extracting this code for your project please remove.
                        //requestMessage.Headers.Add("SampleID", "aspnet-connect-sample");
                    }));
            return graphClient;
        }
    }
}