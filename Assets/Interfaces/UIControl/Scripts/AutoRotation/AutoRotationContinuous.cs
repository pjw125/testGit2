using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIControl
{
    public class AutoRotationContinuous : AutoRotationParent
    {
        [SerializeField] private float angleSens = 1;

        protected override IEnumerator Coroutine_Rotate()
        {
            Vector3 angleStorage = rotateTarget.localEulerAngles;

            do
            {
                yield return null;

                angleStorage.z += Time.deltaTime * angleSens;

                rotateTarget.localEulerAngles = angleStorage;

            } while (true);
        }
    }
}