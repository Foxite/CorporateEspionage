using System.Reflection;
using CorporateEspionage.NUnit;

namespace CorporateEspionage.Tests;

public class NUnitSpyCallsTests {
	private SpyGenerator m_Generator;
	
	[SetUp]
	public void Setup() {
		m_Generator = new SpyGenerator();
	}
	
	[Test]
	public void RegistersSingleParameterlessCall() {
		Spy<ITestInterface> spy = m_Generator.CreateSpy<ITestInterface>();
		spy.Object.Test1();
		
		MethodInfo interfaceTest1 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test1))!;
		
		Assert.Multiple(() => {
			Assert.That(spy, Was.Called(interfaceTest1).Times(1));
			Assert.That(spy, Was.Called(interfaceTest1).Times(Is.EqualTo(1)));
			Assert.That(spy, Was.NoOtherCalls());
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
			Assert.That(spy, Was.Called(interfaceTest1).Times(2));
			Assert.That(spy, Was.Called(interfaceTest1).Times(Is.EqualTo(2)));
			Assert.That(spy, Was.Called(interfaceTest1).Times(Is.GreaterThan(1)));
			Assert.That(spy, Was.Called(interfaceTest1).Times(Is.GreaterThanOrEqualTo(1)));
			
			Assert.That(spy, Was.Called(interfaceTest2).Times(1));
			Assert.That(spy, Was.Called(interfaceTest2).Times(Is.EqualTo(1)));
			
			Assert.That(spy, Was.NotCalled(interfaceTest6));
			
			Assert.That(spy, Was.NoOtherCalls());
		});
	}
	
	[Test]
	public void RegistersSingleParameterCallsOnDifferentSpies() {
		MethodInfo interfaceTest7 = typeof(ITestInterface).GetMethod(nameof(ITestInterface.Test7))!;
		Spy<ITestInterface> spy1 = m_Generator.CreateSpy<ITestInterface>();
		Spy<ITestInterface> spy2 = m_Generator.CreateSpy<ITestInterface>();
		
		spy1.Object.Test7(5);
		spy2.Object.Test7(7);
		
		Assert.Multiple(() => {
			Assert.That(spy1, Was.Called(interfaceTest7).Times(1));
			Assert.That(spy1, Was.Called(interfaceTest7).Times(Is.EqualTo(1)));
			Assert.That(spy1, Was.NoOtherCalls());
			
			Assert.That(spy2, Was.Called(interfaceTest7).Times(1));
			Assert.That(spy2, Was.Called(interfaceTest7).Times(Is.EqualTo(1)));
			Assert.That(spy2, Was.NoOtherCalls());
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

		Assert.Multiple(() => {
			Assert.That(spy1, Was.Called(interfaceTest3Str).Times(1));
			Assert.That(spy1, Was.Called(interfaceTest3Str).Times(1).With(0, 0, "yo"));
			Assert.That(spy1, Was.Called(interfaceTest3Str).Times(1).With(0, 0, Is.EqualTo("yo")));
			Assert.That(spy1, Was.Called(interfaceTest3Str).Times(1).With(0, "ho", "yo"));
			Assert.That(spy1, Was.Called(interfaceTest3Str).Times(1).With(0, "ho", Is.EqualTo("yo")));
			Assert.That(spy1, Was.Called(interfaceTest3Str).With(0, 0, "yo"));
			Assert.That(spy1, Was.Called(interfaceTest3Str).With(0, 0, Is.EqualTo("yo")));
			Assert.That(spy1, Was.Called(interfaceTest3Str).With(0, "ho", "yo"));
			Assert.That(spy1, Was.Called(interfaceTest3Str).With(0, "ho", Is.EqualTo("yo")));
			Assert.That(spy1, Was.NoOtherCalls());
			
			Assert.That(spy2, Was.Called(interfaceTest3Int).Times(1));
			Assert.That(spy2, Was.Called(interfaceTest3Int).Times(1).With(0, 0, 235));
			Assert.That(spy2, Was.Called(interfaceTest3Int).Times(1).With(0, 0, Is.EqualTo(235)));
			Assert.That(spy2, Was.Called(interfaceTest3Int).Times(1).With(0, "hey", 235));
			Assert.That(spy2, Was.Called(interfaceTest3Int).Times(1).With(0, "hey", Is.EqualTo(235)));
			Assert.That(spy2, Was.Called(interfaceTest3Int).With(0, 0, 235));
			Assert.That(spy2, Was.Called(interfaceTest3Int).With(0, 0, Is.EqualTo(235)));
			Assert.That(spy2, Was.Called(interfaceTest3Int).With(0, "hey", 235));
			Assert.That(spy2, Was.Called(interfaceTest3Int).With(0, "hey", Is.EqualTo(235)));
			Assert.That(spy2, Was.NoOtherCalls());
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
		
		Assert.Multiple(() => {
			Assert.That(spy1, Was.Called(interfaceTest4).Times(1));
			Assert.That(spy1, Was.Called(interfaceTest4).With(0, 0, "ello"));
			Assert.That(spy1, Was.Called(interfaceTest4).With(0, 0, Is.EqualTo("ello")));
			Assert.That(spy1, Was.Called(interfaceTest4).With(0, "yo", "ello"));
			Assert.That(spy1, Was.Called(interfaceTest4).With(0, "yo", Is.EqualTo("ello")));
			
			Assert.That(spy1, Was.Called(interfaceTest4).With(0, 1, 235));
			Assert.That(spy1, Was.Called(interfaceTest4).With(0, 1, Is.EqualTo(235)));
			Assert.That(spy1, Was.Called(interfaceTest4).With(0, "hey", 235));
			Assert.That(spy1, Was.Called(interfaceTest4).With(0, "hey", Is.EqualTo(235)));
			
			Assert.That(spy1, Was.NoOtherCalls());
			
			
			Assert.That(spy2, Was.Called(interfaceTest5).Times(1));
			Assert.That(spy2, Was.Called(interfaceTest5).With(0, 0, "yttg"));
			Assert.That(spy2, Was.Called(interfaceTest5).With(0, 0, Is.EqualTo("yttg")));
			Assert.That(spy2, Was.Called(interfaceTest5).With(0, "yo", "yttg"));
			Assert.That(spy2, Was.Called(interfaceTest5).With(0, "yo", Is.EqualTo("yttg")));
			
			Assert.That(spy2, Was.Called(interfaceTest5).With(0, 1, 523));
			Assert.That(spy2, Was.Called(interfaceTest5).With(0, 1, Is.EqualTo(523)));
			Assert.That(spy2, Was.Called(interfaceTest5).With(0, "hey", 523));
			Assert.That(spy2, Was.Called(interfaceTest5).With(0, "hey", Is.EqualTo(523)));
			
			Assert.That(spy2, Was.NoOtherCalls());
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
			Assert.That(spy, Was.Called(interfaceTest3).Times(2));
			
			Assert.That(spy, Was.Called(interfaceTest3).With(0, 0, 512));
			Assert.That(spy, Was.Called(interfaceTest3).With(0, 0, Is.EqualTo(512)));
			Assert.That(spy, Was.Called(interfaceTest3).With(0, "hey", 512));
			Assert.That(spy, Was.Called(interfaceTest3).With(0, "hey", Is.EqualTo(512)));
			
			
			Assert.That(spy, Was.Called(interfaceTest3).With(1, 0, 124));
			Assert.That(spy, Was.Called(interfaceTest3).With(1, 0, Is.EqualTo(124)));
			Assert.That(spy, Was.Called(interfaceTest3).With(1, "hey", 124));
			Assert.That(spy, Was.Called(interfaceTest3).With(1, "hey", Is.EqualTo(124)));
			
			
			
			Assert.That(spy, Was.Called(interfaceTest4).Times(1));
			Assert.That(spy, Was.Called(interfaceTest4).With(0, 0, "oya"));
			Assert.That(spy, Was.Called(interfaceTest4).With(0, 0, Is.EqualTo("oya")));
			Assert.That(spy, Was.Called(interfaceTest4).With(0, "yo", "oya"));
			Assert.That(spy, Was.Called(interfaceTest4).With(0, "yo", Is.EqualTo("oya")));
			
			Assert.That(spy, Was.Called(interfaceTest4).With(0, 1, 125125));
			Assert.That(spy, Was.Called(interfaceTest4).With(0, 1, Is.EqualTo(125125)));
			Assert.That(spy, Was.Called(interfaceTest4).With(0, "hey", 125125));
			Assert.That(spy, Was.Called(interfaceTest4).With(0, "hey", Is.EqualTo(125125)));
			
			Assert.That(spy, Was.NotCalled(interfaceTest3Wrong));
			Assert.That(spy, Was.NotCalled(interfaceTest5));
			
			Assert.That(spy, Was.NoOtherCalls());
		});
	}
}
