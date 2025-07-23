using CurlImpClient.Native;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CurlImpClient;

public class CurlSession : IDisposable
{
   private IntPtr handle;
   private IntPtr headers = IntPtr.Zero;

   private readonly WriteCallback writeCallback;
   private readonly StringBuilder responseBuilder = new();

   private delegate UIntPtr WriteCallback(IntPtr ptr, UIntPtr size, UIntPtr nmemb, IntPtr userdata);

   public CurlSession(string browser = "chrome116", string cookieFile = ":memory:")
   {
      handle = NativeMethods.curl_easy_init();
      if (handle == IntPtr.Zero)
      {
         throw new Exception("curl_easy_init failed");
      }

      // Браузерный JA3/JA4/ClientHello fingerprint
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_impersonate(handle, browser, 1));

      // Общие настройки
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_FOLLOWLOCATION, 1));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_COOKIEFILE, cookieFile));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_COOKIEJAR, cookieFile));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_TIMEOUT, 30));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_CAINFO, "curl-ca-bundle.crt"));
      SetSslVerify(true);

      writeCallback = WriteMemoryCallback;
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_WRITEFUNCTION, writeCallback));
   }

   public void SetUserAgent(string userAgent) => NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_USERAGENT, userAgent));

   public void SetProxy(string proxy) => NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_PROXY, proxy));

   public void SetPostData(string data) => NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_POSTFIELDS, data));

   public void AddHeader(string header) => headers = NativeMethods.curl_slist_append(headers, header);

   public void AddHeaders(Dictionary<string, string> headersDict)
   {
      foreach (var kv in headersDict)
      {
         AddHeader($"{kv.Key}: {kv.Value}");
      }
   }

   public void SetSslVerify(bool enabled)
   {
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_SSL_VERIFYPEER, enabled ? 1 : 0));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_SSL_VERIFYHOST, enabled ? 2 : 0));
   }

   public void SetCaFile(string path) => NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_CAINFO, path));

   private void ApplyAdvancedFingerprint()
   {
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_HTTP2_PSEUDO_HEADERS_ORDER, "masp"));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_HTTP2_SETTINGS, "1:65536;3:1000;4:6291456;6:262144"));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_HTTP2_WINDOW_UPDATE, 15663105));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_SSL_PERMUTE_EXTENSIONS, 1));
   }

   public string BeginRequest(string method, string url)
   {
      responseBuilder.Clear();

      ApplyAdvancedFingerprint();

      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_CUSTOMREQUEST, method.ToUpperInvariant()));
      NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_URL, url));

      if (headers != IntPtr.Zero)
      {
         NativeMethods.ThrowIfError(NativeMethods.curl_easy_setopt(handle, CURLoption.CURLOPT_HTTPHEADER, headers));
      }

      NativeMethods.ThrowIfError(NativeMethods.curl_easy_perform(handle));

      if (headers != IntPtr.Zero)
      {
         NativeMethods.curl_slist_free_all(headers);
         headers = IntPtr.Zero;
      }

      return responseBuilder.ToString();
   }

   private UIntPtr WriteMemoryCallback(IntPtr ptr, UIntPtr size, UIntPtr nmemb, IntPtr userdata)
   {
      var realSize = (int)(size.ToUInt64() * nmemb.ToUInt64());
      var buffer = new byte[realSize];
      Marshal.Copy(ptr, buffer, 0, realSize);
      responseBuilder.Append(Encoding.UTF8.GetString(buffer));

      return (UIntPtr)(ulong)realSize;
   }

   public void Dispose()
   {
      if (headers != IntPtr.Zero)
      {
         NativeMethods.curl_slist_free_all(headers);
         headers = IntPtr.Zero;
      }

      if (handle != IntPtr.Zero)
      {
         NativeMethods.curl_easy_cleanup(handle);
         handle = IntPtr.Zero;
      }
   }
}