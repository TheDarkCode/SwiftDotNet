using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Partitioning;
using SwiftDotNet.WebAPI.Models;
using SwiftDotNet.WebAPI.Helpers;
using SwiftDotNet.WebAPI.PartitionResolvers;

namespace SwiftDotNet.WebAPI.Initializers
{
    /// <summary>
    /// Partition Initializers contains a collection of methods to initialize different partition resolvers
    /// to use such things as hash partitioning, lookup partitioning, and range partitioning.
    /// </summary>
    public class PartitionInitializers
    {
        /// <summary>
        /// The defaultOfferType is set in the config file, ie: S1, S2, S3. Will be used if no DocumentCollectionSpec is included for ManagedHashResolver.
        /// </summary>
        public static string defaultOfferType = AppSettingsConfig.DefaultOfferType;

        /// <summary>
        /// Initialize a HashPartitionResolver.
        /// </summary>
        /// <param name="partitionKeyPropertyName">The property name to be used as the partition Key.</param>
        /// <param name="client">The DocumentDB client instance to use.</param>
        /// <param name="database">The database to run the samples on.</param>
        /// <param name="collectionNames">The names of collections used.</param>
        /// <returns>The created HashPartitionResolver.</returns>
        public static async Task<HashPartitionResolver> InitializeHashResolver(string partitionKeyPropertyName, DocumentClient client, Database database, string[] collectionNames)
        {
            // Set local to input.
            string[] CollectionNames = collectionNames;
            int numCollectionNames = CollectionNames.Length;

            // Create array of DocumentCollections.
            DocumentCollection[] collections = new DocumentCollection[numCollectionNames];

            // Create string array of Self Links to Collections.
            string[] selfLinks = new string[numCollectionNames];

            //Create some collections to partition data.
            for (int i = 0; i < numCollectionNames; i++)
            {
                collections[i] = await DocumentClientHelper.GetCollectionAsync(client, database, CollectionNames[i]);
                selfLinks[i] = collections[i].SelfLink;
            }

            // Join Self Link Array into a comma separated string.
            string selfLinkString = String.Join(", ", selfLinks);

            //Initialize a partition resolver that users hashing, and register with DocumentClient. 
            //Uses User Id for PartitionKeyPropertyName, could also be TenantId, or any other variable.
            HashPartitionResolver hashResolver = new HashPartitionResolver(partitionKeyPropertyName, new[] { selfLinkString });
            client.PartitionResolvers[database.SelfLink] = hashResolver;

            return hashResolver;
        }

        /// <summary>
        /// Initialize a RangePartitionResolver.
        /// </summary>
        /// <param name="partitionKeyPropertyName">The property name to be used as the partition Key.</param>
        /// <param name="client">The DocumentDB client instance to use.</param>
        /// <param name="database">The database to run the samples on.</param>
        /// <param name="collectionNames">The names of collections used.</param>
        /// <returns>The created HashPartitionResolver.</returns>
        public static async Task<RangePartitionResolver<string>> InitializeRangeResolver(string partitionKeyPropertyName, DocumentClient client, Database database, string[] collectionNames)
        {
            // Set local to input.
            string[] CollectionNames = collectionNames;
            int numCollectionNames = CollectionNames.Length;

            // Create array of DocumentCollections.
            DocumentCollection[] collections = new DocumentCollection[numCollectionNames];

            // Create string array of Self Links to Collections.
            string[] selfLinks = new string[numCollectionNames];

            //Create some collections to partition data.
            for (int i = 0; i < numCollectionNames; i++)
            {
                collections[i] = await DocumentClientHelper.GetCollectionAsync(client, database, CollectionNames[i]);
                selfLinks[i] = collections[i].SelfLink;
            }

            // Join Self Link Array into a comma separated string.
            string selfLinkString = String.Join(", ", selfLinks);

            // If each collection represents a range, it will have both a start and end point in its range, thus there are 2 rangeNames for each collection.
            string[] rangeNames = new string[numCollectionNames * 2];

            // Keeping track of where in the rangeNames array to add a new start/end of a range.
            int currentRangePosition = 0;

            for (int y = 0; y < numCollectionNames; y++)
            {
                string[] rangeTemp = collectionNames[y].Split('-');
                for (int z = 0; z < rangeTemp.Length; z++)
                {
                    rangeNames[currentRangePosition] = rangeTemp[z];
                    currentRangePosition++;
                }
            }

            // Dictionary needed to input into RangePartitionResolver with corresponding range and collection self link.
            Dictionary<Range<string>, string> rangeCollections = new Dictionary<Range<string>, string>();

            // TO DO:: Iterate through the ranges and add then to rangeCollections with the appropriate start/end point, with the corresponding collection self link.
            //rangeCollections.Add()

            RangePartitionResolver<string> rangeResolver = new RangePartitionResolver<string>(
                partitionKeyPropertyName,
                rangeCollections);

            client.PartitionResolvers[database.SelfLink] = rangeResolver;
            return rangeResolver;
        }

