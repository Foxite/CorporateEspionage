using System.Reflection;

namespace CorporateEspionage.Tests; 

public class SpyExtensionsTest {
	private SpyGenerator m_Generator;

	[SetUp]
	public void Setup() {
		m_Generator = new SpyGenerator();
	}
	
	[Test]
	public void GetMethodInfoDirectCallTest() {
		Spy<ITestInterface> spy = m_Generator.CreateSpy<ITestInterface>();
		ITestInterface spyObject = spy.Object;
		
		MethodInfo interfaceTest1 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test1))!; 
		MethodInfo interfaceTest2 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test2))!;
		MethodInfo methodInfo = SpyExtensions.GetMethodInfo<Action>(() => spyObject.Test1());

		Assert.Multiple(() => {
			Assert.That(methodInfo, Is.EqualTo(interfaceTest1));
			Assert.That(methodInfo, Is.Not.EqualTo(interfaceTest2));
		});
	}
	
	[Test]
	public void GetMethodInfoTailCallTest() {
		Spy<ITestInterface> spy = m_Generator.CreateSpy<ITestInterface>();
		
		MethodInfo interfaceTest1 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test1))!; 
		MethodInfo interfaceTest2 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test2))!;
		MethodInfo methodInfo = SpyExtensions.GetMethodInfo<Action>(() => spy.Object.Test1());

		Assert.Multiple(() => {
			Assert.That(methodInfo, Is.EqualTo(interfaceTest1));
			Assert.That(methodInfo, Is.Not.EqualTo(interfaceTest2));
		});
	}
}
