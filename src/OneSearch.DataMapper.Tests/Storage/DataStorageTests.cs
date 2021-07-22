using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using OneSearch.DataMapper.Exceptions;
using OneSearch.DataMapper.Options;
using OneSearch.DataMapper.Storage;
using OneSearch.DataMapper.Storage.Compress;
using OneSearch.DataMapper.Storage.Models;

namespace OneSearch.DataMapper.Tests.Storage
{
    internal class DataStorageTests
    {
        [Test]
        public void Save()
        {
            var items = new List<StorageItem>
            {
                new("123", "data1"),
                new("453", "data2")
            };

            var responseItems = new List<StorageResponseItem>
            {
                new() {StatusCode = (int) HttpStatusCode.OK},
                new() {StatusCode = 400}
            };

            var dataStorage = new DataStorage(new Mock<ILogger<DataStorage>>().Object,
                new StorageOptions {IgnoredHttpStatusCodes = new List<int> {400}},
                CreateStorage(HttpStatusCode.OK, responseItems), new GZipCompressor());

            Assert.DoesNotThrowAsync(() => dataStorage.Save(items));
        }

        [Test]
        public void SilentOnErrorStatusCode()
        {
            var items = new List<StorageItem>
            {
                new("123", "data1"),
                new("453", "data2")
            };

            var responseItems = new List<StorageResponseItem>
            {
                new() {StatusCode = (int) HttpStatusCode.OK},
                new() {StatusCode = 500}
            };

            var dataStorage = new DataStorage(new Mock<ILogger<DataStorage>>().Object,
                new StorageOptions {SilentOnError = true},
                CreateStorage(HttpStatusCode.BadRequest, responseItems), new GZipCompressor());

            Assert.DoesNotThrowAsync(() => dataStorage.Save(items));
        }

        [Test]
        public void ThrowExceptionOnErrorStatusCode()
        {
            var items = new List<StorageItem>
            {
                new("123", "data1"),
                new("453", "data2")
            };

            var responseItems = new List<StorageResponseItem>
            {
                new() {StatusCode = (int) HttpStatusCode.OK},
                new() {StatusCode = 500}
            };

            var dataStorage = new DataStorage(new Mock<ILogger<DataStorage>>().Object,
                new StorageOptions {SilentOnError = false},
                CreateStorage(HttpStatusCode.BadRequest, responseItems), new GZipCompressor());

            Assert.ThrowsAsync<StorageException>(() => dataStorage.Save(items));
        }

        [Test]
        public void SilentOnFailedItems()
        {
            var items = new List<StorageItem>
            {
                new("123", "data1"),
                new("453", "data2")
            };

            var responseItems = new List<StorageResponseItem>
            {
                new() {StatusCode = (int) HttpStatusCode.OK},
                new() {StatusCode = 500}
            };

            var dataStorage = new DataStorage(new Mock<ILogger<DataStorage>>().Object,
                new StorageOptions {SilentOnError = true},
                CreateStorage(HttpStatusCode.OK, responseItems), new GZipCompressor());

            Assert.DoesNotThrowAsync(() => dataStorage.Save(items));
        }

        [Test]
        public void ThrowExceptionOnFailedItems()
        {
            var items = new List<StorageItem>
            {
                new("123", "data1"),
                new("453", "data2")
            };

            var responseItems = new List<StorageResponseItem>
            {
                new() {StatusCode = (int) HttpStatusCode.OK},
                new() {StatusCode = 500}
            };

            var dataStorage = new DataStorage(new Mock<ILogger<DataStorage>>().Object,
                new StorageOptions {SilentOnError = false},
                CreateStorage(HttpStatusCode.OK, responseItems), new GZipCompressor());

            Assert.ThrowsAsync<StorageException>(() => dataStorage.Save(items));
        }

        [Test]
        public void SilentOnException()
        {
            var items = new List<StorageItem>
            {
                new("123", "data1"),
                new("453", "data2")
            };

            var dataStorage = new DataStorage(new Mock<ILogger<DataStorage>>().Object,
                new StorageOptions {SilentOnError = true},
                CreateExceptionStorage(), new GZipCompressor());

            Assert.DoesNotThrowAsync(() => dataStorage.Save(items));
        }

        [Test]
        public void ThrowExceptionOnException()
        {
            var items = new List<StorageItem>
            {
                new("123", "data1"),
                new("453", "data2")
            };

            var dataStorage = new DataStorage(new Mock<ILogger<DataStorage>>().Object,
                new StorageOptions {SilentOnError = false},
                CreateExceptionStorage(), new GZipCompressor());

            Assert.ThrowsAsync<StorageException>(() => dataStorage.Save(items));
        }

        [Test]
        public void PreconditionRequired()
        {
            var items = new List<StorageItem>
            {
                new("123", "data1"),
                new("453", "data2")
            };

            var storageMock = new Mock<IRetryableStorage>();
            storageMock.Setup(x => x.Save(It.IsAny<MultipartItem>()))
                .ReturnsAsync((HttpStatusCode.PreconditionRequired, null));
            var dataStorage = new DataStorage(new Mock<ILogger<DataStorage>>().Object,
                new StorageOptions {SilentOnError = false},
                storageMock.Object, new GZipCompressor());

            Assert.ThrowsAsync<StorageException>(() => dataStorage.Save(items));
            storageMock.Verify(x => x.Save(It.IsAny<MultipartItem>()), Times.Exactly(3));
        }

        private static IRetryableStorage CreateStorage(HttpStatusCode code, IList<StorageResponseItem> responseItems)
        {
            var mock = new Mock<IRetryableStorage>();
            mock.Setup(x => x.Save(It.IsAny<MultipartItem>()))
                .ReturnsAsync((code, JsonConvert.SerializeObject(responseItems)));
            return mock.Object;
        }

        private static IRetryableStorage CreateExceptionStorage()
        {
            var mock = new Mock<IRetryableStorage>();
            mock.Setup(x => x.Save(It.IsAny<MultipartItem>()))
                .ThrowsAsync(new Exception());
            return mock.Object;
        }
    }
}