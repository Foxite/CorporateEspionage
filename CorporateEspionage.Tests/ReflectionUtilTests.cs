using System.Reflection;

namespace CorporateEspionage.Tests;

public class ReflectionUtilTests {
	[Test]
	public void CanFindInterfaceDeclarationsForMethod() {
		MethodInfo interfaceTest1 = typeof(ITestInterface).GetMethod("Test1")!;
		MethodInfo classTest1     = typeof(TestClass).GetMethod("Test1")!;
		MethodInfo? classTest2    = classTest1.GetInterfaceDeclarationsForMethod().FirstOrDefault();
		
		Assert.Multiple(() => {
			Assert.That(interfaceTest1, Is.Not.EqualTo(classTest1));
			Assert.That(interfaceTest1, Is.EqualTo(classTest2));
		});
	}
}
