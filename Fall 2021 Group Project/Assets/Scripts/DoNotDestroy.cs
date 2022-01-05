using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroy : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (GameObject.FindGameObjectsWithTag(gameObject.tag).Length > 1)
            DestroyImmediate(gameObject);
    }
}
