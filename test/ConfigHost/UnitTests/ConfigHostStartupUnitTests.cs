using Xunit;

namespace JeremyTCD.PipelinesCE.ConfigHost.Tests.UnitTests
{
    public class ConfigHostStartupUnitTests
    {
        [Fact]
        public void NormalizeFieldName()
        {
            // Act
            string result = ConfigHostStartup.NormalizeFieldName("_TestTest");

            // Assert
            Assert.Equal("testtest", result);
        }
    }
}
