using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelFade : MonoBehaviour
{
    private bool isFaded = false;
    private bool gameOver = false;
    public float duration = .5f;
    [SerializeField] private GameObject DiedMenu;
    [SerializeField] private GameObject player;

    private void Update()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            return;
        }

        if( player.GetComponent<PlayerController>().currentHealth <= 0 && gameOver == false)
        {
            gameOver = true;
            
            Debug.Log("Player dead: Panel fade");
            //DiedMenu.SetActive(true);
            Fade();
        }
    }

    public void Fade()
    {
        CanvasGroup canvGroup = DiedMenu.GetComponent<CanvasGroup>();

        //Toggle the end value depending on the faded state
        StartCoroutine(DoFade(canvGroup, canvGroup.alpha, 1));
        canvGroup.interactable = true;
        isFaded = true ; //Toggles the faded state

    }

    public IEnumerator DoFade(CanvasGroup canvGroup, float start, float end)
    {
        float counter = 0f;

        while(counter < duration) {
            counter += Time.deltaTime;
            canvGroup.alpha = Mathf.Lerp(start, end, counter / duration);

            yield return null;
        }
       

    }
}
