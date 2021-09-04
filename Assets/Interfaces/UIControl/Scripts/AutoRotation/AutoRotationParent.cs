using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIControl
{
    public abstract class AutoRotationParent : MonoBehaviour
    {
        #region Unity Functions
        private void OnEnable()
        {
            StartRotateCoroutine(); // �����̼� ����
        }
        private void OnDisable()
        {
            StopRotateCoroutine(); // �����̼� ����
        }
        #endregion

        [SerializeField] protected Transform rotateTarget;

        protected IEnumerator IE_Rotate = null;

        protected abstract IEnumerator Coroutine_Rotate();

        private void StartRotateCoroutine()
        {
            if (IE_Rotate == null)
            {
                IE_Rotate = Coroutine_Rotate();
                StartCoroutine(IE_Rotate);
            }
        }
        private void StopRotateCoroutine()
        {
            if (IE_Rotate != null)
            {
                StopCoroutine(IE_Rotate);
                IE_Rotate = null;
            }
        }
    }
}