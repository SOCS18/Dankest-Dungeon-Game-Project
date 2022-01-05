using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    private static UIHandler instance;
    /// <summary>
    /// An Instance of the UIHandler. Since there is only going to be one UIHandler, it is convient to use an instance for ease of Access.
    /// Use UIHandler.Instance to access the instance.
    /// </summary>
    public static UIHandler Instance
    {
        get { return instance; }
    }

    public Text pickupText;
    public Text moneyText;
    public Text attackDamagePriceText;
    public Text attackSpeedPriceText;
    public Text moveSpeedPriceText;
    public Text jumpAmountPriceText;

    private void Awake()
    {
        instance = this; // Set an instance of the UIHandler so that it can be easily accessed anywhere.
    }

    void Start()
    {
        pickupText.gameObject.SetActive(false);
    }

    public void PickupText_SetActive(bool b)
    {
        if(pickupText)

        pickupText.gameObject.SetActive(b);
    }

    public void PickupText_SetText(string text)
    {
        pickupText.text = text;
    }

    public void OnMoneyChanged(int newMoney)
    {
        moneyText.text = newMoney + "";
    }

    public void SetAttackDamagePriceText(string text)
    {
        attackDamagePriceText.text = text;
    }
    
    public void SetAttackSpeedPriceText(string text)
    {
        attackSpeedPriceText.text = text;
    }
    
    public void SetMovementSpeedPriceText(string text)
    {
        moveSpeedPriceText.text = text;
    }
    
    public void SetJumpAmountPriceText(string text)
    {
        jumpAmountPriceText.text = text;
    }

}
