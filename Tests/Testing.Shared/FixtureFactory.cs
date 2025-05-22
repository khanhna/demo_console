using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace Testing.Shared;

public static class FixtureFactory
{
    public static IFixture CreateFixture(ICustomization? options = null) =>
        new Fixture().Customize(options ?? new AutoNSubstituteCustomization { ConfigureMembers = true });
}