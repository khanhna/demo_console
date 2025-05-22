using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using AutoFixture.Kernel;

namespace Testing.Shared.Utils;

/// <summary>
/// To address readonly properties when mock them, <a href="https://stackoverflow.com/questions/47391406/autofixture-and-read-only-properties"> visit here</a>
/// </summary>
public class OverridePropertyBuilder<T, TProp> : ISpecimenBuilder
{
    private readonly PropertyInfo _propertyInfo;
    private readonly TProp _value;

    public OverridePropertyBuilder(Expression<Func<T, TProp>> expr, TProp value)
    {
        _propertyInfo = (expr.Body as MemberExpression)?.Member as PropertyInfo ??
                        throw new InvalidOperationException("invalid property expression");
        _value = value;
    }

    public object Create(object request, ISpecimenContext context)
    {
        if (request is not ParameterInfo pi)
            return new NoSpecimen();

        var camelCase = Regex.Replace(_propertyInfo.Name, @"(\w)(.*)",
            m => m.Groups[1].Value.ToLower() + m.Groups[2]);

        if (pi.ParameterType != typeof(TProp) ||
            string.Compare(pi.Name, camelCase, StringComparison.OrdinalIgnoreCase) != 0)
            return new NoSpecimen();

        return _value!;
    }
}