using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Swift.Net.Services
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile imageToUpload);
    }

}
