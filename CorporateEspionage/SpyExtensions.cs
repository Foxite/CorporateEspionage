using System.Linq.Expressions;
using System.Reflection;

namespace CorporateEspionage;

public static class SpyExtensions {
	public static int GetCallCount(this ISpy spy) => spy.GetCalls().Sum(kvp => kvp.Value.Count);
	
	public static int GetCallCount(this ISpy spy, MethodInfo method) => spy.GetCalls().TryGetValue(method, out IReadOnlyList<CallParameters>? calls) ? calls.Count : 0;
	public static IReadOnlyList<CallParameters> GetCalls(this ISpy spy, MethodInfo method) => spy.GetCalls().TryGetValue(method, out IReadOnlyList<CallParameters>? list) ? list : Array.Empty<CallParameters>();

	public static CallParameters? GetCallParameters(this ISpy spy, MethodInfo method, int invocation) {
		IReadOnlyList<CallParameters> calls = spy.GetCalls(method);
		if (invocation < calls.Count) {
			return calls[invocation];
		} else {
			return null;
		}
	}

	public static MethodInfo GetMethodInfo<TDelegate>(this Expression<TDelegate> @delegate) where TDelegate : Delegate {
		if (@delegate.Body is not MethodCallExpression mce) {
			throw new InvalidOperationException("Expression must be a single method call");
		}

		return mce.Method;
	}

	public static int GetCallCount<T>(this ISpy spy, Expression<Func<T>> method) => spy.GetCallCount(method.GetMethodInfo());
	public static IReadOnlyList<CallParameters> GetCalls<T>(this ISpy spy, Expression<Func<T>> method) => spy.GetCalls(method.GetMethodInfo());
	public static CallParameters GetCallParameters<T>(this ISpy spy, Expression<Func<T>> method, int invocation) => spy.GetCallParameters(method.GetMethodInfo(), invocation);

	public static int GetCallCount(this ISpy spy, Expression<Action> method) => spy.GetCallCount(method.GetMethodInfo());
	public static IReadOnlyList<CallParameters> GetCalls(this ISpy spy, Expression<Action> method) => spy.GetCalls(method.GetMethodInfo());
	public static CallParameters GetCallParameters(this ISpy spy, Expression<Action> method, int invocation) => spy.GetCallParameters(method.GetMethodInfo(), invocation);
}
