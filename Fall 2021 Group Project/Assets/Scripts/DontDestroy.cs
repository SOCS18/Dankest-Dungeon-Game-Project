using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private static DontDestroy unique = null;

    void Start()
    {
        DontDestroyOnLoad(this);
        if (unique == null)
        {
            unique = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

}
