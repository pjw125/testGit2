using UnityEngine;

namespace SongDuTouchSpace
{
    public class TouchMoverSimple : TouchMoverParent
    {
        #region Override
        //public override void InitializeThis() { base.InitializeThis(); }

        protected override void StartTouch()
        {
            if (touchCount.Equals(1)) // 최초 터치 입력인 경우
            {
                SetTouchInfomation(); // 초기 터치 정보 설정

                // 위 SetTouchInformation에서 GetPosition을 돌면서 터치가 바로 사라진 경우 touchCount가 0으로 변경될 수 있음
                // 그렇기 때문에 현 위치에서 touchCount가 0이라면 아래 내용을 수행하지 않음
                if (touchCount > 0)
                {
                    StartTouchCoroutine(); // 터치 코루틴 실행

                    // 터치 시작 이벤트 델리게이트 실행
                    if (delegate_StartTouch != null)
                        delegate_StartTouch();
                }
            }
        }

        protected override void EndTouch()
        {
            if (touchCount <= 0) // 터치입력이 완전히 종료된 경우
            {
                StopTouchCoroutine(); // 터치 코루틴 종료

                // 터치 종료 이벤트 델리게이트 실행
                if (delegate_EndTouch != null)
                    delegate_EndTouch();
            }
        }

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
        
        // 터치 시작 이벤트 델리게이트
        private DelegateVoid delegate_StartTouch = null;
        public DelegateVoid DelegateStartTouch { set { this.delegate_StartTouch = value; } }

        // 터치 종료 이벤트 델리게이트
        private DelegateVoid delegate_EndTouch = null;
        public DelegateVoid DelegateEndTouch { set { this.delegate_EndTouch = value; } }

        /// <summary>
        /// 현재 오브젝트 터치위치(손가락/마우스)를 반환
        /// </summary>
        /// <returns>터치 위치</returns>
        public Vector2 GetTouchPosition()
        {
            return GetTouchPosition(0);
        }
    }
}