using System.Reflection;

namespace CorporateEspionage;

// TODO: setup return values
public class Spy<T> : ISpy where T : class {
	private readonly SpiedObject m_SpiedObject;
	
	public T Object { get; }

	internal Spy(T @object) {
		Object = @object;
		m_SpiedObject = @object as SpiedObject ?? throw new InvalidCastException($"Cannot cast T ({typeof(T).FullName}) to SpiedObject");
	}

	public int GetCallCount(MethodInfo method) {
		return m_SpiedObject.Calls.TryGetValue(method, out List<CallParameters>? calls) ? calls.Count : 0;
	}

	public int GetCallCount() {
		return m_SpiedObject.Calls.Sum(kvp => kvp.Value.Count);
	}

	public IReadOnlyDictionary<MethodInfo, IReadOnlyList<CallParameters>> GetCalls() {
		return m_SpiedObject.Calls.WrapReadonly();
	}

	public IReadOnlyList<CallParameters> GetCalls(MethodInfo method) {
		return m_SpiedObject.Calls.TryGetValue(method, out List<CallParameters>? list) ? list.AsReadonly() : Array.Empty<CallParameters>();
	}
	
	public CallParameters GetCallParameters(MethodInfo method, int invocation) {
		return GetCalls(method)[invocation];
	}
}
