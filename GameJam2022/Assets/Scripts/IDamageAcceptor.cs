/// <summary>
///     This simple interface exposes basic damage-accepting
///     options. Used mainly on enemies.
/// </summary>
public interface IDamageAcceptor
{
    void TakeDamage(Damage damage);
}
