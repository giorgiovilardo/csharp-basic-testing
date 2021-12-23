# How to write C# tests

This document describes to non-C#-fluent audiences some best practices on testing, how and where to start, how to include test projects and so on. We will not touch testing philosophies, just the mechanical steps to have a working environment.

To follow the examples in this document, please make sure to have a recent version of .NET installed as we will use the `dotnet` cli command.

Get your version [here](https://get.dot.net)!

We're going to use .NET 6.0.101 with C# 10.0 and its new syntax: if you want to follow the guide with code, `git clone git@github.com:giorgiovilardo/csharp-basic-testing.git`.

If you want to see the complete version, just checkout the `done-tutorial` branch.

## A necessary eagle-eye view of `csproj` and `sln` files

### A small preamble about directory structure

The best practice in the C# world is to structure projects like the repo we just cloned:

```
MyApp
├── src
    ├── MyApp.ConsoleRunner
        └── files...
    ├── MyApp.Core
        └── files...
    └── MyApp.Web
        └── files...
├── tests
    ├── MyApp.ConsoleRunner.Tests
        └── files...
    └── MyApp.Web.Tests
        └── files...
├── MyApp.sln
└── other files...
```

### `csproj` aka Project files

In .NET lingo, a `Project` is a self-contained work; imagine the output of `composer init` or `npm init`, a blank slate where you can start working. There are many types of projects generable from the command `dotnet new`, like `Class library` (usually the most used type, as it's basically a module that you import in other projects and use as it can't be run on its own) or `Console Application` (a console-runnable project).

Projects can completely live on their own, can download external libraries from NuGet (the .NET package manager), are compilable and runnable, and are, in general, a normal programming environment for a module.

Configuration for the project lives in a `csproj` file which usually has the same name as the project (e.g. `ExampleProj.csproj`). This file is very important as it tells .NET how to build and compile the whole project, so let's take a look at it:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net5.0</TargetFramework>
    <Description>Big Mega Project</Description>
    <CustomProperty>Foo</CustomProperty>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\ProjectA\ProjectA.csproj" />
    <ProjectReference Include="..\ProjectB\ProjectB.csproj" />
    <ProjectReference Include="..\ProjectC\ProjectC.csproj" />
  </ItemGroup>

  <ItemGroup>
    <None Include="appsettings.json" CopyToOutputDirectory="Always" CopyToPublishDirectory="Always" />
    <None Include="appsettings.*.json" CopyToOutputDirectory="Always" CopyToPublishDirectory="Always" />
    <None Include="someothernamespace.*.json" CopyToOutputDirectory="Always" CopyToPublishDirectory="Always" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="5.0.0" />
    <PackageReference Include="Microsoft.Extensions.Hosting" Version="5.0.0" />
  </ItemGroup>

</Project>
```

If you want to dive deeper on how to work with this file format, [feel free to browse MS documentation about MSBuild;](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild?view=vs-2022) the important thing is that we can include references to other projects with `ProjectReference`, NuGet libraries with `PackageReference` and even copy files to output directory with other MSBuild directives.

### `sln` files aka Solutions

A Solution is basically a collection of projects. It usually lives in the root directory
of the software, where the `src` and `tests` directories are.

Projects are the smart way to segment responsibilities in the .NET world; you don't want
to have a single huge project like in PHP or JS world; you want a Solution with many
different projects, each one with a very well-defined responsibility and his set of external libraries and so on.

Let's say you want a console application; you might have the pure business logic and the domain objects of your software in a project called `MyProject.Core` and the console runner in `MyProject.Console`. One month later, when you need a web application, you can create a `MyProject.Web` project, add it to the solution, reference `MyProject.Core` and go to town. Tests project usually are marked by the `.Tests` suffix, i.e. `MyProject.Core.Tests`. All these projects are loaded into the Solution file.

We don't care about the `sln` file format as it's complex and is better managed by the `dotnet sln` command.

## Creating test projects

Let's check out again our repo:

```
MyApp
├── src
    ├── MyApp.ConsoleRunner
        └── files...
    ├── MyApp.Core
        └── files...
    └── MyApp.Web
        └── files...
├── tests
    ├── MyApp.ConsoleRunner.Tests
        └── files...
    └── MyApp.Web.Tests
        └── files...
└── MyApp.sln
```

We want to add the missing tests for the `MyApp.Core` project.

### Creation

The first thing to do is create the test project.

From `MyApp` directory, run:

```shell
~/MyApp# dotnet new xunit -o tests/MyApp.Core.Tests
```

This will create a new basic `xUnit` project in the directory `tests/MyApp.Core.Tests`.

Since we didn't specify a name for the project with the `-n name` flag, the output directory (`-o` flag) is used as a name, which is a best practice nonetheless.

The directory will contain a placeholder test file and the new `MyApp.Core.Tests.csproj` file, that looks like this:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>

    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="16.11.0" />
    <PackageReference Include="xunit" Version="2.4.1" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.3">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="coverlet.collector" Version="3.1.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>

</Project>
```

Minor details might be different if you use .NET 5 as this has been generated under .NET 6.

### Add the reference

We now need to add a reference: references is how cross-project visibility of classes is achieved, and we obviously want our test projects to be able to import and use classes from the project under test.

This is achieved very easily with the `dotnet add` command, that has this syntax:

`dotnet add CsprojFileOfAProject reference CsprojFileOfProjectWeWantToImport`

Let's adapt it to our use case and let's launch it from the root `MyApp` dir:

```shell
~/MyApp# dotnet add \
         tests/MyApp.Core.Tests/MyApp.Core.Tests.csproj reference \
         src/MyApp.Core/MyApp.Core.csproj
```

or, in a more compact way and if you used the same name of the directory for the `csproj` file,

```shell
~/MyApp# dotnet add tests/MyApp.Core.Tests reference src/MyApp.Core
```

References are not two-way: `MyApp.Core.Tests` is now able to import everything marked `public` from `MyApp.Core`, but not vice-versa.

If we take a look at the new `MyApp.Core.Tests.csproj` we can see the reference has been added:

```xml
<ItemGroup>
    <ProjectReference Include="..\..\src\MyApp.Core\MyApp.Core.csproj" />
</ItemGroup>
```

You can obviously add reference manually by directly editing the `csproj` files, but it's strongly discouraged.

### Add to solution

Now we need to add the test project to the solution file, so IDEs know that they need to load the files of the test project in the solution view.

The CLI command to interact with solution files is `dotnet sln` and looks similar to `dotnet add`. The basic syntax is:

`dotnet sln SolutionFile add CsprojFileOfTheProjectWeWantToAdd`

or, for our directory structure:

```shell
~/MyApp# dotnet sln MyApp.sln add tests/MyApp.Core.Tests/MyApp.Core.Tests.csproj
```

More succinctly:

```shell
~/MyApp# dotnet sln add tests/MyApp.Core.Tests
```

DO NOT manually add projects to solutions, if you want to know why just `cat` a solution file :) use the command or the IDE.

