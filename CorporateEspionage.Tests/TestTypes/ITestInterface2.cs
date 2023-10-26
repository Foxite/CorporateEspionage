namespace CorporateEspionage.Tests;

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
