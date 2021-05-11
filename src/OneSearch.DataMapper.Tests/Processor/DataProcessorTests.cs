using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moq;
using NUnit.Framework;
using OneSearch.DataMapper.Messaging;
using OneSearch.DataMapper.Options;
using OneSearch.DataMapper.Processor;
using OneSearch.DataMapper.Storage;
using OneSearch.DataMapper.Storage.Models;

namespace OneSearch.DataMapper.Tests.Processor
{
    public class DataProcessorTests
    {
        [Test]
        public void Process()
        {
            var queueMessagesMock = new Mock<IMessages>();
            queueMessagesMock.Setup(x => x.Messages).Returns(Enumerable.Empty<string>());

            var messageProcessorMock = new Mock<IMessageProcessor>();
            messageProcessorMock.Setup(x => x.Process(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new List<StorageItem>());

            var queueMock = new Mock<IChannel>();
            queueMock.Setup(x => x.GetMessages()).ReturnsAsync(queueMessagesMock.Object);

            var storageMock = new Mock<IDataStorage>();

            var processor = new DataProcessor(queueMock.Object, messageProcessorMock.Object, storageMock.Object,
                new DataProcessorOptions());

            _ = processor.Process();

            Thread.Sleep(1000);

            queueMock.Verify(x => x.GetMessages(), Times.AtLeastOnce);
            messageProcessorMock.Verify(x => x.Process(It.IsAny<IEnumerable<string>>()), Times.AtLeastOnce());
            storageMock.Verify(x => x.Save(It.IsAny<IList<StorageItem>>()), Times.AtLeastOnce);
            queueMessagesMock.Verify(x => x.Ack(), Times.AtLeastOnce);
        }

        [Test]
        public void ProducerException()
        {
            var queueMessagesMock = new Mock<IMessages>();
            queueMessagesMock.Setup(x => x.Messages).Returns(Enumerable.Empty<string>());

            var messageProcessorMock = new Mock<IMessageProcessor>();
            messageProcessorMock.Setup(x => x.Process(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new List<StorageItem>());

            var queueMock = new Mock<IChannel>();
            queueMock.Setup(x => x.GetMessages()).ThrowsAsync(new Exception());

            var storageMock = new Mock<IDataStorage>();

            var processor = new DataProcessor(queueMock.Object, messageProcessorMock.Object, storageMock.Object,
                new DataProcessorOptions());

            Assert.ThrowsAsync<Exception>(() => processor.Process());
        }

        [Test]
        public void ConsumerException()
        {
            var queueMessagesMock = new Mock<IMessages>();
            queueMessagesMock.Setup(x => x.Messages).Throws(new Exception());

            var messageProcessorMock = new Mock<IMessageProcessor>();
            messageProcessorMock.Setup(x => x.Process(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new List<StorageItem>());

            var queueMock = new Mock<IChannel>();
            queueMock.Setup(x => x.GetMessages()).ReturnsAsync(queueMessagesMock.Object);

            var storageMock = new Mock<IDataStorage>();

            var processor = new DataProcessor(queueMock.Object, messageProcessorMock.Object, storageMock.Object,
                new DataProcessorOptions());

            Assert.ThrowsAsync<Exception>(() => processor.Process());
        }
    }
}