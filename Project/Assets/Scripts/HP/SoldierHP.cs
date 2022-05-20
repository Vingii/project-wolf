using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierHP : MainHP
{
    GameObject eye;
    MainProperties properties;
    GameObject[] capsules;
    public Material MatOff;
    public GameObject Ammo;

    bool animrunning;
    Coroutine animcor;

    public override void Die()
    {
        GameObject ammo = Instantiate(Ammo, gameObject.transform.position, Quaternion.identity);
        ammo.GetComponent<AmmoPickup>().Ammo = 4;
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
        else
        {
            if (animrunning)
            {
                StopCoroutine(animcor);
                animrunning = false;
            }
            animcor = StartCoroutine(DamageAnim());
            for (int i = 0; i < 10 * (1 - properties.HP / (float)properties.MaxHP) - 1; i++)
            {
                capsules[i].GetComponent<MeshRenderer>().material = MatOff;
            }
        }
    }
    IEnumerator DamageAnim()
    {
        animrunning = true;
        float tmax = 0.2f;
        float tcurr = tmax;
        while (tcurr > 0)
        {
            tcurr -= Time.deltaTime;
            eye.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.white, Color.black, tcurr / tmax);
            yield return null;
        }
        eye.GetComponent<MeshRenderer>().material.color = Color.white;
        animrunning = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        properties = gameObject.GetComponent<MainProperties>();
        capsules = new GameObject[10];
        eye = gameObject.transform.GetChild(2).gameObject;
        for (int i = 0; i < 10; i++)
        {
            capsules[i] = gameObject.transform.GetChild(0).GetChild(i).gameObject;
        }
    }
}
