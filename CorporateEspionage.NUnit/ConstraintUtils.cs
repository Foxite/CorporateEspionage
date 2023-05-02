namespace CorporateEspionage.NUnit;

/// <summary>
/// Provides methods to support consistent checking in constraints.
/// Stolen from NUnit.
/// </summary>
internal static class ConstraintUtils {
	/// <summary>
	/// Requires that the provided object is actually of the type required.
	/// </summary>
	/// <param name="actual">The object to verify.</param>
	/// <param name="paramName">Name of the parameter as passed into the checking method.</param>
	/// <param name="allowNull">
	/// If <see langword="true"/> and <typeparamref name="T"/> can be null, returns null rather than throwing when <paramref name="actual"/> is null.
	/// If <typeparamref name="T"/> cannot be null, this parameter is ignored.</param>
	/// <typeparam name="T">The type to require.</typeparam>
	public static T RequireActual<T>(object? actual, string paramName, bool allowNull = false) {
		if (TryCast(actual, out T? result) && (allowNull || result != null)) {
			return result!;
		}

		string actualDisplay = actual == null ? "null" : actual.GetType().Name;
		throw new ArgumentException($"Expected: {typeof(T).Name} But was: {actualDisplay}", paramName);
	}
	
	/// <summary>
	/// Casts to a value of the given type if possible.
	/// If <paramref name="obj"/> is <see langword="null"/> and <typeparamref name="T"/>
	/// can be <see langword="null"/>, the cast succeeds just like the C# language feature.
	/// </summary>
	/// <param name="obj">The object to cast.</param>
	/// <param name="value">The value of the object, if the cast succeeded.</param>
	public static bool TryCast<T>(object? obj, out T? value) {
		if (obj is T obj1) {
			value = obj1;
			return true;
		}

		value = default;
		return false;
	}
}
