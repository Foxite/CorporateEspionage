using CorporateEspionage.NUnit;

namespace CorporateEspionage.Tests;

public class IgnoringConfigurationTests {
	private SpyGenerator m_Generator;
	private Spy<ITestInterface4> m_Spy;

	[SetUp]
	public void Setup() {
		m_Generator = new SpyGenerator();
		m_Spy = m_Generator.CreateSpy<ITestInterface4>();
	}

	[Test]
	public void TracksNoCallsByDefault() {
		Assert.That(m_Spy, Was.NoOtherCalls());
	}

	[Test]
	public void TracksSingleCallByDefault() {
		m_Spy.Object.Test2();
		
		Assert.Multiple(() => {
			Assert.That(m_Spy, Was.Called(() => m_Spy.Object.Test2()).Times(1));
			Assert.That(m_Spy, Was.NoOtherCalls());
		});
	}

	[Test]
	public void TracksMultipleCallsByDefault1() {
		m_Spy.Object.Test2();
		m_Spy.Object.Test2();
		
		Assert.Multiple(() => {
			Assert.That(m_Spy, Was.Called(() => m_Spy.Object.Test2()).Times(2));
			Assert.That(m_Spy, Was.NoOtherCalls());
		});
	}

	[Test]
	public void TracksMultipleCallsByDefault2() {
		m_Spy.Object.Test2();
		m_Spy.Object.Test1();
		
		Assert.Multiple(() => {
			Assert.That(m_Spy, Was.Called(() => m_Spy.Object.Test2()).Times(1));
			Assert.That(m_Spy, Was.Called(() => m_Spy.Object.Test1()).Times(1));
			Assert.That(m_Spy, Was.NoOtherCalls());
		});
	}

	[Test]
	public void TracksMultipleCallsByDefault3() {
		m_Spy.Object.Test2();
		m_Spy.Object.Test2();
		m_Spy.Object.Test1();
		
		Assert.Multiple(() => {
			Assert.That(m_Spy, Was.Called(() => m_Spy.Object.Test2()).Times(2));
			Assert.That(m_Spy, Was.Called(() => m_Spy.Object.Test1()).Times(1));
			Assert.That(m_Spy, Was.NoOtherCalls());
		});
	}

	[Test]
	public void IgnoresExcludedCallsWithBlankMatcher() {
		m_Spy.ConfigureIgnoring(ti => ti.Test2(), (_, _) => true, true);
		
		m_Spy.Object.Test2();
		m_Spy.Object.Test1();
		
		Assert.Multiple(() => {
			Assert.That(m_Spy, Was.Called(() => m_Spy.Object.Test1()).Times(1));
			Assert.That(m_Spy, Was.NoOtherCalls());
		});
	}

	[Test]
	public void IgnoresExcludedCallsWithParameterMatcher1() {
		m_Spy.ConfigureIgnoring(ti => ti.Test3(0), (p, _) => (int) p[0]! == 5, true);
		
		m_Spy.Object.Test3(5);
		m_Spy.Object.Test3(4);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy, Was.Called(() => m_Spy.Object.Test3(0)).With(1, "hey", Is.EqualTo(4)));
			Assert.That(m_Spy, Was.NoOtherCalls());
		});
	}

	[Test]
	public void IgnoresExcludedCallsWithParameterMatcher2() {
		m_Spy.ConfigureIgnoring(ti => ti.Test3(0), (p, _) => (int) p[0]! == 5, true);
		
		m_Spy.Object.Test3(5);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy, Was.NoOtherCalls());
		});
	}

	[Test]
	public void IgnoresExcludedCallsWithParameterMatcher3() {
		m_Spy.ConfigureIgnoring(ti => ti.Test3(0), (p, _) => (int) p[0]! == 5, true);
		
		m_Spy.Object.Test2();
		m_Spy.Object.Test3(5);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy, Was.Called(() => m_Spy.Object.Test2()).Times(1));
			Assert.That(m_Spy, Was.NoOtherCalls());
		});
	}
}

public interface ITestInterface4 {
	int Test1();
	bool Test2();
	bool Test3(int hey);
}