### Can't I just do it from the IDE?

Obviously, but I don't support your IDE and it's better to know the principles behind. RTFM of your IDE of choice :)

## Test writing + overview on the most used libraries

We finally finished this long winded introduction, set up our project and can finally take a look at the basic instruments needed to write tests: `xUnit` and `Moq`. There are a million more useful libraries but I don't know them all / never used them / I don't care so feel free to experiment!

### xUnit

`xUnit` is the industry-standard, idiomatic C# library to program tests. In `MyApp.Core` lives this very complex class in the `Calculator.cs` file:

```c#
namespace MyApp.Core;

public class Calculator : ICalculator
{
    public int Add(int x, int y) => x + y;
    public int Subtract(int x, int y) => x - y;
    public int Multiply(int x, int y) => x * y;
}
```

that implements `ICalculator.cs`:

```c#
namespace MyApp.Core;

public interface ICalculator
{
    int Add(int x, int y);
    int Subtract(int x, int y);
    int Multiply(int x, int y);
}
```

In `MyApp.Core.Tests` we can delete the basic `UnitTest1.cs` file and create `CalculatorTests.cs`:

```c#
using Xunit;

namespace MyApp.Core.Tests;

public class CalculatorTests
{
    [Fact]
    public void AddOneAndOneIsTwo()
    {
        var calculator = new Calculator();
        var result = calculator.Add(1, 1);
        Assert.Equal(2, result);
    }
}
```

The `Fact` annotation is the way of declaring a xUnit basic test: facts should be facts, things known very well about our application, some sort of invariants that must always be in place.

Let's run everything with the `dotnet test` command. You will see other test from other test projects running with our brand new test.

For more data-driven tests, we use `Theory`, which is the annotation to mark parameterized tests; those tests are theories about our application results, so we need to confirm them.
`InlineData` is the annotation to specify the parameter we are passing to the test method, and we declare in the signature how we're naming and receiving it.

Let's add a `Theory` to test the subtract method:

```c#
[Theory]
[InlineData(4, 2, 2)]
[InlineData(7, 2, 5)]
[InlineData(1, 0, 1)]
[InlineData(-1, -1, 0)]
[InlineData(0, 0, 100)]
public void SubtractsCorrectly(int firstNumber, int secondNumber, int expectedResult)
{
    var calculator = new Calculator();
    var actualResult = calculator.Subtract(firstNumber, secondNumber);
    Assert.Equal(expectedResult, actualResult);
}
```

