# CorporateEspionage
I couldn't figure out how to use Moq to run assertions on calls on mocked objects, so I made my own mocking system to do that.

## Features
- Uses System.Reflection.Emit to create types at runtime, that implement arbitrary interfaces (mock objects) and record the parameters of any method call
- Inspect the parameters of specific invocations, and run assertions using your favorite unit testing framework
- Supports methods with an arbitrary amount of parameters, with arbitrary types, or no parameters at all
- Only supports methods that return void

### Planned features
- Support for methods with return values (and return the default value of the type)
- Support for methods with a Task return value (and return `Task.CompletedTask` or `Task.FromResult(default(T))`)
- Configure the return values of expected method invocations, sequentially or by parameter criteria
- Support for generic types and methods, and recording the generic type parameters

### Wishlist features
- Instead of manually emitting CIL, use a library that can convert expressions into CIL

## How to use
CorporateEspionage is meant to be used from within unit tests, so this repository's unit tests are good examples of how to use it.