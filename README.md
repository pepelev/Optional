# Optional2

![Optional2](https://raw.githubusercontent.com/pepelev/Optional/master/icon/Icon.png)

Optional is a robust option/maybe type for C#. Originally developed by Nils Lück - [Optional](https://github.com/nlkl/Optional)

![](https://img.shields.io/nuget/v/Optional2.svg?style=flat-square&label=nuget)


## What and Why?

Optional is a strongly typed alternative to null values that lets you:

* Avoid those pesky null-reference exceptions
* Signal intent and model your data more explictly
* Cut down on manual null checks and focus on your domain
* Work with optional values of reference and value types in the same way. Two versions of the same generic method with `where T : class` and `where T : struct` are no longer needed


## Installation

`PM> Install-Package Optional2`

[NuGet package](https://www.nuget.org/packages/Optional2/). Supports **.NET 3.5+** and **.NET** (.NET Standard 1.0+)


## Usage

### Using the library

To use Optional simply import the following namespace:

```csharp
using Optional;
```

A few auxiliary namespaces are provided:

```csharp
using Optional.Linq; // Linq query syntax support
using Optional.Unsafe; // Unsafe value retrieval
using Optional.Collections; // Linq like methods with Option specifics
```


### Creating optional values

```csharp
// The most basic way to create optional values is to use the static `Option` class:
var none = Option.None<int>();
var some = Option.Some(10);
// or use extension methods:
var none = 10.None(); // Equivalent to Option.None<int>()
var some = 10.Some();
```

Option can be filtered during creation by [methods](https://github.com/pepelev/Optional/blob/master/src/Optional/OptionExtensions.cs) `.Some*()` or `.None*()`. The most useful from them is `.SomeNotNull()` since [Nullable Reference Types](https://learn.microsoft.com/dotnet/csharp/nullable-references) are supported. Analogue for value types is `.ToOption()`.


### Retrieving values

When retrieving values, Optional forces you to consider both cases (that is if a value is present or not).

Like [`Nullable<T>`](https://learn.microsoft.com/dotnet/api/system.nullable-1) Option can be tested by `HasValue` property.
There are also more precise ways:

```csharp
var isThousand = option.Contains(1000);
var isGreaterThanThousand = option.Exists(val => val > 1000);
```

Ways to retrieve a value from Option are:

```csharp
var value = option.ValueOr(10); // Returns the value if Some, or otherwise an alternative value (10)
var value = option.Match(
  some: x => x + 1, 
  none: () => 10
); // pattern matching
var value = option.ValueOrFailure(); // Unsafe: throws OptionValueMissingException on None
```


### Transforming and filtering values

```csharp
var value = 10.Some();
var doubled = value.Map(x => x * 2); // Some(20)
var odd = doubled.Filter(x => x % 2 == 1); // None
var fallback = odd.Else(1.Some()); // Some(1)
```

For details see [`Option<T>`](https://github.com/pepelev/Optional/blob/master/src/Optional/Option_Maybe.cs) or explore xml doc.


## Other utilities

### Monadic evaluation with LINQ query syntax

```csharp
using Optional.Linq;

var personWithGreenHair =
  from person in FindPersonById(10)
  from hairstyle in GetHairstyle(person)
  where hairstyle.Color == "green"
  select person;
```


### Options with exceptional values aka [Either](https://hackage.haskell.org/package/base-4.18.0.0/docs/Data-Either.html)


An [`Option<T, TException>`](https://github.com/pepelev/Optional/blob/master/src/Optional/Option_Either.cs) type with similar capabilities.

```csharp
var none = Option.None<int, ErrorCode>(ErrorCode.GeneralError);
var some = Option.Some<int, ErrorCode>(10);
```


### Working with collections

- `IEnumerable<T>` related Linq similar methods `(First|Last|Signle)OrNone()`
- `items.Values()` keeps only Some from `items` and unwraps them
- `dictionary.GetValueOrNone(key: 42)` - lookups an entry by key