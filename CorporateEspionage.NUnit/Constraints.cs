using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace CorporateEspionage.NUnit;

public static class Was {
	public static ICalledConstraint Called(MethodInfo methodInfo) => new CalledExtensionsTarget(methodInfo);
	public static CalledTimesConstraint NotCalled(MethodInfo methodInfo) => Called(methodInfo).Times(0);
	public static NoOtherCallsConstraint NoOtherCalls(MethodInfo methodInfo) => new NoOtherCallsConstraint(methodInfo);
	
	public static ICalledConstraint Called(Expression<Action> expression) => Called(expression.GetMethodInfo());
	public static CalledTimesConstraint NotCalled(Expression<Action> expression) => Called(expression.GetMethodInfo()).Times(0);
	public static NoOtherCallsConstraint NoOtherCalls(Expression<Action> expression) => new NoOtherCallsConstraint(expression.GetMethodInfo());
	
	public static ICalledConstraint Called<T>(Expression<Func<T>> expression) where T : Delegate => Called(expression.GetMethodInfo());
	public static CalledTimesConstraint NotCalled<T>(Expression<Func<T>> expression) where T : Delegate => Called(expression.GetMethodInfo()).Times(0);
	public static NoOtherCallsConstraint NoOtherCalls<T>(Expression<Func<T>> expression) where T : Delegate => new NoOtherCallsConstraint(expression.GetMethodInfo());
	
	public static NoOtherCallsConstraint NoOtherCalls() => new NoOtherCallsConstraint(null);
}

public static class CalledConstraintExtensions {
	public static CalledTimesConstraint Times(this ICalledConstraint ca, int expected) => new CalledTimesConstraint(ca.MethodInfo, Is.EqualTo(expected));
	public static CalledTimesConstraint Times(this ICalledConstraint ca, Constraint constraint) => new CalledTimesConstraint(ca.MethodInfo, constraint);
	
	public static CallParameterByIndexConstraint With(this ICalledConstraint ca, int invocationIndex, int parameterIndex, object? expected) => new CallParameterByIndexConstraint(ca.MethodInfo, invocationIndex, parameterIndex, Is.EqualTo(expected));
	public static CallParameterByIndexConstraint With(this ICalledConstraint ca, int invocationIndex, int parameterIndex, Constraint constraint) => new CallParameterByIndexConstraint(ca.MethodInfo, invocationIndex, parameterIndex, constraint);
	
	public static CallParameterByNameConstraint With(this ICalledConstraint ca, int invocationIndex, string parameterName, object? expected) => new CallParameterByNameConstraint(ca.MethodInfo, invocationIndex, parameterName, Is.EqualTo(expected));
	public static CallParameterByNameConstraint With(this ICalledConstraint ca, int invocationIndex, string parameterName, Constraint constraint) => new CallParameterByNameConstraint(ca.MethodInfo, invocationIndex, parameterName, constraint);
}

public interface ICalledConstraint {
	public MethodInfo MethodInfo { get; }
}

public class CalledExtensionsTarget : ICalledConstraint {
	public MethodInfo MethodInfo { get; }

	public CalledExtensionsTarget(MethodInfo methodInfo) {
		MethodInfo = methodInfo;
	}
}

public class CallParameterByIndexConstraint : Constraint, ICalledConstraint {
	private readonly int m_InvocationIndex;
	private readonly int m_ParameterIndex;
	private readonly Constraint m_Constraint;

	public MethodInfo MethodInfo { get; }

	public CallParameterByIndexConstraint(MethodInfo caMethodInfo, int invocationIndex, int parameterIndex, Constraint constraint) {
		MethodInfo = caMethodInfo;
		m_InvocationIndex = invocationIndex;
		m_ParameterIndex = parameterIndex;
		m_Constraint = constraint;
	}

	public override ConstraintResult ApplyTo<TActual>(TActual actual) {
		var spy = ConstraintUtils.RequireActual<ISpy>(actual, nameof(actual));
		CallParameters callParameters = spy.GetCallParameters(MethodInfo, m_InvocationIndex);
		callParameters.Verified = true;
		return m_Constraint.ApplyTo(callParameters.GetParameter(m_ParameterIndex));
	}
}

public class CallParameterByNameConstraint : Constraint, ICalledConstraint {
	private readonly int m_InvocationIndex;
	private readonly string m_ParameterName;
	private readonly Constraint m_Constraint;

	public MethodInfo MethodInfo { get; }

	public CallParameterByNameConstraint(MethodInfo caMethodInfo, int invocationIndex, string parameterName, Constraint constraint) {
		MethodInfo = caMethodInfo;
		m_InvocationIndex = invocationIndex;
		m_ParameterName = parameterName;
		m_Constraint = constraint;
	}

	public override ConstraintResult ApplyTo<TActual>(TActual actual) {
		var spy = ConstraintUtils.RequireActual<ISpy>(actual, nameof(actual));
		CallParameters callParameters = spy.GetCallParameters(MethodInfo, m_InvocationIndex);
		callParameters.Verified = true;
		return m_Constraint.ApplyTo(callParameters.GetParameter(m_ParameterName));
	}
}

public class CalledTimesConstraint : Constraint, ICalledConstraint {
	private readonly Constraint m_Constraint;

	public MethodInfo MethodInfo { get; }

	public CalledTimesConstraint(MethodInfo methodInfo, Constraint constraint) {
		MethodInfo = methodInfo;
		m_Constraint = constraint;
	}

	public override ConstraintResult ApplyTo<TActual>(TActual actual) {
		var spy = ConstraintUtils.RequireActual<ISpy>(actual, nameof(actual));
		IReadOnlyList<CallParameters> calls = spy.GetCalls(MethodInfo);

		foreach (CallParameters call in calls) {
			call.Verified = true;
		}
		
		return m_Constraint.ApplyTo(calls.Count);
	}
}

public class NoOtherCallsConstraint : Constraint {
	private readonly MethodInfo? m_MethodInfo;
	
	public NoOtherCallsConstraint(MethodInfo? methodInfo) {
		m_MethodInfo = methodInfo;
	}

	public override ConstraintResult ApplyTo<TActual>(TActual actual) {
		var spy = ConstraintUtils.RequireActual<ISpy>(actual, nameof(actual));
		
		IEnumerable<CallParameters> callParameters;
		if (m_MethodInfo == null) {
			callParameters = spy.GetCalls().SelectMany(kvp => kvp.Value);
		} else {
			callParameters = spy.GetCalls(m_MethodInfo);
		}
		
		int count = callParameters.Count(cp => !cp.Verified);
		return new ConstraintResult(this, count, count == 0);
	}
}
