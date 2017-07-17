using Xunit;

namespace JeremyTCD.PipelinesCE.PipelineRunner.Tests.UnitTests
{
    public class ProgramUnitTests
    {
        [Fact]
        public void NormalizeFieldName()
        {
            // Act
            string result = Program.NormalizeFieldName("_TestTest");

            // Assert
            Assert.Equal("testtest", result);
        }
    }
}
