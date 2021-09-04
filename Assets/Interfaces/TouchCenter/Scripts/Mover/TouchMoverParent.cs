using System.Collections;
using UnityEngine;

namespace SongDuTouchSpace
{
    /// <summary>
    /// 터치를 통해 오브젝트 움직임을 관리하는 부모 클래스
    /// </summary>
    public abstract class TouchMoverParent : TouchParent
    {
        #region Move target
        [SerializeField] protected Transform target; // 타겟 오브젝트
        /// <summary>
        /// x, y 값을 받아 로컬포지션를 변경하는 함수
        /// </summary>
        public void SetTargetLocalPosition(float x, float y)
        {
            posStorage.x = x;
            posStorage.y = y;
            target.localPosition = posStorage;
        }
        #endregion

        /// <summary>
        /// 초기 설정 함수
        /// </summary>
        public virtual void InitializeThis()
        {
            // 부모 초기 설정 (터치입력/터치종료 함수 전달)
            this.InitTouchParent(StartTouch, EndTouch);
            // 터치 정보 초기 설정
            this.InitTouchInformation();
        }
        /// <summary>
        /// 터치 입력 함수
        /// </summary>
        protected virtual void StartTouch()
        {
            if (touchCount.Equals(1)) // 최초 터치 입력인 경우
            {
                SetTouchInfomation(); // 초기 터치 정보 설정

                // 위 SetTouchInformation에서 GetPosition을 돌면서 터치가 바로 사라진 경우 touchCount가 0으로 변경될 수 있음
                // 그렇기 때문에 현 위치에서 touchCount가 0이라면 아래 내용을 수행하지 않음
                if (touchCount > 0)
                    StartTouchCoroutine(); // 터치 코루틴 실행
            }
        }
        /// <summary>
        /// 터치 종료 함수
        /// </summary>
        protected virtual void EndTouch()
        {
            if (touchCount <= 0) // 터치입력이 완전히 종료된 경우
            {
                StopTouchCoroutine(); // 터치 코루틴 종료
            }
        }

        /// <summary>
        /// 피드백 없이 조용히 터치를 종료하는 함수
        /// * 현재 부모클래스엔 피드백에 대한 내용이 없지만 자식클래스 생성 시 피드백에 대한 내용은 'EndTouch'에만 구성하면 됨
        /// </summary>
        protected void EndTouchSilence()
        {
            ClearTouch(); // 터치 정보 클리어
            StopTouchCoroutine(); // 코루틴 종료
        }

        #region Control Move
        protected Vector2 initPos_Touch; // 초기 터치 위치(유니티 포지션 기준)
        protected Vector3 initPos_Object; // 초기 오브젝트 위치
        protected Vector3 posStorage; // 위치 저장소
        /// <summary>
        /// 터치 정보 초기 설정 함수
        /// </summary>
        protected virtual void InitTouchInformation()
        {
            // 현재 타겟의 위치를 위치 저장소에 설정
            posStorage = target.localPosition;
        }
        /// <summary>
        /// 최초 터치 시 터치 정보 설정
        /// </summary>
        protected virtual void SetTouchInfomation()
        {
            // 초기 터치 위치 설정
            initPos_Touch = TouchPositionToUnityPosition(GetTouchPosition(0));
            // 초기 오브젝트 위치 설정
            initPos_Object = target.localPosition;
        }
        /// <summary>
        /// 이동 함수 
        /// </summary>
#if true
        protected abstract void MoveObject();
#else
        protected virtual void MoveObject()
        {
            // 터치 포지션 가져오고
            Vector2 position = GetTouchPosition(0);

            if (position.x < 0) // x 위치값이 0보다 작은경우 터치정보 가져오기 오류
                return; // 리턴

            // 터치포지션을 유니티 포지션으로 변경
            position = TouchPositionToUnityPosition(position);

            // 위 포지션값과 초기 터치 유니티포지션을 가지고 된 위치를 가져옴
            Vector2 movedPos = position - initPos_Touch;

            posStorage.x = initPos_Object.x + movedPos.x;
            posStorage.y = initPos_Object.y + movedPos.y;
            posStorage.z = target.localPosition.z;
            target.localPosition = posStorage;
        }
#endif
        #endregion

        #region Coroutine
        private IEnumerator touchRoutine = null;
        /// <summary>
        /// 터치 코루틴
        /// </summary>
        protected virtual IEnumerator coroutine_touch()
        {
            while (touchCount > 0) // 터치가 있는 경우 반복
            {
                yield return null;

                if (Input.touchCount == 0 && !(Input.GetMouseButton(0))) // 터치나 마우스입력이 없는 경우
                {
                    // 터치 종료 및 반복문 탈출
                    this.ClearTouch();
                    EndTouch();
                    break;
                }

                if (touchCount > 0) // 터치 입력이 있는 경우
                {
                    this.MoveObject(); // 이동 함수 실행
                }
            }
        }
        /// <summary>
        /// 터치 코루틴 실행 함수
        /// </summary>
        protected void StartTouchCoroutine()
        {
            StopTouchCoroutine();

            touchRoutine = coroutine_touch();

            StartCoroutine(touchRoutine);
        }
        /// <summary>
        /// 터치 코루틴 종료 함수
        /// </summary>
        protected void StopTouchCoroutine()
        {
            if (touchRoutine != null)
            {
                StopCoroutine(touchRoutine);
                touchRoutine = null;
            }
        }
        #endregion
    }
}