using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SwiftDotNet.WebAPI.Entities;
using System.Linq.Expressions;
using Microsoft.Azure.Documents;

namespace SwiftDotNet.WebAPI.Repositories
{
    public interface IRepository<T>
     where T : EntityBase
    {

        Task<Microsoft.Azure.Documents.Client.ResourceResponse<Microsoft.Azure.Documents.Document>> CreateDocumentAsync(T entity);
        Task<Microsoft.Azure.Documents.Client.ResourceResponse<Microsoft.Azure.Documents.Document>> DeleteDocumentAsync(string id);
        IEnumerable<T> Get(Expression<Func<T, bool>> predicate = null);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate = null);
        Task<T> GetById(string id);
        Document GetDocument(string id);
        Task<Microsoft.Azure.Documents.Client.ResourceResponse<Microsoft.Azure.Documents.Document>> UpdateDocumentAsync(T entity);

    }
}
