using Xunit;

namespace JeremyTCD.PipelinesCE.Tools.Tests.UnitTests
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
                Project = testPrimaryProject
            };

            PipelineOptions secondary = new PipelineOptions
            {
                DryRun = true,
                Project = testSecondaryProject,
                Pipeline = testSecondaryPipeline
            };

            // Act
            primary.Combine(secondary);

            // Assert
            Assert.True(primary.DryRun);
            Assert.Equal(testPrimaryProject, primary.Project);
            Assert.Equal(testSecondaryPipeline, secondary.Pipeline);
        }
    }
}
