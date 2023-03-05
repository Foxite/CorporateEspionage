using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;

namespace FridgeBot.Tests.CorporateEspionage; 

public class SpyGenerator {
	private AssemblyBuilder? m_AssemblyBuilder;
	private ModuleBuilder? m_ModuleBuilder;
	private readonly Dictionary<Type, Type> m_SpiedTypes = new();

	public Spy<T> CreateSpy<T>() where T : class {
		Type typeT = typeof(T);
		if (m_SpiedTypes.TryGetValue(typeT, out Type? spiedType)) {
			return new Spy<T>((T) Activator.CreateInstance(spiedType)!);
		}
		
		if (!typeT.IsInterface) {
			throw new ArgumentException("Type must be an interface type");
		}

		MethodInfo getEmptyArrayMethod    = typeof(Array      ).GetMethod(nameof(Array.Empty))!.MakeGenericMethod(typeof(object));
		MethodInfo registerCallMethod     = typeof(SpiedObject).GetMethod(nameof(SpiedObject.RegisterCall   )) ?? throw new Exception("what the fuck? 1");
		MethodInfo getCurrentMethodMethod = typeof(MethodBase ).GetMethod(nameof(MethodBase.GetCurrentMethod)) ?? throw new Exception("what the fuck? 2");

		m_AssemblyBuilder ??= AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("SpyAssembly"), AssemblyBuilderAccess.Run);
		m_ModuleBuilder ??= m_AssemblyBuilder.DefineDynamicModule("SpyModule");
		TypeBuilder typeBuilder = m_ModuleBuilder.DefineType($"{typeT.FullName}Spy", TypeAttributes.Class, typeof(SpiedObject), new[] { typeT });

		typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

		foreach (MethodInfo interfaceMethod in typeT.GetMethods()) {
			ParameterInfo[] interfaceMethodParameters = interfaceMethod.GetParameters();
			
			MethodBuilder spiedMethod = typeBuilder.DefineMethod(interfaceMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), interfaceMethodParameters.Select(pi => pi.ParameterType).ToArray());
			typeBuilder.DefineMethodOverride(spiedMethod, interfaceMethod);

			ILGenerator il = spiedMethod.GetILGenerator();

			il.Emit(OpCodes.Ldarg, 0); // Load the this ref.
			
			il.Emit(OpCodes.Call, getCurrentMethodMethod);
			
			//il.Emit(OpCodes.Ldnull);
			il.Emit(OpCodes.Call, getEmptyArrayMethod);
			
			il.Emit(OpCodes.Call, registerCallMethod);
			
			il.Emit(OpCodes.Ret);
			continue;
			// TODO load `this` reference
			il.Emit(OpCodes.Ldarg, (short)0); // Load the this ref.
			//il.Emit(OpCodes.Ldarg, 0);
			il.Emit(OpCodes.Ldnull);
			il.Emit(OpCodes.Call, getCurrentMethodMethod);
			
			for (int i = 0; i < interfaceMethodParameters.Length; i++) {
				ParameterInfo pi = interfaceMethodParameters[i];
				spiedMethod.DefineParameter(i, pi.Attributes, pi.Name);
				
				il.Emit(OpCodes.Ldarg, i + 2);
			}
			
			il.Emit(OpCodes.Call, registerCallMethod);
		}

		spiedType = typeBuilder.CreateType() ?? throw new Exception("what the fuck? 3");
		m_SpiedTypes.Add(typeT, spiedType);
		return new Spy<T>((T) Activator.CreateInstance(spiedType)!);
		//return (T) typeBuilder.GetConstructor(Array.Empty<Type>())!.Invoke(Array.Empty<object?>());
	}
}
