using System.Reflection;

namespace CorporateEspionage.Tests;

public class SpyCallsTests {
	private SpyGenerator m_Generator;
	
	[SetUp]
	public void Setup() {
		m_Generator = new SpyGenerator();
	}
	
	[Test]
	public void GeneratesSpies() {
		Spy<ITestInterface> spy = m_Generator.CreateSpy<ITestInterface>();

		Assert.Multiple(() => {
			Assert.That(spy, Is.Not.Null);
			Assert.That(spy.Object, Is.Not.Null);
			
			Assert.That(() => spy.Object.Test1()         , Throws.Nothing);
			Assert.That(() => spy.Object.Test2()         , Throws.Nothing);
			Assert.That(() => spy.Object.Test3(12)       , Throws.Nothing);
			Assert.That(() => spy.Object.Test3("hey")    , Throws.Nothing);
			Assert.That(() => spy.Object.Test4("oi", 226), Throws.Nothing);
			Assert.That(() => spy.Object.Test5("yo", 235), Throws.Nothing);
			Assert.That(() => spy.Object.Test6()         , Throws.Nothing);
			Assert.That(() => spy.Object.Test7(521)      , Throws.Nothing);
		});
	}

	[Test]
	public void CachesSpies() {
		Spy<ITestInterface> spy1 = m_Generator.CreateSpy<ITestInterface>();
		Spy<ITestInterface> spy2 = m_Generator.CreateSpy<ITestInterface>();

		Assert.Multiple(() => {
			Assert.That(spy1, Is.Not.SameAs(spy2));
			Assert.That(spy1.Object, Is.Not.SameAs(spy2.Object));
			Assert.That(spy1.Object, Is.TypeOf(spy2.Object.GetType()));
		});
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

		Assert.Multiple(() => {
			Assert.That(callParameters, Is.Not.Null);
			Assert.That(callParameters.MethodInfo, Is.EqualTo(interfaceTest1));
		});
	}
	
