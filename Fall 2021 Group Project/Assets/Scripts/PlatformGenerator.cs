using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    [SerializeField] private GameObject BG1;
    [SerializeField] private GameObject BG2;
    [SerializeField] private GameObject BG3;
    [SerializeField] private GameObject BG4;

    [SerializeField] private GameObject ground1;
    [SerializeField] private GameObject ground2;
    [SerializeField] private GameObject ground3;
    [SerializeField] private GameObject ground4;

    [SerializeField] private GameObject groundLeft1;
    [SerializeField] private GameObject groundLeft2;
    [SerializeField] private GameObject groundLeft3;
    [SerializeField] private GameObject groundLeft4;

    [SerializeField] private GameObject groundRight1;
    [SerializeField] private GameObject groundRight2;
    [SerializeField] private GameObject groundRight3;
    [SerializeField] private GameObject groundRight4;

    [SerializeField] private GameObject exit1;
    [SerializeField] private GameObject exit2;
    [SerializeField] private GameObject exit3;
    [SerializeField] private GameObject exit4;

    [SerializeField] private GameObject door;

    [SerializeField] private GameObject spawner;

    // x should be -15.6 to 15.6
    // y needs to be either 3, 6, or 9
    private Vector3 nextPlatformPos = Vector3.zero;

    private List<GameObject> prefabsToSpawnB = new List<GameObject>();
    private List<GameObject> prefabsToSpawn1 = new List<GameObject>();
    private List<GameObject> prefabsToSpawn2 = new List<GameObject>();
    private List<float> y_coords = new List<float>();

    // Start is called before the first frame update
    public void Awake()
    {
        prefabsToSpawnB.Add(ground1);
        prefabsToSpawnB.Add(ground2);
        prefabsToSpawnB.Add(ground3);
        prefabsToSpawnB.Add(ground4);

        GameObject groundTemp = prefabsToSpawnB[Random.Range(0, (prefabsToSpawnB.Count))];

        Instantiate(groundTemp, groundTemp.transform.position, Quaternion.identity);

        if (groundTemp.name == "Ground")
        {
            prefabsToSpawn1.Add(groundLeft1);
            prefabsToSpawn2.Add(groundRight1);
            prefabsToSpawn1.Add(exit1);
            prefabsToSpawn2.Add(exit1);
            Instantiate(BG1, BG1.transform.position, Quaternion.identity);
        }

        if (groundTemp.name == "Ground Red")
        {
            prefabsToSpawn1.Add(groundLeft2);
            prefabsToSpawn2.Add(groundRight2);
            prefabsToSpawn1.Add(exit2);
            prefabsToSpawn2.Add(exit2);
            Instantiate(BG2, BG2.transform.position, Quaternion.identity);
        }

        if (groundTemp.name == "Ground Green")
        {
            prefabsToSpawn1.Add(groundLeft3);
            prefabsToSpawn2.Add(groundRight3);
            prefabsToSpawn1.Add(exit3);
            prefabsToSpawn2.Add(exit3);
            Instantiate(BG3, BG3.transform.position, Quaternion.identity);
        }

        if (groundTemp.name == "Ground Dark")
        {
            prefabsToSpawn1.Add(groundLeft4);
            prefabsToSpawn2.Add(groundRight4);
            prefabsToSpawn1.Add(exit4);
            prefabsToSpawn2.Add(exit4);
            Instantiate(BG4, BG4.transform.position, Quaternion.identity);
        }

        y_coords.Add(2.2f);
        y_coords.Add(5.2f);
        y_coords.Add(8.2f);
        GameObject tempObstaclePrev1 = null;
        GameObject tempObstaclePrev2 = null;

        float tempY = y_coords[0];

        for (int i = 0; i < 6; i++)
        {

            if ( i <= 2)
            {
                GameObject tempObstacle1 = prefabsToSpawn1[Random.Range(0, (prefabsToSpawn1.Count))];
                
                tempY = y_coords[i];

                //spawner.GetComponent<EnemySpawner>().spawnpoints[i] = tempObstacle1.transform.GetChild(0);

                if (tempObstacle1.name == "Platform Exit" || tempObstacle1.name == "Platform Exit Red" || tempObstacle1.name == "Platform Exit Green" || tempObstacle1.name == "Platform Exit Dark")
                {
                    if (tempObstacle1.name == "Platform Exit")
                    {
                        prefabsToSpawn1.Remove(exit1);
                        prefabsToSpawn2.Remove(exit1);
                    }

                    if (tempObstacle1.name == "Platform Exit Red")
                    {
                        prefabsToSpawn1.Remove(exit2);
                        prefabsToSpawn2.Remove(exit2);
                    }

                    if (tempObstacle1.name == "Platform Exit Green")
                    {
                        prefabsToSpawn1.Remove(exit3);
                        prefabsToSpawn2.Remove(exit3);
                    }

                    if (tempObstacle1.name == "Platform Exit Dark")
                    {
                        prefabsToSpawn1.Remove(exit4);
                        prefabsToSpawn2.Remove(exit4);
                    }

                }

                if (tempObstaclePrev1 != null)
                {

                    tempObstacle1.transform.position = new Vector3(Random.Range((-10f + tempObstaclePrev1.transform.position.x), (10f + tempObstaclePrev1.transform.position.x)), tempY, 0);
                    Debug.Log("second/third x value is " + tempObstacle1.transform.position.x + " y value is " + tempObstacle1.transform.position.y);
                    if (tempObstacle1.transform.position.x > -4f)
                    {
                        tempObstacle1.transform.position = new Vector3(-4f, tempY, 0);
                    }
                    else if (tempObstacle1.transform.position.x < -15.6f)
                    {
                        tempObstacle1.transform.position = new Vector3(-15.6f, tempY, 0);
                    }
                }
                else
                {
                    tempObstacle1.transform.position = new Vector3(Random.Range(-15.6f, -4f), tempY, 0);
                    Debug.Log("first x value is " + tempObstacle1.transform.position.x + " y value is " + tempObstacle1.transform.position.y);
                }

                Instantiate(tempObstacle1, tempObstacle1.transform.position, Quaternion.identity);
                tempObstaclePrev1 = tempObstacle1;
            }
            else
            {


                if (i == 3)
                    tempY = y_coords[0];

                if (i == 4)
                    tempY = y_coords[1];

                if (i == 5)
                    tempY = y_coords[2];

                GameObject tempObstacle2 = prefabsToSpawn2[Random.Range(0, (prefabsToSpawn2.Count))];

                //spawner.GetComponent<EnemySpawner>().spawnpoints[i] = tempObstacle2.transform.GetChild(0);

                if (tempObstacle2.name == "Platform Exit" || tempObstacle2.name == "Platform Exit Red" || tempObstacle2.name == "Platform Exit Green" || tempObstacle2.name == "Platform Exit Dark")
                {
                    if (tempObstacle2.name == "Platform Exit")
                    {
                        prefabsToSpawn1.Remove(exit1);
                        prefabsToSpawn2.Remove(exit1);
                    }

                    if (tempObstacle2.name == "Platform Exit Red")
                    {
                        prefabsToSpawn1.Remove(exit2);
                        prefabsToSpawn2.Remove(exit2);
                    }

                    if (tempObstacle2.name == "Platform Exit Green")
                    {
                        prefabsToSpawn1.Remove(exit3);
                        prefabsToSpawn2.Remove(exit3);
                    }

                    if (tempObstacle2.name == "Platform Exit Dark")
                    {
                        prefabsToSpawn1.Remove(exit4);
                        prefabsToSpawn2.Remove(exit4);
                    }

                }

                if (tempObstaclePrev2 != null)
                {

                    tempObstacle2.transform.position = new Vector3(Random.Range((-10f + tempObstaclePrev2.transform.position.x), (10f + tempObstaclePrev2.transform.position.x)), tempY, 0);

                    if (tempObstacle2.transform.position.x > 15.6f)
                    {
                        tempObstacle2.transform.position = new Vector3(15.6f, tempY, 0);
                    }
                    else if (tempObstacle2.transform.position.x < 4f)
                    {
                        tempObstacle2.transform.position = new Vector3(4f, tempY, 0);
                    }
                }
                else
                {
                    tempObstacle2.transform.position = new Vector3(Random.Range(4f, 15.6f), tempY, 0);
                }

                Instantiate(tempObstacle2, tempObstacle2.transform.position, Quaternion.identity);
                //Debug.Log("x value is " + tempObstacle.transform.position.x + " y value is " + tempObstacle.transform.position.y);
                tempObstaclePrev2 = tempObstacle2;
            }
        }

        if (GameObject.FindGameObjectsWithTag("Exit").Length == 0)
        {
            GameObject go = Instantiate(door);
            go.transform.position = new Vector3(15, .66f, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
