namespace CorporateEspionage.Tests;

public class UnconfiguredSpyReturnValueTests {
	private SpyGenerator m_Generator;
	private Spy<ITestInterface2> m_Spy;

	[SetUp]
	public void Setup() {
		m_Generator = new SpyGenerator();
		m_Spy = m_Generator.CreateSpy<ITestInterface2>();
	}

	[Test]
	public void VoidReturningSpy() {
		Assert.That(() => m_Spy.Object.TestVoid(), Throws.Nothing);
	}

	[Test]
	public void IntReturningSpy() {
		Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(default(int)));
	}

	[Test]
	public void BoolReturningSpy() {
		Assert.That(m_Spy.Object.TestBool(), Is.EqualTo(default(bool)));
	}

	[Test]
	public void StringReturningSpy() {
		Assert.That(m_Spy.Object.TestString(), Is.EqualTo(null));
	}

	[Test]
	public void StructReturningSpy() {
		Assert.That(m_Spy.Object.TestStruct(), Is.EqualTo(default(TestStruct)));
	}

	[Test]
	public void TaskVoidReturningSpy() {
		Assert.That(async () => await m_Spy.Object.TestVoidAsync(), Throws.Nothing);
	}

	[Test]
	public async Task TaskIntReturningSpy() {
		Assert.That(await m_Spy.Object.TestIntAsync(), Is.EqualTo(default(int)));
	}

	[Test]
	public async Task TaskBoolReturningSpy() {
		Assert.That(await m_Spy.Object.TestBoolAsync(), Is.EqualTo(default(bool)));
	}

	[Test]
	public async Task TaskStringReturningSpy() {
		Assert.That(await m_Spy.Object.TestStringAsync(), Is.EqualTo(null));
	}

	[Test]
	public async Task TaskStructReturningSpy() {
		Assert.That(await m_Spy.Object.TestStructAsync(), Is.EqualTo(default(TestStruct)));
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
