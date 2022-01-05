using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private GameObject player;
    [SerializeField]private Collider2D hitBox;
    [SerializeField]private Collider2D playerCollider;
    private PlayerController playerController;

    void Awake(){
        player = GameObject.FindGameObjectWithTag("Player");
        playerCollider = player.GetComponent<BoxCollider2D>();
        playerController = player.GetComponent<PlayerController>();
        hitBox = GetComponent<Collider2D>();
    }

    /// <summary>
    /// Check to see if Player's Collider2D is within our HitBox Collider2D, if so, deal damage.
    /// </summary>
    public void TryToHitPlayer(int damage)
    {
        if (IsTouchingPlayer())
        {
            Debug.Log("Player hit by hitbox");
            playerController.TakeDamage(damage);
            playerController.DamageLocation(transform.position, playerCollider);
        }
    }

    public bool IsTouchingPlayer()
    {
        return hitBox.IsTouching(playerCollider);
    }
}
