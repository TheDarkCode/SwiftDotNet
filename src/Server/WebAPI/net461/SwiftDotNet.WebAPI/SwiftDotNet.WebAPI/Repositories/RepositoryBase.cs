using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SwiftDotNet.WebAPI.Entities;
using SwiftDotNet.WebAPI.Helpers;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace SwiftDotNet.WebAPI.Repositories
{
    /// <summary>
    /// All repository classes must inherit from this base class.  This base class 
    /// contains all the basic CRUD operations.
    /// </summary>
    /// <typeparam name="T">The entity type used for the repository.</typeparam>
    public class RepositoryBase<T> : DocumentDbClient, IRepository<T> where T : EntityBase
    {
        #region ctors

        private Expression<Func<T, bool>> _typePredicate = null;
        private string _type = "";

        /// <summary>
        /// All Repository classes must inherit this base class.
        /// </summary>
        /// <param name="type">The name of the entity (T), which is the same as the name passed into the model (lowercase). Used as the partition key.</param>
        /// <param name="dbName">The name of the database.</param>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="endpoint">The URI of the DocumentDB server used for this repository (https://{DATABASENAME}.documents.azure.com:443).</param>
        /// <param name="authkey">The Auth Key for the DocumentDB server (Primary or Secondary Admin).</param>
        public RepositoryBase(string type, string dbName, string collectionName, string endpoint = null, string authkey = null)
            : base(dbName, collectionName, endpoint, authkey)
        {
            _typePredicate = v => v.docType == type;
            _type = type;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get a list of T, with an optional predicate.
        /// </summary>
        /// <param name="predicate">The linq expression Where clause.</param>
        /// <returns>An IEnumerable of T.</returns>
        public IEnumerable<T> Get(Expression<Func<T, bool>> predicate = null)
        {
            var query = Client.CreateDocumentQuery<T>(Collection.DocumentsLink, new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                .Where(_typePredicate)
                .AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query;
            //return await Task<IEnumerable<T>>.Run(() => query.AsEnumerable().ToList());
        }

        public Task<T> GetById(string id)
        {
            return Task<T>.Run(() =>
                Client.CreateDocumentQuery<T>(Collection.DocumentsLink, new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                .Where(_typePredicate)
                .Where(p => p.Id == id)
                .AsEnumerable()
                .FirstOrDefault());
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null)
        {

            var query = Client.CreateDocumentQuery<T>(Collection.DocumentsLink, new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                        .Where(_typePredicate); //AsQueryable()

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            //return query;
            try
            {
                var result = await QueryAsync(query.AsQueryable()); // await QueryAsync(query); //Task.Run(async() => query.AsEnumerable());

                if (result == null)
                {
                    throw new Exception("GetAsync result was null");
                }

                return result;
            }

            catch (Exception)
            {
                throw;
            }

        }

        public async Task<ResourceResponse<Document>> CreateDocumentAsync(T entity)
        {
            return await Client.CreateDocumentAsync(Collection.DocumentsLink, entity);
        }

        public async Task<ResourceResponse<Document>> UpdateDocumentAsync(T entity)
        {
            var doc = GetDocument(entity.Id);

            return await Client.ReplaceDocumentAsync(doc.SelfLink, entity);
        }

        public async Task<ResourceResponse<Document>> DeleteDocumentAsync(string id)
        {
            var doc = GetDocument(id);

            return await Client.DeleteDocumentAsync(doc.SelfLink, new RequestOptions { PartitionKey = new PartitionKey(_type) });
        }


        #endregion

        #region Helper Methods

        public async Task<IEnumerable<T>> QueryAsync(IQueryable<T> query)
        {
            var documentQuery = query.AsDocumentQuery();
            var results = new List<IEnumerable<T>>();

            do
            {
                var result = await documentQuery.ExecuteNextAsync<T>();

                results.Add(result);
            }

            while (documentQuery.HasMoreResults);

            var documents = results.SelectMany(b => b);

            return documents;
        }

        public Document GetDocument(string id)
        {
            var doc = Client.CreateDocumentQuery<Document>(Collection.DocumentsLink)
                            .Where(d => d.Id == id)
                            .AsEnumerable()
                            .FirstOrDefault();
            return doc;
        }


        #endregion
    }

}
