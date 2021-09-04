using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongDuTouchSpace
{
    /// <summary>
    /// 터치된 영역이 연결된 박스콜리더 영역 내에 있는지 확인하여 델리게이트(delegate_TouchInArea)를 통해 전달하는 클래스
    /// </summary>
    public class TouchBeganColliderArea : MonoBehaviour
    {
        #region OnAwakeInit
        [SerializeField] private bool OnAwakeInit;
        private void Awake()
        {
            if (OnAwakeInit)
                InitializeThis();
        }
        #endregion

        [SerializeField] private BoxCollider boxCollider; // 영역 박스콜리더
        private Transform targetTransform; // 타겟 오브젝트
        public Vector2 BoxColliderArea // 해당 오브젝트에 지정된 BoxCollider의 실제 유니티 화면 영역크기
        {
            get
            {
                if (targetTransform == null || boxCollider == null)
                    return Vector2.zero;
                else
                    return new Vector2(targetTransform.lossyScale.x * boxCollider.size.x
                        , targetTransform.lossyScale.y * boxCollider.size.y);
            }
        }
        public Transform TargetTransform
        { get { return targetTransform; } }

        /// <summary>
        /// 초기 설정 함수
        /// </summary>
        public void InitializeThis()
        {
            if (this.boxCollider == null) // 박스 콜리더 가 없는 경우 해당 오브젝트에서 찾아 설정
                this.boxCollider = this.GetComponent<BoxCollider>();

            if (this.boxCollider == null) // 그래도 박스 콜리더가 없는 경우
            {
                Object.Destroy(this); // 해당 스크립트 삭제
                return;
            }
            
            // 타겟 설정
            this.targetTransform = this.boxCollider.transform;
        }

        private void OnEnable()
        {
            // TouchBegan 이벤트 연결
            Singleton_Settings.getInstance.touchCenter.Event_TouchBegan_Pos += CheckTouch;
        }
        private void OnDisable()
        {
            // TouchBegan 이벤트 연결 해제
            Singleton_Settings.getInstance.touchCenter.Event_TouchBegan_Pos -= CheckTouch;
        }

        #region Delegate Action
        public DelegateBool delegate_TouchInArea; // 영역 내 터치 여부 전달 델리게이트
        /// <summary>
        /// 터치된 영역 체크 함수
        /// </summary>
        /// <param name="pos">터치 위치(픽셀기준)</param>
        private void CheckTouch(Vector2 pos)
        {
            if (delegate_TouchInArea == null)
                return;

#if false
            // 아래 방식처럼 픽셀단위로 계산하여 찾는 방식보단
            // 전달받은 픽셀기준 위치값(pos)을 유니티 포지션으로 변경해 찾는 방식이 더 나을 것 같음
            // 추후 변경해보자. 시간이 된다며 말이지...
#else
            // unity position 1당 픽셀크기 (높이 해상도 1080기준 유니티 위치 1당 540픽셀(1080픽셀 * 0.5))
            float fBaseScale = Singleton_Settings.getInstance.screenSize.y * 0.5f;

            /// basePos : 콜리더 타겟(targetTransform) 위치에 따른 모니터 픽셀 기준 위치값 계산
            /// - x
            ///  * targetTransform.position.x * fBaseScale : 해당 오브젝트 x축 위치값을 픽셀단위로 계산
            ///  * + (Singleton_Settings.getInstance.screenSize.x * 0.5f)
            ///   : 유니티 위치값의 0의 기준은 모니터 가운데위치지만 픽센단위의 0 기준은 모니터 최 좌측이기 때문에
            ///     x픽셀 전체크기 기준 half값을 더해줘서 맞춘다
            /// - y
            ///  * targetTransform.position.y * fBaseScale : 해당 오브젝트 y축 위치값을 픽셀단위로 계산
            ///  * + fBaseScale
            ///   : 유니티 위치값의 0의 기준은 모니터 가운데위치지만 픽센단위의 0 기준은 모니터 최 좌측이기 때문에
            ///     y픽셀 전체크기 기준 half값인 fBaseScale을 더해줘서 맞춘다
            Vector2 basePos = new Vector2((targetTransform.position.x * fBaseScale) + (Singleton_Settings.getInstance.screenSize.x * 0.5f)
                , (targetTransform.position.y * fBaseScale) + fBaseScale);
            /// scale : 박스콜리더 크기에 따른 픽셀기준 크기 계산
            /// - 계산에 half(* 0.5f)값이 적용되는 이유는 아래 min/max값을 계산할 때 좌우로 적용되기 때문에 미리 half해주는 것임
            Vector2 scale = new Vector2(targetTransform.lossyScale.x * this.boxCollider.size.x * 0.5f * fBaseScale
                , targetTransform.lossyScale.y * this.boxCollider.size.y * 0.5f * fBaseScale);

            // 위 계산된 내용을 가지고 픽셀기준 min/max값 계산
            Vector2 minPos = new Vector2(basePos.x - scale.x, basePos.y - scale.y);
            Vector2 maxPos = new Vector2(basePos.x + scale.x, basePos.y + scale.y);
#endif


            // 델리게이트를 통해 영역 내 포함 여부 전달
            delegate_TouchInArea((pos.x > minPos.x && pos.x < maxPos.x && pos.y > minPos.y && pos.y < maxPos.y));
        }
        #endregion

#if false // 사용되고 있지 않음
        #region Static
    private static Vector2 UnityPositionToTouchPosition(Vector2 unityPosition)
    {
        Vector2 halfScreenSize = Singleton_Settings.getInstance.screenSize * 0.5f;
        // UnityPosition 1당 TouchPosition(screenPosition)은 'halfScreenSize.y'와 동일하기 때문에
        // 받아온 unityPosition의 x, y값에 'halfScreenSize.y'를 곱해주어 계산
        return new Vector2((unityPosition.x * halfScreenSize.y) + halfScreenSize.x
            , (unityPosition.y * halfScreenSize.y) + halfScreenSize.y);
    }
        #endregion
#endif
    }
}