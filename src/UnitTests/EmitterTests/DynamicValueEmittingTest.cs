using Appson.Composer.UnitTests.EmitterTests.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Appson.Composer.UnitTests.EmitterTests
{
	[TestClass]
	public class DynamicValueEmittingTest
	{
		public TestContext TestContext { get; set; }
		private TestingEmittedTypeHandler _dth;

		#region Additional test attributes

		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
		}

		[ClassCleanup]
		public static void ClassCleanup()
		{
		}

		[TestInitialize]
		public void TestInitialize()
		{
			_dth = new TestingEmittedTypeHandler();
		}

		[TestCleanup]
		public void TestCleanup()
		{
		}

		#endregion

	}
//* Property of "dynamic" type
//* Method with "dynamic" return value
//* Method with "dynamic" argument
}
