using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongDuTouchSpace
{
    public class TouchLensMove : TouchMoverParent
    {
        #region ObjectCaches
        private GameObject _gameObjectCache = null;
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

        #region Override
        public override void InitializeThis()
        {
            base.InitializeThis();

            // 타겟의 초기 위치를 기본 위치로 지정
            defaultPos = target.localPosition;
        }
        protected override void StartTouch()
        {
            StopAnimation(); // 애니메이션 종료

            if (touchCount.Equals(1)) // 최초 터치 입력인 경우
            {
                SetTouchInfomation(); // 초기 터치 정보 설정

                // 위 SetTouchInformation에서 GetPosition을 돌면서 터치가 바로 사라진 경우 touchCount가 0으로 변경될 수 있음
                // 그렇기 때문에 현 위치에서 touchCount가 0이라면 아래 내용을 수행하지 않음
                if (touchCount > 0)
                {
                    StartTouchCoroutine(); // 터치 코루틴 실행
                }
            }
        }
        //protected override void EndTouch() { base.EndTouch(); }

        #region Override - Control Move
        //protected override void InitTouchInformation() { base.InitTouchInformation(); }
        //protected override void SetTouchInfomation() { base.SetTouchInfomation(); }
        protected override void MoveObject()
        {
            // 터치 포지션 가져오고
            Vector2 position = GetTouchPosition(0);

            if (position.x < 0) // x 위치값이 0보다 작은경우 터치정보 가져오기 오류
                return; // 리턴

            // 터치포지션을 유니티 포지션으로 변경
            position = TouchPositionToUnityPosition(position);

            // 위 포지션값과 초기 터치 유니티포지션을 가지고 된 위치를 가져옴
            Vector2 movedPos = position - initPos_Touch;

            // 위치 설정
            posStorage.x = initPos_Object.x + movedPos.x;
            posStorage.y = initPos_Object.y + movedPos.y;
            target.localPosition = posStorage;
        }
        #endregion

        #region Override - Coroutine
        //protected override IEnumerator coroutine_touch() { return base.coroutine_touch(); }
        #endregion

        #endregion
        
        private Vector2 defaultPos;// 기본 위치
        /// <summary>
        /// 오보젝트의 Depth(z) 설정 함수
        /// </summary>
        /// <param name="z">depth값</param>
        public void SetDepth(float z)
        {
            posStorage.z = z;
            target.localPosition = posStorage;
        }

        #region AnimatePosition
        [Header("MoveToPosition(Animation)")]
        [SerializeField] private float animateTime = 0;
        [SerializeField] private iTween.EaseType easeType = iTween.EaseType.easeOutCirc;
        /// <summary>
        /// 실행중인 애니메이션을 종료하는 함수
        /// </summary>
        private void StopAnimation()
        {
            iTween.Stop(gameObjectCache);
        }
        /// <summary>
        /// 기본 위치로 이동 애니메이션 실행 함수
        /// </summary>
        /// <param name="immediately">즉시 여부</param>
        public void MoveToDefaultPos(bool immediately)
        {
            if (!(this.defaultPos.Equals(posStorage)))
                // 기본위치와 현재 위치가 동일하지 않은 경우
            {
                // 기본위치로 애미네이션 실행
                MoveToTargetPos(this.defaultPos, immediately);
            }
        }
        /// <summary>
        /// 타겟 위치 이동 애니메이션 실행 함수
        /// </summary>
        /// <param name="targetPos">타겟 위치</param>
        /// <param name="immediately">즉시 여부</param>
        public void MoveToTargetPos(Vector2 targetPos, bool immediately)
        {
            EndTouchSilence(); // 터치 종료
            
            StopAnimation(); // 애니메이션 종료

            if (immediately || !(gameObjectCache.activeInHierarchy))
                // 즉시상태거나 오브젝트가 꺼져있는 경우
            {
                SetTargetPositioning(targetPos);
                CompleteTargetPositioning();
            }
            else
            // 그렇지 않은 경우
            {
                Singleton_Settings.iTweenControl(gameObjectCache, (Vector2)posStorage, targetPos
                    , animateTime, easeType, "SetTargetPositioning", "CompleteTargetPositioning");
            }
        }
        /// <summary>
        /// 타겟 위치 변경 애니메이션 함수
        /// </summary>
        /// <param name="pos">위치</param>
        private void SetTargetPositioning(Vector2 pos)
        {
            posStorage.x = pos.x;
            posStorage.y = pos.y;

            target.localPosition = posStorage;
        }
        // 타겟 위치 변경 애니메이션 종료 시 발생하는 이벤트 함수
        private void CompleteTargetPositioning()
        {

        }
        #endregion
    }
}