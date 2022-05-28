using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeEnemyDamageLogger : MonoBehaviour, IDamageAcceptor
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void IDamageAcceptor.TakeDamage(Damage damage)
    {
        // Debug.Log($"Took {damage.amount} of {Damage.format(damage.type)} damage!");
    }
}
