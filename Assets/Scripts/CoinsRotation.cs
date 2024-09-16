using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsRotation : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0, 40 * Time.deltaTime, 0);
    }
}
