using System.Collections;
using UnityEngine;
using SongDuTouchSpace;

namespace UIControl
{
    public class MouseOverFollow : MonoBehaviour
    {
        [SerializeField] private MouseOverTarget mouseOverTarget;
        [SerializeField] private MouseOverFollowItem followItem;

        private void Awake()
        {
            mouseOverTarget.Focused = Focus;
            mouseOverTarget.Unfocused = Unfocus;

            followItem.InitializeThis();
        }

        private void Focus()
        {
            StartFollowCoroutine();
        }

        private void Unfocus()
        {
            StopFollowCoroutine();
        }

        #region Follow Features
        private IEnumerator IE_Follow = null;

        private IEnumerator Coroutine_Follow()
        {
            Vector2 pos = Vector2.zero;

            float screenWidth = (float)Screen.width / (float)Screen.height;

            do
            {
                TouchCenter.TouchPositionToUnityPosition(Input.mousePosition, out pos.x, out pos.y);

                followItem.SetItemPosition(pos);

                yield return null;
            } while (true);
        }

        private void StartFollowCoroutine()
        {
            StopFollowCoroutine();

            IE_Follow = Coroutine_Follow();

            StartCoroutine(IE_Follow);

            followItem.SetActive(true);
        }
        private void StopFollowCoroutine()
        {
            if (IE_Follow != null)
            {
                StopCoroutine(IE_Follow);
                IE_Follow = null;
            }

            followItem.SetActive(false);
        }
        #endregion
    }
}