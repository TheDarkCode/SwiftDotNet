using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Swift.Net.DocumentDB.Entities;
using Swift.Net.DocumentDB.Helpers;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Swift.Net.DocumentDB.Repositories
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

        /// <summary>
        /// All Repository classes must inherit this base class.
        /// </summary>
        /// <param name="type">The name of the entity (T), which is the same as the name passed into the model (lowercase). Used as the partition key.</param>
        /// <param name="client">The DocumentClient instance to use with the repository. Must be same endpoint (account-level).</param>
        /// <param name="dbName">The name of the database.</param>
        /// <param name="collectionName">The name of the collection.</param>
        public RepositoryBase(string type, DocumentClient client, string dbName, string collectionName)
            : base(client, dbName, collectionName)
        {
            _typePredicate = v => v.docType == type;
            _type = type;
        }

        /// <summary>
        /// All Repository classes must inherit this base class.
        /// </summary>
        /// <param name="type">The name of the entity (T), which is the same as the name passed into the model (lowercase). Used as the partition key.</param>
        /// <param name="client">The DocumentClient instance to use with the repository. Must be same endpoint (account-level).</param>
        /// <param name="collection">The DocumentCollection to use with this repository.</param>
        public RepositoryBase(string type, DocumentClient client, DocumentCollection collection)
            : base(client, collection)
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

        public async Task<T> GetById(string id)
        {
            //return Task<T>.Run(() =>
            //    Client.CreateDocumentQuery<T>(Collection.DocumentsLink, new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
            //    .Where(_typePredicate)
            //    .Where(p => p.Id == id)
            //    .AsEnumerable()
            //    .FirstOrDefault());

            var query = Client.CreateDocumentQuery<T>(Collection.DocumentsLink, new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = false })
                .Where(_typePredicate)
                .Where(p => p.Id == id)
                .AsQueryable();

            var result = await QueryAsync(query);

            return result.FirstOrDefault();
        }

        public async Task<double> GetByIdRequestUnits(string id)
        {
            //return Task<T>.Run(() =>
            //    Client.CreateDocumentQuery<T>(Collection.DocumentsLink, new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
            //    .Where(_typePredicate)
            //    .Where(p => p.Id == id)
            //    .AsEnumerable()
            //    .FirstOrDefault());

            var query = Client.CreateDocumentQuery<T>(Collection.DocumentsLink, new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = false })
                .Where(_typePredicate)
                .Where(p => p.Id == id)
                .AsQueryable();

            var charges = await QueryAsyncRequestUnits(query);

            return charges;
        }

        public async Task<double> GetByIdRequestUnitsCrossPartition(string id)
        {
            //return Task<T>.Run(() =>
            //    Client.CreateDocumentQuery<T>(Collection.DocumentsLink, new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
            //    .Where(_typePredicate)
            //    .Where(p => p.Id == id)
            //    .AsEnumerable()
            //    .FirstOrDefault());

            var query = Client.CreateDocumentQuery<T>(Collection.DocumentsLink, new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                .Where(_typePredicate)
                .Where(p => p.Id == id)
                .AsQueryable();

            var charges = await QueryAsyncRequestUnits(query);

            return charges;
        }

        public async Task<T> GetFirstByPredicate(Expression<Func<T, bool>> predicate)
        {
            //return Task<T>.Run(() =>
            //    Client.CreateDocumentQuery<T>(Collection.DocumentsLink, new FeedOptions { MaxItemCount = 1, EnableCrossPartitionQuery = true })
            //    .Where(_typePredicate)
            //    .Where(predicate)
            //    .AsEnumerable()
            //    .FirstOrDefault());

            var query = Client.CreateDocumentQuery<T>(Collection.DocumentsLink, new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                .Where(_typePredicate)
                .Where(predicate)
                .AsQueryable();

            var result = await QueryAsync(query);

            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<T>> GetByPredicate(Expression<Func<T, bool>> predicate, int take = 0)
        {
            var query = Client.CreateDocumentQuery<T>(Collection.DocumentsLink, new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                .Where(_typePredicate)
                .Where(predicate);


            if (take != 0)
            {
                query = query.Take(take);
            }

            try
            {
                var result = await QueryAsync(query.AsQueryable());

                if (result == null)
                {
                    throw new Exception("GetByPredicate result was null");
                }

                return result;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null, int take = 0)
        {

            var query = Client.CreateDocumentQuery<T>(Collection.DocumentsLink, new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                        .Where(_typePredicate);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (take != 0)
            {
                query = query.Take(take);
            }

            try
            {
                var result = await QueryAsync(query.AsQueryable());

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

        public async Task<IEnumerable<dynamic>> GetAsyncWithSelect(Expression<Func<T, bool>> predicate = null, Func<T, dynamic> selector = null, int take = 0)
        {

            var query = Client.CreateDocumentQuery<T>(Collection.DocumentsLink, new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                     .Where(_typePredicate);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (take != 0)
            {
                query = query.Take(take);
            }

            try
            {
                var result = await QueryAsyncWithSelect(query.AsQueryable(), selector);

                if (result == null)
                {
                    throw new Exception("GetAsyncWithSelect result was null");
                }

                return result;
            }

            catch (Exception)
            {
                throw;
            }

        }

        public async Task<IEnumerable<T>> GetAsyncOrderBy(Expression<Func<T, bool>> predicate = null, Expression<Func<T, dynamic>> ordering = null, int take = 0)
        {

            var query = Client.CreateDocumentQuery<T>(Collection.DocumentsLink, new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                        .Where(_typePredicate);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (take != 0)
            {
                query = query.Take(take);
            }

            if (ordering != null)
            {
                query = query.OrderBy(ordering);
            }

            try
            {
                var result = await QueryAsync(query.AsQueryable());

                if (result == null)
                {
                    throw new Exception("GetAsyncOrderBy result was null");
                }

                return result;
            }

            catch (Exception)
            {
                throw;
            }

        }

        public async Task<IEnumerable<T>> GetAsyncOrderByDescending(Expression<Func<T, bool>> predicate = null, Expression<Func<T, dynamic>> ordering = null, int take = 0)
        {

            var query = Client.CreateDocumentQuery<T>(Collection.DocumentsLink, new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true })
                        .Where(_typePredicate);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (take != 0)
            {
                query = query.Take(take);
            }

            if (ordering != null)
            {
                query = query.OrderByDescending(ordering);
            }

            try
            {
                var result = await QueryAsync(query.AsQueryable());

                if (result == null)
                {
                    throw new Exception("GetAsyncOrderByDescending result was null");
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

        public async Task<double> QueryAsyncRequestUnits(IQueryable<T> query)
        {
            var documentQuery = query.AsDocumentQuery();
            double charges = 0;

            do
            {
                var result = await documentQuery.ExecuteNextAsync<T>();

                charges += result.RequestCharge;
            }

            while (documentQuery.HasMoreResults);

            //var documents = results.SelectMany(b => b);

            return charges;
        }

        public async Task<IEnumerable<dynamic>> QueryAsyncWithSelect(IQueryable<T> query, Func<T, dynamic> selector)
        {
            var documentQuery = query.AsDocumentQuery();
            var results = new List<IEnumerable<T>>();

            do
            {
                var result = await documentQuery.ExecuteNextAsync<T>();

                results.Add(result);
            }

            while (documentQuery.HasMoreResults);

            var documents = results.SelectMany(b => b).Select(selector);

            return documents;
        }

        public Document GetDocument(string id)
        {
            var doc = Client.CreateDocumentQuery<Document>(Collection.DocumentsLink, new FeedOptions { EnableCrossPartitionQuery = true })
                            .Where(d => d.Id == id)
                            .AsEnumerable()
                            .FirstOrDefault();
            return doc;
        }


        #endregion
    }

}
