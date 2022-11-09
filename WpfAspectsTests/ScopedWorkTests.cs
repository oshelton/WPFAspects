using System.Linq;
using System.Reflection;
using WPFAspects.Utils;
using Xunit;

namespace UtilTests
{
	public class ScopedWorkTests
	{
		// Test default ScopedWork behavior.
		[Fact]
		public void TestScopedWork()
		{
			var testValue = 0;
			using (var scopedWork = new ScopedWork(() => testValue = 1, () => testValue = -1))
			{
				Assert.Equal(1, testValue);
			}
			Assert.Equal(-1, testValue);
		}
	}
}
