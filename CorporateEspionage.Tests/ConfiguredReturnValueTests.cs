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
	public void IntConfigureOnlyFirstParameterlessCallByIndex() {
		m_Spy.ConfigureCall(ti => ti.TestInt(), (_, i) => i == 0, 5);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(5));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(default(int)));
		});
	}

	[Test]
	public void IntConfigureOnlyNonFirstParameterlessCallByIndex() {
		m_Spy.ConfigureCall(ti => ti.TestInt(), (_, i) => i == 1, 5);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(5));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(default(int)));
		});
	}

	[Test]
	public void IntConfigureFirstAndNonFirstParameterlessCallByIndex1() {
		m_Spy.ConfigureCall(ti => ti.TestInt(), (_, i) => i == 0, 5);
		m_Spy.ConfigureCall(ti => ti.TestInt(), (_, i) => i == 1, 4);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(5));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(4));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(default(int)));
		});
	}

	[Test]
	public void IntConfigureFirstAndNonFirstParameterlessCallByIndex2() {
		m_Spy.ConfigureCall(ti => ti.TestInt(), (_, i) => i == 0, 3);
		m_Spy.ConfigureCall(ti => ti.TestInt(), (_, i) => i == 2, 4);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(3));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(4));
		});
	}

	[Test]
	public void IntConfigureMultipleNonFirstParameterlessCallByIndex2() {
		m_Spy.ConfigureCall(ti => ti.TestInt(), (_, i) => i == 1, 8);
		m_Spy.ConfigureCall(ti => ti.TestInt(), (_, i) => i == 2, 7);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(8));
			Assert.That(m_Spy.Object.TestInt(), Is.EqualTo(7));
		});
	}
	
	
	[Test]
	public void IntConfigureOnlyFirstParameterizedCallByIndex1() {
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (_, i) => i == 0, 5);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(1), Is.EqualTo(5));
			Assert.That(m_Spy.Object.TestInt(2), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(3), Is.EqualTo(default(int)));
		});
	}
	
	[Test]
	public void IntConfigureOnlyFirstParameterizedCallByIndex2() {
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (_, i) => i == 0, 5);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(4), Is.EqualTo(5));
			Assert.That(m_Spy.Object.TestInt(5), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(6), Is.EqualTo(default(int)));
		});
	}

	[Test]
	public void IntConfigureOnlyNonFirstParameterizedCallByIndex1() {
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (_, i) => i == 1, 5);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(1), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(2), Is.EqualTo(5));
			Assert.That(m_Spy.Object.TestInt(3), Is.EqualTo(default(int)));
		});
	}

	[Test]
	public void IntConfigureOnlyNonFirstParameterizedCallByIndex2() {
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (_, i) => i == 1, 5);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(4), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(5), Is.EqualTo(5));
			Assert.That(m_Spy.Object.TestInt(6), Is.EqualTo(default(int)));
		});
	}

	[Test]
	public void IntConfigureFirstAndNonFirstParameterizedCallByIndex1_1() {
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (_, i) => i == 0, 5);
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (_, i) => i == 1, 4);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(1), Is.EqualTo(5));
			Assert.That(m_Spy.Object.TestInt(2), Is.EqualTo(4));
			Assert.That(m_Spy.Object.TestInt(3), Is.EqualTo(default(int)));
		});
	}

	[Test]
	public void IntConfigureFirstAndNonFirstParameterizedCallByIndex1_2() {
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (_, i) => i == 0, 5);
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (_, i) => i == 1, 4);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(4), Is.EqualTo(5));
			Assert.That(m_Spy.Object.TestInt(5), Is.EqualTo(4));
			Assert.That(m_Spy.Object.TestInt(6), Is.EqualTo(default(int)));
		});
	}

	[Test]
	public void IntConfigureFirstAndNonFirstParameterizedCallByIndex2_1() {
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (_, i) => i == 0, 3);
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (_, i) => i == 2, 4);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(1), Is.EqualTo(3));
			Assert.That(m_Spy.Object.TestInt(2), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(3), Is.EqualTo(4));
		});
	}

	[Test]
	public void IntConfigureFirstAndNonFirstParameterizedCallByIndex2_2() {
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (_, i) => i == 0, 3);
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (_, i) => i == 2, 4);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(4), Is.EqualTo(3));
			Assert.That(m_Spy.Object.TestInt(5), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(6), Is.EqualTo(4));
		});
	}
	
	
	[Test]
	public void IntConfigureSingleCallByParameters1() {
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (p, _) => (int) p[0]! == 1, 3);
		m_Spy.ConfigureCall(ti => ti.TestInt(1), (p, _) => (int) p[0]! == 2, 4);
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestInt(1), Is.EqualTo(3));
			Assert.That(m_Spy.Object.TestInt(1), Is.EqualTo(3));
			Assert.That(m_Spy.Object.TestInt(2), Is.EqualTo(4));
			Assert.That(m_Spy.Object.TestInt(2), Is.EqualTo(4));
			Assert.That(m_Spy.Object.TestInt(3), Is.EqualTo(default(int)));
			Assert.That(m_Spy.Object.TestInt(3), Is.EqualTo(default(int)));
		});
	}
	
	
	
	
	
	[Test]
	public void StringConfigureOnlyFirstParameterizedCallByIndex1() {
		m_Spy.ConfigureCall(ti => ti.TestString(1), (_, i) => i == 0, "hey");
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestString(1), Is.EqualTo("hey"));
			Assert.That(m_Spy.Object.TestString(2), Is.EqualTo(default(string)));
			Assert.That(m_Spy.Object.TestString(3), Is.EqualTo(default(string)));
		});
	}
	
	[Test]
	public void StringConfigureOnlyFirstParameterizedCallByIndex2() {
		m_Spy.ConfigureCall(ti => ti.TestString(1), (_, i) => i == 0, "hey");
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestString(4), Is.EqualTo("hey"));
			Assert.That(m_Spy.Object.TestString(5), Is.EqualTo(default(string)));
			Assert.That(m_Spy.Object.TestString(6), Is.EqualTo(default(string)));
		});
	}

	[Test]
	public void StringConfigureOnlyNonFirstParameterizedCallByIndex1() {
		m_Spy.ConfigureCall(ti => ti.TestString(1), (_, i) => i == 1, "hey");
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestString(1), Is.EqualTo(default(string)));
			Assert.That(m_Spy.Object.TestString(2), Is.EqualTo("hey"));
			Assert.That(m_Spy.Object.TestString(3), Is.EqualTo(default(string)));
		});
	}

	[Test]
	public void StringConfigureOnlyNonFirstParameterizedCallByIndex2() {
		m_Spy.ConfigureCall(ti => ti.TestString(1), (_, i) => i == 1, "hey");
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestString(4), Is.EqualTo(default(string)));
			Assert.That(m_Spy.Object.TestString(5), Is.EqualTo("hey"));
			Assert.That(m_Spy.Object.TestString(6), Is.EqualTo(default(string)));
		});
	}

	[Test]
	public void StringConfigureFirstAndNonFirstParameterizedCallByIndex1_1() {
		m_Spy.ConfigureCall(ti => ti.TestString(1), (_, i) => i == 0, "hey");
		m_Spy.ConfigureCall(ti => ti.TestString(1), (_, i) => i == 1, "hi");
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestString(1), Is.EqualTo("hey"));
			Assert.That(m_Spy.Object.TestString(2), Is.EqualTo("hi"));
			Assert.That(m_Spy.Object.TestString(3), Is.EqualTo(default(string)));
		});
	}

	[Test]
	public void StringConfigureFirstAndNonFirstParameterizedCallByIndex1_2() {
		m_Spy.ConfigureCall(ti => ti.TestString(1), (_, i) => i == 0, "hey");
		m_Spy.ConfigureCall(ti => ti.TestString(1), (_, i) => i == 1, "hi");
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestString(4), Is.EqualTo("hey"));
			Assert.That(m_Spy.Object.TestString(5), Is.EqualTo("hi"));
			Assert.That(m_Spy.Object.TestString(6), Is.EqualTo(default(string)));
		});
	}

	[Test]
	public void StringConfigureFirstAndNonFirstParameterizedCallByIndex2_1() {
		m_Spy.ConfigureCall(ti => ti.TestString(1), (_, i) => i == 0, "hey");
		m_Spy.ConfigureCall(ti => ti.TestString(1), (_, i) => i == 2, "hi");
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestString(1), Is.EqualTo("hey"));
			Assert.That(m_Spy.Object.TestString(2), Is.EqualTo(default(string)));
			Assert.That(m_Spy.Object.TestString(3), Is.EqualTo("hi"));
		});
	}

	[Test]
	public void StringConfigureFirstAndNonFirstParameterizedCallByIndex2_2() {
		m_Spy.ConfigureCall(ti => ti.TestString(1), (_, i) => i == 0, "hey");
		m_Spy.ConfigureCall(ti => ti.TestString(1), (_, i) => i == 2, "hi");
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestString(4), Is.EqualTo("hey"));
			Assert.That(m_Spy.Object.TestString(5), Is.EqualTo(default(string)));
			Assert.That(m_Spy.Object.TestString(6), Is.EqualTo("hi"));
		});
	}
	
	
	[Test]
	public void StringConfigureSingleCallByParameters1() {
		m_Spy.ConfigureCall(ti => ti.TestString(1), (p, _) => (int) p[0]! == 1, "hey");
		m_Spy.ConfigureCall(ti => ti.TestString(1), (p, _) => (int) p[0]! == 2, "hi");
		
		Assert.Multiple(() => {
			Assert.That(m_Spy.Object.TestString(1), Is.EqualTo("hey"));
			Assert.That(m_Spy.Object.TestString(1), Is.EqualTo("hey"));
			Assert.That(m_Spy.Object.TestString(2), Is.EqualTo("hi"));
			Assert.That(m_Spy.Object.TestString(2), Is.EqualTo("hi"));
			Assert.That(m_Spy.Object.TestString(3), Is.EqualTo(default(string)));
			Assert.That(m_Spy.Object.TestString(3), Is.EqualTo(default(string)));
		});
	}
}