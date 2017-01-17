using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SwiftDotNet.DocumentDB.Entities;
using System.Linq.Expressions;
using Microsoft.Azure.Documents;

namespace SwiftDotNet.DocumentDB.Repositories
{
    public interface IRepository<T>
     where T : EntityBase
    {

        Task<Microsoft.Azure.Documents.Client.ResourceResponse<Document>> CreateDocumentAsync(T entity);
        Task<Microsoft.Azure.Documents.Client.ResourceResponse<Document>> DeleteDocumentAsync(string id);
        IEnumerable<T> Get(Expression<Func<T, bool>> predicate = null);
        Task<IEnumerable<T>> GetAsyncOrderByDescending(Expression<Func<T, bool>> predicate = null, Expression<Func<T, dynamic>> ordering = null, int take = 0);
        Task<IEnumerable<T>> GetAsyncOrderBy(Expression<Func<T, bool>> predicate = null, Expression<Func<T, dynamic>> ordering = null, int take = 0);
        Task<IEnumerable<dynamic>> GetAsyncWithSelect(Expression<Func<T, bool>> predicate = null, Func<T, dynamic> selector = null, int take = 0);
        Task<IEnumerable<T>> GetByPredicate(Expression<Func<T, bool>> predicate, int take = 0);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null, int take = 0);
        Task<T> GetFirstByPredicate(Expression<Func<T, bool>> predicate);
        Task<T> GetById(string id);
        Document GetDocument(string id);
        Task<Microsoft.Azure.Documents.Client.ResourceResponse<Document>> UpdateDocumentAsync(T entity);

    }
}
