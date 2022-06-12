using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ShoppingStore.Infrastructure.Contracts;

namespace ShoppingStore.Infrastructure
{
    public class SmsSender : ISmsSender
    {
        public async Task<string> SendAuthSmsAsync(string Code, string PhoneNumber)
        {
            HttpClient httpClient = new HttpClient();
            var httpResponse = await httpClient.GetAsync($"https://api.kavenegar.com/v1/989898989898989898/verify/lookup.json?receptor={PhoneNumber}&token={Code}&template=AuthVerify");

            if (httpResponse.StatusCode == HttpStatusCode.OK)
                return "Success";
            else
                return "Failed";
        }
    }
}
