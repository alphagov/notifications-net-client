using NUnit.Framework;
using System;
namespace TestProject
{
	[TestFixture()]
	public class Test
	{
		[Test()]
		public void TestCase()
		{
			Assert.IsTrue(false);
		}

		[Test()]
		public void TestCase1()
		{
			Assert.IsTrue(false);
		}

		[Test, Category("Integration")]
		public void SendSmsTestWithPersonalisation()
		{
			Assert.IsTrue(false);
		}
	}
}