        /// <summary>
        /// Initialize a HashPartitionResolver that uses a custom function to extract the partition key.
        /// </summary>
        /// <param name="partitionKeyExtractor">The partition key extractor function.</param>
        /// <param name="client">The DocumentDB client instance to use.</param>
        /// <param name="database">The database to run the samples on.</param>
        /// <param name="collectionNames">The names of collections used.</param>
        /// <returns>The created HashPartitionResolver.</returns>
        public static async Task<HashPartitionResolver> InitializeCustomHashResolver(Func<object, string> partitionKeyExtractor, DocumentClient client, Database database, string[] collectionNames)
        {
            // Set local to input.
            string[] CollectionNames = collectionNames;
            int numCollectionNames = CollectionNames.Length;

            // Create array of DocumentCollections.
            DocumentCollection[] collections = new DocumentCollection[numCollectionNames];

            // Create string array of Self Links to Collections.
            string[] selfLinks = new string[numCollectionNames];

            //Create some collections to partition data.
            for (int i = 0; i < numCollectionNames; i++)
            {
                collections[i] = await DocumentClientHelper.GetCollectionAsync(client, database, CollectionNames[i]);
                selfLinks[i] = collections[i].SelfLink;
            }

            // Join Self Link Array into a comma separated string.
            string selfLinkString = String.Join(", ", selfLinks);

            var hashResolver = new HashPartitionResolver(
                partitionKeyExtractor,
                new[] { selfLinkString });

            client.PartitionResolvers[database.SelfLink] = hashResolver;
            return hashResolver;
        }

        /// <summary>
        /// Initialize a LookupPartitionResolver. Default is for US East/West to go to first collection name, Europe to second, and AsiaPacific / Other to third.
        /// </summary>
        /// <param name="partitionKeyPropertyName">The property name to be used as the partition Key.</param>
        /// <param name="client">The DocumentDB client instance to use.</param>
        /// <param name="database">The database to run the samples on.</param>
        /// <param name="collectionNames">The names of collections used.</param>
        /// <returns>The created HashPartitionResolver.</returns>
        public static async Task<LookupPartitionResolver<string>> InitializeLookupPartitionResolver(string partitionKeyPropertyName, DocumentClient client, Database database, string[] collectionNames)
        {

            // Set local to input.
            string[] CollectionNames = collectionNames;
            int numCollectionNames = CollectionNames.Length;

            // Create array of DocumentCollections.
            DocumentCollection[] collections = new DocumentCollection[numCollectionNames];

            // Create string array of Self Links to Collections.
            string[] selfLinks = new string[numCollectionNames];

            //Create some collections to partition data.
            for (int i = 0; i < numCollectionNames; i++)
            {
                collections[i] = await DocumentClientHelper.GetCollectionAsync(client, database, CollectionNames[i]);
                selfLinks[i] = collections[i].SelfLink;
            }

            // This implementation takes strings as input. If you'd like to implement a strongly typed LookupPartitionResolver, 
            // take a look at EnumLookupPartitionResolver for an example.
            var lookupResolver = new LookupPartitionResolver<string>(
                partitionKeyPropertyName,
                new Dictionary<string, string>()
                {
                    { Region.UnitedStatesEast.ToString(), selfLinks[0] },
                    { Region.UnitedStatesWest.ToString(), selfLinks[0] },
                    { Region.Europe.ToString(), selfLinks[1] },
                    { Region.AsiaPacific.ToString(), selfLinks[2] },
                    { Region.Other.ToString(), selfLinks[2] },
                });

            client.PartitionResolvers[database.SelfLink] = lookupResolver;
            return lookupResolver;
        }

        /// <summary>
        /// Initialize a "managed" HashPartitionResolver that also takes care of creating collections, and cloning collection properties like
        /// stored procedures, offer type and indexing policy.
        /// </summary>
        /// <param name="partitionKeyExtractor">The partition key extractor function. (Ex: "u => ((UserProfile)u).UserId")</param>
        /// <param name="client">The DocumentDB client instance to use.</param>
        /// <param name="database">The database to run the samples on.</param>
        /// <param name="numCollections">The number of collections to be used.</param>
        /// <param name="collectionSpec">The DocumentCollectionSpec to be used for each collection created. If null returns Spec with defaultOfferType as set in config.</param>
        /// <returns>The created HashPartitionResolver.</returns>
        public static ManagedHashPartitionResolver InitializeManagedHashResolver(Func<object, string> partitionKeyExtractor, DocumentClient client, Database database, int numCollections, DocumentCollectionSpec collectionSpec)
        {
            // If no collectionSpec used, use a default spec with the offerType (ie: S1, S2, S3) that is specified in config.
            if (collectionSpec == null)
            {
                var hashResolver = new ManagedHashPartitionResolver(partitionKeyExtractor, client, database, numCollections, null, new DocumentCollectionSpec { OfferType = defaultOfferType });
                client.PartitionResolvers[database.SelfLink] = hashResolver;
                return hashResolver;
            }
            // If there is a collectionSpec passed as a parameter, use that instead of default.
            else
            {
                var hashResolver = new ManagedHashPartitionResolver(partitionKeyExtractor, client, database, numCollections, null, collectionSpec);
                client.PartitionResolvers[database.SelfLink] = hashResolver;
                return hashResolver;
            }
        }
    }
}
