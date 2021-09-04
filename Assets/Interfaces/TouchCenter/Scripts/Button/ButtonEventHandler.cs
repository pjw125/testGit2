using System.Collections;
using UnityEngine;

namespace SongDuTouchSpace
{
    public abstract class ButtonEventHandler : TouchParent
    {
        // 초기 터치 포지션값을 갖는 변수
        private Vector2 initPosition_Touch;

        [Header("ButtonEventHandler")]
        [SerializeField] private TouchParent touchParent; // 터치 이동 시 터치를 이관할 TouchParent
        /// <summary>
        /// 터치를 이관할 TouchParent 설정 함수
        /// </summary>
        /// <param name="tp">터치를 이관할 TouchParent</param>
        public void SetTouchParent(TouchParent tp)
        { this.touchParent = tp; }

        protected const float fDoubleClickTime = 0.2f; // 더블클릭 시 첫클릭 이 후 다음클릭 유효 시간(초)
        protected float DoubleTouchStartTime = -1f; // 터치 시작 시간

        private bool bClickState = false; // 클릭 상태인지 확인하는 변수

        #region Unity Functions
        protected virtual void Awake()
        {
            InitEventHandler(); // 이벤트핸들러 초기설정
        }

        protected abstract void OnDestroy();

        protected virtual void OnDisable()
        {
            RemoveAllTouch(); // 모든 터치 삭제
        }
        #endregion

        #region Initialize / Setting / Clear
        /// <summary>
        /// 이벤트 핸들러 초기 설정 함수
        /// </summary>
        protected virtual void InitEventHandler()
        {
            this.InitTouchParent(StartTouch, EndTouch);
        }

        /// <summary>
        /// 첫 터치 시작 시 초기 터치 위치를 설정하는 함수
        /// </summary>
        protected void SetTouchInfomation()
        {
            initPosition_Touch = GetTouchPosition(0);
        }

        /// <summary>
        /// 클릭에 대한 액션을 취하지 않도록 하며 모든 터치를 삭제하는 함수
        /// </summary>
        public void RemoveTouchUnusualy()
        {
            bClickState = false;

            RemoveAllTouch();
        }

        /// <summary>
        /// 터치 입력 확인 코루틴을 종료하는 함수
        /// </summary>
        protected void StopTouchCoroutine()
        {
            if (touchRoutine != null)
                StopCoroutine(touchRoutine);
            touchRoutine = null;
        }
        #endregion

        #region Touch Start/End
        /// <summary>
        /// 터치 시작 시 전달받는 이벤트 함수
        /// </summary>
        private void StartTouch()
        {
            if (touchCount.Equals(1))
                // 첫 터치라면
            {
                // 클릭상태를 true로 만들고
                bClickState = true;

                // 초기 터치 위치 설정
                SetTouchInfomation();
                // 위 SetTouchInformation에서 GetPosition을 돌면서 터치가 바로 사라진 경우 touchCount가 0으로 변경될 수 있음
                // 그렇기 때문에 현 위치에서 touchCount가 0이라면 아래 내용을 수행하지 않음
                if (touchCount > 0)
                    // 터치가 있다면
                {
                    // 터치 입력 확인 코루틴을 실행한 후
                    touchRoutine = coroutine_touch();
                    StartCoroutine(touchRoutine);

                    // Press에 대한 이벤트를 수행한다
                    EventPress();
                }
            }
        }

