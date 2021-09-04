using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIControl
{
    /// <summary>
    /// �ִϸ��̼� �����ϸ� �˾� Ȱ��ȭ ���¸� �����ϱ� ���� �θ� Ŭ����
    /// </summary>
    public abstract class AnimatePopupParent : PopupParent
    {
        #region Override
        public override void OpenPopup()
        {
            this.ActivePopup(true, false);
        }
        public override void ClosePopup()
        {
            this.ActivePopup(false, false);
        }
        #endregion

        private void OnDisable()
        {
            iTween.Stop(gameObjectCache); // ���� �� �ִϸ��̼� ����

            ActivePopup(false, true); // ��� �˾� ��Ȱ��ȭ
        }

        /// <summary>
        /// �ʱ� ���� �Լ�
        /// </summary>
        public abstract void InitializeThis();

        #region Animation
        [Header("Animation")]
        [SerializeField] private float animateTime; // �ִϸ��̼� �ð�
        [SerializeField] private iTween.EaseType easeType; // �ִϸ��̼� Ÿ��
        protected float animateRatio = 0; // �ִϸ��̼� ����
        protected bool isActivePopup = false; // Ȱ��ȭ ����

        /// <summary>
        /// �˾� Ȱ��ȭ �ִϸ��̼� ���� �Լ�
        /// </summary>
        /// <param name="active">Ȱ��ȭ ����</param>
        /// <param name="immediately">��� ���� ���� true : ���, false : �ִϸ��̼�</param>
        private void ActivePopup(bool active, bool immediately)
        {
            if (isActivePopup.Equals(active)) // Ȱ��ȭ ���°� �����ϴٸ� ���� 
                return;

            iTween.Stop(gameObjectCache); // ���� �� �ִϸ��̼� ����

            isActivePopup = active; // �˾� Ȱ��ȭ ���� ����

            float _to = 0f; // ��� ������ ����(�⺻ ��Ȱ��ȭ��ġ)
            if (isActivePopup) // Ȱ��ȭ������ ���
            {
                base.OpenPopup(); // �켱 �˾� ������Ʈ Ȱ��ȭ
                _to = 1f; // ��� �������� Ȱ��ȭ ��ġ�� ����
            }

            if (gameObjectCache.activeInHierarchy && !(immediately)) // ������Ʈ�� �����ְų� ��û��°� �ƴ� ��� �ִϸ��̼�
            {
                Singleton_Settings.iTweenControl(gameObjectCache, animateRatio, _to, animateTime, easeType, "SetAnimateRatio", "CompleteAnimateRatio");
            }
            else // �ƴ϶�� ���
            {
                SetAnimateRatio(_to);
                CompleteAnimateRatio();
            }
        }
        /// <summary>
        /// �ִϸ��̼� ���� ���� �Լ�
        /// </summary>
        /// <param name="ratio"></param>
        protected virtual void SetAnimateRatio(float ratio)
        {
            animateRatio = ratio;
        }
        /// <summary>
        /// �ִϸ��̼� ������ �Լ�
        /// </summary>
        protected virtual void CompleteAnimateRatio()
        {
            if (!isActivePopup)
                base.ClosePopup();
        }
        #endregion
    }
}