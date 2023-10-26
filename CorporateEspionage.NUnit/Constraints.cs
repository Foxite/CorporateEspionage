using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace CorporateEspionage.NUnit;

public static class Was {
	public static SpyConstraint Called(MethodInfo methodInfo) => new SpyTimesConstraint(methodInfo, null, Is.GreaterThan(0));
	public static SpyTimesConstraint NotCalled(MethodInfo methodInfo) => new SpyTimesConstraint(methodInfo, null, Is.EqualTo(0));
	public static NoOtherCallsConstraint NoOtherCalls(MethodInfo methodInfo) => new NoOtherCallsConstraint(methodInfo, null, false);
	
	public static SpyConstraint Called(Expression<Action> expression) => Called(expression.GetMethodInfo());
	public static SpyTimesConstraint NotCalled(Expression<Action> expression) => NotCalled(expression.GetMethodInfo());
	public static NoOtherCallsConstraint NoOtherCalls(Expression<Action> expression) => NoOtherCalls(expression.GetMethodInfo());
	
	public static SpyConstraint Called<T>(Expression<Func<T>> expression) => Called(expression.GetMethodInfo());
	public static SpyTimesConstraint NotCalled<T>(Expression<Func<T>> expression) => NotCalled(expression.GetMethodInfo());
	public static NoOtherCallsConstraint NoOtherCalls<T>(Expression<Func<T>> expression) => NoOtherCalls(expression.GetMethodInfo());
	
	public static NoOtherCallsConstraint NoOtherCalls() => new NoOtherCallsConstraint(null, null, false);
}

public static class CalledConstraintExtensions {
	public static SpyTimesConstraint Times(this SpyConstraint ca, int expected) => new SpyTimesConstraint(ca.MethodInfo, ca, Is.EqualTo(expected));
	public static SpyTimesConstraint Times(this SpyConstraint ca, Constraint constraint) => new SpyTimesConstraint(ca.MethodInfo, ca, constraint);
	
	public static CallParameterByIndexConstraint With(this SpyConstraint ca, int invocationIndex, int parameterIndex, object? expected) => new CallParameterByIndexConstraint(ca.MethodInfo, ca, invocationIndex, parameterIndex, Is.EqualTo(expected));
	public static CallParameterByIndexConstraint With(this SpyConstraint ca, int invocationIndex, int parameterIndex, Constraint constraint) => new CallParameterByIndexConstraint(ca.MethodInfo, ca, invocationIndex, parameterIndex, constraint);
	
	public static CallParameterByNameConstraint With(this SpyConstraint ca, int invocationIndex, string parameterName, object? expected) => new CallParameterByNameConstraint(ca.MethodInfo, ca, invocationIndex, parameterName, Is.EqualTo(expected));
	public static CallParameterByNameConstraint With(this SpyConstraint ca, int invocationIndex, string parameterName, Constraint constraint) => new CallParameterByNameConstraint(ca.MethodInfo, ca, invocationIndex, parameterName, constraint);
}

public abstract class SpyConstraint : Constraint {
	private readonly Constraint? m_Base;
	public MethodInfo MethodInfo { get; }

	protected SpyConstraint(MethodInfo methodInfo, Constraint? @base) {
		MethodInfo = methodInfo;
		m_Base = @base;
	}
	
	public sealed override ConstraintResult ApplyTo<TActual>(TActual actual) {
		if (m_Base != null) {
			ConstraintResult baseResult = m_Base.ApplyTo(actual);
			if (!baseResult.IsSuccess) {
				return baseResult;
			}
		}

		var spy = ConstraintUtils.RequireActual<ISpy>(actual, nameof(actual));
		return ApplyTo(spy);
	}

	protected abstract ConstraintResult ApplyTo(ISpy spy);
}

public class CallParameterByIndexConstraint : SpyConstraint {
	private readonly int m_InvocationIndex;
	private readonly int m_ParameterIndex;
	private readonly Constraint m_Constraint;

	public CallParameterByIndexConstraint(MethodInfo methodInfo, Constraint? @base, int invocationIndex, int parameterIndex, Constraint constraint) : base(methodInfo, @base) {
		m_InvocationIndex = invocationIndex;
		m_ParameterIndex = parameterIndex;
		m_Constraint = constraint;
	}

	protected override ConstraintResult ApplyTo(ISpy spy) {
		CallParameters? callParameters = spy.GetCallParameters(MethodInfo, m_InvocationIndex);
		if (callParameters == null) {
			return new ConstraintResult(this, null, false);
		} else {
			callParameters.Verified = true;
			IConstraint resolvedConstraint = ((IResolveConstraint) m_Constraint).Resolve();
			return resolvedConstraint.ApplyTo(callParameters.GetParameter(m_ParameterIndex));
		}
	}
}

public class CallParameterByNameConstraint : SpyConstraint {
	private readonly int m_InvocationIndex;
	private readonly string m_ParameterName;
	private readonly Constraint m_Constraint;

	public CallParameterByNameConstraint(MethodInfo methodInfo, Constraint? @base, int invocationIndex, string parameterName, Constraint constraint) : base(methodInfo, @base) {
		m_InvocationIndex = invocationIndex;
		m_ParameterName = parameterName;
		m_Constraint = constraint;
	}

	protected override ConstraintResult ApplyTo(ISpy spy) {
		CallParameters? callParameters = spy.GetCallParameters(MethodInfo, m_InvocationIndex);
		if (callParameters == null) {
			return new ConstraintResult(this, null, false);
		} else {
			callParameters.Verified = true;
			IConstraint resolvedConstraint = ((IResolveConstraint) m_Constraint).Resolve();
			return resolvedConstraint.ApplyTo(callParameters.GetParameter(m_ParameterName));
		}
	}
}

public class SpyTimesConstraint : SpyConstraint {
	private readonly Constraint m_Constraint;

	public SpyTimesConstraint(MethodInfo methodInfo, Constraint? @base, Constraint constraint) : base(methodInfo, @base) {
		m_Constraint = constraint;
	}

	protected override ConstraintResult ApplyTo(ISpy spy) {
		IReadOnlyList<CallParameters> calls = spy.GetCalls(MethodInfo);

		foreach (CallParameters call in calls) {
			call.Verified = true;
		}
		
		IConstraint resolvedConstraint = ((IResolveConstraint) m_Constraint).Resolve();
		return resolvedConstraint.ApplyTo(calls.Count);
	}
}

public class NoOtherCallsConstraint : SpyConstraint {
	public bool IncludeIgnored { get; }
	
	public NoOtherCallsConstraint(MethodInfo? methodInfo, Constraint? @base, bool includeIgnored) : base(methodInfo, @base) {
		IncludeIgnored = includeIgnored;
	}

	protected override ConstraintResult ApplyTo(ISpy spy) {
		IEnumerable<CallParameters> callParameters;
		if (MethodInfo == null) {
			callParameters = spy.GetCalls().SelectMany(kvp => kvp.Value);
		} else {
			callParameters = spy.GetCalls(MethodInfo);
		}
		
		int count = callParameters.Count(cp => !cp.Verified && (IncludeIgnored || !cp.Ignored));
		return new ConstraintResult(this, count, count == 0);
	}
}
