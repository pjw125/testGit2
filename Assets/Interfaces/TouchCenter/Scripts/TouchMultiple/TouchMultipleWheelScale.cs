using System.Collections;
using UnityEngine;

namespace SongDuTouchSpace
{
    public class TouchMultipleWheelScale : TouchMultiple
    {
        #region Override
        //public override void InitializeThis() { base.InitializeThis(); }
        public override void DestroyObject()
        {
            // �������̴� �� �ڷ�ƾ ����
            StopWheelCoroutine();

            base.DestroyObject();
        }
        protected override void StartTouch()
        {
            base.StartTouch();

            CheckWheelAction();
        }
        protected override void EndTouch()
        {
            base.EndTouch();

            CheckWheelAction();
        }

        //protected override void SetTouchInfomation() { base.SetTouchInfomation(); }

        //protected override void MultiTouch() { base.MultiTouch(); }

        protected override void SetScale(float scale)
        {
            // ũ�Ⱚ �ּ�/�ִ밪�� ���� ����
            if (scale < this.minimumScale)
                scale = this.minimumScale;
            else if (scale > this.maximumScale)
                scale = this.maximumScale;

            if (!(scale.Equals(scaleTarget.localScale.x)))
            // Ÿ���� ũ��� �������� �ϴ� ũ�Ⱑ �ٸ� ���
            {
                // ����ҿ� ũ�� ����
                scaleStorage.x = scale;
                scaleStorage.y = scale;
                // ũ�� ����
                scaleTarget.localScale = scaleStorage;

                if (delegate_ChangeScale != null)
                    // ũ�� ���� �̺�Ʈ ��������Ʈ ����
                    delegate_ChangeScale();
            }
        }
        #endregion

        // ũ�⿡ ������ �ִ� ��� ����Ǵ� �̺�Ʈ ��������Ʈ
        private DelegateVoid delegate_ChangeScale = null;
        public DelegateVoid DelegateChangeScale { set { this.delegate_ChangeScale = value; } }
        /// <summary>
        /// �ܺο��� ũ�⸦ �����ϵ��� �ϴ� �Ӽ�
        /// </summary>
        public float ObjectScale
        {
            set
            {
                // ũ�� ���� �Լ� ����
                this.SetScale(value);
            }
        }

        #region Wheel Action
        [Header("Wheel")]
        [SerializeField] private float wheelSens = 0f; // �� �ΰ���
        private IEnumerator IE_Wheel = null; // ���콺 �� ���濡 ���� ũ������ �ڷ�ƾ ����
        /// <summary>
        /// ���콺 �� ���濡 ���� ũ������ �ڷ�ƾ �Լ�
        /// </summary>
        private IEnumerator Coroutine_Wheel()
        {
            while (true)
            {
                yield return null;

                if (bInitMultiTouch) // ��ġ �Է¿� ���� �ʱ� ������ �Ϸ�� ���
                {
                    // �� �ΰ����� �ݿ��� ���콺 �� ��ũ�� ��Ÿ��
                    float wheelDelta = Input.GetAxis("Mouse ScrollWheel") * wheelSens;
                    // ���� ũ�� ���
                    float scale = scaleTarget.localScale.x + wheelDelta;
                    // ũ�� ����
                    SetScale(scale);
                }
            }
        }
        /// <summary>
        /// ���콺 �� ���濡 ���� ũ������ �ڷ�ƾ ���� �Լ�
        /// </summary>
        private void StartWheelCoroutine()
        {
            if (!bScale) // ũ�� ������ �����ִ� ��� ����
                return;

            StopWheelCoroutine();

            IE_Wheel = Coroutine_Wheel();
            StartCoroutine(IE_Wheel);
        }
        /// <summary>
        /// ���콺 �� ���濡 ���� ũ������ �ڷ�ƾ ���� �Լ�
        /// </summary>
        private void StopWheelCoroutine()
        {
            if (IE_Wheel != null)
            {
                StopCoroutine(IE_Wheel);
                IE_Wheel = null;
            }
        }
        /// <summary>
        /// ��ġ ���¸� üũ�Ͽ� �� �ڷ�ƾ�� ����/�����ϴ� �Լ�
        /// </summary>
        private void CheckWheelAction()
        {
            if (this.touchCount.Equals(1)) // ��ġ�� 1�� �ִ� ���
                StartWheelCoroutine(); // �� �ڷ�ƾ ����
            else // �׷��� ���� ���
                StopWheelCoroutine(); // ����
        }
        #endregion
    }
}