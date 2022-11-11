using WPFAspects.Utils;
using Xunit;

namespace WpfAspects.Tests;

/// <summary>
/// Tests basic ScopedWork behavior.
/// </summary>
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
