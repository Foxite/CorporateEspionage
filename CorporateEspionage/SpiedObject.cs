using System.Reflection;

namespace CorporateEspionage;

public abstract class SpiedObject {
	internal readonly Dictionary<MethodInfo, List<CallParameters>> Calls;
	
	protected SpiedObject() {
		Calls = new Dictionary<MethodInfo, List<CallParameters>>();
	}

	public void RegisterCall(MethodInfo method, params object?[] @params) {
		// TODO generic parameters
		Calls.GetOrAdd(method.GetInterfaceDeclarationsForMethod().FirstOrDefault(method), _ => new List<CallParameters>()).Add(new CallParameters(method.GetInterfaceDeclarationsForMethod().FirstOrDefault(method), @params, new Type[] {}));
	}

	protected internal T GetDefaultValue<T>() => (T) GetDefaultValue(typeof(T));
	protected internal object GetDefaultValue(Type t) {
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
