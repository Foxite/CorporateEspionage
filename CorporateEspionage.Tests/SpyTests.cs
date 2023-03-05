using System.Reflection;
using FridgeBot.Tests.CorporateEspionage;

namespace CorporateEspionage.Tests;

public class SpyTests {
	private SpyGenerator m_Generator;
	
	[SetUp]
	public void Setup() {
		m_Generator = new SpyGenerator();
	}
	
	[Test]
	public void CanGenerateSpy() {
		Spy<ITestInterface> spy = m_Generator.CreateSpy<ITestInterface>();
		
		Assert.That(spy, Is.Not.Null);
		Assert.That(spy.Object, Is.Not.Null);
	}

	[Test]
	public void CachesSpies() {
		Spy<ITestInterface> spy1 = m_Generator.CreateSpy<ITestInterface>();
		Spy<ITestInterface> spy2 = m_Generator.CreateSpy<ITestInterface>();
		
		Assert.That(spy1, Is.Not.SameAs(spy2));
		Assert.That(spy1.Object, Is.Not.SameAs(spy2.Object));
		Assert.That(spy1.Object, Is.TypeOf(spy2.Object.GetType()));
	}

	[Test]
	public void DeclaresInterfaceImplementations() {
		Spy<ITestInterface> spy = m_Generator.CreateSpy<ITestInterface>();

		MethodInfo interfaceTest1 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test1))!;
		MethodInfo classTest1 = spy.Object.GetType().GetMethod(nameof(ITestInterface.Test1))!;

		Assert.That(interfaceTest1, Is.EqualTo(classTest1.GetInterfaceDeclarationsForMethod().First()));
	}
	
	[Test]
	public void RegistersSingleParameterlessCall() {
		Spy<ITestInterface> spy = m_Generator.CreateSpy<ITestInterface>();
		spy.Object.Test1();
		
		MethodInfo interfaceTest1 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test1))!;
		CallParameters callParameters = spy.GetCallParameters(interfaceTest1, 0);
		Assert.That(callParameters, Is.Not.Null);
		Assert.That(callParameters.MethodInfo, Is.EqualTo(interfaceTest1));
	}
	
	[Test]
	public void RegistersMultipleParameterlessCall() {
		Spy<ITestInterface> spy = m_Generator.CreateSpy<ITestInterface>();
		spy.Object.Test1();
		spy.Object.Test1();
		spy.Object.Test2();
		
		MethodInfo interfaceTest1 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test1))!;
		MethodInfo interfaceTest2 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test2))!;
		MethodInfo interfaceTest6 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test6))!;

		Assert.Throws<ArgumentOutOfRangeException>(() => spy.GetCallParameters(interfaceTest1, 2));
		Assert.Throws<ArgumentOutOfRangeException>(() => spy.GetCallParameters(interfaceTest2, 1));
		Assert.Throws<KeyNotFoundException>(() => spy.GetCallParameters(interfaceTest6, 0));

		{
			CallParameters callParameters = spy.GetCallParameters(interfaceTest1, 0);
			Assert.That(callParameters, Is.Not.Null);
			Assert.That(callParameters.MethodInfo, Is.EqualTo(interfaceTest1));
		}

		{
			CallParameters callParameters = spy.GetCallParameters(interfaceTest1, 1);
			Assert.That(callParameters, Is.Not.Null);
			Assert.That(callParameters.MethodInfo, Is.EqualTo(interfaceTest1));
		}

		{
			CallParameters callParameters = spy.GetCallParameters(interfaceTest2, 0);
			Assert.That(callParameters, Is.Not.Null);
			Assert.That(callParameters.MethodInfo, Is.EqualTo(interfaceTest2));
		}
	}
	
	[Test]
	public void RegistersSingleParameterizedCall1() {
		Spy<ITestInterface> spy = m_Generator.CreateSpy<ITestInterface>();
		spy.Object.Test7(5);
		
		MethodInfo interfaceTest7 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test7))!;
		CallParameters callParameters = spy.GetCallParameters(interfaceTest7, 0);
		Assert.That(callParameters, Is.Not.Null);
		Assert.That(callParameters.MethodInfo, Is.EqualTo(interfaceTest7));
		Assert.That(callParameters.GetParameter(0), Is.EqualTo(5));
		Assert.That(callParameters.GetParameter("hey"), Is.EqualTo(5));
	}
}
