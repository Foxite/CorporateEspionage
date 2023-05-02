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
	
	public void ConfigureCall(MethodInfo method, InvocationPredicate predicate, Func<object?> factory) => m_SpiedObject.ConfigureReturnValue(method, predicate, factory);
	public void ConfigureCall(MethodInfo method, InvocationPredicate predicate, object? value) => m_SpiedObject.ConfigureReturnValue(method, predicate, () => value);
	public void ConfigureCall<TRet>(Expression<Func<T, TRet>> expression, InvocationPredicate predicate, Func<TRet> factory) => m_SpiedObject.ConfigureReturnValue(expression.GetMethodInfo(), predicate, () => factory());
	public void ConfigureCall<TRet>(Expression<Func<T, TRet>> expression, InvocationPredicate predicate, TRet value) => m_SpiedObject.ConfigureReturnValue(expression.GetMethodInfo(), predicate, () => value);
	
	public void ConfigureIgnoring(MethodInfo method, InvocationPredicate predicate, bool ignoring) => m_SpiedObject.ConfigureIgnoring(method, predicate, ignoring);
	public void ConfigureIgnoring(Expression<Action<T>> expression, InvocationPredicate predicate, bool ignoring) => m_SpiedObject.ConfigureIgnoring(expression.GetMethodInfo(), predicate, ignoring);
	public void ConfigureIgnoring<TRet>(Expression<Func<T, TRet>> expression, InvocationPredicate predicate, bool ignoring) => m_SpiedObject.ConfigureIgnoring(expression.GetMethodInfo(), predicate, ignoring);
}
