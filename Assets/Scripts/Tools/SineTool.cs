using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ru1t3rl.Tools
{
    public class SineTool : MonoBehaviour
    {
        [SerializeField] bool useZeroStartPos = false;
        [SerializeField] float freqeunce = 1, amplitude = 1;
        [SerializeField] Vector3 centre = Vector2.zero;
        [SerializeField] SineTransform sineTransformAction = SineTransform.Scale;
        [SerializeField] bool fixedX = false, fixedY = false, fixedZ = false;

        Vector3 startPos, localStartPos, startScale;
        Vector3 scale, pos;

        Vector3 localStartRot, rot;

        float angle;
        float angleIncrement = 0.01f;

        void Awake()
        {
            if (useZeroStartPos)
            {
                startPos = Vector3.zero;
                localStartPos = Vector3.zero;
                centre = Vector3.zero;
            }
            else
            {
                startPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                localStartPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
            }

            startScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            localStartRot = transform.localRotation.eulerAngles;
        }

        void FixedUpdate()
        {
            angle += angleIncrement;

            if (sineTransformAction == SineTransform.Scale)
            {
                scale.x = startScale.x + ((!fixedX) ? (amplitude * Mathf.Sin((2 * Mathf.PI / freqeunce) * angle) + centre.x) : centre.x);
                scale.y = startScale.y + ((!fixedY) ? (amplitude * Mathf.Cos((2 * Mathf.PI / freqeunce) * angle) + centre.y) : centre.y);
                scale.z = startScale.z + ((!fixedZ) ? (amplitude * Mathf.Cos((2 * Mathf.PI / freqeunce) * angle) + centre.z) : centre.z);

                transform.localScale = scale;
            }

            if (sineTransformAction == SineTransform.Position)
            {
                pos.x = localStartPos.x + ((!fixedX) ? (amplitude * Mathf.Sin((2 * Mathf.PI / freqeunce) * angle) + centre.x) : centre.x);
                pos.y = localStartPos.y + ((!fixedY) ? (amplitude * Mathf.Cos((2 * Mathf.PI / freqeunce) * angle) + centre.y) : centre.y);
                pos.z = localStartPos.z + ((!fixedZ) ? (amplitude * Mathf.Cos((2 * Mathf.PI / freqeunce) * angle) + centre.z) : centre.z);

                transform.localPosition = pos;
            }
        }

        enum SineTransform
        {
            Scale,
            Position
        }
    }
}