using System.Collections;
using UnityEngine;

namespace SongDuTouchSpace
{
    public class TouchMultipleWheelScaleWithInfos : TouchMultipleWheelScale
    {
        #region Override
        public override void AddTouch(int fingerID)
        {
            // ��ġ �Է� ��ġ ��ȯ �̺�Ʈ ����
            AddedTouchPos(fingerID);

            // �������� �ִϸ��̼� ����
            StopAnimatePosition();
            
            base.AddTouch(fingerID);
        }

        public override bool RemoveTouch(int fingerID)
        {
            // ��ġ ���� ��ġ ��ȯ �̺�Ʈ ����
            RemovedTouchPos(fingerID);

            return base.RemoveTouch(fingerID);
        }

        protected override IEnumerator Coroutine_Touch()
        {
            // ��ġ�� �ִ� ��� �ݺ�
            while (touchCount > 0)
            {
                yield return null;

                // �ý��ۻ� ��ġ�� ���� ���콺 ��Ŭ�� ���°� �ƴ� ���
                if (Input.touchCount.Equals(0) && !(Input.GetMouseButton(0)))
                {
                    for (int i = 0; i < this.touchIDs.Count; i++)
                    {
                        RemovedTouchPos(this.touchIDs[i]);
                    }

                    // ��ġ ���� Ŭ���� �� ����
                    this.ClearTouch();
                    EndTouch();
                    continue;
                }

                if (bInitMultiTouch) // ��ġ �Է� �ʱ� ������ �� ���¶��
                    MultiTouch(); // ��ġ�� ���� ������Ʈ ���� �Լ� ����
                else // �ʱ⼳���� ���� ���� ���¶��
                    SetTouchInfomation(); // �ʱ� ���� ����
            }
        }

        protected override void MultiTouch()
        {
            Vector2[] positions = new Vector2[touchCount];
            if (!GetTouchPositions(positions, ref this.touchCenterPos)) // ��ġ ��ġ�� �������� �� ������ �߻��Ͽ��ٸ� ����
                return;

            // Scale
            if (touchCount > 1 && bScale) // ��ġ�� 1�� �̻��̰� ũ�������� �����ִ� ��� ũ�� ����
                ChangeScale(touchCenterPos, positions);

            // Position
            if (touchCount > 0)
            {
                // ��ġ�� ���� ��Ÿ�� ���, ����Ƽ��ġ��(* ratio)
                Vector3 deltaPos = (touchCenterPos - touchCenterPosPrev) * ratio;
                // ��Ÿ�� �߰� �� ��ġ����
                this.moveTarget.Translate(deltaPos.x, deltaPos.y, 0f);
                // ���� ��ġ�� ������ġ�� �̰�
                touchCenterPosPrev = touchCenterPos;
                // delta position �߻� �̺�Ʈ ����
                OccurDeltaPos(deltaPos);
            }
        }
        #endregion

        /// 
        /// ��ġ �Է� ��ġ ��ȯ �̺�Ʈ
        #region Event Add
        // �ϳ��� ��ġ �Է� �� �ش� ��ġ ��ġ ��ȯ �̺�Ʈ ��������Ʈ
        private DelegateVector2 delegate_AddedTouchPos = null;
        public DelegateVector2 DelegateAddedTouchPos { set { this.delegate_AddedTouchPos = value; } }

        /// <summary>
        /// ��ġ ���� �̺�Ʈ �Լ�
        /// </summary>
        /// <param name="fingerID"></param>
        private void AddedTouchPos(int fingerID)
        {
            if (delegate_AddedTouchPos == null) // �ش� ��������Ʈ�� ���ٸ� ����
                return;

            Vector2 pos = FingerIDToTouchPosition(fingerID); // ���޹��� fingerID�� ��ġ Ȯ��
            if (!(pos.x < 0 || pos.y < 0f)) // �ش� ��ġ�� ������ ���ԵǾ����� �ʴٸ�
                delegate_AddedTouchPos(pos); // ��ġ ����
        }
        #endregion

        /// 
        /// ��ġ �Ϸ� ��ġ ��ȯ �̺�Ʈ
        #region Event Remove
        // �ϳ��� ��ġ �Ϸ� �� �ش� ��ġ ��ġ ��ȯ �̺�Ʈ ��������Ʈ
        private DelegateVector2 delegate_RemovedTouchPos = null;
        public DelegateVector2 DelegateRemovedTouchPos { set { this.delegate_RemovedTouchPos = value; } }

        /// <summary>
        /// ��ġ ���� �̺�Ʈ �Լ�
        /// </summary>
        /// <param name="fingerID">����� ��ġ ID</param>
        private void RemovedTouchPos(int fingerID)
        {
            if (delegate_RemovedTouchPos == null) // �ش� ��������Ʈ�� ���ٸ� ����
                return;

            Vector2 pos = FingerIDToTouchPosition(fingerID); // ���޹��� fingerID�� ��ġ Ȯ��
            if (!(pos.x < 0 || pos.y < 0f)) // �ش� ��ġ�� ������ ���ԵǾ����� �ʴٸ�
                delegate_RemovedTouchPos(pos); // ��ġ ����
        }
        #endregion

