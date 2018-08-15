using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMethods : MonoBehaviour {

    public GameObject card;

    public void Start()
    {
        Vector3 randomSpawn = new Vector3(Random.Range(-7.12f, 2.37f), Random.Range(1.04f, -0.05f), Random.Range(-8.17f, -5.12f));
        Instantiate(card, randomSpawn, transform.rotation);
    }

    public void SpawnCard()
    {
        Vector3 randomSpawn = new Vector3(Random.Range(-7.12f, 2.37f), Random.Range(1.04f, -0.05f), Random.Range(-8.17f, -5.12f));
        Instantiate(card, randomSpawn, transform.rotation);
    }
}
