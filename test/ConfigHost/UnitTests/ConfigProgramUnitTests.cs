using Xunit;

namespace JeremyTCD.PipelinesCE.Config.Tests.UnitTests
{
    public class ConfigProgramUnitTests
    {
        [Fact]
        public void NormalizeFieldName()
        {
            // Act
            string result = ConfigProgram.NormalizeFieldName("_TestTest");

            // Assert
            Assert.Equal("testtest", result);
        }
    }
}
