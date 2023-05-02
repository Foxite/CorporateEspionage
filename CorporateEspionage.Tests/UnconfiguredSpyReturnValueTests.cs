namespace CorporateEspionage.Tests;

public class UnconfiguredSpyReturnValueTests {
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
	public void StructReturningSpy() {
		Spy<ITestInterface2> spy = m_Generator.CreateSpy<ITestInterface2>();
		Assert.That(spy.Object.TestStruct(), Is.EqualTo(default(TestStruct)));
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

	[Test]
	public async Task TaskStructReturningSpy() {
		Spy<ITestInterface2> spy = m_Generator.CreateSpy<ITestInterface2>();
		Assert.That(await spy.Object.TestStructAsync(), Is.EqualTo(default(TestStruct)));
	}
}

public interface ITestInterface2 {
	void TestVoid();
	int TestInt();
	bool TestBool();
	string TestString();
	TestStruct TestStruct();
	Task TestVoidAsync();
	Task<int> TestIntAsync();
	Task<bool> TestBoolAsync();
	Task<string> TestStringAsync();
	Task<TestStruct> TestStructAsync();
}

public struct TestStruct {
	public int field;
}
