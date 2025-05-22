using System;
using System.Collections.Generic;
using AutoFixture;
using NSubstitute;

namespace Testing.Shared;

public abstract class TestingContext<T> where T: class
{
    private readonly Dictionary<Type, object> _injectedMocks = new();
    private readonly Dictionary<Type, object> _injectedConcreteClasses = new();
    protected IFixture Fixture { get; } = FixtureFactory.CreateFixture();
    private T? _instance;

    protected TMockType GetMockFor<TMockType>() where TMockType : class
    {
        if(_injectedMocks.TryGetValue(typeof(TMockType), out var existingMock))
            return (existingMock as TMockType)!;
        
        if(_injectedConcreteClasses.TryGetValue(typeof(TMockType), out var existingConcreteClassMocked))
            return (existingConcreteClassMocked as TMockType)!;
            
        var newMock = Substitute.For<TMockType>();
        var newMockKv = new KeyValuePair<Type, object>(typeof(TMockType), newMock);  
        _injectedMocks.Add(newMockKv.Key, newMockKv.Value);
        Fixture.Inject(newMock);
            
        return newMock;
    }
        
    public void InjectMock<TClassType>(TClassType injectedClass) where TClassType : class  
    {
        if(_injectedConcreteClasses.TryGetValue(typeof(TClassType), out _))
            throw new Exception($"Object with type <{injectedClass.GetType().Name}> has been injected more than once");
            
        _injectedConcreteClasses.Add(typeof(TClassType), injectedClass);  
        Fixture.Inject(injectedClass);
    }

    public T Iut => _instance ??= Fixture.Build<T>().OmitAutoProperties().Create();
}