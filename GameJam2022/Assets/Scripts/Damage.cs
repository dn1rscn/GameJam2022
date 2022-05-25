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
}
