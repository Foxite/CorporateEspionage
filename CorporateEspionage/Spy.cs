using System.Reflection;

namespace CorporateEspionage;

// TODO: setup return values
public class Spy<T> where T : class {
	private readonly SpiedObject m_SpiedObject;
	
	public T Object { get; }

	internal Spy(T @object) {
		Object = @object;
		m_SpiedObject = @object as SpiedObject ?? throw new InvalidCastException($"Cannot cast T ({typeof(T).FullName}) to SpiedObject");
	}

	public IReadOnlyDictionary<MethodInfo, IReadOnlyList<CallParameters>> GetCalls() {
		return m_SpiedObject.Calls.WrapReadonly();
	}

	public IReadOnlyList<CallParameters> GetCalls(MethodInfo method) {
		return m_SpiedObject.Calls[method];
	}
	
	public CallParameters GetCallParameters(MethodInfo method, int invocation) {
		return m_SpiedObject.Calls[method][invocation];
	}
}
