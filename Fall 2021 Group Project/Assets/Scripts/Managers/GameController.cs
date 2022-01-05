using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
   
    public static GameController Instance = null;
    [SerializeField] private GameObject player;

    public int levelCounter = 1;
    [SerializeField] private EnemySpawner enemySpawner;
    public PlayerController playerController;
    public bool hasReloaded = false;

    public int maxHealth     = 0;
    public int jumpAmount    = 0;
    public float attackDamage  = 0;
    public float attackRate    = 0;
    public int currentHealth = 0;
    public int Money         = 0;
    public float maxSpeed = 0;

    // Start is called before the first frame update

    public int attackDamagePrice    = 3;
    public int attackSpeedPrice     = 3;
    public int movementSpeedPrice   = 3;
    public int jumpAmountPrice      = 5;
    
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

    void Start()
    {
        enemySpawner = GetComponent<EnemySpawner>();
        playerController = player.GetComponent<PlayerController>();
        DontDestroyOnLoad(gameObject);
    }

    public void OnReloadExit()
    {
        Debug.Log("Exiting...");
        levelCounter++;
        maxHealth    = playerController.maxHealth;
        jumpAmount   = playerController.jumpAmount;
        attackDamage = playerController.attackDamage;
        attackRate   = playerController.attackRate;
        currentHealth = playerController.currentHealth;
        Money        = playerController.Money;
        maxSpeed     = playerController.maxSpeed;

        Shop shop = GameObject.FindGameObjectWithTag("UIHandler").GetComponent<Shop>();
        attackDamagePrice   = shop.attackDamagePrice  ;
        attackSpeedPrice    = shop.attackSpeedPrice   ;
        movementSpeedPrice  = shop.movementSpeedPrice ;
        jumpAmountPrice = shop.jumpAmountPrice;
        hasReloaded = true;
    }

    public void OnReloadEnter(GameObject player)
    {
        Debug.Log("Reloading...");
        GetComponent<PlatformGenerator>().Awake();
        enemySpawner.Start();
        this.player = player;
        playerController = player.GetComponent<PlayerController>();

        playerController.maxHealth =    maxHealth;
        playerController.jumpAmount =   jumpAmount;
        playerController.attackDamage = attackDamage;
        playerController.attackRate =   attackRate;
        playerController.currentHealth =currentHealth;
        playerController.Money =        Money;
        playerController.maxSpeed =     maxSpeed;

        if(levelCounter % 5 == 0)
        {
            enemySpawner.bossRoom = true;
            enemySpawner.maxSpawns = Mathf.FloorToInt(levelCounter / 20) + 1;
            enemySpawner.spawnsLeft = enemySpawner.maxSpawns;
            enemySpawner.EnemySpawnTimeInterval = enemySpawner.EnemySpawnTimeInterval - (enemySpawner.EnemySpawnTimeInterval * .1f);
            enemySpawner.EnemySpawnTime = enemySpawner.EnemySpawnTimeInterval - (enemySpawner.EnemySpawnTimeInterval * .1f);
        }
        else
        {
            enemySpawner.bossRoom = false;
            enemySpawner.maxSpawns = Mathf.FloorToInt(levelCounter / 4) + 2;
            enemySpawner.spawnsLeft = Mathf.FloorToInt(levelCounter / 2) + 2;
            enemySpawner.EnemySpawnTimeInterval = enemySpawner.EnemySpawnTimeInterval - (enemySpawner.EnemySpawnTimeInterval * .1f);
            enemySpawner.EnemySpawnTime = enemySpawner.EnemySpawnTimeInterval - (enemySpawner.EnemySpawnTimeInterval * .1f);
        }

        Shop shop = GameObject.FindGameObjectWithTag("UIHandler").GetComponent<Shop>();
        shop.attackDamagePrice = attackDamagePrice;
        shop.attackSpeedPrice = attackSpeedPrice;
        shop.movementSpeedPrice = movementSpeedPrice;
        shop.jumpAmountPrice = jumpAmountPrice;
        enemySpawner.currentSpawns = 0;
    }
}
