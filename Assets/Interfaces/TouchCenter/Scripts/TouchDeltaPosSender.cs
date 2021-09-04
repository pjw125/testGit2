using System.Collections;
using UnityEngine;

namespace SongDuTouchSpace
{
    /// <summary>
    /// 단순히 현재 입력되어 있는 터치가 유니티 오브젝트상 얼마나 움직였는지 수치를 반환하는 함수
    /// </summary>
    public class TouchDeltaPosSender : TouchParent
    {
        private DelegateVector2 delegate_SendDelta = null;
        public DelegateVector2 DelegateSendDelta { set { delegate_SendDelta = value; } }

        private void Awake()
        {
            InitTouchParent(StartTouch, EndTouch);
        }

        private void StartTouch()
        {
            if (touchCount == 1)
            {
                SetTouchInformation();

                // 위 SetTouchInformation에서 GetPosition을 돌면서 터치가 바로 사라진 경우 touchCount가 0으로 변경될 수 있음
                // 그렇기 때문에 현 위치에서 touchCount가 0이라면 아래 내용을 수행하지 않음
                if (touchCount > 0)
                    StartTouchCoroutine();
            }
        }
        private void EndTouch()
        {
            if (touchCount <= 0)
            {
                StopTouchCoroutine();
            }
        }

        #region Coroutine
        private IEnumerator touchRoutine = null;
        private IEnumerator coroutine_touch()
        {
            while (touchCount > 0)
            {
                yield return null;

                if (Input.touchCount == 0 && !(Input.GetMouseButton(0)))
                {
                    this.ClearTouch();
                    EndTouch();
                    break;
                }

                if (touchCount > 0)
                    CheckPosition();
            }
        }

        private void StartTouchCoroutine()
        {
            StopTouchCoroutine();

            touchRoutine = coroutine_touch();

            StartCoroutine(touchRoutine);
        }

        private void StopTouchCoroutine()
        {
            if (touchRoutine != null)
            {
                StopCoroutine(touchRoutine);
                touchRoutine = null;
            }
        }
        #endregion

        #region Control Touch
        private Vector2 prevPos_Touch; // 이전 터치 위치

        private void SetTouchInformation()
        {
            prevPos_Touch = TouchPositionToUnityPosition(GetTouchPosition(0));
        }

        private void CheckPosition()
        {
            // 터치 포지션 가져오고
            Vector2 position = GetTouchPosition(0);

            if (position.x < 0)
                return;

            // 터치포지션을 유니티 포지션으로 변경
            position = TouchPositionToUnityPosition(position);

            // 위 포지션값과 초기 터치 유니티포지션을 가지고 된 위치를 가져옴
            Vector2 movedPos = position - prevPos_Touch;
            prevPos_Touch = position;

            if (delegate_SendDelta != null)
                delegate_SendDelta(movedPos);
        }
        #endregion
    }
}