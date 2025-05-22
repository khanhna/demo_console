using Demo_Console.Playground;
using FluentAssertions;
using NSubstitute;
using Testing.Shared;
using Testing.Shared.Extensions;
using Xunit.Abstractions;

namespace DemoConsole.UnitTests;

/// <summary>
/// Each test class if needed resources to be share between tests, it can be done by inject the mock object in this place.
/// </summary>
public class PlaygroundTestsFixture : IDisposable
{
    public static int Counter;
    public IAnimal Cat { get; }

    public PlaygroundTestsFixture()
    {
        Cat = Substitute.For<IAnimal>();
        Cat.Say().Returns("Woof");

        Counter++;
    }

    public void Dispose()
    {
        Console.WriteLine("Should be executed once for the whole test class");
    }
}

public class PlaygroundTests : TestingContext<Playground>, IClassFixture<PlaygroundTestsFixture>, IDisposable
{
    public static int Counter;
    
    private readonly ITestOutputHelper _output;

    public PlaygroundTests(PlaygroundTestsFixture fixture, ITestOutputHelper output)
    {
        _output = output;
        InjectMock(fixture.Cat);
        var person = GetMockFor<IPerson>();
        person.Say().Returns("Hi");
        
        Counter++;
    }

    [Fact]
    public void Cat_Should_Say_Woof()
    {
        // Act
        // Iut. should do something
        var test = GetMockFor<IAnimal>();
        var result = test.Say();
        
        result.Should().Be("Woof", $"We already mock it in {nameof(PlaygroundTestsFixture)}");
    }
    
    [Fact]
    public void Person_Should_Say_Hi()
    {
        // Act
        // Iut. should do something

        GetMockFor<IPerson>().Say().Should().Be("Hi", "We already mock it in constructor");
        Fixture.For<Person>().With(x => x.Name, "Khanh").Create()
            .Name.Should().Be("Khanh", "We just use AutoFixture to create the object");
    }
    
    [Theory]
    [InlineData("Expected")]
    public void Both_Should_Say_Expected(string expected)
    {
        // Act
        // Iut. should do something
        GetMockFor<IAnimal>().Say().Returns(expected);
        GetMockFor<IPerson>().Say().Returns(expected);
        
        GetMockFor<IAnimal>().Say().Should().Be(expected, "We just mock it in the test"); 
        GetMockFor<IPerson>().Say().Should().Be(expected, "We just mock it in the test"); 
    }
    
    public void Dispose()
    {
        _output.WriteLine($"--> {nameof(PlaygroundTests)}:{nameof(PlaygroundTestsFixture)} should be initialized just once per class, while actual, it is {PlaygroundTestsFixture.Counter} times.");
        _output.WriteLine(
            $"This cleanup block code should be executed once with each tests. It is executed {Counter}th times.");
    }
}

public interface IAnimal
{
    string Say();
}

public interface IPerson
{
    string Say();
}

public class Cat : IAnimal
{
    public static int Counter;
    
    public Cat()
    {
        Counter++;
    }
    
    public string Say() => "Meow";
}

public class Person : IPerson
{
    public static int Counter;
    
    public Person(string name)
    {
        Counter++;
        
        Name = name;
    }

    public string Name { get; }
    public string Say() => "Hello";
}