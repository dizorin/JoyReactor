﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace JoyReactor.Core.Model.Web
{
	static class WebDownloaderExtensions
	{
		[Obsolete]
		public static DocumentReponse GetDocument (this IWebDownloader instance, Uri uri, RequestParams reqParams = null)
		{
			using (var response = instance.ExecuteAsync (uri, reqParams).Result) {
				var doc = new HtmlDocument ();
				doc.Load (response.Data);
				return new DocumentReponse { Document = doc, Cookies = response.Cookies };
			}
		}

		public static async Task<HtmlDocument> GetDocumentAsync (this IWebDownloader instance, Uri uri, RequestParams reqParams = null)
		{
			using (var response = await instance.ExecuteAsync (uri, reqParams)) {
				var doc = new HtmlDocument ();
				await Task.Run (() => doc.Load (response.Data));
				return doc;
			}
		}

        public static async Task PostAsync(this IWebDownloader instance, Uri uri, RequestParams requestParams)
        {
            using (var response = await instance.ExecuteAsync(uri, requestParams)) { }
        }

        [Obsolete]
		public static Stream GetResource (this IWebDownloader instance, Uri uri, RequestParams reqParams = null)
		{
			return instance.ExecuteAsync (uri, reqParams).Result.Data;
		}

		[Obsolete]
		public static IDictionary<string, string> PostForCookies (this IWebDownloader instance, Uri uri, RequestParams reqParams = null)
		{
			if (reqParams == null || reqParams.Form == null)
				throw new Exception ();
			using (var response = instance.ExecuteAsync (uri, reqParams).Result) {
				return response.Cookies;
			}
		}

        public static async Task<IDictionary<string, string>> PostForCookiesAsync (this IWebDownloader instance, Uri uri, RequestParams reqParams = null)
        {
            if (reqParams == null || reqParams.Form == null)
                throw new Exception ();
            using (var response = await instance.ExecuteAsync (uri, reqParams)) {
                return response.Cookies;
            }
        }

		[Obsolete]
		public static string GetText (this IWebDownloader instance, Uri uri, RequestParams reqParams = null)
		{
			using (var response = instance.ExecuteAsync (uri, reqParams).Result) {
				return new StreamReader (response.Data).ReadToEnd ();
			}
		}

        public static async Task<string> GetTextAsync (this IWebDownloader instance, Uri uri, RequestParams reqParams = null)
        {
            using (var response = await instance.ExecuteAsync (uri, reqParams)) {
                return await new StreamReader(response.Data).ReadToEndAsync();
            }
        }

		[Obsolete]
		public static HtmlDocument Get (this IWebDownloader instance, Uri uri)
		{
			using (var response = instance.ExecuteAsync (uri).Result) {
				var doc = new HtmlDocument ();
				doc.Load (response.Data);
				return doc;
			}
		}
	}

	public class DocumentReponse
	{
		public HtmlDocument Document { get; set; }

		public IDictionary<string, string> Cookies { get; set; }
	}
}