using System.Collections;
using UnityEngine;

namespace SongDuTouchSpace
{
    /// <summary>
    /// ��ġ�� ���� ������Ʈ �������� �����ϴ� �θ� Ŭ����
    /// </summary>
    public abstract class TouchMoverParent : TouchParent
    {
        #region Move target
        [SerializeField] protected Transform target; // Ÿ�� ������Ʈ
        /// <summary>
        /// x, y ���� �޾� ���������Ǹ� �����ϴ� �Լ�
        /// </summary>
        public void SetTargetLocalPosition(float x, float y)
        {
            posStorage.x = x;
            posStorage.y = y;
            target.localPosition = posStorage;
        }
        #endregion

        /// <summary>
        /// �ʱ� ���� �Լ�
        /// </summary>
        public virtual void InitializeThis()
        {
            // �θ� �ʱ� ���� (��ġ�Է�/��ġ���� �Լ� ����)
            this.InitTouchParent(StartTouch, EndTouch);
            // ��ġ ���� �ʱ� ����
            this.InitTouchInformation();
        }
        /// <summary>
        /// ��ġ �Է� �Լ�
        /// </summary>
        protected virtual void StartTouch()
        {
            if (touchCount.Equals(1)) // ���� ��ġ �Է��� ���
            {
                SetTouchInfomation(); // �ʱ� ��ġ ���� ����

                // �� SetTouchInformation���� GetPosition�� ���鼭 ��ġ�� �ٷ� ����� ��� touchCount�� 0���� ����� �� ����
                // �׷��� ������ �� ��ġ���� touchCount�� 0�̶�� �Ʒ� ������ �������� ����
                if (touchCount > 0)
                    StartTouchCoroutine(); // ��ġ �ڷ�ƾ ����
            }
        }
        /// <summary>
        /// ��ġ ���� �Լ�
        /// </summary>
        protected virtual void EndTouch()
        {
            if (touchCount <= 0) // ��ġ�Է��� ������ ����� ���
            {
                StopTouchCoroutine(); // ��ġ �ڷ�ƾ ����
            }
        }

        /// <summary>
        /// �ǵ�� ���� ������ ��ġ�� �����ϴ� �Լ�
        /// * ���� �θ�Ŭ������ �ǵ�鿡 ���� ������ ������ �ڽ�Ŭ���� ���� �� �ǵ�鿡 ���� ������ 'EndTouch'���� �����ϸ� ��
        /// </summary>
        protected void EndTouchSilence()
        {
            ClearTouch(); // ��ġ ���� Ŭ����
            StopTouchCoroutine(); // �ڷ�ƾ ����
        }

        #region Control Move
        protected Vector2 initPos_Touch; // �ʱ� ��ġ ��ġ(����Ƽ ������ ����)
        protected Vector3 initPos_Object; // �ʱ� ������Ʈ ��ġ
        protected Vector3 posStorage; // ��ġ �����
        /// <summary>
        /// ��ġ ���� �ʱ� ���� �Լ�
        /// </summary>
        protected virtual void InitTouchInformation()
        {
            // ���� Ÿ���� ��ġ�� ��ġ ����ҿ� ����
            posStorage = target.localPosition;
        }
        /// <summary>
        /// ���� ��ġ �� ��ġ ���� ����
        /// </summary>
        protected virtual void SetTouchInfomation()
        {
            // �ʱ� ��ġ ��ġ ����
            initPos_Touch = TouchPositionToUnityPosition(GetTouchPosition(0));
            // �ʱ� ������Ʈ ��ġ ����
            initPos_Object = target.localPosition;
        }
        /// <summary>
        /// �̵� �Լ� 
        /// </summary>
#if true
        protected abstract void MoveObject();
#else
        protected virtual void MoveObject()
        {
            // ��ġ ������ ��������
            Vector2 position = GetTouchPosition(0);

            if (position.x < 0) // x ��ġ���� 0���� ������� ��ġ���� �������� ����
                return; // ����

            // ��ġ�������� ����Ƽ ���������� ����
            position = TouchPositionToUnityPosition(position);

            // �� �����ǰ��� �ʱ� ��ġ ����Ƽ�������� ������ �� ��ġ�� ������
            Vector2 movedPos = position - initPos_Touch;

            posStorage.x = initPos_Object.x + movedPos.x;
            posStorage.y = initPos_Object.y + movedPos.y;
            posStorage.z = target.localPosition.z;
            target.localPosition = posStorage;
        }
#endif
        #endregion

        #region Coroutine
        private IEnumerator touchRoutine = null;
        /// <summary>
        /// ��ġ �ڷ�ƾ
        /// </summary>
        protected virtual IEnumerator coroutine_touch()
        {
            while (touchCount > 0) // ��ġ�� �ִ� ��� �ݺ�
            {
                yield return null;

                if (Input.touchCount == 0 && !(Input.GetMouseButton(0))) // ��ġ�� ���콺�Է��� ���� ���
                {
                    // ��ġ ���� �� �ݺ��� Ż��
                    this.ClearTouch();
                    EndTouch();
                    break;
                }

                if (touchCount > 0) // ��ġ �Է��� �ִ� ���
                {
                    this.MoveObject(); // �̵� �Լ� ����
                }
            }
        }
        /// <summary>
        /// ��ġ �ڷ�ƾ ���� �Լ�
        /// </summary>
        protected void StartTouchCoroutine()
        {
            StopTouchCoroutine();

            touchRoutine = coroutine_touch();

            StartCoroutine(touchRoutine);
        }
        /// <summary>
        /// ��ġ �ڷ�ƾ ���� �Լ�
        /// </summary>
        protected void StopTouchCoroutine()
        {
            if (touchRoutine != null)
            {
                StopCoroutine(touchRoutine);
                touchRoutine = null;
            }
        }
        #endregion
    }
}