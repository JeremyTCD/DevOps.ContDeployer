using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.PipelinesCE.Core.Tests.UnitTests
{
    public class SharedPluginOptionsUnitTests
    {
        [Theory]
        [MemberData(nameof(CombineData))]
        public void Combine_CombinesTwoPipelineOptionsInstancesWithCorrectPrecedence(SharedPluginOptions primary, SharedPluginOptions secondary, SharedPluginOptions expectedResult)
        {
            // Act
            SharedPluginOptions result = primary.Combine(secondary);

            // Assert
            SharedPluginOptionsEqualityComparer comparer = new SharedPluginOptionsEqualityComparer();
            Assert.True(comparer.Equals(result, expectedResult));
        }

        public static IEnumerable<object[]> CombineData()
        {
            yield return new object[] { new SharedPluginOptions(), new SharedPluginOptions { DryRun = true }, new SharedPluginOptions { DryRun = true } };
            yield return new object[] { new SharedPluginOptions { DryRun = true }, new SharedPluginOptions { DryRun = false }, new SharedPluginOptions { DryRun = true } };
            yield return new object[] { new SharedPluginOptions(), new SharedPluginOptions(), new SharedPluginOptions ()};
        }

        private class SharedPluginOptionsEqualityComparer : IEqualityComparer<SharedPluginOptions>
        {
            public bool Equals(SharedPluginOptions x, SharedPluginOptions y)
            {
                return x.DryRun == y.DryRun;
            }

            public int GetHashCode(SharedPluginOptions obj)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
