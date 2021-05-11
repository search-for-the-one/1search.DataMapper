using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using OneSearch.DataMapper.Mappers;
using OneSearch.DataMapper.Options;
using OneSearch.DataMapper.Processor;
using OneSearch.DataMapper.Storage.Models;
using OneSearch.DataMapper.Validators;

namespace OneSearch.DataMapper.Tests.Processor
{
    internal class MessageProcessorTests
    {
        [Test]
        public void Process()
        {
            var validatorMock = new Mock<IMessageValidator>();
            validatorMock.Setup(x => x.IsValid(It.IsAny<string>())).ReturnsAsync(true);

            var mapperMock = new Mock<IMessageMapper>();
            mapperMock.Setup(x => x.Map(It.IsAny<string>())).ReturnsAsync(Enumerable.Empty<StorageItem>());

            var idDeDuplicatorMock = new Mock<IIdDeDuplicator>();

            var processor = new MessageProcessor(validatorMock.Object, mapperMock.Object, idDeDuplicatorMock.Object,
                new MessageProcessorOptions());

            var messages = new List<string> {"one", "two"};
            Assert.DoesNotThrowAsync(() => processor.Process(messages));

            validatorMock.Verify(x => x.IsValid(It.IsAny<string>()), Times.Exactly(2));
            mapperMock.Verify(x => x.Map(It.IsAny<string>()), Times.Exactly(2));
            idDeDuplicatorMock.Verify(x => x.DeDuplicate(It.IsAny<IList<StorageItem>>()), Times.Once);
        }

        [Test]
        public void ProcessInParallel()
        {
            var validatorMock = new Mock<IMessageValidator>();
            validatorMock.Setup(x => x.IsValid(It.IsAny<string>())).ReturnsAsync(true);

            var mapperMock = new Mock<IMessageMapper>();
            mapperMock.Setup(x => x.Map(It.IsAny<string>())).ReturnsAsync(Enumerable.Empty<StorageItem>());

            var idDeDuplicatorMock = new Mock<IIdDeDuplicator>();

            var processor = new MessageProcessor(validatorMock.Object, mapperMock.Object, idDeDuplicatorMock.Object,
                new MessageProcessorOptions {ProcessInParallel = true});

            var messages = new List<string> {"one", "two"};
            Assert.DoesNotThrowAsync(() => processor.Process(messages));

            validatorMock.Verify(x => x.IsValid(It.IsAny<string>()), Times.Exactly(2));
            mapperMock.Verify(x => x.Map(It.IsAny<string>()), Times.Exactly(2));
            idDeDuplicatorMock.Verify(x => x.DeDuplicate(It.IsAny<IList<StorageItem>>()), Times.Once);
        }
    }
}