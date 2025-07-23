using CurlImpClient;
using CurlImpClient.Exceptions;

using HttpMethod = CurlImpClient.Enums.HttpMethod;

internal class Program
{
   private const string Proxy = "http://192.160.0.1/handler";
   private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36";

   private static void Main()
   {
      try
      {
         using var session = new CurlSession(new CurlSessionConfig
         {
            TimeoutSeconds = 10,
            EnableHttp2Options = false
         });

         session.SetProxy(Proxy);
         session.SetUserAgent(UserAgent);

         session.AddHeader("Accept: application/json");
         session.AddHeader("Accept-Language: en-US,en;q=0.9");

         var response = session.ExecuteRequest(HttpMethod.Get, "https://tls.browserleaks.com/json");
         Console.WriteLine(response);
      }
      catch (CurlException ex)
      {
         Console.WriteLine($"CURL error occurred: {ex.Message}");
      }
      catch (Exception ex)
      {
         Console.WriteLine($"Unexpected error: {ex.Message}");
      }
   }
}