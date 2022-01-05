using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class destroyPickUps : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject healthObject;
    [SerializeField] private GameObject playerObject;
    
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void DestroyHealth()
    {
        Destroy(healthObject);
    }
}
