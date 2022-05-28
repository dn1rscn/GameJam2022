/// <summary>
///     A damage descriptor, based on types. You can apply
///     specific armor reduction, if desired.
/// </summary>
public class Damage
{
    public enum Type
    {
        KINETIC,
        ELECTRIC,
        HEAT,
        EXPLOSIVE,
        TRUE
    }
    public Type type;
    public float amount;
    public Damage(float amount, Type type)
    {
        this.amount = amount;
        this.type = type;
    }
    public Damage ApplyReduction(float factor)
    {
        return new Damage(amount - amount * factor, Type.TRUE);
    }
    public static string format(Type type)
    {
        switch (type)
        {
            case Type.KINETIC:
                return "kinetic";
            case Type.ELECTRIC:
                return "electric";
            case Type.HEAT:
                return "fire";
            case Type.EXPLOSIVE:
                return "blast";
            case Type.TRUE:
                return "true";
        }
        throw new System.Exception($"Not allowed: {type}, unknown type.");
    }
}
