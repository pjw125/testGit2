using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIControl
{
    /// <summary>
    /// 애니메이션 수행하며 팝업 활성화 상태를 변경하기 위한 부모 클래스
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
            iTween.Stop(gameObjectCache); // 진행 중 애니메이션 종료

            ActivePopup(false, true); // 즉시 팝업 비활성화
        }

        /// <summary>
        /// 초기 설정 함수
        /// </summary>
        public abstract void InitializeThis();

        #region Animation
        [Header("Animation")]
        [SerializeField] private float animateTime; // 애니메이션 시간
        [SerializeField] private iTween.EaseType easeType; // 애니메이션 타입
        protected float animateRatio = 0; // 애니메이션 비율
        protected bool isActivePopup = false; // 활성화 상태

        /// <summary>
        /// 팝업 활성화 애니메이션 실행 함수
        /// </summary>
        /// <param name="active">활성화 여부</param>
        /// <param name="immediately">즉시 수행 여부 true : 즉시, false : 애니메이션</param>
        private void ActivePopup(bool active, bool immediately)
        {
            if (isActivePopup.Equals(active)) // 활성화 상태가 동일하다면 리터 
                return;

            iTween.Stop(gameObjectCache); // 진행 중 애니메이션 종료

            isActivePopup = active; // 팝업 활성화 상태 변경

            float _to = 0f; // 결과 비율값 설정(기본 비활성화수치)
            if (isActivePopup) // 활성화상태인 경우
            {
                base.OpenPopup(); // 우선 팝업 오브젝트 활성화
                _to = 1f; // 결과 비율값을 활성화 수치로 변경
            }

            if (gameObjectCache.activeInHierarchy && !(immediately)) // 오브젝트가 꺼져있거나 즉시상태가 아닌 경우 애니메이션
            {
                Singleton_Settings.iTweenControl(gameObjectCache, animateRatio, _to, animateTime, easeType, "SetAnimateRatio", "CompleteAnimateRatio");
            }
            else // 아니라면 즉시
            {
                SetAnimateRatio(_to);
                CompleteAnimateRatio();
            }
        }
        /// <summary>
        /// 애니메이션 비율 설정 함수
        /// </summary>
        /// <param name="ratio"></param>
        protected virtual void SetAnimateRatio(float ratio)
        {
            animateRatio = ratio;
        }
        /// <summary>
        /// 애니메이션 마무리 함수
        /// </summary>
        protected virtual void CompleteAnimateRatio()
        {
            if (!isActivePopup)
                base.ClosePopup();
        }
        #endregion
    }
}