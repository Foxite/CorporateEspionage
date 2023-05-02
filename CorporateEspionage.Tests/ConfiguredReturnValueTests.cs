namespace CorporateEspionage.Tests;

public class ConfiguredReturnValueTests {
	private SpyGenerator m_Generator;

	[SetUp]
	public void Setup() {
		m_Generator = new SpyGenerator();
	}

	[Test]
	public void ConfigureOnlyFirstCallByIndex() {
		Spy<ITestInterface2> spy = m_Generator.CreateSpy<ITestInterface2>();
		spy.ConfigureCallByIndex();
		
		ITestInterface2 spyObject = spy.Object;
		
	}
}
