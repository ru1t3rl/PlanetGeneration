using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatory : MonoBehaviour
{
    [SerializeField] Vector3 rotationSpeed = Vector3.one;
    [SerializeField] bool fixedX = true, fixedY = false, fixedZ = true;

    void FixedUpdate()
    {
        if (!fixedX)
            transform.Rotate(Vector3.right, rotationSpeed.x);
        if (!fixedY)
            transform.Rotate(Vector3.up, rotationSpeed.y);
        if (!fixedZ)
            transform.Rotate(Vector3.forward, rotationSpeed.z);
    }
}
