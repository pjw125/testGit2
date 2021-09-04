using UnityEngine;

namespace SongDuTouchSpace
{
    public class TouchDropTarget : MonoBehaviour
    {
        // ��Ŀ�� ����
        [SerializeField] private bool isFocused = false;

        /// <summary>
        /// ��Ŀ�� ���� ���� �Լ�
        /// </summary>
        /// <param name="isFocus">��Ŀ�� ����</param>
        public virtual void SetFocus(bool isFocus)
        {
            this.isFocused = isFocus;
        }

        #region Drop Action
        private DelegateInt delegate_ActionDropIndex = null;
        public DelegateInt DelegateActionDropIndex { set { this.delegate_ActionDropIndex = value; } }

        /// <summary>
        /// ��� �������� ����� ��� ����� �����ۿ� ������ �ε����� ���޹޴� �Լ�
        /// </summary>
        /// <param name="idx"></param>
        public void ActionDropIndex(int idx)
        {
            if (delegate_ActionDropIndex != null)
                delegate_ActionDropIndex(idx);
        }
        #endregion
    }
}