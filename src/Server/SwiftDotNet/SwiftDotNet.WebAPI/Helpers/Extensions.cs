using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;

namespace SwiftDotNet.WebAPI.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// This method is used for DateTime properties to convert the date 
        /// to an Epoch date that can be used for DocumentDB range indexes.
        /// See this article for more information: 
        /// http://azure.microsoft.com/blog/2014/11/19/working-with-dates-in-azure-documentdb-4/
        /// </summary>
        /// <param name="date">The incoming date to convert to epoch integer.</param>
        /// <returns></returns>
        public static int ToEpoch(this DateTime date)
        {
            if (date == null) return int.MinValue;
            DateTime epoch = new DateTime(1970, 1, 1);
            TimeSpan epochTimeSpan = date - epoch;
            return (int)epochTimeSpan.TotalSeconds;
        }

        /// <summary>
        /// Handy function to remove a suffix from the end of a string (ie: Urls).
        /// </summary>
        /// <param name="s"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string RemoveFromEnd(this string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }
            else
            {
                return s;
            }
        }

        public static void RemoveAt<T>(ref T[] arr, int index)
        {
            for (int a = index; a < arr.Length - 1; a++)
            {
                arr[a] = arr[a + 1];
            }
            Array.Resize(ref arr, arr.Length - 1);
        }

        public static string RemoveBetween(string s, char begin, char end)
        {
            Regex regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
            return regex.Replace(s, string.Empty);
        }

        public static string ExtractString(string s, string begin, string end)
        {
            int startIndex = s.IndexOf(begin) + begin.Length;
            int endIndex = s.IndexOf(end, startIndex);
            return s.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// Format Body of an Email to Send via Mail Service.
        /// </summary>
        /// <param name="contactName"></param>
        /// <param name="contactEmail"></param>
        /// <param name="contactMessage"></param>
        /// <returns></returns>
        public static string FormatBody(string contactName, string contactEmail, string contactMessage)
        {
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter writer = new HtmlTextWriter(stringWriter);

            //<html>
            writer.RenderBeginTag(HtmlTextWriterTag.Html);

            // <body>
            writer.RenderBeginTag(HtmlTextWriterTag.Body);


            //Timestamp
            writer.RenderBeginTag(HtmlTextWriterTag.Font);
            writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "12px");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Color, "gray");
            writer.RenderBeginTag(HtmlTextWriterTag.B);
            writer.Write(string.Format("Timestamp: {0} EST", DateTime.UtcNow.Subtract(new TimeSpan(5, 0, 0))));
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.Write("<br/>");

            //Message
            writer.RenderBeginTag(HtmlTextWriterTag.Font);
            writer.AddStyleAttribute(HtmlTextWriterStyle.FontSize, "14px");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Color, "black");
            writer.RenderBeginTag(HtmlTextWriterTag.B);
            writer.Write(string.Format("Contact Name: {0}", contactName));
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.Write("<br/>");

            writer.Write(string.Format("Contact Email: {0}", contactEmail));
            writer.Write("<br/>");

            writer.Write(string.Format("Message: {0}", contactMessage));
            writer.Write("<br/>");



            //</body>
            writer.RenderEndTag();

            // </html>
            writer.RenderEndTag();

            return stringWriter.ToString();
        }
    }
}
