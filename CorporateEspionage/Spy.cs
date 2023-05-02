using System.Linq.Expressions;
using System.Reflection;

namespace CorporateEspionage;

public class Spy<T> : ISpy where T : class {
	private readonly SpiedObject m_SpiedObject;
	
	public T Object { get; }

	internal Spy(T @object) {
		Object = @object;
		m_SpiedObject = @object as SpiedObject ?? throw new InvalidCastException($"Cannot cast T ({typeof(T).FullName}) to SpiedObject");
	}

	public IReadOnlyDictionary<MethodInfo, IReadOnlyList<CallParameters>> GetCalls() => m_SpiedObject.GetCalls();
	public void ConfigureCallByIndex(MethodInfo method, int index, Func<object?> factory) => m_SpiedObject.ConfigureCallByIndex(method, index, factory);
	public void ConfigureCallByMatcher(MethodInfo method, InvocationMatcher matcher) => m_SpiedObject.ConfigureCallByMatcher(method, matcher);
	
	public void ConfigureCallByIndex<TDelegate>(Expression<TDelegate> expression, int index, Func<object?> factory) where TDelegate : Delegate => m_SpiedObject.ConfigureCallByIndex(expression.GetMethodInfo(), index, factory);
	public void ConfigureCallByMatcher<TDelegate>(Expression<TDelegate> expression, InvocationMatcher matcher) where TDelegate : Delegate => m_SpiedObject.ConfigureCallByMatcher(expression.GetMethodInfo(), matcher);
}
