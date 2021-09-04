using System.Collections;
using UnityEngine;

namespace SongDuTouchSpace
{
    /// <summary>
    /// �ܼ��� ���� �ԷµǾ� �ִ� ��ġ�� ����Ƽ ������Ʈ�� �󸶳� ���������� ��ġ�� ��ȯ�ϴ� �Լ�
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

                // �� SetTouchInformation���� GetPosition�� ���鼭 ��ġ�� �ٷ� ����� ��� touchCount�� 0���� ����� �� ����
                // �׷��� ������ �� ��ġ���� touchCount�� 0�̶�� �Ʒ� ������ �������� ����
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
        private Vector2 prevPos_Touch; // ���� ��ġ ��ġ

        private void SetTouchInformation()
        {
            prevPos_Touch = TouchPositionToUnityPosition(GetTouchPosition(0));
        }

        private void CheckPosition()
        {
            // ��ġ ������ ��������
            Vector2 position = GetTouchPosition(0);

            if (position.x < 0)
                return;

            // ��ġ�������� ����Ƽ ���������� ����
            position = TouchPositionToUnityPosition(position);

            // �� �����ǰ��� �ʱ� ��ġ ����Ƽ�������� ������ �� ��ġ�� ������
            Vector2 movedPos = position - prevPos_Touch;
            prevPos_Touch = position;

            if (delegate_SendDelta != null)
                delegate_SendDelta(movedPos);
        }
        #endregion
    }
}