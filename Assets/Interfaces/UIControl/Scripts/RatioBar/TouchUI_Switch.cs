using UnityEngine;

namespace UIControl
{
    public class TouchUI_Switch : MonoBehaviour
    {
        #region ObjectCaches
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
        #endregion

        // bar ��Ʈ��, �ִϸ��̼��� ���� ����� ������ �����ϴ� ��������Ʈ
        private DelegateFloat delegate_ChangeRatio = null;
        public DelegateFloat DelegateChangeRatio { set { this.delegate_ChangeRatio = value; } }

        [SerializeField] private TouchUI_RatioBar bar; // ���� Bar

        // ����ġ Ȱ��ȭ ����
        // bar �� ���� 1�� Ȱ��ȭ, 0�� ��Ȱ��ȭ
        // bar ��ġ �������� Ȱ��ȭ ����
        private bool isActive;

        /// <summary>
        /// �ʱ� ���� �Լ�
        /// </summary>
        public void InitControl()
        {
            // bar �ʱ� ����
            bar.InitControl();
            // bar �̺�Ʈ ��������Ʈ ����
            bar.SetEventDelegates(Bar_ChangeRatio, Bar_EndControl);
        }
        /// <summary>
        /// Bar ������ ����� �� �ش� ������ ���޹޴� �̺�Ʈ�Լ�
        /// </summary>
        /// <param name="ratio">����</param>
        private void Bar_ChangeRatio(float ratio)
        {
            // ���� ���� ��ġ�� ���� Ȱ��ȭ ���� ����
            bool isActive = ratio > 0.5f;
            this.isActive = isActive;

            // ���� ���濡 ���� ��������Ʈ�� ���� ����� ���� ����
            if (delegate_ChangeRatio != null)
                delegate_ChangeRatio(ratio);
        }
        /// <summary>
        /// Bar ������ �Ϸ�� �� �߻��ϴ� �̺�Ʈ �Լ�
        /// </summary>
        private void Bar_EndControl()
        {
            SetSwitchType();
        }

        #region Animation
        [SerializeField] private float animateTime;
        [SerializeField] private iTween.EaseType easeType;
        
        /// <summary>
        /// ���� �����Ǿ��ִ� Ȱ��ȭ ���¿� ���� Switch Ȱ��ȭ ���θ� �����ϴ� �Լ�
        /// </summary>
        private void SetSwitchType()
        {
            SetSwitchType(this.isActive, false);
        }
        /// <summary>
        /// Ȱ��ȭ ���¸� ����ϴ� �Լ�
        /// </summary>
        public void ToggleSwitchType()
        {
            SetSwitchType(!(this.isActive), false);
        }
        public void SetSwitchType(bool active, bool immediately)
        {
            // ���� �� �ִϸ��̼� ����
            iTween.Stop(gameObjectCache);

            // Ȱ��ȭ ���� ����
            this.isActive = active;

            // ��� ������ ����
            float _to = this.isActive ? 1f : 0f;

            if (bar.fRatio.Equals(_to))
                // ���� bar�� ������ ��� ������ ������ ��� ����
                return;

            if (immediately || !(gameObjectCache.activeInHierarchy))
                // ��û����̰ų� ������Ʈ�� �����ִ� ���
            {
                SetSwitchRatio(_to);
                CompleteSwitchRatio();
            }
            else
            // �׷��� ���� ���
            {
                Singleton_Settings.iTweenControl(gameObjectCache, bar.fRatio, _to
                    , animateTime, easeType, "SetSwitchRatio", "CompleteSwitchRatio");
            }
        }
        /// <summary>
        /// bar�� �������� �����ϴ� �Լ� (�ִϸ��̼� ���)
        /// </summary>
        /// <param name="ratio">����</param>
        private void SetSwitchRatio(float ratio)
        {
            bar.SetRatio(ratio);

            // ���� ���濡 ���� ��������Ʈ�� ���� ����� ���� ����
            if (delegate_ChangeRatio != null)
                delegate_ChangeRatio(ratio);
        }
        /// <summary>
        /// bar ���� ���� �ִϸ��̼� ���� �� �̺�Ʈ �Լ�
        /// </summary>
        private void CompleteSwitchRatio()
        {
            // �ش� ���¿� �´� ���������� ��� ����
            float _to = this.isActive ? 1f : 0f;
            SetSwitchRatio(_to);
        }
        #endregion
    }
}