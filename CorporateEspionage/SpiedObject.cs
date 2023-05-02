using System.Reflection;

namespace CorporateEspionage;

public delegate bool InvocationPredicate(object?[] @params, int invocationIndex);

public abstract class SpiedObject {
	private record CallConfiguration(InvocationPredicate Predicate, Func<object?> Factory);
	
	private readonly Dictionary<MethodInfo, List<CallConfiguration>> m_ReturnValueConfigurations = new();
	private readonly Dictionary<MethodInfo, List<CallParameters>> m_Calls = new();

	internal IReadOnlyDictionary<MethodInfo, IReadOnlyList<CallParameters>> GetCalls() => m_Calls.WrapReadonly();
	internal void ConfigureCall(MethodInfo method, InvocationPredicate predicate, Func<object?> factory) => m_ReturnValueConfigurations.GetOrAdd(method, _ => new List<CallConfiguration>()).Add(new CallConfiguration(predicate, factory));

	private int RegisterCall(MethodInfo method, object?[] @params) {
		// TODO generic parameters
		var callsList = m_Calls.GetOrAdd(method.GetInterfaceDeclarationsForMethod().FirstOrDefault(method), _ => new List<CallParameters>());
		int index = callsList.Count;
		callsList.Add(new CallParameters(method.GetInterfaceDeclarationsForMethod().FirstOrDefault(method), @params, new Type[] {}));
		return index;
	}

	public void OnCallVoid(MethodInfo method, object?[] @params) {
		RegisterCall(method, @params);
	}

	public object? OnCallValue(MethodInfo method, object?[] @params) {
		int invocationIndex = RegisterCall(method, @params);

		if (m_ReturnValueConfigurations.TryGetValue(method, out List<CallConfiguration>? configurations)) {
			foreach (CallConfiguration configuration in configurations) {
				if (configuration.Predicate(@params, invocationIndex)) {
					return configuration.Factory();
				}
			}
		}

		object onCallValue = GetDefaultValue(method.ReturnType);
		Console.WriteLine("A " + onCallValue);
		return onCallValue;
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
