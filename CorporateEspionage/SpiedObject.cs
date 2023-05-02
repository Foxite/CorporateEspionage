using System.Reflection;

namespace CorporateEspionage;

public delegate bool InvocationPredicate(object?[] @params, int invocationIndex);

public abstract class SpiedObject {
	private record ReturnValueConfiguration(InvocationPredicate Predicate, Func<object?> Factory);
	private record CallIgnoringConfiguration(InvocationPredicate Predicate, bool Ignore);
	
	private readonly Dictionary<MethodInfo, List<ReturnValueConfiguration>> m_ReturnValueConfigurations = new();
	private readonly Dictionary<MethodInfo, List<CallIgnoringConfiguration>> m_CallIgnoringConfigurations = new();
	private readonly Dictionary<MethodInfo, List<CallParameters>> m_Calls = new();

	internal IReadOnlyDictionary<MethodInfo, IReadOnlyList<CallParameters>> GetCalls() => m_Calls.WrapReadonly();
	internal void ConfigureReturnValue(MethodInfo method, InvocationPredicate predicate, Func<object?> factory) => m_ReturnValueConfigurations.GetOrAdd(method, _ => new List<ReturnValueConfiguration>()).Add(new ReturnValueConfiguration(predicate, factory));
	internal void ConfigureIgnoring(MethodInfo method, InvocationPredicate predicate, bool ignore) => m_CallIgnoringConfigurations.GetOrAdd(method, _ => new List<CallIgnoringConfiguration>()).Add(new CallIgnoringConfiguration(predicate, ignore));

	private int RegisterCall(MethodInfo method, object?[] @params) {
		// TODO generic parameters
		var callsList = m_Calls.GetOrAdd(method, _ => new List<CallParameters>());
		int invocationIndex = callsList.Count;
		bool ignored = false;
		
		if (m_CallIgnoringConfigurations.TryGetValue(method, out List<CallIgnoringConfiguration>? ignoringConfigurations)) {
			foreach (CallIgnoringConfiguration ignoringConfiguration in ignoringConfigurations) {
				if (ignoringConfiguration.Ignore && ignoringConfiguration.Predicate(@params, invocationIndex)) {
					ignored = true;
					break;
				}
			}
		}
		
		callsList.Add(new CallParameters(method, @params, new Type[] {}, ignored));
		return invocationIndex;
	}

	public void OnCallVoid(MethodInfo method, object?[] @params) {
		method = method.GetInterfaceDeclarationsForMethod().First();
		
		RegisterCall(method, @params);
	}

	public object? OnCallValue(MethodInfo method, object?[] @params) {
		method = method.GetInterfaceDeclarationsForMethod().First();
		
		int invocationIndex = RegisterCall(method, @params);

		if (m_ReturnValueConfigurations.TryGetValue(method, out List<ReturnValueConfiguration>? configurations)) {
			foreach (ReturnValueConfiguration configuration in configurations) {
				if (configuration.Predicate(@params, invocationIndex)) {
					return configuration.Factory();
				}
			}
		}

		return GetDefaultValue(method.ReturnType);
	}

	private object GetDefaultValue(Type t) {
		if (t.IsValueType) {
			return Activator.CreateInstance(t)!;
		} else if (t == typeof(Task)) {
			return Task.CompletedTask;
		} else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Task<>)) {
			MethodInfo fromResult = typeof(Task).GetMethod(nameof(Task.FromResult))!.MakeGenericMethod(t.GenericTypeArguments);
			return fromResult.Invoke(null, new[] { GetDefaultValue(t.GenericTypeArguments[0]) })!;
		} else {
			return null!;
		}
	}
}
