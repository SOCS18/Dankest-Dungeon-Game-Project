using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private AudioClip[] shopKeepBuy;
    [SerializeField] private AudioClip[] shopKeepNotEnoughMoney;
    [SerializeField] private AudioClip purchase;
    [SerializeField] private AudioSource audioSource;

    public bool purchased = false;

    [SerializeField] public int attackDamagePrice = 3;
    [SerializeField] public int attackSpeedPrice = 3;
    [SerializeField] public int movementSpeedPrice = 3;
    [SerializeField] public int jumpAmountPrice = 5;

    [SerializeField] private PlayerController player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        UIHandler.Instance.SetJumpAmountPriceText(jumpAmountPrice + "");
        UIHandler.Instance.SetMovementSpeedPriceText(movementSpeedPrice + "");
        UIHandler.Instance.SetAttackDamagePriceText(attackDamagePrice + "");
        UIHandler.Instance.SetAttackSpeedPriceText(attackSpeedPrice + "");
    }

    private void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        UIHandler.Instance.SetJumpAmountPriceText(jumpAmountPrice + "");
        UIHandler.Instance.SetMovementSpeedPriceText(movementSpeedPrice + "");
        UIHandler.Instance.SetAttackDamagePriceText(attackDamagePrice + "");
        UIHandler.Instance.SetAttackSpeedPriceText(attackSpeedPrice + "");
    }

    public void BuyMovementSpeed()
    {
        if(player.Money >= movementSpeedPrice)
        {
            player.Money -= movementSpeedPrice;
            player.maxSpeed += 5;
            movementSpeedPrice += movementSpeedPrice;
            UIHandler.Instance.SetMovementSpeedPriceText(movementSpeedPrice + "");
            audioSource.PlayOneShot(purchase);
            if (Random.Range(0f, 1f) > .5f)
                StartCoroutine("ShopKeep");
        }
        else
        {
            audioSource.PlayOneShot(shopKeepNotEnoughMoney[Random.Range(0, shopKeepNotEnoughMoney.Length)]);
        }
    }

    public void BuyAttackDamage()
    {
        if(player.Money >= attackDamagePrice)
        {
            player.Money -= attackDamagePrice;
            player.attackDamage += 1;
            attackDamagePrice += attackDamagePrice;
            UIHandler.Instance.SetAttackDamagePriceText(attackDamagePrice + "");
            audioSource.PlayOneShot(purchase);
            if (Random.Range(0f, 1f) > .5f)
                StartCoroutine("ShopKeep");
        }
        else
        {
            audioSource.PlayOneShot(shopKeepNotEnoughMoney[Random.Range(0, shopKeepNotEnoughMoney.Length)]);
        }
    }

    public void BuyAttackSpeed()
    {
        if(player.Money >= attackSpeedPrice)
        {
            player.Money -= attackSpeedPrice;
            player.attackRate += 1;
            attackSpeedPrice += attackSpeedPrice;
            UIHandler.Instance.SetAttackSpeedPriceText(attackSpeedPrice + "");
            audioSource.PlayOneShot(purchase);
            if (Random.Range(0f, 1f) > .5f)
                StartCoroutine("ShopKeep");
        }
        else
        {
            audioSource.PlayOneShot(shopKeepNotEnoughMoney[Random.Range(0, shopKeepNotEnoughMoney.Length)]);
        }
    }

    public void BuyJumpAmount()
    {
        if(player.Money >= jumpAmountPrice)
        {
            player.Money -= jumpAmountPrice;
            player.maxSpeed += 1;
            jumpAmountPrice += jumpAmountPrice;
            UIHandler.Instance.SetJumpAmountPriceText(jumpAmountPrice + "");
            audioSource.PlayOneShot(purchase);
            if(Random.Range(0f, 1.5f) > 1f)
                StartCoroutine("ShopKeep");
        }
        else
        {
            audioSource.PlayOneShot(shopKeepNotEnoughMoney[Random.Range(0, shopKeepNotEnoughMoney.Length)]);
        }
    }

    public IEnumerator ShopKeep()
    {
        while (audioSource.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }

        audioSource.PlayOneShot(shopKeepBuy[Random.Range(0, shopKeepBuy.Length)]);
        purchased = false;
    }
}
