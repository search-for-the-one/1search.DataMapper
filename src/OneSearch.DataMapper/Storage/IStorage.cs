using System.Net;
using System.Threading.Tasks;
using OneSearch.DataMapper.Storage.Models;

namespace OneSearch.DataMapper.Storage
{
    internal interface IStorage
    {
        Task<(HttpStatusCode statusCode, string response)> Save(MultipartItem multipartItem);
    }
}