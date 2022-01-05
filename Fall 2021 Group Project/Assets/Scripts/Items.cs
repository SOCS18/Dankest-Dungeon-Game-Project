using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
    /// <summary>
    /// A list of nearby pickups.
    /// </summary>
    [SerializeField] private List<GameObject> nearbyPickups = null;

    [SerializeField] private PlayerController refScript;
    public AudioSource audioSource;
    public float volume = .5f;

    #region Monobehaviour

    void Start()
    {
        nearbyPickups = new List<GameObject>();
        //PlayerController refScript = this.GetComponent<PlayerController>().shootRange
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // If we are nearby any pickups and we have pressed E
        if(Input.GetKeyDown(KeyCode.E) && nearbyPickups.Count > 0)
        {
            CollectPickup();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Pickup")
        {
            if (nearbyPickups.Contains(collision.gameObject))
                nearbyPickups.Remove(collision.gameObject);

            if(nearbyPickups.Count == 0)
                UIHandler.Instance.PickupText_SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Pickup")
        {
            nearbyPickups.Add(collision.gameObject);
        }
    }

    #endregion

    #region Collection Handling

    /// <summary>
    /// Handles the collection of Pickups.
    /// </summary>
    private void CollectPickup()
    {
        PickUp pickUp = null;
        try
        {
            pickUp = nearbyPickups[0].GetComponent<PickUp>(); // Retrieve the first pickup we got near.
        }
        catch
        {
            nearbyPickups.RemoveAt(0);
            if (nearbyPickups.Count == 0)
                return;
            else
                CollectPickup();
        }
        switch (pickUp.GetPickupType()) // Change player values based on the pickup type.
        {
            case PICKUP_TYPE.HEALTH:
                CollectHealth(pickUp.GetEffectValue(), pickUp.GetPickupSound());
                break;

            case PICKUP_TYPE.HEAL:
                CollectHeal(pickUp.GetEffectValue(), pickUp.GetPickupSound());
                break;

            case PICKUP_TYPE.AD:
                CollectAD(pickUp.GetEffectValue(), pickUp.GetPickupSound());
                break;

            case PICKUP_TYPE.AS:
                CollectAS(pickUp.GetEffectValue(), pickUp.GetPickupSound());
                break;

            case PICKUP_TYPE.JUMP_AMOUNT:
                CollectJumpAmount(pickUp.GetEffectValue(), pickUp.GetPickupSound());
                break;

            case PICKUP_TYPE.MONEY:
                CollectMoney(pickUp.GetEffectValue(), pickUp.GetPickupSound());
                break;

            case PICKUP_TYPE.MOVEMENT_SPEED:
                CollectMovementSpeed(pickUp.GetEffectValue(), pickUp.GetPickupSound());
                break;
        }

        nearbyPickups.RemoveAt(0); // Remove that recently used pickup from the list
        Destroy(pickUp.gameObject);  // and then destroy it.
    }

    private void CollectMovementSpeed(float amount, AudioClip sound)
    {
        PlaySound(sound);
        refScript.maxSpeed += amount;
    }

    private void CollectMoney(float amount, AudioClip sound)
    {
        PlaySound(sound);
        refScript.Money += (int)amount;
    }

    private void CollectJumpAmount(float amount, AudioClip sound)
    {
        PlaySound(sound);
        refScript.jumpAmount += (int)amount;
    }

    private void CollectAS(float amount, AudioClip sound)
    {
        PlaySound(sound);
        refScript.attackRate += amount;
    }

    private void CollectAD(float amount, AudioClip sound)
    {
        PlaySound(sound);
        refScript.attackDamage += amount;
    }

    private void CollectHealth(float amount, AudioClip sound)
    {
        PlaySound(sound);
        refScript.maxHealth += (int)amount;
        refScript.healthBar.SetMaxHealth(refScript.maxHealth);
    }

    private void CollectHeal(float amount, AudioClip sound)
    {
        PlaySound(sound);
        refScript.currentHealth += (int)amount;
        refScript.healthBar.SetHealth(refScript.currentHealth);
    }

    private void PlaySound(AudioClip audioClip)
    {
        audioSource.pitch = UnityEngine.Random.Range(.75f, 1.25f); // Pitch variation
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    #endregion
}
