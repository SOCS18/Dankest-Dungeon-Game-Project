using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] float degrees = 50;

    private void Update()
    {
        transform.Rotate(0, degrees * Time.unscaledDeltaTime, 0);
    }

}
