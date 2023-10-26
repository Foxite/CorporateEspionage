using CorporateEspionage.NUnit;

namespace CorporateEspionage.Tests; 

public class InheritanceTests {
	private SpyGenerator m_Generator;
	private Spy<IDerivedInterface1> m_Spy;

	[SetUp]
	public void Setup() {
		m_Generator = new SpyGenerator();
		m_Spy = m_Generator.CreateSpy<IDerivedInterface1>();
	}

	[Test]
	public void InheritedInterfaceTests() {
		m_Spy.ConfigureCall(ti => ti.Test1(), (_, i) => i == 0, 2);
		m_Spy.ConfigureCall(ti => ti.Test1(), (_, i) => i == 1, 3);
		m_Spy.ConfigureCall(ti => ti.Test1(), (_, i) => i == 2, 4);
		
		m_Spy.ConfigureCall(ti => ti.Test3(), (_, i) => i == 0, true);
		m_Spy.ConfigureCall(ti => ti.Test3(), (_, i) => i == 1, false);
		m_Spy.ConfigureCall(ti => ti.Test3(), (_, i) => i == 2, true);
		
		m_Spy.ConfigureCall(ti => ti.Test4(), (_, i) => i == 0, "hi");
		m_Spy.ConfigureCall(ti => ti.Test4(), (_, i) => i == 1, "ho");
		m_Spy.ConfigureCall(ti => ti.Test4(), (_, i) => i == 2, "hey");
		
		m_Spy.Object.Test2();
		m_Spy.Object.Test2();
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.Test1(), Is.EqualTo(2));
			Assert.That(m_Spy.Object.Test1(), Is.EqualTo(3));
			Assert.That(m_Spy.Object.Test1(), Is.EqualTo(4));
			Assert.That(m_Spy.Object.Test1(), Is.EqualTo(default(int)));
			Assert.That(m_Spy, Was.Called(() => m_Spy.Object.Test1()).Times(4));
			
			Assert.That(m_Spy, Was.Called(() => m_Spy.Object.Test2()).Times(2));
			
			Assert.That(m_Spy.Object.Test3(), Is.EqualTo(true));
			Assert.That(m_Spy.Object.Test3(), Is.EqualTo(false));
			Assert.That(m_Spy.Object.Test3(), Is.EqualTo(true));
			Assert.That(m_Spy.Object.Test3(), Is.EqualTo(default(bool)));
			Assert.That(m_Spy, Was.Called(() => m_Spy.Object.Test3()).Times(4));
			
			Assert.That(m_Spy.Object.Test4(), Is.EqualTo("hi"));
			Assert.That(m_Spy.Object.Test4(), Is.EqualTo("ho"));
			Assert.That(m_Spy.Object.Test4(), Is.EqualTo("hey"));
			Assert.That(m_Spy.Object.Test4(), Is.EqualTo(default(string)));
			Assert.That(m_Spy, Was.Called(() => m_Spy.Object.Test4()).Times(4));
		});
	}
}
