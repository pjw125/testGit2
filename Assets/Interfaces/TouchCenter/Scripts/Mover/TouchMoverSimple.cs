using UnityEngine;

namespace SongDuTouchSpace
{
    public class TouchMoverSimple : TouchMoverParent
    {
        #region Override
        //public override void InitializeThis() { base.InitializeThis(); }

        protected override void StartTouch()
        {
            if (touchCount.Equals(1)) // ���� ��ġ �Է��� ���
            {
                SetTouchInfomation(); // �ʱ� ��ġ ���� ����

                // �� SetTouchInformation���� GetPosition�� ���鼭 ��ġ�� �ٷ� ����� ��� touchCount�� 0���� ����� �� ����
                // �׷��� ������ �� ��ġ���� touchCount�� 0�̶�� �Ʒ� ������ �������� ����
                if (touchCount > 0)
                {
                    StartTouchCoroutine(); // ��ġ �ڷ�ƾ ����

                    // ��ġ ���� �̺�Ʈ ��������Ʈ ����
                    if (delegate_StartTouch != null)
                        delegate_StartTouch();
                }
            }
        }

        protected override void EndTouch()
        {
            if (touchCount <= 0) // ��ġ�Է��� ������ ����� ���
            {
                StopTouchCoroutine(); // ��ġ �ڷ�ƾ ����

                // ��ġ ���� �̺�Ʈ ��������Ʈ ����
                if (delegate_EndTouch != null)
                    delegate_EndTouch();
            }
        }

        #region Override - Control Move
        //protected override void InitTouchInformation() { base.InitTouchInformation(); }
        //protected override void SetTouchInfomation() { base.SetTouchInfomation(); }
        protected override void MoveObject()
        {
            // ��ġ ������ ��������
            Vector2 position = GetTouchPosition(0);

            if (position.x < 0) // x ��ġ���� 0���� ������� ��ġ���� �������� ����
                return; // ����

            // ��ġ�������� ����Ƽ ���������� ����
            position = TouchPositionToUnityPosition(position);

            // �� �����ǰ��� �ʱ� ��ġ ����Ƽ�������� ������ �� ��ġ�� ������
            Vector2 movedPos = position - initPos_Touch;

            // ��ġ ����
            posStorage.x = initPos_Object.x + movedPos.x;
            posStorage.y = initPos_Object.y + movedPos.y;
            target.localPosition = posStorage;
        }
        #endregion

        #region Override - Coroutine
        //protected override IEnumerator coroutine_touch() { return base.coroutine_touch(); }
        #endregion

        #endregion
        
        // ��ġ ���� �̺�Ʈ ��������Ʈ
        private DelegateVoid delegate_StartTouch = null;
        public DelegateVoid DelegateStartTouch { set { this.delegate_StartTouch = value; } }

        // ��ġ ���� �̺�Ʈ ��������Ʈ
        private DelegateVoid delegate_EndTouch = null;
        public DelegateVoid DelegateEndTouch { set { this.delegate_EndTouch = value; } }

        /// <summary>
        /// ���� ������Ʈ ��ġ��ġ(�հ���/���콺)�� ��ȯ
        /// </summary>
        /// <returns>��ġ ��ġ</returns>
        public Vector2 GetTouchPosition()
        {
            return GetTouchPosition(0);
        }
    }
}