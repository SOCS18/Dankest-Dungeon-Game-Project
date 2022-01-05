using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PICKUP_TYPE { HEALTH, AS, AD, JUMP_AMOUNT, MONEY, MOVEMENT_SPEED, HEAL }

public class PickUp : MonoBehaviour
{
    /// <summary>
    /// The type of the pickup. Ex. PICKUP_TYPE.HEALTH effects the player's health stat.
    /// </summary>
    [SerializeField] private PICKUP_TYPE pickupType = PICKUP_TYPE.HEALTH; // The type of pickup.
    
    /// <summary>
    /// The numerical value of the stat effected. If type is health and effectValue is 1, the player will heal 1 health when picked up.
    /// </summary>
    [SerializeField] private float effectValue = 1;

    /// <summary>
    /// Sound played when picked up.
    /// </summary>
    [SerializeField] private AudioClip pickupSound;

    #region UNITY
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            UIHandler.Instance.PickupText_SetActive(true);
        }

        if (collision.gameObject.tag == "Platform")
        {
            Rigidbody2D _rigidbody2D = GetComponent<Rigidbody2D>();
            _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            transform.position = new Vector2(transform.position.x, transform.position.y + .2f);
        }
    }
    #endregion

    #region GETS

    public PICKUP_TYPE GetPickupType()
    {
        return pickupType;
    }

    public float GetEffectValue()
    {
        return effectValue;
    }

    public AudioClip GetPickupSound()
    {
        return pickupSound;
    }

    #endregion
}
