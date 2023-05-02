using System.Reflection;

namespace CorporateEspionage;

public class CallParameters {
	public IReadOnlyList<object?> Parameters { get; }
	public IReadOnlyList<Type> GenericParameters { get; }
	public bool Ignored { get; }

	public MethodInfo MethodInfo { get; }
	public bool Verified { get; set; } = false;
	
	internal CallParameters(MethodInfo methodInfo, IReadOnlyList<object?> parameters, IReadOnlyList<Type> genericParameters, bool ignored) {
		Parameters = parameters;
		GenericParameters = genericParameters;
		Ignored = ignored;
		MethodInfo = methodInfo;
	}

	public object? GetParameter(int index) => Parameters[index];
	public object? GetParameter(string name) => Parameters[MethodInfo.GetParameters().IndexOf(pi => pi.Name == name)];
	public Type GetGenericParameter(int index) => GenericParameters[index];
}
