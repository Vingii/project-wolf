using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogHP : MainHP
{
    MainProperties properties;
    public override void Die()
    {
        GlobalStats.Score += properties.Score;
        Destroy(gameObject);
    }
    public override void TakeDamage(int damage)
    {
        properties.HP -= damage;
        if (properties.HP <= 0)
        {
            Die();
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        properties = gameObject.GetComponent<MainProperties>();
    }
}
