using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongDuTouchSpace
{
    /// <summary>
    /// 이 스크립트 중요
    /// 터치로 크기 변경('ChangeScale' 함수) 시 오브젝트 위치 대비 터치 위치를 고려해
    /// 터치 위치를 중심으로 크기가 커지도록 한 클래스
    /// 추후 필요시 사용해야 하기 때문에 남겨둠
    /// </summary>
    public class TouchMultipleNewExample : TouchParent
    {
        #region OnAwakeInit
        [SerializeField] private bool OnAwakeInit = false;
        private void Awake()
        {
            if (OnAwakeInit)
                InitializeThis();
        }
        #endregion

        /// <summary>
        /// 최초 터치 시작 시 발생 이벤트 델리게이트
        /// </summary>
        private DelegateVoid delegate_InitiateTouch = null;
        public DelegateVoid DelegateInitiateTouch { set { this.delegate_InitiateTouch = value; } }

        /// <summary>
        /// 모든 터치 컨트롤 종료 시 발생 이벤트 델리게이트
        /// </summary>
        private DelegateVoid delegate_CompleteTouch = null;
        public DelegateVoid DelegateCompleteTouch { set { this.delegate_CompleteTouch = value; } }

        // 위치이동 타겟 오브젝트
        [SerializeField] protected Transform moveTarget;
        // 크기변경 타겟 오브젝트
        [SerializeField] protected Transform scaleTarget;
        // 신규 터치 입력 시 초기화 완료 확인 변수
        protected bool bInitMultiTouch = false;

        /// <summary>
        /// 초기 설정 함수
        /// </summary>
        public virtual void InitializeThis()
        {
            InitTouchParent(StartTouch, EndTouch);
        }

        /// <summary>
        /// 오브젝트 삭제 함수
        /// </summary>
        public virtual void DestroyObject()
        {
            // 진행중이던 터치 코루틴 종료
            StopTouchCoroutine();

            Object.Destroy(this);
        }

        /// <summary>
        /// 한개의 터치 입력 시 이벤트 함수
        /// </summary>
        protected virtual void StartTouch()
        {
            bInitMultiTouch = false; // 터치 입력 초기 설정 미완료 상태로 변경

            if (touchCount.Equals(1)) // 최초 터치가 인지된 경우
            {
                StartTouchCoroutine(); // 터치 코루틴 실행

                if (delegate_InitiateTouch != null) // 최초 터치 이벤트 실행
                    delegate_InitiateTouch();
            }
        }
        /// <summary>
        /// 한개의 터치 종료 시 이벤트 함수
        /// </summary>
        protected virtual void EndTouch()
        {
            if (touchCount <= 0) // 모든 터치가 종료된 경우
            {
                StopTouchCoroutine(); // 터치 코루틴 종료

                if (delegate_CompleteTouch != null) // 모든 터치 종료 이벤트 실행
                    delegate_CompleteTouch();
            }
        }

        #region Touch Action
        // 터치 진행 코루틴 변수
        private IEnumerator IE_Touch = null;
        /// <summary>
        /// 터치 진행 코루틴 함수
        /// </summary>
        protected virtual IEnumerator Coroutine_Touch()
        {
            // 터치가 있는 경우 반복
            while (touchCount > 0)
            {
                yield return null;

                // 시스템상 터치가 없고 마우스 좌클릭 상태가 아닌 경우
                if (Input.touchCount.Equals(0) && !(Input.GetMouseButton(0)))
                {
                    // 터치 정보 클리어 및 종료
                    this.ClearTouch();
                    EndTouch();
                    continue;
                }

                if (bInitMultiTouch) // 터치 입력 초기 설정이 된 상태라면
                    MultiTouch(); // 터치에 따른 오브젝트 변경 함수 수행
                else // 초기설정이 되지 않은 상태라면
                    SetTouchInfomation(); // 초기 설정 진행
            }
        }
        /// <summary>
        /// 터치 진행 코루틴 실행 함수
        /// </summary>
        private void StartTouchCoroutine()
        {
            StopTouchCoroutine();

            this.IE_Touch = Coroutine_Touch();
            StartCoroutine(this.IE_Touch);
        }
        /// <summary>
        /// 터치 진행 코루틴 종료 함수
        /// </summary>
        private void StopTouchCoroutine()
        {
            if (this.IE_Touch != null)
            {
                StopCoroutine(this.IE_Touch);
                this.IE_Touch = null;
            }
        }
        #endregion

        #region Move
        protected float touchCountForMult = 0f; // 곱셈용 터치 수 (1 / TouchCount)
        protected Vector2 touchCenterPos = new Vector2(); // 현재 터치의 중앙 위치
        protected Vector2 touchCenterPosPrev = new Vector2(); // 이전 터치의 중앙 위치

        protected float fInitialDistance = 0f; // 초기 터치의 평균 거리값
        protected float fInitialScale = 0f; // 초기 오브젝트 크기

        /// <summary>
        /// 터치 (추가) 입력 시 기본 터치 정보를 설정하는 함수
        /// </summary>
        protected virtual void SetTouchInfomation()
        {
            // 터치가 없는 경우 리턴
            if (touchCount == 0)
                return;

            // 곱셈용 TouchCount 계산
            touchCountForMult = 1f / (float)touchCount;

            Vector2[] positions = new Vector2[touchCount];

            if (!GetTouchPositions(positions, ref touchCenterPos))
            // 터치 위치를 정상적으로 가져오지 못했다면
            {
                bInitMultiTouch = false; // 터치 설정
                return;
            }

            touchCenterPosPrev = touchCenterPos; // 이전 중앙 위치 설정
            this.fInitialDistance = this.CalculateDistanceAvg(touchCenterPos, positions); // 최초 터치 거리 설정
            this.fInitialScale = this.scaleTarget.localScale.x; // 최초 타겟 크기 설정

            this.scaleStorage = this.scaleTarget.localScale; // 크기 저장소 초기값 설정

            // 터치 설정 완료 처리
            bInitMultiTouch = true;
        }

        /// <summary>
        /// 터치에 따른 이동 함수
        /// </summary>
        protected virtual void MultiTouch()
        {
            Vector2[] positions = new Vector2[touchCount];
            if (!GetTouchPositions(positions, ref this.touchCenterPos)) // 터치 위치를 가져오는 중 문제가 발생하였다면 리턴
                return;

            // Scale
            if (touchCount > 1)
                ChangeScale(touchCenterPos, positions);

            // Position
            if (touchCount > 0)
            {
                // 터치에 대한 델타값 계산, 유니티위치값(* ratio)
                Vector3 deltaPos = (touchCenterPos - touchCenterPosPrev) * ratio;
                // 델타값 추가 및 위치적용
                this.moveTarget.Translate(deltaPos.x, deltaPos.y, 0f);
                // 현재 위치를 이전위치로 이관
                touchCenterPosPrev = touchCenterPos;
            }
        }
        #endregion

        #region Position
        /// <summary>
        /// 오브젝트 위치
        /// </summary>
        public Vector3 localPosition
        {
            get { return this.moveTarget.localPosition; }
            set { this.moveTarget.localPosition = value; }
        }
        #endregion

        #region Scale
        [Header("Scale")]
        // 최소 크기값
        [SerializeField] protected float minimumScale = 0;
        public float MinimumScale { set { this.minimumScale = value; } }

        // 최대 크기값
        [SerializeField] protected float maximumScale = float.MaxValue;
        public float MaximumScale { set { this.maximumScale = value; } }

        // 크기 저장소
        protected Vector3 scaleStorage = Vector3.one;

        /// <summary>
        /// 외부 참조용 오브젝트 크기값
        /// </summary>
        public float Scale { get { return this.scaleTarget.localScale.x; } }

        /// <summary>
        /// 터치에 따른 크기 조절 함수
        /// </summary>
        /// <param name="centerPos">센터위치</param>
        /// <param name="positions">터치 위치 배열</param>
        protected void ChangeScale(Vector2 centerPos, Vector2[] positions)
        {
            float fChangedScale = CalculateDistanceAvg(centerPos, positions); // 터치 평균 거리값 추출
            fChangedScale = (fChangedScale - fInitialDistance) * this.ratio; // 최초 터치거리 기준 변경된 터치 거리 유니티값(* this.ratio)
            fChangedScale *= 2f; // 거리는 양쪽 기준이니 2배...맞나??...

            float fPrevScale = scaleStorage.x; // 변경 이전의 크기 저장

            this.SetScale(fInitialScale + fChangedScale); // 크기 설정

            float fChangeRatio = scaleStorage.x / fPrevScale; // 변경 이전 크기 대비 현재 크기 비율
            
            // 터치 중심 위치 유니티 포지션화
            centerPos = TouchCenter.TouchPositionToUnityPosition(centerPos);

            // 오브젝트 위치에서 터치 위치사이의 거리
            centerPos.x = localPosition.x - centerPos.x;
            centerPos.y = localPosition.y - centerPos.y;

            // 
            float deltaX = (centerPos.x * fChangeRatio) - centerPos.x;
            float deltaY = (centerPos.y * fChangeRatio) - centerPos.y;

            //centerPos *= fChangeRatio;
            
            this.moveTarget.Translate(deltaX, deltaY, 0f);
        }
        /// <summary>
        /// 타겟 크기 변경 함수
        /// </summary>
        /// <param name="scale">크기값</param>
        protected virtual void SetScale(float scale)
        {
            // 크기값 최소/최대값에 따른 보정
            if (scale < this.minimumScale)
                scale = this.minimumScale;
            else if (scale > this.maximumScale)
                scale = this.maximumScale;

            if (!(scale.Equals(scaleTarget.localScale.x)))
            // 타겟의 크기와 변경코자 하는 크기가 다른 경우
            {
                // 저장소에 크기 적용
                scaleStorage.x = scale;
                scaleStorage.y = scale;
                // 크기 설정
                scaleTarget.localScale = scaleStorage;
            }
        }
        /// <summary>
        /// 각 터치의 센터 기준 거리 평균 반환 함수
        /// </summary>
        /// <param name="center">센터 위치</param>
        /// <param name="positions">터치 위치</param>
        /// <returns>터치 거리 평균값</returns>
        protected float CalculateDistanceAvg(Vector2 center, Vector2[] positions)
        {
            // 결과 변수
            float result = 0f;

            for (int i = 0; i < positions.Length; i++)
            {
                // 각 터치의 센터간 거리 누적 계산
                result += Vector2.Distance(center, positions[i]);
            }

            // 평균 계산
            result = result * touchCountForMult;

            return result;
        }
        #endregion

        #region GetTouchPositions
        /// <summary>
        /// 각 터치의 위치와 중앙위치를 추출하는 함수
        /// </summary>
        /// <param name="positions">위치 배열</param>
        /// <param name="centerPos">중앙 위치값</param>
        /// <returns>정상여부</returns>
        protected bool GetTouchPositions(Vector2[] positions, ref Vector2 centerPos)
        {
            centerPos.Set(0f, 0f); // 중앙 위치 초기화

            // 터치 삭제 리스트
            List<int> removeList = new List<int>();

            for (int a = 0; a < touchCount; a++)
            {
                bool bCorrectTouch = false; // 터치 유효값

                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (this.touchIDs[a].Equals(Input.touches[i].fingerId)) // 터치id가 동일한 경우
                    {
                        positions[a] = Input.touches[i].position; // 배열에 터치 위치값 설정

                        if (Input.touches[i].phase == TouchPhase.Ended || Input.touches[i].phase == TouchPhase.Canceled)
                            // 해당 터치가 종료/취소 상태인 경우 삭제 리스트에 추가
                            removeList.Add(this.touchIDs[a]);

                        // 유효 터치
                        bCorrectTouch = true;
                        break;
                    }
                }

                // 터치가 유효하지 않으면서 해당 인덱스가 마우스 좌클릭인 경우
                if (!bCorrectTouch && (this.touchIDs[a] == TouchCenter.MouseIndexLeft))
                {
                    positions[a] = Input.mousePosition; // 배열에 마우스 위치 설정

                    if (Input.GetMouseButtonUp(0) || !Input.GetMouseButton(0))
                        // 마우스 좌클릭이 떨어진 상태거나 마우스 좌클릭이 눌려있지 않다면 해당 터치 삭제 리스트에 추가
                        removeList.Add(this.touchIDs[a]);

                    // 유효 터치
                    bCorrectTouch = true;
                }

                if (bCorrectTouch) // 유효터치인 경우
                {
                    // 해당 터치 위치를 중앙 터치 위치에 더하기
                    centerPos.x += positions[a].x;
                    centerPos.y += positions[a].y;
                }
                else // 해당 터치가 유효하지 않은 경우 터치 삭제 리스트에 추가
                    removeList.Add(this.touchIDs[a]);
            }

            if (removeList.Count > 0) // 터치 삭제 리스트에 값이 있는 경우
            {
                bInitMultiTouch = false; // 터치 초기화를 위해 false로 설정
                for (int i = 0; i < removeList.Count; i++)
                {
                    // 삭제 리스트에 있는 모든 터치 삭제
                    this.RemoveTouch(removeList[i]);
                }
                // 비정상 반환
                return false;
            }

            // 중앙위치 계산을 위해 평균 계산
            centerPos *= touchCountForMult;
            // 정상 반환
            return true;
        }
        #endregion

        /// <summary>
        /// 예전 코드, 각도계산 들어가는건 알겠는데 어떻게 쓰였던 건지 모르겟다... 확인 필요
        /// </summary>
#if false
        #region SimulationAngleAmend
        protected float deratioSimulAngle = 1f;
        protected float ratioSimulAngle = 0f;
        protected void InitAngleData()
        {
            float angle = targetObject.localEulerAngles.z;
            bool bOverAngle180 = false;
            if (angle > 180)
            {
                angle = angle - 360f;
                angle *= -1f;
                bOverAngle180 = true;
            }
            ratioSimulAngle = angle * 0.01111111f;
            deratioSimulAngle = 1f - ratioSimulAngle;
            if (ratioSimulAngle > 1f)
                ratioSimulAngle = 2f - ratioSimulAngle;
            if (bOverAngle180)
                ratioSimulAngle *= -1;
        }
        protected Vector2 CalculateDeltaPositionForAngle(Vector2 deltaPos)
        {
            Vector2 resultPos = new Vector2((deltaPos.x * deratioSimulAngle) + (deltaPos.y * ratioSimulAngle), (deltaPos.y * deratioSimulAngle) + (deltaPos.x * -ratioSimulAngle));

            return resultPos;
        }
        protected void CalculateDeltaPositionForAngle(ref Vector2 deltaPos)
        {
            deltaPos.x = (deltaPos.x * deratioSimulAngle) + (deltaPos.y * ratioSimulAngle);
            deltaPos.y = (deltaPos.y * deratioSimulAngle) + (deltaPos.x * -ratioSimulAngle);
        }
        #endregion
#endif
    }
}