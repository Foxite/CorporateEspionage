using System.Reflection;

namespace FridgeBot.Tests.CorporateEspionage;

public class CallParameters {
	private readonly IReadOnlyList<object?> m_Parameters;
	private readonly IReadOnlyList<Type> m_GenericParameters;

	public MethodInfo MethodInfo { get; }
	
	internal CallParameters(MethodInfo methodInfo, IReadOnlyList<object?> parameters, IReadOnlyList<Type> genericParameters) {
		m_Parameters = parameters;
		m_GenericParameters = genericParameters;
		MethodInfo = methodInfo;
	}

	public object? GetParameter(int index) => m_Parameters[index];
	public object? GetParameter(string name) => m_Parameters[MethodInfo.GetParameters().IndexOf(pi => pi.Name == name)];
	public Type GetGenericParameter(int index) => m_GenericParameters[index];
}
