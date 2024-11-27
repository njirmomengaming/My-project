using System.Collections;
using UnityEngine;
using Alteruna;

public class ShieldPowerUp : AttributesSync
{
    public static Alteruna.Avatar powerUpOwnerAvatar;
    public Shield shield;
    public Alteruna.Avatar avatar;
    public Collider collider;

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
        shield = player.gameObject.GetComponentInParent<Shield>();
        if (shield == null)
        {
            Debug.LogWarning("Shield not found");
            yield break;
        }

        BroadcastRemoteMethod("ActivateShield");
        Destroy(gameObject);

    }

    [SynchronizableMethod]
    public void ActivateShield()
    {
        if (shield != null)
            shield.ActivateShield();
    }
}
