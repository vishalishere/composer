﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compositional.Composer.UnitTests.InitializePlugs.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Compositional.Composer.Utility;

namespace Compositional.Composer.UnitTests.InitializePlugs
{
	[TestClass]
	public class InitializePlugsTest
	{
		public TestContext TestContext { get; set; }
		private ComponentContext _context;

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
			_context = new ComponentContext();
			_context.ProcessCompositionXmlFromResource("Compositional.Composer.UnitTests.InitializePlugs.Xmls.Composition.xml");
		}

		[TestCleanup]
		public void TestCleanup()
		{
		}

		#endregion

		[TestMethod]
		public void InitializePlugsOfT()
		{
			var c = new ComponentWithInitializationPoints();
			_context.InitializePlugs(c);

			Assert.IsNotNull(c.SampleContract);
			Assert.AreEqual(c.InitPoint, 999);
		}

		[TestMethod]
		public void InitializePlugsWithType()
		{
			var c = new ComponentWithInitializationPoints();
			_context.InitializePlugs(c, typeof (ClassWithInitializtionPoints));

			Assert.IsNotNull(c.SampleContract);
			Assert.AreEqual(c.InitPoint, 999);
		}

		[TestMethod]
		public void InitializePlugsForNonComponent()
		{
			var c = new ClassWithInitializtionPoints();
			_context.InitializePlugs(c);

			Assert.IsNotNull(c.SampleContract);
			Assert.AreEqual(c.InitPoint, 999);
		}
	}
}
