using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class Shield : AttributesSync
{
    public GameObject shieldPrefab;  // The shield effect (could be a visual effect like a sphere or shield model)
    public float powerUpDuration = 10f;  // Duration of the shield
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [SynchronizableMethod]
    public void ActivateShield()
    {
        shieldPrefab.SetActive(true);
        Debug.LogWarning("shieldPrefab activated");
        StartCoroutine(WaitShield());
    }

    IEnumerator WaitShield()
    {
        yield return new WaitForSeconds(powerUpDuration);
        shieldPrefab.SetActive(false);
    }
}

