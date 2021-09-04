using System.Collections;
using UnityEngine;
using SongDuTouchSpace;

namespace UIControl
{
    /// <summary>
    /// 좌우 Scroll Ratio Bar UI
    /// </summary>
    public class TouchUI_RatioBar : TouchParent
    {
        #region OnAwakeInit
        [SerializeField] private bool OnAwakeInit;
        private void Awake()
        {
            if (OnAwakeInit)
                InitControl();
        }
        #endregion

        private void OnDisable()
        {
            if (touchCount > 0)
                RemoveAllTouch();
        }

        /// <summary>
        /// 초기 설정 함수
        /// </summary>
        public void InitControl()
        {
            this.InitTouchParent(StartTouch, EndTouch);

            this.InitTouchInformation();
        }

        /// <summary>
        /// 터치 시작 함수
        /// </summary>
        private void StartTouch()
        {
            if (touchCount == 1) // 최초 터치인 경우(터치수가 1인 경우)
            {
                SetTouchInfomation(); // 초기 터치 기본 설정

                // 위 SetTouchInformation에서 GetPosition을 돌면서 터치가 바로 사라진 경우 touchCount가 0으로 변경될 수 있음
                // 그렇기 때문에 현 위치에서 touchCount가 0이라면 아래 내용을 수행하지 않음
                if (touchCount > 0)
                    StartTouchCoroutine(); // 터치 코루틴 실행
            }
        }

        /// <summary>
        /// 터치 종료 함수
        /// </summary>
        private void EndTouch()
        {
            if (touchCount <= 0) // 마지막 터치인 경우(터치수가 0이하인 경우)
            {
                StopTouchCoroutine(); // 터치 코루틴 종료

                // 터치 종료 델리게이트가 존재하는 경우 수행
                if (delegate_EndControl != null)
                    delegate_EndControl();
            }
        }

        #region Coroutine
        private IEnumerator touchRoutine = null;
        /// <summary>
        /// 실제 터치 코루틴
        /// </summary>
        private IEnumerator coroutine_touch()
        {
            while (touchCount > 0)
            {
                yield return null;

                if (Input.touchCount == 0 && !(Input.GetMouseButton(0)))
                {
                    this.ClearTouch();
                    EndTouch();
                    break;
                }

                if (touchCount == 1)
                {
                    this.MoveObject();
                }
            }
        }

        private void StartTouchCoroutine()
        {
            StopTouchCoroutine();

            touchRoutine = coroutine_touch();

            StartCoroutine(touchRoutine);
        }

        private void StopTouchCoroutine()
        {
            if (touchRoutine != null)
            {
                StopCoroutine(touchRoutine);
                touchRoutine = null;
            }
        }
        #endregion

        #region Control Ratio
        private DelegateFloat delegate_SendRatio; // 현재 RatioBar의 비율값을 전달하는 델리게이트(0 ~ 1)
        private DelegateVoid delegate_EndControl; // 터치 종료 시 전달하는 델리게이트
        /// <summary>
        /// 사용 델리게이트를 설정하는 함수
        /// </summary>
        /// <param name="dele_SendRatio">비율전달 델리게이트</param>
        /// <param name="dele_EndControl">터치 종료 이벤트 델리게이트</param>
        public void SetEventDelegates(DelegateFloat dele_SendRatio, DelegateVoid dele_EndControl)
        {
            this.delegate_SendRatio = dele_SendRatio;
            this.delegate_EndControl = dele_EndControl;
        }

        [SerializeField] private float MinPosVal; // 유니티상 최소 위치값
        [SerializeField] private float MaxPosVal; // 유니티상 최대 위치값
        private float DifferPosVal; // 최소/최대 위치값의 차이
        private float DifferPosVal_forMult; // 최소/최대 위치값의 차이(나눗셈용)
        private Vector2 initPos_Touch; // 첫 터치의 터치포지션값(유니티 포지션)
        private Vector3 initPos_Object; // 첫 터치의 오브젝트 포지션값
        private Vector3 posStorage;
        private float fBarRatio = 0f; // Bar의 비율값 (0~1)
        public float fRatio { get { return this.fBarRatio; } }
        /// <summary>
        /// 터치 관련 초기 설정 함수
        /// </summary>
        public void InitTouchInformation()
        {
            // 실시간 위치 저장 설정
            posStorage = transformCache.localPosition;


            // 전체 거리
            DifferPosVal = MaxPosVal - MinPosVal;
            DifferPosVal_forMult = 1f / DifferPosVal;   // 나누기용 전체 거리
        }
        /// <summary>
        /// 첫 터치 시 터치관련 설정 함수
        /// </summary>
        private void SetTouchInfomation()
        {
            initPos_Touch = TouchPositionToUnityPosition(GetTouchPosition(0)); // 첫 터치의 터치포지션 설정(유니티 포지션)
            initPos_Object = transformCache.localPosition; // 첫 터치의 오브젝트 위치 설정
        }
        /// <summary>
        /// 실제 터치 처리 함수
        /// </summary>
        private void MoveObject()
        {
            // 터치 포지션 가져오고
            Vector2 position = GetTouchPosition(0);

            if (position.x < 0)
                return;

            // 터치포지션을 유니티 포지션으로 변경
            position = TouchPositionToUnityPosition(position);

            // 위 포지션값과 초기 터치 유니티포지션을 가지고 된 위치를 가져옴
            float movedPos = position.x - initPos_Touch.x;

            // Bar 위치 설정
            this.SetBarPos(initPos_Object.x + movedPos);

            // 비율 전달 델리게이트가 존재한다면 수행
            if (delegate_SendRatio != null)
                delegate_SendRatio(fBarRatio);
        }
        /// <summary>
        /// Bar 위치 설정 함수
        /// </summary>
        /// <param name="x">Bar의 x좌표 위치</param>
        private void SetBarPos(float x)
        {
            posStorage.x = x;

            // 최댓값/최솟값에 맞춰 위치 보정
            if (posStorage.x < MinPosVal)
                posStorage.x = MinPosVal;
            else if (posStorage.x > MaxPosVal)
                posStorage.x = MaxPosVal;
            // 위치 설정
            transformCache.localPosition = posStorage;

            // 현재 Bar 위치에 따른 비율 설정
            this.fBarRatio = (transformCache.localPosition.x - MinPosVal) * DifferPosVal_forMult;
        }
        /// <summary>
        /// 외부에서의 비율값 설정 함수
        /// </summary>
        /// <param name="fRatio">비율</param>
        public void SetRatio(float fRatio)
        {
            this.RemoveAllTouchNoneFeedback(); // 모든 터치 종료

            // 터치 비율값 보정
            if (fRatio < 0f)
                fRatio = 0f;
            else if (fRatio > 1f)
                fRatio = 1f;

            // 터치 위치 및 비율 설정
            SetBarPos(MinPosVal + (DifferPosVal * fRatio));
        }
        /// <summary>
        /// 현재 바 오브젝트의 위치에 따라 제한값보정, 비율설정을 수행하는 함수
        /// </summary>
        public void SetRatioByCurrentPos()
        {
            this.SetBarPos(transformCache.localPosition.x);
        }
        #endregion
    }
}