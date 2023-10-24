using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.Utilities.ErrorHandler;
using WeHire.Entity.IRepositories;
using WeHire.Infrastructure.Services.UserServices;
using static WeHire.Application.Utilities.GlobalVariables.GlobalVariable;

namespace WeHire.Infrastructure.Services.FileServices
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private string _apiKey, _authEmail, _authPassword, _bucket;

        public FileService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _apiKey = _configuration["Firebase:ApiKey"];
            _authEmail = _configuration["Firebase:AuthEmail"];
            _authPassword = _configuration["Firebase:AuthPassword"];
            _bucket = _configuration["Firebase:Bucket"];
        }

        public async Task<string> UploadFileAsync(IFormFile file, string fullName, string childFolder)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(_apiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(_authEmail, _authPassword);

            string fileName = GetFileNameAsync(fullName);
            var stream = file.OpenReadStream();

            var task = new FirebaseStorage(
                _bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                    ThrowOnCancel = true,   
                })
                .Child(childFolder)
                .Child(fileName)
                .PutAsync(stream);

            String urlImage = await task;
            return urlImage;
        }

        private string GetFileNameAsync(string fullName)
        {
            DateTime currentDateTime = DateTime.Now;
            string fileName = $"{fullName}_{currentDateTime:yyyyMMdd_HHmmss.fff}.jpg";
            return fileName;
        }
    }
}
