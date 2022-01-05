using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance;

    [SerializeField] private TextMeshProUGUI attackDamageText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;
    [SerializeField] private TextMeshProUGUI movementSpeedText;
    [SerializeField] private TextMeshProUGUI jumpAmountText;
    [SerializeField] private GameObject menu = null;
    public static bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Player hit pause");
            isPaused = true;
            PauseGame();
        }

        if (GameController.Instance.playerController != null)
        {
            attackDamageText.text = GameController.Instance.playerController.attackDamage.ToString();
            attackSpeedText.text = GameController.Instance.playerController.attackRate.ToString();
            movementSpeedText.text = GameController.Instance.playerController.maxSpeed.ToString();
            jumpAmountText.text = GameController.Instance.playerController.jumpAmount.ToString();
        }
    }

    void PauseGame()
    {
        if (isPaused)
        {
            Time.timeScale = 0f;
            menu.gameObject.SetActive(true);
        }
        /* else
         {
             Time.timeScale = 1;
             menu.gameObject.SetActive(false);
         }
        */
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        menu.gameObject.SetActive(false);
        isPaused = false;
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1;
        isPaused = false;
        SceneManager.LoadScene(0);
    }

}
