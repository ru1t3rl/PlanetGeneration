using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineTool : MonoBehaviour
{
    [SerializeField] float freqeunce = 1, amplitude = 1, centre = 0;
    [SerializeField] SineTransform sineTransformAction = SineTransform.Scale;
    [SerializeField] bool fixedX = false, fixedY = false, fixedZ = false;

    Vector3 startPos, localStartPos, startScale;
    Vector3 scale, pos;

    Vector3 localStartRot, rot;

    float angle;
    float angleIncrement = 0.01f;

    void Awake()
    {
        startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        localStartPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        startScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        localStartRot = transform.localRotation.eulerAngles;
    }

    void FixedUpdate()
    {
        angle += angleIncrement;

        if (sineTransformAction == SineTransform.Scale)
        {
            scale.x = startScale.x + ((!fixedX) ? (amplitude * Mathf.Sin((2 * Mathf.PI / freqeunce) * angle) + centre) : centre);
            scale.y = startScale.y + ((!fixedY) ? (amplitude * Mathf.Cos((2 * Mathf.PI / freqeunce) * angle) + centre) : centre);
            scale.z = startScale.z + ((!fixedZ) ? (amplitude * Mathf.Cos((2 * Mathf.PI / freqeunce) * angle) + centre) : centre);

            transform.localScale = scale;
        }

        if (sineTransformAction == SineTransform.Position)
        {
            pos.x = localStartPos.x + ((!fixedX) ? (amplitude * Mathf.Sin((2 * Mathf.PI / freqeunce) * angle) + centre) : centre);
            pos.y = localStartPos.y + ((!fixedY) ? (amplitude * Mathf.Cos((2 * Mathf.PI / freqeunce) * angle) + centre) : centre);
            pos.z = localStartPos.z + ((!fixedZ) ? (amplitude * Mathf.Cos((2 * Mathf.PI / freqeunce) * angle) + centre) : centre);

            transform.localPosition = pos;
        }
    }

    enum SineTransform
    {
        Scale,
        Position
    }
}
