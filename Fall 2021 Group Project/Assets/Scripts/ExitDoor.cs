using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{

    private bool isTouchingPlayer = false;
    private GameObject player = null;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("levelNum");
    }

    // Update is called once per frame
    void Update()
    {
        if(isTouchingPlayer && (Input.GetKey("up") || Input.GetKey(KeyCode.W)))
        {
            LoadNewLevel();
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isTouchingPlayer = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isTouchingPlayer = true;
        }
    }

    public void LoadNewLevel()
    {
        GameController.Instance.OnReloadExit();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
