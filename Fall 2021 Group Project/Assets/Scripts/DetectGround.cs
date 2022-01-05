using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectGround : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            playerController.isGrounded = true;
            playerController.animator.SetBool("IsJumping", false);
            playerController.currentJump = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            playerController.isGrounded = false;
            playerController.animator.SetBool("IsJumping", true);
        }
    }

}
