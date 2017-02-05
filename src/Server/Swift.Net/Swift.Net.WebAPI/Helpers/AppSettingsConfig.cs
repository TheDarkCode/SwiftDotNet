using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swift.Net.WebAPI.Helpers
{
    class AppSettingsConfig
    {
        /// <summary>
        /// The comma separated string of whitelisted IPs for the API.
        /// </summary>
        public static string IPWhitelist { get { return ConfigurationManager.AppSettings["ipWhitelist"]; } }

        /// <summary>
        /// The name of the DocumentDB database.
        /// </summary>
        public static string Db { get { return ConfigurationManager.AppSettings["db"]; } }

        /// <summary>
        /// The default offer type to set new collections. Ie: S1, S2, S3
        /// </summary>
        public static string DefaultOfferType { get { return ConfigurationManager.AppSettings["defaultOfferType"]; } }

        /// <summary>
        /// The DocumentDB collection to store User information.
        /// </summary>
        public static string UserCollection { get { return ConfigurationManager.AppSettings["userCollection"]; } }

        /// <summary>
        /// The DocumentDB collection (name) for storing main set of documents.
        /// You can add additional settings using this template to pass them into new custom repositories.
        /// Simply add the corresponding field to the web.config or update in the Azure Portal's app settings.
        /// Alternatively, if you want to specify a collection name directly in the repository, you can do that by
        /// passing the string directly in quotes.
        /// </summary>
        public static string MainCollection { get { return ConfigurationManager.AppSettings["mainCollection"]; } }

        /// <summary>
        /// The DocumentDB collection (name) used as a partitioned collection.
        /// </summary>
        public static string PartitionedCollection { get { return ConfigurationManager.AppSettings["partitionedCollection"]; } }

        /// <summary>
        /// The DocumentDB endpoint Uri.
        /// </summary>
        public static string EndPoint { get { return ConfigurationManager.AppSettings["endpoint"]; } }

        /// <summary>
        /// The DocumentDB authorization key.
        /// </summary>
        public static string AuthKey { get { return ConfigurationManager.AppSettings["authkey"]; } }

        /// <summary>
        /// The comma-separated list of domains, with no trailing slash!
        /// </summary>
        public static string CorsPolicyOrigins { get { return ConfigurationManager.AppSettings["CorsPolicyOrigins"]; } }

        /// <summary>
        /// The base Url for the host website that is calling the WebApi service.
        /// </summary>
        public static string ClientSite { get { return ConfigurationManager.AppSettings["clientSite"]; } }

        /// <summary>
        /// The SendGrid mail account for your accress credentials.
        /// </summary>
        public static string MailAccount { get { return ConfigurationManager.AppSettings["mailAccount"]; } }

        /// <summary>
        /// The SendGrid password for your access credentials.
        /// </summary>
        public static string MailPassword { get { return ConfigurationManager.AppSettings["mailPassword"]; } }

        /// <summary>
        /// The SendGrid From address used for emails to users.
        /// </summary>
        public static string MailFromAddress { get { return ConfigurationManager.AppSettings["mailPassword"]; } }

        /// <summary>
        /// The Name used in all Sent Emails.
        /// </summary>
        public static string MailFromName { get { return ConfigurationManager.AppSettings["mailFromName"]; } }

        /// <summary>
        /// Twilio Sid used in SMS messages.
        /// </summary>
        public static string TwilioSid { get { return ConfigurationManager.AppSettings["twilioSid"]; } }

        /// <summary>
        /// Twilio Token used for verifying with SMS service.
        /// </summary>
        public static string TwilioToken { get { return ConfigurationManager.AppSettings["twilioToken"]; } }

        /// <summary>
        /// Twilio From Phone (the phone number to send text messages).
        /// </summary>
        public static string TwilioFromPhone { get { return ConfigurationManager.AppSettings["twilioFromPhone"]; } }

        /// <summary>
        /// Blob Storage Connection String
        /// </summary>
        public static string BlobStorageConnectionString { get { return ConfigurationManager.AppSettings["blobStorageConnectionString"]; } }

        /// <summary>
        /// Blob Storage Container Name
        /// </summary>
        public static string BlobStorageContainerName { get { return ConfigurationManager.AppSettings["blobStorageContainerName"]; } }
    }
}
