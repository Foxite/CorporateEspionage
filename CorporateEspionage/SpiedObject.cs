using System.Reflection;
using Optional;
using Optional.Unsafe;

namespace CorporateEspionage;

public abstract class SpiedObject {
	private readonly Dictionary<(MethodInfo Method, int InvocationIndex), Func<object?>> m_ReturnValueByIndex = new();
	private readonly Dictionary<MethodInfo, InvocationMatcher> m_ReturnValueByMatcher = new();
	private readonly Dictionary<MethodInfo, List<CallParameters>> m_Calls = new();

	internal IReadOnlyDictionary<MethodInfo, IReadOnlyList<CallParameters>> GetCalls() => m_Calls.WrapReadonly();
	internal void ConfigureCallByIndex(MethodInfo method, int index, Func<object?> factory) => m_ReturnValueByIndex[(method, index)] = factory;
	internal void ConfigureCallByMatcher(MethodInfo method, InvocationMatcher matcher) => m_ReturnValueByMatcher[method] = matcher;

	private int RegisterCall(MethodInfo method, object?[] @params) {
		// TODO generic parameters
		var callsList = m_Calls.GetOrAdd(method.GetInterfaceDeclarationsForMethod().FirstOrDefault(method), _ => new List<CallParameters>());
		int index = callsList.Count;
		callsList.Add(new CallParameters(method.GetInterfaceDeclarationsForMethod().FirstOrDefault(method), @params, new Type[] {}));
		return index;
	}

	protected internal void OnCallVoid(MethodInfo method, object?[] @params) {
		RegisterCall(method, @params);
	}

	protected internal object? OnCallValue(MethodInfo method, object?[] @params) {
		int invocationIndex = RegisterCall(method, @params);

		if (m_ReturnValueByIndex.TryGetValue((method, invocationIndex), out Func<object?>? valueFactory)) {
			return valueFactory();
		} else if (m_ReturnValueByMatcher.TryGetValue(method, out InvocationMatcher? matcher)) {
			Option<object?> result = matcher(@params);
			if (result.HasValue) {
				return result.ValueOrFailure();
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
