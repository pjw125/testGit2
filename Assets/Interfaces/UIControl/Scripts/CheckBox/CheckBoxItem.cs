namespace UIControl
{
    public abstract class CheckBoxItem : CheckBoxParent
    {
        /// <summary>
        /// 선택 상태 변경 시 실행되는 델리게이트
        /// </summary>
        private DelegateBool delegate_Changed = null;
        public DelegateBool Delegate_Changed
        { set { delegate_Changed = value; } }

        #region Override
        protected override void SetCheckState(bool isCheck)
        {
            base.SetCheckState(isCheck);

            if (delegate_Changed != null)
                delegate_Changed(IsChecked);
        }
        #endregion

        #region Initialize
        /// <summary>
        /// 초기 설정 함수
        /// </summary>
        public abstract void InitializeThis();
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