        /// <summary>
        /// 터치 종료 시 전달받는 이벤트 함수
        /// </summary>
        private void EndTouch()
        {
            if (touchCount <= 0)
                // 터치가 더 이상 없다면
            {
                // 터치 입력 확인 코루틴을 종료한 후
                if (touchRoutine != null)
                    StopCoroutine(touchRoutine);

                // Up에 대한 이벤트를 수행한다
                EventUp();

                if (bClickState)
                    // 현재 클릭상태라면
                {
                    // Click에 대한 이벤트를 수행한다
                    EventClick();

                    if (this.DoubleTouchStartTime > 0f)
                        // 이전 터치 종료 시간이 있다면
                    {
                        // 이 전 터치와의 시간을 확인하여
                        float fTime = Time.time - this.DoubleTouchStartTime;
                        if (fTime < fDoubleClickTime)
                        // 더블클릭 유효시간 보다 작다면
                        {
                            // DoubleClick에 대한 이벤트 수행
                            EventDoubleClick();
                            // 더블클릭 시작 시간 초기화
                            this.DoubleTouchStartTime = -1f;
                        }
                        else
                            // 그렇지 않다면 더블클릭 시작 시간에 현재 시작 설정
                            this.DoubleTouchStartTime = Time.time;
                    }
                    else // 그렇지 않다면 더블클릭 시작 시간에 현재 시작 설정
                        this.DoubleTouchStartTime = Time.time;
                }

                // 터치 입력 화인 코루틴 null 처리
                touchRoutine = null;
            }
        }
        #endregion

        #region Touch Check (Coroutine)
        /// <summary>
        /// 터치 입력 확인 코루틴
        /// </summary>
        private IEnumerator touchRoutine = null;
        private IEnumerator coroutine_touch()
        {
            while (touchCount > 0) // 현재 터치가 존재할 경우 반복
            {
                yield return null;

                if (Input.touchCount == 0 && !(Input.GetMouseButton(0)))
                    //터치가 없고 마우스 클릭상태가 아니라면
                {
                    this.ClearTouch(); // 터치를 클리어하고
                    EndTouch(); // 터치 종료 처리 후
                    break; // 반복문에서 빠져나간다
                }
                if (touchCount == 1) // 터치가 하나 있다면
                    this.CheckTouch(); // 터치 상태를 체크한다
            }
        }

        /// <summary>
        /// 현재 터치 상태를 확인하는 함수
        /// </summary>
        private void CheckTouch()
        {
            // 현재 터치 위치를 가져온다
            Vector2 position = GetTouchPosition(0);

            if (position.x < 0)
                // position.x가 0보다 작다면 이상값을 가져온 것이기 때문에 리턴한다
                return;

            if (touchParent != null)
                // 터치 이관 오브젝트가 연결되어 있다면
            {
                // 이동거리를 체크하여 터치를 이관하였는지 확인하고
                bool bSendParent = CheckDistance_SendTouchToParent(position);
                if (bSendParent)
                    // 이관하였다면 리턴한다
                    return;
            }

            // 현재 터치 포지션을 통해 클릭상태인지 확인한다
            bClickState = CheckTouchObject(position);
        }

        private const float touchDist = 0.01f; // 터치 이관 픽셀단위 거리
        /// <summary>
        /// 터치 이동 거리를 계산하여 이관 오브젝트로 이관하는 함수
        /// </summary>
        /// <param name="position">현재 터치 위치</param>
        /// <returns>이관이 되었는지 아닌지 확인</returns>
        private bool CheckDistance_SendTouchToParent(Vector2 position)
        {
            if (touchCount <= 0)
                return false;

            // 픽셀 단위 터치 이동 거리 계산
            float dist = Mathf.Sqrt((position - initPosition_Touch).sqrMagnitude);
            // 픽셀 단위로 계산된 거리를 유니티 거리 단위로 변경
            dist *= Singleton_Settings.getInstance.WorldPosPerOnePixel;

            //if (dist > touchDist && touchCount > 0)
            if (dist > touchDist) // 이동 거리가 지정된 거리를 초과했다면
            {
                touchParent.AddTouch(touchIDs[0]); // 가지고 있는 터치를 부모로 이관한 후
                RemoveTouchUnusualy(); // 모든 터치를 삭제한다

                return true;
            }

            return false;
        }
        #endregion

        #region Events
        protected virtual void EventClick() { }
        protected virtual void EventPress() { }
        protected virtual void EventUp() { }
        protected virtual void EventDoubleClick() { }
        #endregion
    }
}