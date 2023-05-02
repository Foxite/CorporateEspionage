using System.Reflection;
using System.Reflection.Emit;

namespace CorporateEspionage; 

public class SpyGenerator {
	private AssemblyBuilder? m_AssemblyBuilder;
	private ModuleBuilder? m_ModuleBuilder;
	private readonly Dictionary<Type, Type> m_SpiedTypes = new();

	public Spy<T> CreateSpy<T>(bool printIl = false) where T : class {
		Type typeT = typeof(T);
		if (m_SpiedTypes.TryGetValue(typeT, out Type? spiedType)) {
			return new Spy<T>((T) Activator.CreateInstance(spiedType)!);
		}
		
		if (!typeT.IsInterface) {
			throw new ArgumentException("Type must be an interface type");
		}

		MethodInfo onCallVoidMethod       = typeof(SpiedObject).GetMethod(nameof(SpiedObject.OnCallVoid     )) ?? throw new Exception($"{nameof(SpiedObject)}.{nameof(SpiedObject.OnCallVoid)} is missing");
		MethodInfo onCallValueMethod      = typeof(SpiedObject).GetMethod(nameof(SpiedObject.OnCallValue    )) ?? throw new Exception($"{nameof(SpiedObject)}.{nameof(SpiedObject.OnCallValue)} is missing");
		MethodInfo getCurrentMethodMethod = typeof(MethodBase ).GetMethod(nameof(MethodBase.GetCurrentMethod)) ?? throw new Exception($"{nameof(MethodBase)}.{nameof(MethodBase.GetCurrentMethod)} is missing");

		m_AssemblyBuilder ??= AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("SpyAssembly"), AssemblyBuilderAccess.Run);
		m_ModuleBuilder ??= m_AssemblyBuilder.DefineDynamicModule("SpyModule");
		
		TypeBuilder typeBuilder = m_ModuleBuilder.DefineType($"{typeT.FullName}Spy", TypeAttributes.Class, typeof(SpiedObject), new[] { typeT });

		typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

		if (printIl) {
			Console.WriteLine(typeT.FullName);
		}
		
		foreach (MethodInfo interfaceMethod in typeT.GetMethods()) {
			if (printIl) {
				Console.WriteLine(interfaceMethod.Name);
			}
			
			ParameterInfo[] interfaceMethodParameters = interfaceMethod.GetParameters();
			MethodBuilder spiedMethod = typeBuilder.DefineMethod(interfaceMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual, interfaceMethod.ReturnType, interfaceMethodParameters.Select(pi => pi.ParameterType).ToArray());
			typeBuilder.DefineMethodOverride(spiedMethod, interfaceMethod);
			
			// MSIL resources:
			// https://www.codeguru.com/dotnet/msil-tutorial/
			// https://en.wikipedia.org/wiki/List_of_CIL_instructions
			ILGenerator il = spiedMethod.GetILGenerator();
			LocalBuilder argsArray = il.DeclareLocal(typeof(object[]));
			LocalBuilder currentMethod = il.DeclareLocal(typeof(MethodInfo));
			
			void IlEmit(OpCode opcode, dynamic parameter) {
				if (printIl) {
					Console.WriteLine($"{opcode.ToString()} {parameter.ToString()}");
				}
				il.Emit(opcode, parameter);
			}
			
			void IlEmit2(OpCode opcode) {
				if (printIl) {
					Console.WriteLine(opcode.ToString());
				}
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
			
			
			
			Also, if the function returns void:
				return; // every CIL function needs a return instruction at the end, otherwise it's invalid
			
			Otherwise:
				return this.GetDefaultValue<return type>();
			
			This becomes CIL:
				push_arg 0; // `this` reference
				call GetDefaultValue[return type()]; // the method is defined below, with the generic parameter, and passed into the IL generator
				return;
			
			 */
			
			// Load this
			IlEmit(OpCodes.Ldarg, 0);
			
			// Load result of MethodBase.GetCurrentMethod()
			IlEmit(OpCodes.Call, getCurrentMethodMethod);
			// And store in local (keep on stack)
			IlEmit(OpCodes.Stloc, currentMethod);
			IlEmit(OpCodes.Ldloc, currentMethod);
			
			// Load parameters:
			// Allocate array
			IlEmit(OpCodes.Ldc_I4, interfaceMethodParameters.Length); // desired array length
			IlEmit(OpCodes.Newarr, typeof(object)); // pop the stack item, and push a reference to an array with that size.
			IlEmit(OpCodes.Stloc, argsArray); // store the array reference in a local.
			
			// Populate the array
			for (int i = 0; i < interfaceMethodParameters.Length; i++) {
				ParameterInfo pi = interfaceMethodParameters[i];
				spiedMethod.DefineParameter(i, pi.Attributes, pi.Name);
				
				// arg 0: this (not visible to c# reflection)
				// arg 1: MethodInfo
				// arg 2+: actual args
				IlEmit(OpCodes.Ldloc, argsArray); // Push the array reference
				IlEmit(OpCodes.Ldc_I4, i); // Push the index
				IlEmit(OpCodes.Ldarg, i + 1); // Push the value
				if (pi.ParameterType.IsValueType) {
					IlEmit(OpCodes.Box, pi.ParameterType); // Box the value
				}
				IlEmit2(OpCodes.Stelem_Ref); // Pop 3 items and do the store operation
			}
			
			IlEmit(OpCodes.Ldloc, argsArray);
			if (spiedMethod.ReturnType == typeof(void)) {
				IlEmit(OpCodes.Call, onCallVoidMethod);
			} else {
				IlEmit(OpCodes.Call, onCallValueMethod);

				if (spiedMethod.ReturnType.IsValueType) {
					IlEmit(OpCodes.Unbox_Any, interfaceMethod.ReturnType);
				}
			}
			
			IlEmit2(OpCodes.Ret);

			if (printIl) {
				Console.WriteLine();
			}
			
			//printIl = false;
		}
		if (printIl) {
			Console.WriteLine();
			Console.WriteLine();
		}

		spiedType = typeBuilder.CreateType() ?? throw new Exception("what the fuck? 3");
		m_SpiedTypes.Add(typeT, spiedType);
		return new Spy<T>((T) Activator.CreateInstance(spiedType)!);
	}
}
