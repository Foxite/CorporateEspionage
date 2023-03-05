using System.Reflection;
using CorporateEspionage.Tests;

namespace FridgeBot.Tests.CorporateEspionage;

public abstract class SpiedObject {
	internal readonly Dictionary<MethodInfo, List<CallParameters>> Calls;
	
	protected SpiedObject() {
		Calls = new Dictionary<MethodInfo, List<CallParameters>>();
	}

	public void RegisterCall(MethodInfo method, params object?[] @params) {
		// TODO generic parameters
		Calls.GetOrAdd(method.GetInterfaceDeclarationsForMethod().FirstOrDefault(method), _ => new List<CallParameters>()).Add(new CallParameters(method.GetInterfaceDeclarationsForMethod().FirstOrDefault(method), @params, new Type[] {}));
	}
}
