using System.Reflection;

namespace CorporateEspionage.Tests;

public class ReflectionUtilTests {
	[Test]
	public void CanGenerateSpy() {
		MethodInfo interfaceTest1 = typeof(ITestInterface).GetMethod("Test1")!;
		MethodInfo classTest1     = typeof(TestClass).GetMethod("Test1")!;
		MethodInfo? classTest2    = classTest1.GetInterfaceDeclarationsForMethod().FirstOrDefault();
		
		Assert.That(interfaceTest1, Is.Not.EqualTo(classTest1));
		Assert.That(interfaceTest1, Is.EqualTo(classTest2));
	}
}
