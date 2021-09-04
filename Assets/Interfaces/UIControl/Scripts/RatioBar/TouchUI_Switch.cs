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

        // bar 컨트롤, 애니메이션을 통해 변경된 비율을 전달하는 델리게이트
        private DelegateFloat delegate_ChangeRatio = null;
        public DelegateFloat DelegateChangeRatio { set { this.delegate_ChangeRatio = value; } }

        [SerializeField] private TouchUI_RatioBar bar; // 사용될 Bar

        // 스위치 활성화 상태
        // bar 의 비율 1이 활성화, 0이 비활성화
        // bar 위치 오른쪽이 활성화 상태
        private bool isActive;

        /// <summary>
        /// 초기 설정 함수
        /// </summary>
        public void InitControl()
        {
            // bar 초기 설정
            bar.InitControl();
            // bar 이벤트 델리게이트 연결
            bar.SetEventDelegates(Bar_ChangeRatio, Bar_EndControl);
        }
        /// <summary>
        /// Bar 비율이 변경될 시 해당 비율을 전달받는 이벤트함수
        /// </summary>
        /// <param name="ratio">비율</param>
        private void Bar_ChangeRatio(float ratio)
        {
            // 현재 비율 수치에 따라 활성화 상태 적용
            bool isActive = ratio > 0.5f;
            this.isActive = isActive;

            // 비율 변경에 따라 델리게이트를 통해 변경된 비율 전달
            if (delegate_ChangeRatio != null)
                delegate_ChangeRatio(ratio);
        }
        /// <summary>
        /// Bar 조절이 완료될 시 발생하는 이벤트 함수
        /// </summary>
        private void Bar_EndControl()
        {
            SetSwitchType();
        }

        #region Animation
        [SerializeField] private float animateTime;
        [SerializeField] private iTween.EaseType easeType;
        
        /// <summary>
        /// 현재 설정되어있는 활성화 상태에 따라 Switch 활성화 여부를 설정하는 함수
        /// </summary>
        private void SetSwitchType()
        {
            SetSwitchType(this.isActive, false);
        }
        /// <summary>
        /// 활성화 상태를 토글하는 함수
        /// </summary>
        public void ToggleSwitchType()
        {
            SetSwitchType(!(this.isActive), false);
        }
        public void SetSwitchType(bool active, bool immediately)
        {
            // 진행 중 애니메이션 종료
            iTween.Stop(gameObjectCache);

            // 활성화 여부 설정
            this.isActive = active;

            // 결과 비율값 설정
            float _to = this.isActive ? 1f : 0f;

            if (bar.fRatio.Equals(_to))
                // 현재 bar의 비율과 결과 비율이 동일한 경우 리턴
                return;

            if (immediately || !(gameObjectCache.activeInHierarchy))
                // 즉시상태이거나 오브젝트가 꺼져있는 경우
            {
                SetSwitchRatio(_to);
                CompleteSwitchRatio();
            }
            else
            // 그렇지 않은 경우
            {
                Singleton_Settings.iTweenControl(gameObjectCache, bar.fRatio, _to
                    , animateTime, easeType, "SetSwitchRatio", "CompleteSwitchRatio");
            }
        }
        /// <summary>
        /// bar의 비율값을 설정하는 함수 (애니메이션 사용)
        /// </summary>
        /// <param name="ratio">비율</param>
        private void SetSwitchRatio(float ratio)
        {
            bar.SetRatio(ratio);

            // 비율 변경에 따라 델리게이트를 통해 변경된 비율 전달
            if (delegate_ChangeRatio != null)
                delegate_ChangeRatio(ratio);
        }
        /// <summary>
        /// bar 비율 조절 애니메이션 종료 시 이벤트 함수
        /// </summary>
        private void CompleteSwitchRatio()
        {
            // 해당 상태에 맞는 비율값으로 즉시 설정
            float _to = this.isActive ? 1f : 0f;
            SetSwitchRatio(_to);
        }
        #endregion
    }
}