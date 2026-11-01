using System.Threading.Tasks;
using Vision.Data.Models;
using Xunit;

namespace Vision.Data.Tests
{
    public class UnitTests
    {
        [Fact]
        public async Task PlaceholderTest()
        {
            var model = new UserToken { IdToken = "blah" };
            await Task.CompletedTask;
            Assert.NotNull(model);
        }
    }
}
