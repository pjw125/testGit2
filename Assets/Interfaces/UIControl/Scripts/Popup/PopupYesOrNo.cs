using UnityEngine;

namespace UIControl
{
    public class PopupYesOrNo : PopupParent
    {
        #region Popup Result
        /// <summary>
        /// �˾� ��� ���� �̺�Ʈ ��������Ʈ
        /// </summary>
        private DelegateBool delegate_Result = null;
        public DelegateBool Event_Result { set { delegate_Result = value; } }
        #endregion

        /// <summary>
        /// Yes ���� �� ���� �Լ�
        /// </summary>
        private void ExecuteYes()
        {
            if (delegate_Result != null)
                // ��������Ʈ�� ���� true ����
                delegate_Result(true);

            ClosePopup(); // �˾� ����
        }

        private void ExecuteNo()
        {
            if (delegate_Result != null)
                // ��������Ʈ�� ���� false ����
                delegate_Result(false);

            ClosePopup(); // �˾� ����
        }
    }
}