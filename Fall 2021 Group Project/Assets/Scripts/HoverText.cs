using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverText : MonoBehaviour
{
    [SerializeField] private GameObject text;
    // Start is called before the first frame update
    void Start()
    {
        text.SetActive(false);
    }

    private void OnMouseOver()
    {
        text.SetActive(true);
    }

    private void OnMouseExit()
    {
        text.SetActive(false);
    }
}
