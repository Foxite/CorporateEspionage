using System.Reflection;

namespace CorporateEspionage;

public class Spy<T> where T : class {
	private readonly SpiedObject m_SpiedObject;
	
	public T Object { get; }

	internal Spy(T @object) {
		Object = @object;
		m_SpiedObject = @object as SpiedObject ?? throw new InvalidCastException($"Cannot cast T ({typeof(T).FullName}) to SpiedObject");
	}

	public CallParameters GetCallParameters(MethodInfo method, int invocation) {
		return m_SpiedObject.Calls[method][invocation];
	}
}
