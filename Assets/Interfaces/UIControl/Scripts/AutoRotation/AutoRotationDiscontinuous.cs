using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIControl
{
    public class AutoRotationDiscontinuous : AutoRotationParent
    {
        [SerializeField] private float delaySeconds = 0f;
        [SerializeField] private float angleStep = 45f;

        protected override IEnumerator Coroutine_Rotate()
        {
            Vector3 angleStorage = rotateTarget.localEulerAngles;
            WaitForSeconds wait = new WaitForSeconds(delaySeconds);

            do
            {
                yield return wait;

                angleStorage.z += angleStep;

                rotateTarget.localEulerAngles = angleStorage;

            } while (true);
        }
    }
}