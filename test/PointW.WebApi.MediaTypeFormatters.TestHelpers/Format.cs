﻿using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Resources;
using System.Text;
using System.Threading;
using System.Web.Http;
using PointW.WebApi.ResourceModel;

namespace PointW.WebApi.MediaTypeFormatters.TestHelpers
{
    public class Format
    {
        public static string FormatObject(object toFormat, JsonMediaTypeFormatter formatter)
        {
            string result;
            using (var stream = new MemoryStream())
            {
                formatter.WriteToStream(toFormat.GetType(), toFormat, stream, new UTF8Encoding());
                stream.Seek(0, SeekOrigin.Begin);
                result = new StreamReader(stream).ReadToEnd();
            }
            Console.WriteLine(result);
            return result;
        }



        public static TResource PerformRoundTrip<TResource>(object o, JsonMediaTypeFormatter formatter) where TResource : class
        {
            var json = FormatObject(o, formatter);

            var stream = GenerateStreamFromString(json);
            var content = new StreamContent(stream);

            return formatter.ReadFromStreamAsync(typeof(TResource), stream, content, null).Result as TResource;
        }



        public static HttpResponseMessage GetResponseFromAction(IHttpActionResult action)
        {
            var ct = new CancellationToken();
            return action.ExecuteAsync(ct).Result;
        }



        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
