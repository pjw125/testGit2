using UnityEngine;

namespace UIControl
{
    public class PopupYesOrNo : PopupParent
    {
        #region Popup Result
        /// <summary>
        /// 팝업 결과 전달 이벤트 델리게이트
        /// </summary>
        private DelegateBool delegate_Result = null;
        public DelegateBool Event_Result { set { delegate_Result = value; } }
        #endregion

        /// <summary>
        /// Yes 선택 시 실행 함수
        /// </summary>
        private void ExecuteYes()
        {
            if (delegate_Result != null)
                // 델리게이트를 통해 true 전달
                delegate_Result(true);

            ClosePopup(); // 팝업 종료
        }

        private void ExecuteNo()
        {
            if (delegate_Result != null)
                // 델리게이트를 통해 false 전달
                delegate_Result(false);

            ClosePopup(); // 팝업 종료
        }
    }
}