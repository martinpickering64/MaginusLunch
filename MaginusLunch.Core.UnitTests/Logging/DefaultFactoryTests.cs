using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MaginusLunch.Logging.UnitTests
{
    [TestClass]
    public class DefaultFactoryTests
    {
        [TestMethod]
        public void When_directory_is_bad_should_throw()
        {
            var defaultFactory = new DefaultFactory();
            var nonExistingDirectoryException = Assert.ThrowsException<DirectoryNotFoundException>(() => defaultFactory.Directory("baddir"));
            Assert.AreEqual("Could not find logging directory: 'baddir'", nonExistingDirectoryException.Message);
            Assert.ThrowsException<ArgumentNullException>(() => defaultFactory.Directory(null));
            Assert.ThrowsException<ArgumentNullException>(() => defaultFactory.Directory(""));
            Assert.ThrowsException<ArgumentNullException>(() => defaultFactory.Directory(" "));
        }

        [TestMethod]
        public void When_not_running_ASP_NET_should_choose_BaseDirectory_as_logging_directory()
        {
            var directory = DefaultFactory.FindDefaultLoggingDirectory();

            Assert.AreEqual(AppDomain.CurrentDomain.BaseDirectory, directory);
        }
    }
}
