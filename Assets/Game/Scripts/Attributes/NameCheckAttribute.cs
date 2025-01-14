using System;

[AttributeUsage(AttributeTargets.Field)]
public class NameCheckAttribute : Attribute
{
    public readonly Type type;

    public NameCheckAttribute(Type type)
    {
        this.type = type;
    }
}
