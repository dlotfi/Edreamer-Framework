using System.Collections.Generic;
using System.Linq;
using Edreamer.Framework.Composition;
using Edreamer.Framework.Domain;
using Edreamer.Framework.Settings;
using Edreamer.Framework.Settings.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Edreamer.Framework.Test.Settings.Providers
{
    [TestClass]
    public class RepositorySettingsProviderTest
    {
        [TestMethod]
        public void DefineSettingTest()
        {
            //Arrange
            var compositionContainerAccessorMock = new Mock<ICompositionContainerAccessor> { DefaultValue = DefaultValue.Mock };
            var compositionContainerMock = Mock.Get(compositionContainerAccessorMock.Object.Container);
            var dataContextMock = new Mock<IFrameworkDataContext> { DefaultValue = DefaultValue.Mock };
            var settingsRepoMock = Mock.Get(dataContextMock.Object.Settings);
            var settings = new List<Setting>();
            settingsRepoMock.Setup(r => r.Add(It.IsAny<Setting>()))
                            .Callback((Setting s) => settings.Add(s));
            var repositorySettingsProvider = new RepositorySettingsProvider(dataContextMock.Object);
            
            //Act
            repositorySettingsProvider.DefineSetting(new SettingEntryKey { Category = "TestCat", Name = "TestName" }, null, "TestModule");

            //Assert
            Assert.AreEqual(1, settings.Count);
            Assert.IsTrue(settings.Any(s => s.Category == "TestCat" && s.Name == "TestName" && s.Value == null && s.ModuleName == "TestModule"));
            dataContextMock.Verify(dc => dc.SaveChanges(), Times.AtLeastOnce());
        }
    }
}
