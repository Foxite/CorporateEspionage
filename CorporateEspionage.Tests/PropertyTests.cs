using CorporateEspionage.NUnit;

namespace CorporateEspionage.Tests; 

public class PropertyTests {
	private SpyGenerator m_Generator;
	private Spy<ITestInterface3> m_Spy;

	[SetUp]
	public void Setup() {
		m_Generator = new SpyGenerator();
		m_Spy = m_Generator.CreateSpy<ITestInterface3>();
	}
	
	[Test]
	public void TestPropertyImplementation() {
		m_Spy.ConfigureCall(ti => ti.ReadonlyProperty, (_, i) => i == 0, 5);
		m_Spy.ConfigureCall(ti => ti.ReadonlyProperty, (_, i) => i == 1, 6);
		m_Spy.ConfigureCall(ti => ti.ReadonlyProperty, (_, i) => i == 2, 7);
		
		m_Spy.ConfigureCall(ti => ti.ReadwriteProperty, (_, i) => i == 0, 1);
		m_Spy.ConfigureCall(ti => ti.ReadwriteProperty, (_, i) => i == 1, 2);
		m_Spy.ConfigureCall(ti => ti.ReadwriteProperty, (_, i) => i == 2, 3);

		var propertySetter = typeof(ITestInterface3).GetProperty(nameof(ITestInterface3.WriteonlyProperty))!.SetMethod!;

		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.ReadonlyProperty, Is.EqualTo(5));
			Assert.That(m_Spy.Object.ReadonlyProperty, Is.EqualTo(6));
			Assert.That(m_Spy.Object.ReadonlyProperty, Is.EqualTo(7));
			Assert.That(m_Spy.Object.ReadonlyProperty, Is.EqualTo(default(int)));
			
			Assert.That(m_Spy.Object.ReadwriteProperty, Is.EqualTo(1));
			Assert.That(m_Spy.Object.ReadwriteProperty, Is.EqualTo(2));
			Assert.That(m_Spy.Object.ReadwriteProperty, Is.EqualTo(3));
			Assert.That(m_Spy.Object.ReadwriteProperty, Is.EqualTo(default(int)));
			
			Assert.That(() => m_Spy.Object.WriteonlyProperty = 7, Throws.Nothing);
			Assert.That(() => m_Spy.Object.WriteonlyProperty = 8, Throws.Nothing);
			Assert.That(() => propertySetter.Invoke(m_Spy.Object, new object?[] { 9 }), Throws.Nothing);
			
			Assert.That(m_Spy, Was.Called(() => m_Spy.Object.ReadonlyProperty).Times(4));
			Assert.That(m_Spy, Was.Called(() => m_Spy.Object.ReadwriteProperty).Times(4));
			Assert.That(m_Spy, Was
				.Called(propertySetter)
				.Times(3)
				
				.With(0, 0, 7)
				.With(1, 0, 8)
				.With(2, 0, 9)
				
				.With(0, "value", 7)
				.With(1, "value", 8)
				.With(2, "value", 9)
			);
		});
	}
}
