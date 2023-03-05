using System.Reflection;
using System.Reflection.Emit;

namespace CorporateEspionage; 

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

		MethodInfo registerCallMethod     = typeof(SpiedObject).GetMethod(nameof(SpiedObject.RegisterCall   )) ?? throw new Exception("what the fuck? 1");
		MethodInfo getCurrentMethodMethod = typeof(MethodBase ).GetMethod(nameof(MethodBase.GetCurrentMethod)) ?? throw new Exception("what the fuck? 2");

		m_AssemblyBuilder ??= AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("SpyAssembly"), AssemblyBuilderAccess.Run);
		m_ModuleBuilder ??= m_AssemblyBuilder.DefineDynamicModule("SpyModule");
		
		TypeBuilder typeBuilder = m_ModuleBuilder.DefineType($"{typeT.FullName}Spy", TypeAttributes.Class, typeof(SpiedObject), new[] { typeT });

		typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

		Console.WriteLine(typeT.FullName);
		foreach (MethodInfo interfaceMethod in typeT.GetMethods()) {
			Console.WriteLine(interfaceMethod.Name);
			ParameterInfo[] interfaceMethodParameters = interfaceMethod.GetParameters();
			
			MethodBuilder spiedMethod = typeBuilder.DefineMethod(interfaceMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), interfaceMethodParameters.Select(pi => pi.ParameterType).ToArray());
			typeBuilder.DefineMethodOverride(spiedMethod, interfaceMethod);
			
			// MSIL resources:
			// https://www.codeguru.com/dotnet/msil-tutorial/
			// https://en.wikipedia.org/wiki/List_of_CIL_instructions
			ILGenerator il = spiedMethod.GetILGenerator();
			il.DeclareLocal(typeof(object[]));
			
			void IlEmit(OpCode opcode, dynamic parameter) {
				Console.WriteLine($"{opcode.ToString()} {parameter.ToString()}");
				il.Emit(opcode, parameter);
			}
			
			void IlEmit2(OpCode opcode) {
				Console.WriteLine(opcode.ToString());
				il.Emit(opcode);
			}
			
			/*
			
			C#:
				this.RegisterCall(MethodBase.GetCurrentMethod(), new object[] { arg1, arg2, ... });
			
			A non static function has the `this` reference as its first parameter, so:
				SpyGenerator.RegisterCall(this, MethodBase.GetCurrentMethod(), new object[] { arg1, arg2, ... });
			
			That's a one-liner, but it looks a bit more like this internally:
				var currentMethod = MethodBase.GetCurrentMethod();
				var parameterList = new object[n];
				
				parameterList[0] = arg1;
				parameterList[1] = arg2;
				...
				
				SpyGenerator.RegisterCall(this, currentMethod, parameterList);
				
			MSIL is stack based, so this is what it ends up being:
				push_arg 0; // this reference (will be the first parameter for the call at the very end)
				call MethodBase.GetCurrentMethod; // and push the return value (the second parameter)
				push_constant argument_count;
				new_array object; // create an object[] and push the reference
				store_local 0; // pop the reference and store it in a local
				
				// For every argument: (this is an unrolled loop)
				push_local 0; // the array reference
				push_constant n; // the array index
				push_arg m; // the value to store (could be a value type or a reference or something)
				store_element; // pops the 3 items and stores the value in the array at the index
				
				// Finally:
				push_local 0; // the array reference, third parameter for the call
				call SpiedObject.RegisterCall;
				return; // every function needs a return instruction at the end, otherwise it's invalid
			
			 */

			// Load this
			IlEmit(OpCodes.Ldarg, 0);
			
			// Load result of MethodBase.GetCurrentMethod()
			IlEmit(OpCodes.Call, getCurrentMethodMethod);
			
			// Load parameters:
			// Allocate array
			IlEmit(OpCodes.Ldc_I4, interfaceMethodParameters.Length); // desired array length
			IlEmit(OpCodes.Newarr, typeof(object)); // pop the stack item, and push a reference to an array with that size.
			IlEmit2(OpCodes.Stloc_0); // store the array reference in local 0.
			
			// Populate the array
			for (int i = 0; i < interfaceMethodParameters.Length; i++) {
				ParameterInfo pi = interfaceMethodParameters[i];
				spiedMethod.DefineParameter(i, pi.Attributes, pi.Name);
				
				// arg 0: this (not visible to c# reflection)
				// arg 1: MethodInfo
				// arg 2+: actual args
				IlEmit2(OpCodes.Ldloc_0); // Push the array reference
				IlEmit(OpCodes.Ldc_I4, i); // Push the index
				IlEmit(OpCodes.Ldarg, i + 1); // Push the value
				if (pi.ParameterType.IsValueType) {
					IlEmit(OpCodes.Box, pi.ParameterType); // Box the value
				}
				IlEmit2(OpCodes.Stelem_Ref); // Pop 3 items and do the store operation
			}
			
			// Push the array reference in local 0 to the stack.
			IlEmit2(OpCodes.Ldloc_0);
			
			IlEmit(OpCodes.Call, registerCallMethod);
			IlEmit2(OpCodes.Ret);
			
			Console.WriteLine();
		}
		Console.WriteLine();
		Console.WriteLine();

		spiedType = typeBuilder.CreateType() ?? throw new Exception("what the fuck? 3");
		m_SpiedTypes.Add(typeT, spiedType);
		return new Spy<T>((T) Activator.CreateInstance(spiedType)!);
	}
}
