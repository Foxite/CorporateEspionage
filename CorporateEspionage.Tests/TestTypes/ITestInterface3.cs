namespace CorporateEspionage.Tests;

public interface ITestInterface3 {
	int ReadonlyProperty { get; }
	int ReadwriteProperty { get; set; }
	int WriteonlyProperty { set; }
	
	int TestInt();
	int TestInt(int one);
	int TestInt(int one, string two);
	string TestString();
	string TestString(int one);
	string TestString(int one, string two);
}
