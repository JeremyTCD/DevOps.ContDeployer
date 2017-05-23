using System;
using System.IO;
using Xunit;

namespace JeremyTCD.PipelinesCE.Tests.IntegrationTests
{
    [Collection(nameof(PipelinesCECollection))]
    public class PipelinesCEIntegrationTests
    {
        private string _tempDir { get; set; }

        public PipelinesCEIntegrationTests(PipelinesCEFixture fixture)
        {
            fixture.ResetTempDir();
            _tempDir = fixture.TempDir;
        }

        [Fact]
        public void Start_ThrowsExceptionIfPipelinesCEProjectFileDoesNotExist()
        {
            // Arrange
            PipelinesCE pipelinesCE = new PipelinesCE(null);

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => pipelinesCE.Start());
        }

        [Fact]
        public void Start_ThrowsExceptionIfMoreThanOnePipelinesCEProjectFilesExist()
        {
            // Arrange
            PipelinesCE pipelinesCE = new PipelinesCE(null);
            File.WriteAllText("PipelinesCE.csproj", "");
            Directory.CreateDirectory("test");
            File.WriteAllText("test/PipelinesCE.csproj", "");

            // Act and Asssert
            Assert.Throws<InvalidOperationException>(() => pipelinesCE.Start());
        }

        [Fact]
        public void Start_BuildsPipelinesCEProject()
        {
            // Copy dummy project from MSBuildClient tests
            // Verify that it gets built
        }
    }
}
