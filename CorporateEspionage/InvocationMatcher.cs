using Optional;

namespace CorporateEspionage;

public delegate Option<object?> InvocationMatcher(object?[] @params);
