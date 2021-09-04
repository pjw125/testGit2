using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongDuTouchSpace
{
    public class MouseRightTarget : MonoBehaviour
    {
        [SerializeField] private GameObject eventTarget;
        [SerializeField] private string functionName;

        public void ActionMouseDown()
        {
            if (eventTarget != null && !functionName.Equals(string.Empty))
                eventTarget.SendMessage(functionName);
        }
    }
}