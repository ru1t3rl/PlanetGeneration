using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    [SerializeField] GameObject focusObject;

    void FixedUpdate()
    {
        transform.LookAt(focusObject.transform, Vector3.up);
    }
}
