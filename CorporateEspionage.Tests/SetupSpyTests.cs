namespace CorporateEspionage.Tests;

public class SetupSpyTests {
	private SpyGenerator m_Generator;

	[SetUp]
	public void Setup() {
		m_Generator = new SpyGenerator();
	}

	[Test]
	public void VoidReturningSpy() {
		Spy<ITestInterface2> spy = m_Generator.CreateSpy<ITestInterface2>();
		Assert.That(() => spy.Object.TestVoid(), Throws.Nothing);
	}

	[Test]
	public void IntReturningSpy() {
		Spy<ITestInterface2> spy = m_Generator.CreateSpy<ITestInterface2>();
		Assert.That(spy.Object.TestInt(), Is.EqualTo(default(int)));
	}

	[Test]
	public void BoolReturningSpy() {
		Spy<ITestInterface2> spy = m_Generator.CreateSpy<ITestInterface2>();
		Assert.That(spy.Object.TestBool(), Is.EqualTo(default(bool)));
	}

	[Test]
	public void StringReturningSpy() {
		Spy<ITestInterface2> spy = m_Generator.CreateSpy<ITestInterface2>();
		Assert.That(spy.Object.TestString(), Is.EqualTo(null));
	}

	[Test]
	public void TaskVoidReturningSpy() {
		Spy<ITestInterface2> spy = m_Generator.CreateSpy<ITestInterface2>();
		Assert.That(async () => await spy.Object.TestVoidAsync(), Throws.Nothing);
	}

	[Test]
	public async Task TaskIntReturningSpy() {
		Spy<ITestInterface2> spy = m_Generator.CreateSpy<ITestInterface2>();
		Assert.That(await spy.Object.TestIntAsync(), Is.EqualTo(default(int)));
	}

	[Test]
	public async Task TaskBoolReturningSpy() {
		Spy<ITestInterface2> spy = m_Generator.CreateSpy<ITestInterface2>();
		Assert.That(await spy.Object.TestBoolAsync(), Is.EqualTo(default(bool)));
	}

	[Test]
	public async Task TaskStringReturningSpy() {
		Spy<ITestInterface2> spy = m_Generator.CreateSpy<ITestInterface2>();
		Assert.That(await spy.Object.TestStringAsync(), Is.EqualTo(null));
	}
}

public interface ITestInterface2 {
	void TestVoid();
	int TestInt();
	bool TestBool();
	string TestString();
	Task TestVoidAsync();
	Task<int> TestIntAsync();
	Task<bool> TestBoolAsync();
	Task<string> TestStringAsync();
}