We added a wrong result just to see the test runner fail, so run `dotnet test`, see it fail, fix the wrong `InlineData`.

A cool way to pass `InlineData` is via a static property of the class, instead of having to specify all the cases in the annotations. The properties must return `IEnumerable<object[]>`. `IEnumerable<T>` is a C# interface that defines how to iterate over collections of type `T`.

Let's use this method to test `Multiply`; let's write the data-generating property:

```c#
public static IEnumerable<object[]> MultiplyTestCases =>
    new List<object[]>
    {
        new object[] { 1, 2, 2 },
        new object[] { 2, 2, 4 },
        new object[] { 8, 2, 16 },
        new object[] { 10, 10, 100 },
        new object[] { 15, 10, 150 },
    };
```

and then the test method:

```c#
[Theory]
[MemberData(nameof(MultiplyTestCases))]
private void MultipliesCorrectly(int firstNumber, int secondNumber, int expectedResult)
{
    var calculator = new Calculator();
    var actualResult = calculator.Multiply(firstNumber, secondNumber);
    Assert.Equal(expectedResult, actualResult);
}
```

There are more options, feel free to consult [xUnit website](https://xunit.net/) for more.

One potential improvement is test setup, look at the single test in `MyApi.Web.Tests` to see how to create a single context shared between all tests.

If you want a `setUp` method that runs before every test, just declare the constructor of your test class and setup there.

### Moq

Moq is the standard mocking library in the .NET ecosystem. It's an external library, so we have to go get it from NuGet.
You can obviously program your own mocks from the interface, but it's a bit *unwieldy*.

Go on [nuget.org](https://www.nuget.org/) and search for Moq. 
Click on it (will bring you [here](https://www.nuget.org/packages/Moq/)) to be brought to the package page, where you can see some tabs
with some commands to import the library. We can use `PackageReference`, directly pasting that line in the `csproj` file followed by `dotnet restore`
in that directory, or we can use the `.NET CLI` tab. This tab will show the command to import the library in your Project via `dotnet` CLI.

Let's run the command.

```shell
~/MyApp# dotnet add tests/MyApp.Core.Tests package Moq --version 4.16.1
```

As usual we need to specify the project directory. If we don't want to, we can just `cd` there and run the command straight from nuget.org.

Let's write a new class that has a dependency, so we can mock it:

```shell
~/MyApp# touch src/MyApp.Core/MegaCalculator.cs
```

```c#
namespace MyApp.Core;

public class MegaCalculator
{
    private readonly ICalculator _calculator;

    public MegaCalculator(ICalculator calculator)
    {
        _calculator = calculator;
    }
    
    public int MegaAlgorithm(int firstNumber, int secondNumber)
    {
        var added = _calculator.Add(firstNumber, secondNumber);
        var subOne = _calculator.Subtract(firstNumber, 1);
        var multBoth = _calculator.Multiply(added, subOne);

        return multBoth;
    }
}
```

`MegaAlgorithm` wants an `ICalculator` instance, and we will mock it. Create the test file

```shell
~/MyApp# touch tests/MyApp.Core.Tests/MegaCalculatorTests.cs
```

and write the code:

```c#
using Moq;
using Xunit;

namespace MyApp.Core.Tests;

public class MegaCalculatorTests
{
    [Theory]
    [InlineData(4, 10, 42)]
    public void TestMegaAlgorithm(int firstNumber, int secondNumber, int expectedResult)
    {
        // Arrange
        var mockCalculator = new Mock<ICalculator>();
        mockCalculator.Setup(c => c.Add(firstNumber, secondNumber)).Returns(14);
        mockCalculator.Setup(c => c.Subtract(firstNumber, 1)).Returns(3);
        mockCalculator.Setup(c => c.Multiply(14, 3)).Returns(expectedResult);
        var megaCalculator = new MegaCalculator(mockCalculator.Object);
        
        // Act
        var actualResult = megaCalculator.MegaAlgorithm(firstNumber, secondNumber);
        
        // Assert
        mockCalculator.Verify(c => c.Add(firstNumber, secondNumber));
        mockCalculator.Verify(c => c.Subtract(firstNumber, 1));
        mockCalculator.Verify(c => c.Multiply(14, 3));
        Assert.Equal(expectedResult, actualResult);
    }
}
```

Pretty simple code: we create a new instance of `Mock<ICalculator>`, `.Setup` the method calls that the mock will receive with a return value, pass it to the `MegaCalculator` and then verify that all calls were made.

Run `dotnet test` and it will give you green bar. If you want to break the verification, change `MegaCalculator.MegaAlgorithm` to hardcoded `42` as a return value, comment the method calls and rerun `dotnet test`.

Moq has many many other possibilities that you can see [in the documentation pages](https://github.com/moq/moq4/wiki/Quickstart).
