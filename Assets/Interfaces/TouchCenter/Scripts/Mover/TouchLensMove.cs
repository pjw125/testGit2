using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongDuTouchSpace
{
    public class TouchLensMove : TouchMoverParent
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
        public override void InitializeThis()
        {
            base.InitializeThis();

            // Ÿ���� �ʱ� ��ġ�� �⺻ ��ġ�� ����
            defaultPos = target.localPosition;
        }
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
                }
            }
        }
        //protected override void EndTouch() { base.EndTouch(); }

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
        
        private Vector2 defaultPos;// �⺻ ��ġ
        /// <summary>
        /// ������Ʈ�� Depth(z) ���� �Լ�
        /// </summary>
        /// <param name="z">depth��</param>
        public void SetDepth(float z)
        {
            posStorage.z = z;
            target.localPosition = posStorage;
        }

        #region AnimatePosition
        [Header("MoveToPosition(Animation)")]
        [SerializeField] private float animateTime = 0;
        [SerializeField] private iTween.EaseType easeType = iTween.EaseType.easeOutCirc;
        /// <summary>
        /// �������� �ִϸ��̼��� �����ϴ� �Լ�
        /// </summary>
        private void StopAnimation()
        {
            iTween.Stop(gameObjectCache);
        }
        /// <summary>
        /// �⺻ ��ġ�� �̵� �ִϸ��̼� ���� �Լ�
        /// </summary>
        /// <param name="immediately">��� ����</param>
        public void MoveToDefaultPos(bool immediately)
        {
            if (!(this.defaultPos.Equals(posStorage)))
                // �⺻��ġ�� ���� ��ġ�� �������� ���� ���
            {
                // �⺻��ġ�� �ֹ̳��̼� ����
                MoveToTargetPos(this.defaultPos, immediately);
            }
        }
        /// <summary>
        /// Ÿ�� ��ġ �̵� �ִϸ��̼� ���� �Լ�
        /// </summary>
        /// <param name="targetPos">Ÿ�� ��ġ</param>
        /// <param name="immediately">��� ����</param>
        public void MoveToTargetPos(Vector2 targetPos, bool immediately)
        {
            EndTouchSilence(); // ��ġ ����
            
            StopAnimation(); // �ִϸ��̼� ����

            if (immediately || !(gameObjectCache.activeInHierarchy))
                // ��û��°ų� ������Ʈ�� �����ִ� ���
            {
                SetTargetPositioning(targetPos);
                CompleteTargetPositioning();
            }
            else
            // �׷��� ���� ���
            {
                Singleton_Settings.iTweenControl(gameObjectCache, (Vector2)posStorage, targetPos
                    , animateTime, easeType, "SetTargetPositioning", "CompleteTargetPositioning");
            }
        }
        /// <summary>
        /// Ÿ�� ��ġ ���� �ִϸ��̼� �Լ�
        /// </summary>
        /// <param name="pos">��ġ</param>
        private void SetTargetPositioning(Vector2 pos)
        {
            posStorage.x = pos.x;
            posStorage.y = pos.y;

            target.localPosition = posStorage;
        }
        // Ÿ�� ��ġ ���� �ִϸ��̼� ���� �� �߻��ϴ� �̺�Ʈ �Լ�
        private void CompleteTargetPositioning()
        {

        }
        #endregion
    }
}