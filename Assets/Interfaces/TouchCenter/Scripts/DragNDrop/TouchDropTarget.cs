using UnityEngine;

namespace SongDuTouchSpace
{
    public class TouchDropTarget : MonoBehaviour
    {
        // 포커스 상태
        [SerializeField] private bool isFocused = false;

        /// <summary>
        /// 포커스 상태 설정 함수
        /// </summary>
        /// <param name="isFocus">포커스 상태</param>
        public virtual void SetFocus(bool isFocus)
        {
            this.isFocused = isFocus;
        }

        #region Drop Action
        private DelegateInt delegate_ActionDropIndex = null;
        public DelegateInt DelegateActionDropIndex { set { this.delegate_ActionDropIndex = value; } }

        /// <summary>
        /// 드랍 아이템이 드랍된 경우 드랍된 아이템에 설정된 인덱스를 전달받는 함수
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