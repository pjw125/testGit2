using UnityEngine;

namespace UIControl
{
    /// <summary>
    /// 체크박스 관련 부모 클래스
    /// 체크박스(CheckBoxItem)와 라디오버튼(RadioItem)에 상속된다
    /// </summary>
    public abstract class CheckBoxParent : MonoBehaviour
    {
        [SerializeField] private bool isChecked = false;
        public bool IsChecked { get { return isChecked; } }

        /// Change Check State
        /// 체크 상태를 변화 관련
        #region Change Check State
        /// <summary>
        /// 체크 상태를 토글하는 함수
        /// </summary>
        public void ToggleCheckState()
        {
            SetCheckState(!IsChecked);
        }
        /// <summary>
        /// 체크 상태를 설정하는 함수
        /// </summary>
        /// <param name="isCheck">체크상태</param>
        protected virtual void SetCheckState(bool isCheck)
        {
            isChecked = isCheck;
        }
        #endregion
    }
}