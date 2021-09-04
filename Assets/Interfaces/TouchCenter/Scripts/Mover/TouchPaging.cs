using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongDuTouchSpace
{
    public class TouchPaging : TouchMoverParent
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
        //public override void InitializeThis() { base.InitializeThis(); }
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

                    // 터치 시작에 대한 이벤트 함수 실행
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

                AnimateCurrentPage(); // 페이지 이동 애니메이션 실행
            }
        }

        #region Override - Control Move
        //protected override void InitTouchInformation() { base.InitTouchInformation(); }
        protected override void SetTouchInfomation()
        {
            base.SetTouchInfomation();

            previousTouchPos = initPos_Touch.x;
        }
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

            // x 위치만 변경
            posStorage.x = initPos_Object.x + movedPos.x;
            // 제한값 보정
            if (isApplyLimitPos)
            {
                if (posStorage.x < limitPosMin)
                    posStorage.x = limitPosMin;
                else if (posStorage.x > limitPosMax)
                    posStorage.x = limitPosMax;
            }

            // 값 설정
            target.localPosition = posStorage;

            // 가속도값 추가 및 이전 터치위치를 현재 위치로 설정
            AddAccelValue(position.x - previousTouchPos);
            previousTouchPos = position.x;
        }
        #endregion

        #region Override - Coroutine
        //protected override IEnumerator coroutine_touch() { return base.coroutine_touch(); }
        #endregion

        #endregion

        #region Delegates
        // 최초 터치가 시작된 경우 발생하는 이벤트 델리게이트
        private DelegateVoid delegate_StartTouch = null;
        public DelegateVoid DelegateStartTouch { set { this.delegate_StartTouch = value; } }

        // 페이지 설정이 완료된 경우 발생하는 이벤트 델리게이트
        private DelegateVoid delegate_EndPaging = null;
        public DelegateVoid DelegateEndPaging { set { this.delegate_EndPaging = value; } }

        // 선택 인덱스 변경 시 발생하는 이벤트 델리게이트
        private DelegateVoid delegate_ChangeIndex = null;
        public DelegateVoid DelegateChangeIndex { set { this.delegate_ChangeIndex = value; } }
        #endregion

        #region Page
        private int pageCount = 0; // 전체 페이지 수
        private int selectedIdx = 0; // 현재 페이지 인덱스
        public int SelectedIdx { get { return this.selectedIdx; } }

        [SerializeField] private bool isApplyLimitPos = true; // 제한 위치를 벗어날 수 있는지에 대한 여부
        private float limitPosMin = 0f; // 최소 위치값
        private const float limitPosMax = 0f; // 페이지설정에서의 최대값은 항상 0이기 때문에 상수화
        private float pageGap = 0f; // 페이지간 간격
        private float pageGapForMult = 0f; // 페이지간 간격 곱셈처리용

        /// <summary>
        /// 페이징 관련 초기 설정 함수
        /// </summary>
        /// <param name="pageCount">총 페이지 수</param>
        /// <param name="pageGap">페이지간 간격</param>
        public void InitializeThis(int pageCount, float pageGap)
        {
            MaxTouchCount = 1; // 페이징은 1터치로 수행

            this.pageCount = pageCount; // 총 페이지 수 설정
            limitPosMin = -pageGap * (float)(this.pageCount - 1); // 총 페이지 수와 페이지간 간격에 따라 최소값 설정

            // 페이지 간격 관련 값 설정
            this.pageGap = pageGap;
            this.pageGapForMult = 1f / this.pageGap;

            this.InitializeThis();
        }

        #endregion

        /// *
        /// 페이지 이동 애니메이션 관련
        #region Animation
        private const float animateTime = 0.3f; // 애니메이션 시간
        private const iTween.EaseType easeType = iTween.EaseType.easeOutCirc; // 애니메이션 타입
        /// <summary>
        /// 터치가 완료된 후 현재 선택된 인덱스의 페이지로 이동하는 애니메이션 실행 함수
        /// </summary>
        private void AnimateCurrentPage()
        {
            // 현재 타겟 오브젝트 위치(target.localPosition.x)에 따른 페이지 인덱스 계산
            // 타겟 오브젝트는 음수이기 때문에 계산 시 '-'를 붙여줌
            int currentIdx = (int)Mathf.Round(AmendData.RoundFloat(-target.localPosition.x, this.pageGap) * pageGapForMult);

            if (this.selectedIdx.Equals(currentIdx)) // 타겟 위치에 따른 인덱스와 기존 선택 인덱스가 동일한 경우
                // Accel 값을 반영하여 보정된 인덱스로 변경
                currentIdx = AmendIndexByAcceleration(currentIdx);

            // 인덱스 설정
            SetSelectedPageIndex(currentIdx, false);
        }
        /// <summary>
        /// 선택 페이지 인덱스를 변경하는 함수 (애니메이션)
        /// </summary>
        /// <param name="idx">인덱스</param>
        /// <param name="immediately">즉시여부</param>
        public void SetSelectedPageIndex(int idx, bool immediately)
        {
            EndTouchSilence(); // 피드백 없이 터치 종료

            StopAnimation(); // 현재 수행중이던 애니메이션 종료

            // 전달받은 인덱스를 제한값 보정하며 설정
            if (idx < 0)
                this.selectedIdx = 0;
            else if (idx >= pageCount)
                this.selectedIdx = pageCount - 1;
            else
                this.selectedIdx = idx;

            // 선택 인덱스 변경 델리게이트 실행
            if (delegate_ChangeIndex != null)
                delegate_ChangeIndex();

            float _to = (float)selectedIdx * -pageGap; // 결과 위치 계산

            if (immediately || !(gameObjectCache.activeInHierarchy)) // 즉시모드이거나 게임오브젝트가 꺼져있다면 즉시 이동
            {
                SetTargetPos(_to);
                CompleteTargetPos();
            }
            else // 그렇지 않다면 애니메이션 이동
            {
                Singleton_Settings.iTweenControl(gameObjectCache, target.localPosition.x, _to
                    , animateTime, easeType, "SetTargetPos", "CompleteTargetPos");
            }
        }
        /// <summary>
        /// 현재 수행중인 애미네이션을 종료하는 함수
        /// </summary>
        private void StopAnimation()
        {
            iTween.Stop(gameObjectCache);
        }
        /// <summary>
        /// 타겟의 위치를 변경하는 함수(애니메이션)
        /// </summary>
        /// <param name="x"></param>
        private void SetTargetPos(float x)
        {
            posStorage.x = x;

            target.localPosition = posStorage;
        }
        /// <summary>
        /// 타겟 위치 변경 애니메이셔 종료 시 발생 이벤트 함수
        /// </summary>
        private void CompleteTargetPos()
        {
            // 페이징 종료 델리게이트 실행
            if (delegate_EndPaging != null)
                delegate_EndPaging();
        }
        #endregion

        /// *
        /// 터치 이동 가속도 관련
        /// 터치 이동에 대한 가속도를 판단하여 이전/다음 페이지로 이동할 수 잇도록 하는 부분
        #region Acceleration
        private float[] accelValues = new float[2] { 0f, 0f }; // 최근 2 프레임동안의 터치 속도값
        private bool bToggleAccel = false; // 가속도 값 추가 시 인덱스 토글 변수
        private float previousTouchPos = float.MinValue; // 터치 위치에 대한 이전 터치 위치값, 가속도 설정에 따른 delta값 계산을 위함
        /// <summary>
        /// 가속도값 추가 함수
        /// </summary>
        /// <param name="val"></param>
        private void AddAccelValue(float val)
        {
            if (bToggleAccel) // 토글변수가 true인 경우 1번 인덱스에 값 적용
                accelValues[1] = val;
            else // false인 경우 0번 인덱스에 값 적용
                accelValues[0] = val;

            bToggleAccel = !bToggleAccel; // 토글변수 반전
        }
        /// <summary>
        /// 가속도배열 초기화 함수
        /// </summary>
        private void ClearAccelValues()
        {
            // 전체 가속도값 0으로 설정
            for (int i = 0; i < accelValues.Length; i++)
            {
                accelValues[i] = 0f;
            }

            bToggleAccel = false; // 토글변수 false
        }
        /// <summary>
        /// 가속도값을 반영하여 보정된 결과 인덱스 반환 함수
        /// </summary>
        private int AmendIndexByAcceleration(int idx)
        {
            // 가속도값 2개 중 절대값 기준 더 큰 값을 반영 가속도값으로 설정
            float accelValue = (Mathf.Abs(accelValues[0]) > Mathf.Abs(accelValues[1])) ? accelValues[0] : accelValues[1];

            if (accelValue > 0.01f) // 가속도값이 0.01보다 큰 경우 이전 페이지 인덱스로 설정
                idx--;
            else if (accelValue < -0.01f) // 가속도값이 0.01보다 큰 경우 다음 페이지 인덱스로 설정
                idx++;

            ClearAccelValues(); // 가속도 배열 초기화

            return idx;
        }
        #endregion
    }
}