using System;

[Serializable]
public struct Currency
{
    public int Food, Wood, Metal;

    public static Currency operator +(Currency a, Currency b)
    {
        return new Currency
        {
            Food = a.Food + b.Food,
            Wood = a.Wood + b.Wood,
            Metal = a.Metal + b.Metal
        };
    }
    public static Currency operator -(Currency a, Currency b)
    {
        return new Currency
        {
            Food = a.Food - b.Food,
            Wood = a.Wood - b.Wood,
            Metal = a.Metal - b.Metal
        };
    }

    public static Currency operator *(Currency a, int b)
    {
        return new Currency
        {
            Food = a.Food * b,
            Wood = a.Wood * b,
            Metal = a.Metal * b
        };
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType() != typeof(Currency)) return false;
        return ((Currency)obj) == this;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator <(Currency a, Currency b) => a.Food < b.Food && a.Wood < b.Wood && a.Metal < b.Metal;
    public static bool operator >(Currency a, Currency b) => a.Food > b.Food && a.Wood > b.Wood && a.Metal > b.Metal;
    public static bool operator <=(Currency a, Currency b) => a.Food <= b.Food && a.Wood <= b.Wood && a.Metal <= b.Metal;
    public static bool operator >=(Currency a, Currency b) => a.Food >= b.Food && a.Wood >= b.Wood && a.Metal >= b.Metal;
    public static bool operator ==(Currency a, Currency b) => a.Food == b.Food && a.Wood == b.Wood && a.Metal == b.Metal;
    public static bool operator !=(Currency a, Currency b) => a.Food != b.Food || a.Wood != b.Wood || a.Metal != b.Metal;
}