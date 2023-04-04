using Riok.Mapperly.Abstractions;

namespace Demo_Console.Mapperly;

[Mapper(EnumMappingStrategy = EnumMappingStrategy.ByName)]
public static partial class CarMapper
{
    [MapProperty(nameof(Car.Manufacturer), nameof(CarDto.Producer))] // Map property with a different name in the target type
    [MapProperty(nameof(Car.Tires), nameof(CarDto.NumberOfTires))]
    [MapperIgnoreTarget(nameof(CarDto.NumberOfTiresMultiplyByFour))]
    public static partial CarDto MapCarToDto(Car car);

    private static int TiresToNumber(List<Tire> tires) => tires.Count;
}

public static class MapperlyTest
{
    public static readonly Car Source = new Car
        { Name = "This car", Color = CarColor.Blue, Tires = { new() { Description = "The single tire" } } };

    public static CarDto MappingFromSource() => CarMapper.MapCarToDto(Source);
}