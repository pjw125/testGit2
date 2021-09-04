using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongDuTouchSpace
{
    public class TouchPaging : TouchMoverParent
    {
        #region ObjectCaches
        private GameObject _gameObjectCache = null;
        private GameObject gameObjectCache
        {
            get
            {
                if (this._gameObjectCache == null)
                    this._gameObjectCache = this.gameObject;

                return this._gameObjectCache;
            }
        }
        #endregion

        #region Override
        //public override void InitializeThis() { base.InitializeThis(); }
        protected override void StartTouch()
        {
            StopAnimation(); // �ִϸ��̼� ����

            if (touchCount.Equals(1)) // ���� ��ġ �Է��� ���
            {
                SetTouchInfomation(); // �ʱ� ��ġ ���� ����

                // �� SetTouchInformation���� GetPosition�� ���鼭 ��ġ�� �ٷ� ����� ��� touchCount�� 0���� ����� �� ����
                // �׷��� ������ �� ��ġ���� touchCount�� 0�̶�� �Ʒ� ������ �������� ����
                if (touchCount > 0)
                {
                    StartTouchCoroutine(); // ��ġ �ڷ�ƾ ����

                    // ��ġ ���ۿ� ���� �̺�Ʈ �Լ� ����
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

                AnimateCurrentPage(); // ������ �̵� �ִϸ��̼� ����
            }
        }

        #region Override - Control Move
        //protected override void InitTouchInformation() { base.InitTouchInformation(); }
        protected override void SetTouchInfomation()
        {
            base.SetTouchInfomation();

            previousTouchPos = initPos_Touch.x;
        }
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

            // x ��ġ�� ����
            posStorage.x = initPos_Object.x + movedPos.x;
            // ���Ѱ� ����
            if (isApplyLimitPos)
            {
                if (posStorage.x < limitPosMin)
                    posStorage.x = limitPosMin;
                else if (posStorage.x > limitPosMax)
                    posStorage.x = limitPosMax;
            }

            // �� ����
            target.localPosition = posStorage;

            // ���ӵ��� �߰� �� ���� ��ġ��ġ�� ���� ��ġ�� ����
            AddAccelValue(position.x - previousTouchPos);
            previousTouchPos = position.x;
        }
        #endregion

        #region Override - Coroutine
        //protected override IEnumerator coroutine_touch() { return base.coroutine_touch(); }
        #endregion

        #endregion

        #region Delegates
        // ���� ��ġ�� ���۵� ��� �߻��ϴ� �̺�Ʈ ��������Ʈ
        private DelegateVoid delegate_StartTouch = null;
        public DelegateVoid DelegateStartTouch { set { this.delegate_StartTouch = value; } }

        // ������ ������ �Ϸ�� ��� �߻��ϴ� �̺�Ʈ ��������Ʈ
        private DelegateVoid delegate_EndPaging = null;
        public DelegateVoid DelegateEndPaging { set { this.delegate_EndPaging = value; } }

        // ���� �ε��� ���� �� �߻��ϴ� �̺�Ʈ ��������Ʈ
        private DelegateVoid delegate_ChangeIndex = null;
        public DelegateVoid DelegateChangeIndex { set { this.delegate_ChangeIndex = value; } }
        #endregion

        #region Page
        private int pageCount = 0; // ��ü ������ ��
        private int selectedIdx = 0; // ���� ������ �ε���
        public int SelectedIdx { get { return this.selectedIdx; } }

        [SerializeField] private bool isApplyLimitPos = true; // ���� ��ġ�� ��� �� �ִ����� ���� ����
        private float limitPosMin = 0f; // �ּ� ��ġ��
        private const float limitPosMax = 0f; // ���������������� �ִ밪�� �׻� 0�̱� ������ ���ȭ
        private float pageGap = 0f; // �������� ����
        private float pageGapForMult = 0f; // �������� ���� ����ó����

        /// <summary>
        /// ����¡ ���� �ʱ� ���� �Լ�
        /// </summary>
        /// <param name="pageCount">�� ������ ��</param>
        /// <param name="pageGap">�������� ����</param>
        public void InitializeThis(int pageCount, float pageGap)
        {
            MaxTouchCount = 1; // ����¡�� 1��ġ�� ����

            this.pageCount = pageCount; // �� ������ �� ����
            limitPosMin = -pageGap * (float)(this.pageCount - 1); // �� ������ ���� �������� ���ݿ� ���� �ּҰ� ����

            // ������ ���� ���� �� ����
            this.pageGap = pageGap;
            this.pageGapForMult = 1f / this.pageGap;

            this.InitializeThis();
        }

        #endregion

        /// *
        /// ������ �̵� �ִϸ��̼� ����
        #region Animation
        private const float animateTime = 0.3f; // �ִϸ��̼� �ð�
        private const iTween.EaseType easeType = iTween.EaseType.easeOutCirc; // �ִϸ��̼� Ÿ��
        /// <summary>
        /// ��ġ�� �Ϸ�� �� ���� ���õ� �ε����� �������� �̵��ϴ� �ִϸ��̼� ���� �Լ�
        /// </summary>
        private void AnimateCurrentPage()
        {
            // ���� Ÿ�� ������Ʈ ��ġ(target.localPosition.x)�� ���� ������ �ε��� ���
            // Ÿ�� ������Ʈ�� �����̱� ������ ��� �� '-'�� �ٿ���
            int currentIdx = (int)Mathf.Round(AmendData.RoundFloat(-target.localPosition.x, this.pageGap) * pageGapForMult);

            if (this.selectedIdx.Equals(currentIdx)) // Ÿ�� ��ġ�� ���� �ε����� ���� ���� �ε����� ������ ���
                // Accel ���� �ݿ��Ͽ� ������ �ε����� ����
                currentIdx = AmendIndexByAcceleration(currentIdx);

            // �ε��� ����
            SetSelectedPageIndex(currentIdx, false);
        }
        /// <summary>
        /// ���� ������ �ε����� �����ϴ� �Լ� (�ִϸ��̼�)
        /// </summary>
        /// <param name="idx">�ε���</param>
        /// <param name="immediately">��ÿ���</param>
        public void SetSelectedPageIndex(int idx, bool immediately)
        {
            EndTouchSilence(); // �ǵ�� ���� ��ġ ����

            StopAnimation(); // ���� �������̴� �ִϸ��̼� ����

            // ���޹��� �ε����� ���Ѱ� �����ϸ� ����
            if (idx < 0)
                this.selectedIdx = 0;
            else if (idx >= pageCount)
                this.selectedIdx = pageCount - 1;
            else
                this.selectedIdx = idx;

            // ���� �ε��� ���� ��������Ʈ ����
            if (delegate_ChangeIndex != null)
                delegate_ChangeIndex();

            float _to = (float)selectedIdx * -pageGap; // ��� ��ġ ���

            if (immediately || !(gameObjectCache.activeInHierarchy)) // ��ø���̰ų� ���ӿ�����Ʈ�� �����ִٸ� ��� �̵�
            {
                SetTargetPos(_to);
                CompleteTargetPos();
            }
            else // �׷��� �ʴٸ� �ִϸ��̼� �̵�
            {
                Singleton_Settings.iTweenControl(gameObjectCache, target.localPosition.x, _to
                    , animateTime, easeType, "SetTargetPos", "CompleteTargetPos");
            }
        }
        /// <summary>
        /// ���� �������� �ֹ̳��̼��� �����ϴ� �Լ�
        /// </summary>
        private void StopAnimation()
        {
            iTween.Stop(gameObjectCache);
        }
        /// <summary>
        /// Ÿ���� ��ġ�� �����ϴ� �Լ�(�ִϸ��̼�)
        /// </summary>
        /// <param name="x"></param>
        private void SetTargetPos(float x)
        {
            posStorage.x = x;

            target.localPosition = posStorage;
        }
        /// <summary>
        /// Ÿ�� ��ġ ���� �ִϸ��̼� ���� �� �߻� �̺�Ʈ �Լ�
        /// </summary>
        private void CompleteTargetPos()
        {
            // ����¡ ���� ��������Ʈ ����
            if (delegate_EndPaging != null)
                delegate_EndPaging();
        }
        #endregion

        /// *
        /// ��ġ �̵� ���ӵ� ����
        /// ��ġ �̵��� ���� ���ӵ��� �Ǵ��Ͽ� ����/���� �������� �̵��� �� �յ��� �ϴ� �κ�
        #region Acceleration
        private float[] accelValues = new float[2] { 0f, 0f }; // �ֱ� 2 �����ӵ����� ��ġ �ӵ���
        private bool bToggleAccel = false; // ���ӵ� �� �߰� �� �ε��� ��� ����
        private float previousTouchPos = float.MinValue; // ��ġ ��ġ�� ���� ���� ��ġ ��ġ��, ���ӵ� ������ ���� delta�� ����� ����
        /// <summary>
        /// ���ӵ��� �߰� �Լ�
        /// </summary>
        /// <param name="val"></param>
        private void AddAccelValue(float val)
        {
            if (bToggleAccel) // ��ۺ����� true�� ��� 1�� �ε����� �� ����
                accelValues[1] = val;
            else // false�� ��� 0�� �ε����� �� ����
                accelValues[0] = val;

            bToggleAccel = !bToggleAccel; // ��ۺ��� ����
        }
        /// <summary>
        /// ���ӵ��迭 �ʱ�ȭ �Լ�
        /// </summary>
        private void ClearAccelValues()
        {
            // ��ü ���ӵ��� 0���� ����
            for (int i = 0; i < accelValues.Length; i++)
            {
                accelValues[i] = 0f;
            }

            bToggleAccel = false; // ��ۺ��� false
        }
        /// <summary>
        /// ���ӵ����� �ݿ��Ͽ� ������ ��� �ε��� ��ȯ �Լ�
        /// </summary>
        private int AmendIndexByAcceleration(int idx)
        {
            // ���ӵ��� 2�� �� ���밪 ���� �� ū ���� �ݿ� ���ӵ������� ����
            float accelValue = (Mathf.Abs(accelValues[0]) > Mathf.Abs(accelValues[1])) ? accelValues[0] : accelValues[1];

            if (accelValue > 0.01f) // ���ӵ����� 0.01���� ū ��� ���� ������ �ε����� ����
                idx--;
            else if (accelValue < -0.01f) // ���ӵ����� 0.01���� ū ��� ���� ������ �ε����� ����
                idx++;

            ClearAccelValues(); // ���ӵ� �迭 �ʱ�ȭ

            return idx;
        }
        #endregion
    }
}