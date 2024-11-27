using System.Collections;
using UnityEngine;
using Alteruna;
using AlterunaFPS;

public class HealthPowerUp : AttributesSync
{
    public static Alteruna.Avatar powerUpOwnerAvatar;
    public Health health;
    public Alteruna.Avatar avatar;
    public Collider collider;
    public float amountHealth;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            collider = other.GetComponent<Collider>();
            avatar = other.gameObject.GetComponentInParent<Alteruna.Avatar>();
            if (avatar != null)
            {
                Debug.LogWarning("Avatar found");
                powerUpOwnerAvatar = avatar;
                StartCoroutine(PickUp(other));
            }
        }
    }

    IEnumerator PickUp(Collider player)
    {
        health = player.gameObject.GetComponentInParent<Health>();
        if (health == null)
        {
            Debug.LogWarning("Health not found");
            yield break;
        }

        BroadcastRemoteMethod("AddHealth");
        Destroy(gameObject);

    }

    [SynchronizableMethod]
    public void AddHealth()
    {
        if (health != null)
        {
            amountHealth = health.maxHealth * 0.3f;
            health.HealthPoints = Mathf.Min(health.HealthPoints + amountHealth, health.maxHealth);
            health.CallUpdateHealthBar();
        }
    }

}
