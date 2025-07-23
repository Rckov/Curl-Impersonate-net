using CurlImpClient.Enums;
using CurlImpClient.Exceptions;

using System;
using System.Runtime.InteropServices;

namespace CurlImpClient.Natives;

internal static class NativeMethods
{
   private const string Dll = "libcurl";

   [DllImport(Dll)]
   public static extern IntPtr curl_easy_init();

   [DllImport(Dll)]
   public static extern void curl_easy_cleanup(IntPtr handle);

   [DllImport(Dll)]
   public static extern CURLcode curl_easy_perform(IntPtr handle);

   [DllImport(Dll)]
   public static extern CURLcode curl_easy_impersonate(IntPtr handle, string target, int headers);

   [DllImport(Dll)]
   public static extern CURLcode curl_easy_setopt(IntPtr handle, CURLoption option, string value);

   [DllImport(Dll)]
   public static extern CURLcode curl_easy_setopt(IntPtr handle, CURLoption option, int value);

   [DllImport(Dll)]
   public static extern CURLcode curl_easy_setopt(IntPtr handle, CURLoption option, long value);

   [DllImport(Dll)]
   public static extern CURLcode curl_easy_setopt(IntPtr handle, CURLoption option, IntPtr value);

   [DllImport(Dll)]
   public static extern CURLcode curl_easy_setopt(IntPtr handle, CURLoption option, Delegate callback);

   [DllImport(Dll)]
   public static extern IntPtr curl_slist_append(IntPtr list, string header);

   [DllImport(Dll)]
   public static extern void curl_slist_free_all(IntPtr list);

   /// <summary>
   /// Throws a <see cref="CurlException"/> if the CURL operation did not succeed.
   /// </summary>
   /// <param name="code">The CURL error code to check.</param>
   public static void ThrowIfError(CURLcode code)
   {
      if (code != CURLcode.CURLE_OK)
      {
         throw new CurlException(code);
      }
   }
}