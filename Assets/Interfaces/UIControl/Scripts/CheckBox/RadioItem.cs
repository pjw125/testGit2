using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIControl
{
    public abstract class RadioItem : CheckBoxParent
    {
        // 변경사항이 있는 경우 변경되었음을 알리는 델리게이트
        private DelegateRadioItem delegate_Changed = null;

        #region Override
        protected override void SetCheckState(bool isCheck)
        {
            base.SetCheckState(isCheck);

            if (delegate_Changed != null)
                delegate_Changed(this);
#if UNITY_EDITOR
            else
                Debug.LogError("RadioItem : 변경 알림 델리게이트 없음");
#endif
        }
        #endregion

        #region Initialize
        /// <summary>
        /// 초기 설정 함수
        /// </summary>
        /// <param name="dele_Changed">체크 상태 변경 시 전달하는 델리게이트</param>
        public virtual void InitializeThis(DelegateRadioItem dele_Changed)
        {
            delegate_Changed = dele_Changed;
        }
        #endregion

        #region External
        /// <summary>
        /// 상태변경알림 없이 단순히 체크 상태만 변경하는 함수
        /// </summary>
        public virtual void ExternalCheckState(bool isCheck)
        {
            base.SetCheckState(isCheck);
        }
        #endregion
    }
}