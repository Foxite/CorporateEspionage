namespace CorporateEspionage.Tests;

public class ConfiguredReturnValueTests {
	private SpyGenerator m_Generator;
	private Spy<ITestInterface3> m_Spy;

	[SetUp]
	public void Setup() {
		m_Generator = new SpyGenerator();
		m_Spy = m_Generator.CreateSpy<ITestInterface3>();
	}

	// Can we make this DRY?
	[Test]
	public void ConfigureOnlyFirstParameterlessCallByIndex() {
		m_Spy.ConfigureCall(ti => ti.TestInt(), (p, i) => i == 0, 5);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(5));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(default(int)));
		});
	}

	[Test]
	public void ConfigureOnlyNonFirstParameterlessCallByIndex() {
		m_Spy.ConfigureCall(ti => ti.TestInt(), (p, i) => i == 1, 5);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(5));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(default(int)));
		});
	}

	[Test]
	public void ConfigureFirstAndNonFirstParameterlessCallByIndex1() {
		m_Spy.ConfigureCall(ti => ti.TestInt(), (p, i) => i == 0, 5);
		m_Spy.ConfigureCall(ti => ti.TestInt(), (p, i) => i == 1, 4);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(5));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(4));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(default(int)));
		});
	}

	[Test]
	public void ConfigureFirstAndNonFirstParameterlessCallByIndex2() {
		m_Spy.ConfigureCall(ti => ti.TestInt(), (p, i) => i == 0, 3);
		m_Spy.ConfigureCall(ti => ti.TestInt(), (p, i) => i == 2, 4);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(3));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(4));
		});
	}

	[Test]
	public void ConfigureMultipleNonFirstParameterlessCallByIndex2() {
		m_Spy.ConfigureCall(ti => ti.TestInt(), (p, i) => i == 1, 8);
		m_Spy.ConfigureCall(ti => ti.TestInt(), (p, i) => i == 2, 7);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(8));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(7));
		});
	}
	
	
	[Test]
	public void ConfigureOnlyFirstParameterizedCallByIndex1() {
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (p, i) => i == 0, 5);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(1), Is.EqualTo(5));
			Assert.That(m_Spy.Object.TestInt(2), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(3), Is.EqualTo(default(int)));
		});
	}
	
	[Test]
	public void ConfigureOnlyFirstParameterizedCallByIndex2() {
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (p, i) => i == 0, 5);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(4), Is.EqualTo(5));
			Assert.That(m_Spy.Object.TestInt(5), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(6), Is.EqualTo(default(int)));
		});
	}

	[Test]
	public void ConfigureOnlyNonFirstParameterizedCallByIndex1() {
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (p, i) => i == 1, 5);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(1), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(2), Is.EqualTo(5));
			Assert.That(m_Spy.Object.TestInt(3), Is.EqualTo(default(int)));
		});
	}

	[Test]
	public void ConfigureOnlyNonFirstParameterizedCallByIndex2() {
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (p, i) => i == 1, 5);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(4), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(5), Is.EqualTo(5));
			Assert.That(m_Spy.Object.TestInt(6), Is.EqualTo(default(int)));
		});
	}

	[Test]
	public void ConfigureFirstAndNonFirstParameterizedCallByIndex1_1() {
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (p, i) => i == 0, 5);
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (p, i) => i == 1, 4);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(1), Is.EqualTo(5));
			Assert.That(m_Spy.Object.TestInt(2), Is.EqualTo(4));
			Assert.That(m_Spy.Object.TestInt(3), Is.EqualTo(default(int)));
		});
	}

	[Test]
	public void ConfigureFirstAndNonFirstParameterizedCallByIndex1_2() {
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (p, i) => i == 0, 5);
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (p, i) => i == 1, 4);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(4), Is.EqualTo(5));
			Assert.That(m_Spy.Object.TestInt(5), Is.EqualTo(4));
			Assert.That(m_Spy.Object.TestInt(6), Is.EqualTo(default(int)));
		});
	}

	[Test]
	public void ConfigureFirstAndNonFirstParameterizedCallByIndex2_1() {
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (p, i) => i == 0, 3);
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (p, i) => i == 2, 4);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(1), Is.EqualTo(3));
			Assert.That(m_Spy.Object.TestInt(2), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(3), Is.EqualTo(4));
		});
	}

	[Test]
	public void ConfigureFirstAndNonFirstParameterizedCallByIndex2_2() {
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (p, i) => i == 0, 3);
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (p, i) => i == 2, 4);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(4), Is.EqualTo(3));
			Assert.That(m_Spy.Object.TestInt(5), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(6), Is.EqualTo(4));
		});
	}
	
	
	[Test]
	public void ConfigureSingleCallByParameters1() {
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (p, i) => (int) p[0]! == 1, 3);
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (p, i) => (int) p[0]! == 2, 4);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(1), Is.EqualTo(3));
			Assert.That(m_Spy.Object.TestInt(1), Is.EqualTo(3));
			Assert.That(m_Spy.Object.TestInt(2), Is.EqualTo(4));
			Assert.That(m_Spy.Object.TestInt(2), Is.EqualTo(4));
			Assert.That(m_Spy.Object.TestInt(3), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(3), Is.EqualTo(default(int)));
		});
	}
}

public interface ITestInterface3 {
	int TestInt();
	int TestInt(int one);
	int TestInt(int one, string two);
	string TestString();
	string TestString(int one);
	string TestString(int one, string two);
}
