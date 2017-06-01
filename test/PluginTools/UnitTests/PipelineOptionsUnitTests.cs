using Xunit;

namespace JeremyTCD.PipelinesCE.PluginTools.Tests.UnitTests
{
    public class PipelineOptionsUnitTests
    {
        [Fact]
        public void Combine_CombinesTwoPipelineOptionsInstancesWithCorrectPrecedence()
        {
            // Arrange
            string testPrimaryProject = "testPrimaryProject";
            string testSecondaryProject = "testSecondaryProject";
            string testSecondaryPipeline = "testSecondaryPipeline";

            PipelineOptions primary = new PipelineOptions
            {
                Verbose = false,
                Project = testPrimaryProject
            };

            PipelineOptions secondary = new PipelineOptions
            {
                Verbose = true,
                DryRun = true,
                Project = testSecondaryProject,
                Pipeline = testSecondaryPipeline
            };

            // Act
            primary.Combine(secondary);

            // Assert
            Assert.False(primary.Verbose);
            Assert.True(primary.DryRun);
            Assert.Equal(testPrimaryProject, primary.Project);
            Assert.Equal(testSecondaryPipeline, primary.Pipeline);
        }
    }
}
