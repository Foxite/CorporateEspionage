using System.Reflection;

namespace CorporateEspionage;

public interface ISpy {
	IReadOnlyDictionary<MethodInfo, IReadOnlyList<CallParameters>> GetCalls();
}
