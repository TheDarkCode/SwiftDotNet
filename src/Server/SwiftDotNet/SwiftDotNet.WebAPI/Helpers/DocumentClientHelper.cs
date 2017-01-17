using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace SwiftDotNet.WebAPI.Helpers
{
    /// <summary>
    /// Providers common helper methods for working with DocumentClient.
    /// </summary>
    public class DocumentClientHelper
    {
        public static string defaultOfferType = AppSettingsConfig.DefaultOfferType;

        /// <summary>
        /// Get a Database by id, or create a new one if one with the id provided doesn't exist.
        /// </summary>
        /// <param name="client">The DocumentDB client instance.</param>
        /// <param name="id">The id of the Database to search for, or create.</param>
        /// <returns>The matched, or created, Database object</returns>
        public static async Task<Database> GetDatabaseAsync(DocumentClient client, string id)
        {
            Database database = client.CreateDatabaseQuery().Where(db => db.Id == id).ToArray().FirstOrDefault();
            if (database == null)
            {
                database = await client.CreateDatabaseAsync(new Database { Id = id });
            }

            return database;
        }

        /// <summary>
        /// Get a Database by id, or create a new one if one with the id provided doesn't exist.
        /// </summary>
        /// <param name="client">The DocumentDB client instance.</param>
        /// <param name="id">The id of the Database to search for, or create.</param>
        /// <returns>The matched, or created, Database object</returns>
        private static async Task<Database> GetOrCreateDatabaseAsync(DocumentClient client, string id)
        {
            // Get the database by name, or create a new one if one with the name provided doesn't exist.
            // Create a query object for database, filter by name.
            IEnumerable<Database> query = from db in client.CreateDatabaseQuery()
                                          where db.Id == id
                                          select db;

            // Run the query and get the database (there should be only one) or null if the query didn't return anything.
            // Note: this will run synchronously. If async exectution is preferred, use IDocumentServiceQuery<T>.ExecuteNextAsync.
            Database database = query.FirstOrDefault();
            if (database == null)
            {
                // Create the database.
                database = await client.CreateDatabaseAsync(new Database { Id = id });
            }

            return database;
        }

        /// <summary>
        /// This method uses a ReadDatabaseFeedAsync method to read a list of all databases on the account.
        /// It demonstrates a pattern for how to control paging and deal with continuations
        /// This should not be needed for reading a list of databases as there are unlikely to be many hundred,
        /// but this same pattern is introduced here and can be used on other ReadFeed methods.
        /// </summary>
        /// <param name="client">The DocumentDB client instance.</param>
        /// <returns>A List of Database entities</returns>
        private static async Task<List<Database>> ListDatabasesAsync(DocumentClient client)
        {
            string continuation = null;
            List<Database> databases = new List<Database>();

            do
            {
                FeedOptions options = new FeedOptions
                {
                    RequestContinuation = continuation,
                    MaxItemCount = 50
                };

                FeedResponse<Database> response = await client.ReadDatabaseFeedAsync(options);
                foreach (Database db in response)
                {
                    databases.Add(db);
                }

                continuation = response.ResponseContinuation;
            }
            while (!String.IsNullOrEmpty(continuation));

            return databases;
        }

        /// <summary>
        /// Get a Database by id, or create a new one if one with the id provided doesn't exist.
        /// </summary>
        /// <param name="client">The DocumentDB client instance.</param>
        /// <param name="id">The id of the Database to search for, or create.</param>
        /// <returns>The matched, or created, Database object</returns>
        public static async Task<Database> GetNewDatabaseAsync(DocumentClient client, string id)
        {
            Database database = client.CreateDatabaseQuery().Where(db => db.Id == id).ToArray().FirstOrDefault();
            if (database != null)
            {
                await client.DeleteDatabaseAsync(database.SelfLink);
            }

            database = await client.CreateDatabaseAsync(new Database { Id = id });
            return database;
        }

        /// <summary>
        /// Deletes a Database resource
        /// </summary>
        /// <param name="client">The DocumentDB client instance.</param>
        /// <param name="databaseLink">The SelfLink of the Database resource to be deleted</param>
        /// <returns></returns>
        private static async Task DeleteDatabaseAsync(DocumentClient client, string databaseLink)
        {
            await client.DeleteDatabaseAsync(databaseLink);
        }

        /// <summary>
        /// Get a DocumentCollection by id, or create a new one if one with the id provided doesn't exist.
        /// </summary>
        /// <param name="client">The DocumentDB client instance.</param>
        /// <param name="database">The Database where this DocumentCollection exists / will be created</param>
        /// <param name="collectionId">The id of the DocumentCollection to search for, or create.</param>
        /// <returns>The matched, or created, DocumentCollection object</returns>
        public static async Task<DocumentCollection> GetCollectionAsync(DocumentClient client, Database database, string collectionId)
        {
            DocumentCollection collection = client.CreateDocumentCollectionQuery(database.SelfLink)
                .Where(c => c.Id == collectionId).ToArray().FirstOrDefault();

            if (collection == null)
            {
                collection = await CreateDocumentCollectionWithRetriesAsync(client, database, new DocumentCollection { Id = collectionId }, defaultOfferType);
            }

            return collection;
        }

        /// <summary>
        /// Get a DocumentCollection by id, or create a new one if one with the id provided doesn't exist.
        /// </summary>
        /// <param name="client">The DocumentDB client instance.</param>
        /// <param name="database">The Database where this DocumentCollection exists / will be created</param>
        /// <param name="collectionId">The id of the DocumentCollection to search for, or create.</param>
        /// <param name="collectionSpec">The spec/template to create collections from.</param>
        /// <returns>The matched, or created, DocumentCollection object</returns>
        public static async Task<DocumentCollection> GetCollectionAsync(
            DocumentClient client,
            Database database,
            string collectionId,
            DocumentCollectionSpec collectionSpec)
        {
            DocumentCollection collection = client.CreateDocumentCollectionQuery(database.SelfLink)
                .Where(c => c.Id == collectionId).ToArray().FirstOrDefault();

            if (collection == null)
            {
                collection = await CreateNewCollection(client, database, collectionId, collectionSpec);
            }

            return collection;
        }

        /// <summary>
        /// Get a DocumentCollection by id, or create a new one if one with the id provided doesn't exist.
        /// </summary>
        /// <param name="client">The DocumentDB client instance.</param>
        /// <param name="dbLink">The Database SelfLink property where this DocumentCollection exists / will be created</param>
        /// <param name="id">The id of the DocumentCollection to search for, or create.</param>
        /// <returns>The matched, or created, DocumentCollection object</returns>
        private static async Task<DocumentCollection> GetOrCreateCollectionAsync(DocumentClient client, string dbLink, string id)
        {
            DocumentCollection collection = client.CreateDocumentCollectionQuery(dbLink).Where(c => c.Id == id).ToArray().FirstOrDefault();
            if (collection == null)
            {
                collection = await client.CreateDocumentCollectionAsync(dbLink, new DocumentCollection { Id = id });
            }

            return collection;
        }

        /// <summary>
        /// Creates a new collection.
        /// </summary>
        /// <param name="client">The DocumentDB client instance.</param>
        /// <param name="database">The Database where this DocumentCollection exists / will be created</param>
        /// <param name="collectionId">The id of the DocumentCollection to search for, or create.</param>
        /// <param name="collectionSpec">The spec/template to create collections from.</param>
        /// <returns>The created DocumentCollection object</returns>
        public static async Task<DocumentCollection> CreateNewCollection(
            DocumentClient client,
            Database database,
            string collectionId,
            DocumentCollectionSpec collectionSpec)
        {
            DocumentCollection collectionDefinition = new DocumentCollection { Id = collectionId };
            if (collectionSpec != null)
            {
                CopyIndexingPolicy(collectionSpec, collectionDefinition);
            }

            DocumentCollection collection = await CreateDocumentCollectionWithRetriesAsync(
                client,
                database,
                collectionDefinition,
                (collectionSpec != null) ? collectionSpec.OfferType : null);

            if (collectionSpec != null)
            {
                await RegisterScripts(client, collectionSpec, collection);
            }

            return collection;
        }

        /// <summary>
        /// This method uses a ReadCollectionsFeedAsync method to read a list of all collections on a database.
        /// It demonstrates a pattern for how to control paging and deal with continuations
        /// This should not be needed for reading a list of databases as there are unlikely to be many hundred,
        /// but this same pattern is introduced here and can be used on other ReadFeed methods.
        /// </summary>
        /// <param name="client">The DocumentDB client instance.</param>
        /// <param name="databaseSelfLink">The self link for the database.</param>
        /// <returns>A List of Collection entities</returns>
        private static async Task<List<DocumentCollection>> ReadCollectionsFeedAsync(
            DocumentClient client,
            string databaseSelfLink)
        {

            string continuation = null;
            List<DocumentCollection> collections = new List<DocumentCollection>();

            do
            {
                FeedOptions options = new FeedOptions
                {
                    RequestContinuation = continuation,
                    MaxItemCount = 50
                };

                FeedResponse<DocumentCollection> response = (FeedResponse<DocumentCollection>)await client.ReadDocumentCollectionFeedAsync(databaseSelfLink, options);

                foreach (DocumentCollection col in response)
                {
                    collections.Add(col);
                }

                continuation = response.ResponseContinuation;

            } while (!String.IsNullOrEmpty(continuation));

            return collections;
        }

        /// <summary>
        /// Registers the stored procedures, triggers and UDFs in the collection spec/template.
        /// </summary>
        /// <param name="client">The DocumentDB client.</param>
        /// <param name="collectionSpec">The collection spec/template.</param>
        /// <param name="collection">The collection.</param>
        /// <returns>The Task object for asynchronous execution.</returns>
        public static async Task RegisterScripts(DocumentClient client, DocumentCollectionSpec collectionSpec, DocumentCollection collection)
        {
            if (collectionSpec.StoredProcedures != null)
            {
                foreach (StoredProcedure sproc in collectionSpec.StoredProcedures)
                {
                    await client.CreateStoredProcedureAsync(collection.SelfLink, sproc);
                }
            }

            if (collectionSpec.Triggers != null)
            {
                foreach (Trigger trigger in collectionSpec.Triggers)
                {
                    await client.CreateTriggerAsync(collection.SelfLink, trigger);
                }
            }

            if (collectionSpec.UserDefinedFunctions != null)
            {
                foreach (UserDefinedFunction udf in collectionSpec.UserDefinedFunctions)
                {
                    await client.CreateUserDefinedFunctionAsync(collection.SelfLink, udf);
                }
            }
        }

        /// <summary>
        /// Copies the indexing policy from the collection spec.
        /// </summary>
        /// <param name="collectionSpec">The collection spec/template</param>
        /// <param name="collectionDefinition">The collection definition to create.</param>
        public static void CopyIndexingPolicy(DocumentCollectionSpec collectionSpec, DocumentCollection collectionDefinition)
        {
            if (collectionSpec.IndexingPolicy != null)
            {
                collectionDefinition.IndexingPolicy.Automatic = collectionSpec.IndexingPolicy.Automatic;
                collectionDefinition.IndexingPolicy.IndexingMode = collectionSpec.IndexingPolicy.IndexingMode;

                if (collectionSpec.IndexingPolicy.IncludedPaths != null)
                {
                    foreach (IncludedPath path in collectionSpec.IndexingPolicy.IncludedPaths)
                    {
                        collectionDefinition.IndexingPolicy.IncludedPaths.Add(path);
                    }
                }

                if (collectionSpec.IndexingPolicy.ExcludedPaths != null)
                {
                    foreach (ExcludedPath path in collectionSpec.IndexingPolicy.ExcludedPaths)
                    {
                        collectionDefinition.IndexingPolicy.ExcludedPaths.Add(path);
                    }
                }
            }
        }

        /// <summary>
        /// Create a DocumentCollection, and retry when throttled.
        /// </summary>
        /// <param name="client">The DocumentDB client instance.</param>
        /// <param name="database">The database to use.</param>
        /// <param name="collectionDefinition">The collection definition to use.</param>
        /// <param name="offerType">The offer type for the collection.</param>
        /// <returns>The created DocumentCollection.</returns>
        public static async Task<DocumentCollection> CreateDocumentCollectionWithRetriesAsync(
            DocumentClient client,
            Database database,
            DocumentCollection collectionDefinition,
            string offerType)
        {
            return await ExecuteWithRetries(
                client,
                () => client.CreateDocumentCollectionAsync(
                        database.SelfLink,
                        collectionDefinition,
                        new RequestOptions
                        {
                            OfferType = offerType
                        }));
        }

        /// <summary>
        /// Execute the function with retries on throttle.
        /// </summary>
        /// <typeparam name="V">The type of return value from the execution.</typeparam>
        /// <param name="client">The DocumentDB client instance.</param>
        /// <param name="function">The function to execute.</param>
        /// <returns>The response from the execution.</returns>
        public static async Task<V> ExecuteWithRetries<V>(DocumentClient client, Func<Task<V>> function)
        {
            TimeSpan sleepTime = TimeSpan.Zero;

            while (true)
            {
                try
                {
                    return await function();
                }
                catch (DocumentClientException de)
                {
                    if ((int)de.StatusCode != 429)
                    {
                        throw;
                    }

                    sleepTime = de.RetryAfter;
                }
                catch (AggregateException ae)
                {
                    if (!(ae.InnerException is DocumentClientException))
                    {
                        throw;
                    }

                    DocumentClientException de = (DocumentClientException)ae.InnerException;
                    if ((int)de.StatusCode != 429)
                    {
                        throw;
                    }

                    sleepTime = de.RetryAfter;
                }

                await Task.Delay(sleepTime);
            }
        }

        /// <summary>
        /// Bulk import using a stored procedure.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="collection"></param>
        /// <param name="inputDirectory"></param>
        /// <param name="inputFileMask"></param>
        /// <returns></returns>
        public static async Task RunBulkImport(
            DocumentClient client,
            DocumentCollection collection,
            string inputDirectory,
            string inputFileMask = "*.json")
        {
            int maxFiles = 2000;
            int maxScriptSize = 50000;

            // 1. Get the files. 
            string[] fileNames = Directory.GetFiles(inputDirectory, inputFileMask);
            DirectoryInfo di = new DirectoryInfo(inputDirectory);
            FileInfo[] fileInfos = di.GetFiles(inputFileMask);

            int currentCount = 0;
            int fileCount = maxFiles != 0 ? Math.Min(maxFiles, fileNames.Length) : fileNames.Length;

            string body = File.ReadAllText(@".\JS\BulkImport.js");
            StoredProcedure sproc = new StoredProcedure
            {
                Id = "BulkImport",
                Body = body
            };

            await TryDeleteStoredProcedure(client, collection, sproc.Id);
            sproc = await ExecuteWithRetries<ResourceResponse<StoredProcedure>>(client, () => client.CreateStoredProcedureAsync(collection.SelfLink, sproc));

            while (currentCount < fileCount)
            {
                string argsJson = CreateBulkInsertScriptArguments(fileNames, currentCount, fileCount, maxScriptSize);
                var args = new dynamic[] { JsonConvert.DeserializeObject<dynamic>(argsJson) };

                StoredProcedureResponse<int> scriptResult = await ExecuteWithRetries<StoredProcedureResponse<int>>(client, () => client.ExecuteStoredProcedureAsync<int>(sproc.SelfLink, args));

                int currentlyInserted = scriptResult.Response;
                currentCount += currentlyInserted;
            }
        }

        public static async Task TryDeleteStoredProcedure(DocumentClient client, DocumentCollection collection, string sprocId)
        {
            StoredProcedure sproc = client.CreateStoredProcedureQuery(collection.SelfLink).Where(s => s.Id == sprocId).AsEnumerable().FirstOrDefault();
            if (sproc != null)
            {
                await ExecuteWithRetries<ResourceResponse<StoredProcedure>>(client, () => client.DeleteStoredProcedureAsync(sproc.SelfLink));
            }
        }

        /// <summary> 
        /// Creates the script for insertion 
        /// </summary>
        /// <param name="docFileNames">The </param>
        /// <param name="currentIndex">The current number of documents inserted. this marks the starting point for this script</param> 
        /// <param name="maxCount">The max count.</param>
        /// <param name="maxScriptSize">The maximum number of characters that the script can have</param>
        /// <returns>Script as a string</returns> 
        public static string CreateBulkInsertScriptArguments(string[] docFileNames, int currentIndex, int maxCount, int maxScriptSize)
        {
            var jsonDocumentArray = new StringBuilder();
            jsonDocumentArray.Append("[");

            if (currentIndex >= maxCount)
            {
                return string.Empty;
            }

            jsonDocumentArray.Append(File.ReadAllText(docFileNames[currentIndex]));

            int scriptCapacityRemaining = maxScriptSize;
            string separator = string.Empty;

            int i = 1;
            while (jsonDocumentArray.Length < scriptCapacityRemaining && (currentIndex + i) < maxCount)
            {
                jsonDocumentArray.Append(", " + File.ReadAllText(docFileNames[currentIndex + i]));
                i++;
            }

            jsonDocumentArray.Append("]");
            return jsonDocumentArray.ToString();
        }

        /// <summary>
        /// Log the number of documents in each collection within the database to the console.
        /// </summary>
        /// <param name="client">The DocumentDB client instance.</param>
        /// <param name="database">The database to enumerate.</param>
        /// <returns>The Task for the asynchronous execution.</returns>
        public async Task<Dictionary<string, int>> LogDocumentCountsPerCollection(DocumentClient client, Database database)
        {
            Dictionary<string, int> log = new Dictionary<string, int>();

            foreach (DocumentCollection collection in await client.ReadDocumentCollectionFeedAsync(database.SelfLink))
            {
                int numDocuments = 0;
                foreach (int document in client.CreateDocumentQuery<int>(collection.SelfLink, "SELECT VALUE 1 FROM ROOT"))
                {
                    numDocuments++;
                }

                if (log.ContainsKey(collection.Id))
                {
                    log[collection.Id] = numDocuments;
                }
                else
                {
                    log.Add(collection.Id, numDocuments);
                }
                //Console.WriteLine("Collection {0}: {1} documents", collection.Id, numDocuments);
            }

            return log;
        }
    }
}