        ///
        /// ������Ʈ ��ġ ��ȭ �߻� �� delta position ��ȯ �̺�Ʈ
        #region Event Delta Position
        // �ش� ������Ʈ �̵��� ���� delta position�� �߻��� ��� �ش� delta position�� ��ȯ�ϴ� �̺�Ʈ ��������Ʈ
        private DelegateVector2 delegate_OccurDeltaPos = null;
        public DelegateVector2 DelegateOccurDeltaPos { set { this.delegate_OccurDeltaPos = value; } }

        /// <summary>
        /// delta position �߻� �̺�Ʈ �Լ�
        /// </summary>
        /// <param name="delta">delta position</param>
        private void OccurDeltaPos(Vector2 delta)
        {
            if (delegate_OccurDeltaPos != null) // �ش� ��������Ʈ�� �����ϴ� ���
                delegate_OccurDeltaPos(delta); // delta position ����
        }
        #endregion

        ///
        /// �ܺ� ������ ���� ��ġ �̵�(�ִϸ��̼�) ����
        /// ���� �θ� Ŭ������ �ʿ��� ��� �̵��ϸ� ��
        #region AnimatePosition (iTween)
        // iTween ������ ���� gameObjectCache
        private GameObject _gameObjectCache;
        private GameObject gameObjectCache
        {
            get
            {
                if (this._gameObjectCache == null)
                    this._gameObjectCache = this.gameObject;

                return this._gameObjectCache;
            }
        }

        private bool isAnimating = false;

        /// <summary>
        /// ��ġ�̵� �ִϸ��̼� ���� �Լ�
        /// </summary>
        private void StopAnimatePosition()
        {
            if (isAnimating)
            {
                iTween.Stop(gameObjectCache);
                isAnimating = false;
            }
        }
        /// <summary>
        /// ��ġ�̵� �ִϸ��̼� ���� �Լ�
        /// </summary>
        /// <param name="pos">Ÿ����ġ</param>
        /// <param name="animateTime">�ִϸ��̼� �ð�</param>
        /// <param name="easeType">�ִϸ��̼� Ÿ��</param>
        public void AnimatePosition(Vector3 pos, float animateTime, iTween.EaseType easeType)
        {
            this.ClearTouch(); // ��� ��ġ ����

            StopAnimatePosition(); // �������� �ִϸ��̼� ����

            pos.z = localPosition.z; // ���̰� ������ �����ϰ� ����

            if (animateTime <= 0f || !(gameObjectCache.activeInHierarchy))
                // �ִϸ��̼� �ð��� 0���� �۰ų� ������Ʈ�� �����ִ� ��� ��� �̵�
            {
                SetAnimatePosition(pos);
                CompleteAnimatePosition();
            }
            else
            // �׷��� ���� ��� �ִϸ��̼�
            {
                isAnimating = true;
                Singleton_Settings.iTweenControl(gameObjectCache, localPosition, pos
                    , animateTime, easeType, "SetAnimatePosition", "CompleteAnimatePosition");
            }
        }
        /// <summary>
        /// ��ġ�̵� �ִϸ��̼��� ���� ��ġ �̵� �Լ�
        /// </summary>
        /// <param name="pos">��ġ</param>
        private void SetAnimatePosition(Vector3 pos)
        {
            localPosition = pos;
        }
        /// <summary>
        /// ��ġ�̵� �ִϸ��̼� ���� �� �̺�Ʈ �Լ�
        /// </summary>
        private void CompleteAnimatePosition()
        {
            isAnimating = false;
        }
        #endregion

        #region Static
        /// <summary>
        /// fingerID�� �޾� �ش� ��ġ�� ��ġ�� ��ȯ�ϴ� �Լ�
        /// </summary>
        /// <param name="fingerID">fingerID</param>
        /// <returns>��ġ</returns>
        private static Vector2 FingerIDToTouchPosition(int fingerID)
        {
            if (fingerID.Equals(TouchCenter.MouseIndexLeft)) // fingerID�� ���콺 ��Ŭ���� ���
            {
                return Input.mousePosition; // ���콺 ��ġ ��ȯ
            }
            else // �ƴ� ���(��ġ)
            {
                for (int i = 0; i < Input.touches.Length; i++)
                {
                    if (Input.touches[i].fingerId.Equals(fingerID)) // �ش� �ε����� fingerID�� ���޹��� fingerID�� ������ ���
                        return Input.touches[i].position; // �ش� �ε����� ��ġ ��ġ ��ȯ
                }
            }

            return new Vector2(-1f, -1f); // ã�� ���� ��� ������ȯ
        }
        #endregion
    }
}