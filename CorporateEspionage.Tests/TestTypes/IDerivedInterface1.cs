namespace CorporateEspionage.Tests;

public interface IBaseInterface1 {
	int Test1();
}

public interface IBaseInterface2 {
	void Test2();
}

public interface IBaseInterface3 : IBaseInterface2 {
	bool Test3();
}

public interface IDerivedInterface1 : IBaseInterface1, IBaseInterface3 {
	string Test4();
}
