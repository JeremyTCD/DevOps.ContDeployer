using Xunit;

namespace JeremyTCD.PipelinesCE.Core.Tests.UnitTests
{
    public class PipelineOptionsUnitTests
    {
        // TODO incomplete, does not test all properties
        [Fact]
        public void Combine_CombinesTwoPipelineOptionsInstancesWithCorrectPrecedence()
        {
            // Arrange
            string testPrimaryProject = "testPrimaryProject";
            string testSecondaryProject = "testSecondaryProject";
            string testSecondaryPipeline = "testSecondaryPipeline";

            PipelinesCEOptions primary = new PipelinesCEOptions
            {
                Project = testPrimaryProject
            };

            PipelinesCEOptions secondary = new PipelinesCEOptions
            {
                Project = testSecondaryProject,
                Pipeline = testSecondaryPipeline
            };

            // Act
            primary.Combine(secondary);

            // Assert
            Assert.Equal(testPrimaryProject, primary.Project);
            Assert.Equal(testSecondaryPipeline, secondary.Pipeline);
        }
    }
}