	[Test]
	public void RegistersMultipleParameterlessCalls() {
		Spy<ITestInterface> spy = m_Generator.CreateSpy<ITestInterface>();
		spy.Object.Test1();
		spy.Object.Test1();
		spy.Object.Test2();
		
		MethodInfo interfaceTest1 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test1))!;
		MethodInfo interfaceTest2 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test2))!;
		MethodInfo interfaceTest6 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test6))!;

		Assert.Multiple(() => {
			Assert.Throws<ArgumentOutOfRangeException>(() => spy.GetCallParameters(interfaceTest1, 2));
			Assert.Throws<ArgumentOutOfRangeException>(() => spy.GetCallParameters(interfaceTest2, 1));
			Assert.Throws<KeyNotFoundException>(() => spy.GetCallParameters(interfaceTest6, 0));

			CallParameters callParameters = spy.GetCallParameters(interfaceTest1, 0);
			Assert.That(callParameters, Is.Not.Null);
			Assert.That(callParameters.MethodInfo, Is.EqualTo(interfaceTest1));
			
			callParameters = spy.GetCallParameters(interfaceTest1, 1);
			Assert.That(callParameters, Is.Not.Null);
			Assert.That(callParameters.MethodInfo, Is.EqualTo(interfaceTest1));
			
			callParameters = spy.GetCallParameters(interfaceTest2, 0);
			Assert.That(callParameters, Is.Not.Null);
			Assert.That(callParameters.MethodInfo, Is.EqualTo(interfaceTest2));
		});
	}
	
	[Test]
	public void RegistersSingleParameterCallsOnDifferentSpies() {
		MethodInfo interfaceTest7 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test7))!;
		Spy<ITestInterface> spy1 = m_Generator.CreateSpy<ITestInterface>();
		Spy<ITestInterface> spy2 = m_Generator.CreateSpy<ITestInterface>();
		
		spy1.Object.Test7(5);
		spy2.Object.Test7(7);
		
		CallParameters callParameters1 = spy1.GetCallParameters(interfaceTest7, 0);
		CallParameters callParameters2 = spy2.GetCallParameters(interfaceTest7, 0);

		Assert.Multiple(() => {
			Assert.Throws<ArgumentOutOfRangeException>(() => spy1.GetCallParameters(interfaceTest7, 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => spy2.GetCallParameters(interfaceTest7, 1));
			
			Assert.That(callParameters1, Is.Not.Null);
			Assert.That(callParameters2, Is.Not.Null);
			Assert.That(callParameters1.MethodInfo, Is.EqualTo(interfaceTest7));
			Assert.That(callParameters2.MethodInfo, Is.EqualTo(interfaceTest7));
			
			Assert.That(callParameters1.GetParameter(0    ), Is.EqualTo(5));
			Assert.That(callParameters1.GetParameter("hey"), Is.EqualTo(5));
			Assert.That(callParameters2.GetParameter(0    ), Is.EqualTo(7));
			Assert.That(callParameters2.GetParameter("hey"), Is.EqualTo(7));
		});
	}
	
	[Test]
	public void RegistersSingleParameterCallsOnMultipleOverloads() {
		Spy<ITestInterface> spy1 = m_Generator.CreateSpy<ITestInterface>();
		Spy<ITestInterface> spy2 = m_Generator.CreateSpy<ITestInterface>();
		spy1.Object.Test3("yo");
		spy2.Object.Test3(235);
		
		MethodInfo interfaceTest3Str = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test3), new[] { typeof(string) })!;
		MethodInfo interfaceTest3Int = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test3), new[] { typeof(int) })!;
		
		CallParameters callParameters1 = spy1.GetCallParameters(interfaceTest3Str, 0);
		CallParameters callParameters2 = spy2.GetCallParameters(interfaceTest3Int, 0);

		Assert.Multiple(() => {
			Assert.Throws<ArgumentOutOfRangeException>(() => spy1.GetCallParameters(interfaceTest3Str, 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => spy2.GetCallParameters(interfaceTest3Int, 1));
			Assert.Throws<KeyNotFoundException>(() => spy1.GetCallParameters(interfaceTest3Int, 0));
			Assert.Throws<KeyNotFoundException>(() => spy2.GetCallParameters(interfaceTest3Str, 0));

			Assert.That(callParameters1, Is.Not.Null);
			Assert.That(callParameters2, Is.Not.Null);
			Assert.That(callParameters1.MethodInfo, Is.EqualTo(interfaceTest3Str));
			Assert.That(callParameters2.MethodInfo, Is.EqualTo(interfaceTest3Int));
			
			Assert.That(callParameters1.GetParameter(0    ), Is.EqualTo("yo"));
			Assert.That(callParameters1.GetParameter("ho" ), Is.EqualTo("yo"));
			Assert.That(callParameters2.GetParameter(0    ), Is.EqualTo(235));
			Assert.That(callParameters2.GetParameter("hey"), Is.EqualTo(235));
		});
	}
	
	[Test]
	public void RegistersMultipleParameterCall() {
		MethodInfo interfaceTest4 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test4), new[] { typeof(string), typeof(int) })!;
		MethodInfo interfaceTest5 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test5), new[] { typeof(string), typeof(int) })!;
		
		Spy<ITestInterface> spy1 = m_Generator.CreateSpy<ITestInterface>();
		Spy<ITestInterface> spy2 = m_Generator.CreateSpy<ITestInterface>();
		spy1.Object.Test4("ello", 235);
		spy2.Object.Test5("yttg", 523);
		
		CallParameters callParameters1 = spy1.GetCallParameters(interfaceTest4, 0);
		CallParameters callParameters2 = spy2.GetCallParameters(interfaceTest5, 0);

		Assert.Multiple(() => {
			Assert.That(callParameters1, Is.Not.Null);
			Assert.That(callParameters2, Is.Not.Null);
			Assert.That(callParameters1.MethodInfo, Is.EqualTo(interfaceTest4));			
			Assert.That(callParameters2.MethodInfo, Is.EqualTo(interfaceTest5));

			Assert.That(callParameters1.GetParameter(0    ), Is.EqualTo("ello"));
			Assert.That(callParameters1.GetParameter("yo" ), Is.EqualTo("ello"));
			Assert.That(callParameters1.GetParameter(1    ), Is.EqualTo(235));
			Assert.That(callParameters1.GetParameter("hey"), Is.EqualTo(235));

			Assert.That(callParameters2.GetParameter(0    ), Is.EqualTo("yttg"));
			Assert.That(callParameters2.GetParameter("yo" ), Is.EqualTo("yttg"));
			Assert.That(callParameters2.GetParameter(1    ), Is.EqualTo(523));
			Assert.That(callParameters2.GetParameter("hey"), Is.EqualTo(523));
		});
	}
	
	[Test]
	public void RegistersMultipleParameterCalls() {
		Spy<ITestInterface> spy = m_Generator.CreateSpy<ITestInterface>();
		spy.Object.Test3(512);
		spy.Object.Test3(124);
		spy.Object.Test4("oya", 125125);

		MethodInfo interfaceTest3 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test3), new[] { typeof(int) })!;
		MethodInfo interfaceTest3Wrong = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test3), new[] { typeof(string) })!;
		MethodInfo interfaceTest4 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test4))!;
		MethodInfo interfaceTest5 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test5))!;

		Assert.Multiple(() => {
			Assert.Throws<ArgumentOutOfRangeException>(() => spy.GetCallParameters(interfaceTest3, 2));
			Assert.Throws<ArgumentOutOfRangeException>(() => spy.GetCallParameters(interfaceTest4, 1));
			Assert.Throws<KeyNotFoundException>(() => spy.GetCallParameters(interfaceTest5, 0));
			Assert.Throws<KeyNotFoundException>(() => spy.GetCallParameters(interfaceTest3Wrong, 0));

			CallParameters callParameters = spy.GetCallParameters(interfaceTest3, 0);
			Assert.That(callParameters, Is.Not.Null);
			Assert.That(callParameters.MethodInfo, Is.EqualTo(interfaceTest3));
			Assert.That(callParameters.GetParameter("hey"), Is.EqualTo(512));
			Assert.That(callParameters.GetParameter(0    ), Is.EqualTo(512));

			callParameters = spy.GetCallParameters(interfaceTest3, 1);
			Assert.That(callParameters, Is.Not.Null);
			Assert.That(callParameters.MethodInfo, Is.EqualTo(interfaceTest3));
			Assert.That(callParameters.GetParameter("hey"), Is.EqualTo(124));
			Assert.That(callParameters.GetParameter(0    ), Is.EqualTo(124));

			callParameters = spy.GetCallParameters(interfaceTest4, 0);
			Assert.That(callParameters, Is.Not.Null);
			Assert.That(callParameters.MethodInfo, Is.EqualTo(interfaceTest4));
			Assert.That(callParameters.GetParameter("yo" ), Is.EqualTo("oya"));
			Assert.That(callParameters.GetParameter(0    ), Is.EqualTo("oya"));
			Assert.That(callParameters.GetParameter("hey"), Is.EqualTo(125125));
			Assert.That(callParameters.GetParameter(1    ), Is.EqualTo(125125));
		});
	}
}
