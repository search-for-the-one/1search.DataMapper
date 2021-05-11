using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using OneSearch.DataMapper.Options;
using OneSearch.DataMapper.Storage;
using OneSearch.DataMapper.Storage.Models;

namespace OneSearch.DataMapper.Tests.Storage
{
    internal class RetryableStorageTests
    {
        [TestCase(HttpStatusCode.OK, false)]
        [TestCase(HttpStatusCode.BadGateway, true)]
        public void IsRetryable(HttpStatusCode code, bool isRetryable)
        {
            var retryableStorage = new RetryableStorage(
                new StorageOptions {RetryHttpStatusCodes = new List<int> {(int) HttpStatusCode.BadGateway}},
                new Mock<ILogger<RetryableStorage>>().Object, null);

            Assert.AreEqual(isRetryable, retryableStorage.IsRetryable(code));
        }

        [TestCase(HttpStatusCode.OK, false)]
        [TestCase(HttpStatusCode.BadGateway, true)]
        public void IsRetryableOnSubStatusCode(HttpStatusCode code, bool isRetryable)
        {
            var retryableStorage = new RetryableStorage(
                new StorageOptions {RetryHttpStatusCodes = new List<int> {(int) HttpStatusCode.BadGateway}},
                new Mock<ILogger<RetryableStorage>>().Object, null);

            Assert.AreEqual(isRetryable, retryableStorage.IsRetryable(code));
        }

        [Test]
        public async Task Save()
        {
            var storageMock = CreateStorageMock(HttpStatusCode.OK, null);
            var retryableStorage = new RetryableStorage(new StorageOptions(),
                new Mock<ILogger<RetryableStorage>>().Object, storageMock.Object);

            var response = await retryableStorage.Save(new MultipartItem());

            Assert.AreEqual(HttpStatusCode.OK, response.statusCode);
            storageMock.Verify(x => x.Save(It.IsAny<MultipartItem>()), Times.Once);
        }

        [Test]
        public async Task KeepRetrySave()
        {
            var storageMock = CreateStorageMock(HttpStatusCode.BadGateway, null);
            var options = new StorageOptions
            {
                WaitBeforeRetryMilliseconds = 10,
                RetryHttpStatusCodes = new List<int> {(int) HttpStatusCode.BadGateway}
            };
            var retryableStorage = new RetryableStorage(options,
                new Mock<ILogger<RetryableStorage>>().Object, storageMock.Object);

            var response = await retryableStorage.Save(new MultipartItem());

            Assert.AreEqual(HttpStatusCode.BadGateway, response.statusCode);
            storageMock.Verify(x => x.Save(It.IsAny<MultipartItem>()), Times.Exactly(options.Retries + 1));
        }

        [Test]
        public async Task SaveAfterRetry()
        {
            var storageMock = new Mock<IStorage>();
            storageMock.SetupSequence(x => x.Save(It.IsAny<MultipartItem>()))
                .ReturnsAsync((HttpStatusCode.BadGateway, null))
                .ReturnsAsync((HttpStatusCode.BadGateway, null))
                .ReturnsAsync((HttpStatusCode.BadGateway, null))
                .ReturnsAsync((HttpStatusCode.OK, null))
                .ReturnsAsync((HttpStatusCode.BadGateway, null));

            var options = new StorageOptions
            {
                WaitBeforeRetryMilliseconds = 10, RetryHttpStatusCodes = new List<int> {(int) HttpStatusCode.BadGateway}
            };
            var retryableStorage = new RetryableStorage(options,
                new Mock<ILogger<RetryableStorage>>().Object, storageMock.Object);

            var response = await retryableStorage.Save(new MultipartItem());

            Assert.AreEqual(HttpStatusCode.OK, response.statusCode);
            storageMock.Verify(x => x.Save(It.IsAny<MultipartItem>()), Times.Exactly(4));
        }

        private static Mock<IStorage> CreateStorageMock(HttpStatusCode code, IList<StorageResponseItem> responseItems)
        {
            var storageMock = new Mock<IStorage>();
            storageMock.Setup(x => x.Save(It.IsAny<MultipartItem>()))
                .ReturnsAsync((code, JsonConvert.SerializeObject(responseItems)));

            return storageMock;
        }
    }
}