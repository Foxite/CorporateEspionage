using System.Reflection;

namespace CorporateEspionage;

public interface ISpy {
	int GetCallCount(MethodInfo method);
	int GetCallCount();
	IReadOnlyDictionary<MethodInfo, IReadOnlyList<CallParameters>> GetCalls();
	IReadOnlyList<CallParameters> GetCalls(MethodInfo method);
	CallParameters GetCallParameters(MethodInfo method, int invocation);
}
