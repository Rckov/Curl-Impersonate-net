using CurlImpClient.Exceptions;

using System;
using System.Runtime.InteropServices;

namespace CurlImpClient.Native;

internal static class NativeMethods
{
   private const string Lib = "libcurl";

   [DllImport(Lib)]
   public static extern IntPtr curl_easy_init();

   [DllImport(Lib)]
   public static extern void curl_easy_cleanup(IntPtr handle);

   [DllImport(Lib)]
   public static extern CURLcode curl_easy_perform(IntPtr handle);

   [DllImport(Lib)]
   public static extern CURLcode curl_easy_impersonate(IntPtr handle, string target, int headers);

   [DllImport(Lib)]
   public static extern CURLcode curl_easy_setopt(IntPtr handle, CURLoption option, string value);

   [DllImport(Lib)]
   public static extern CURLcode curl_easy_setopt(IntPtr handle, CURLoption option, int value);

   [DllImport(Lib)]
   public static extern CURLcode curl_easy_setopt(IntPtr handle, CURLoption option, long value);

   [DllImport(Lib)]
   public static extern CURLcode curl_easy_setopt(IntPtr handle, CURLoption option, IntPtr value);

   [DllImport(Lib)]
   public static extern CURLcode curl_easy_setopt(IntPtr handle, CURLoption option, Delegate callback);

   [DllImport(Lib)]
   public static extern IntPtr curl_slist_append(IntPtr list, string header);

   [DllImport(Lib)]
   public static extern void curl_slist_free_all(IntPtr list);

   public static void ThrowIfError(CURLcode code)
   {
      if (code != CURLcode.CURLE_OK)
      {
         throw new CurlException(code);
      }
   }
}