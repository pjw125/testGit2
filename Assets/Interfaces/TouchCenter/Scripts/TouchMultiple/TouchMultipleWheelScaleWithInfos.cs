using System.Collections;
using UnityEngine;

namespace SongDuTouchSpace
{
    public class TouchMultipleWheelScaleWithInfos : TouchMultipleWheelScale
    {
        #region Override
        public override void AddTouch(int fingerID)
        {
            // 터치 입력 위치 반환 이벤트 실행
            AddedTouchPos(fingerID);

            // 진행중인 애니메이션 종료
            StopAnimatePosition();
            
            base.AddTouch(fingerID);
        }

        public override bool RemoveTouch(int fingerID)
        {
            // 터치 종료 위치 반환 이벤트 실행
            RemovedTouchPos(fingerID);

            return base.RemoveTouch(fingerID);
        }

        protected override IEnumerator Coroutine_Touch()
        {
            // 터치가 있는 경우 반복
            while (touchCount > 0)
            {
                yield return null;

                // 시스템상 터치가 없고 마우스 좌클릭 상태가 아닌 경우
                if (Input.touchCount.Equals(0) && !(Input.GetMouseButton(0)))
                {
                    for (int i = 0; i < this.touchIDs.Count; i++)
                    {
                        RemovedTouchPos(this.touchIDs[i]);
                    }

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

        protected override void MultiTouch()
        {
            Vector2[] positions = new Vector2[touchCount];
            if (!GetTouchPositions(positions, ref this.touchCenterPos)) // 터치 위치를 가져오는 중 문제가 발생하였다면 리턴
                return;

            // Scale
            if (touchCount > 1 && bScale) // 터치가 1개 이상이고 크기조절이 켜져있는 경우 크기 변경
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
                // delta position 발생 이벤트 실행
                OccurDeltaPos(deltaPos);
            }
        }
        #endregion

        /// 
        /// 터치 입력 위치 반환 이벤트
        #region Event Add
        // 하나의 터치 입력 시 해당 터치 위치 반환 이벤트 델리게이트
        private DelegateVector2 delegate_AddedTouchPos = null;
        public DelegateVector2 DelegateAddedTouchPos { set { this.delegate_AddedTouchPos = value; } }

        /// <summary>
        /// 터치 시작 이벤트 함수
        /// </summary>
        /// <param name="fingerID"></param>
        private void AddedTouchPos(int fingerID)
        {
            if (delegate_AddedTouchPos == null) // 해당 델리게이트가 없다면 리턴
                return;

            Vector2 pos = FingerIDToTouchPosition(fingerID); // 전달받은 fingerID의 위치 확인
            if (!(pos.x < 0 || pos.y < 0f)) // 해당 위치에 음수가 포함되어있지 않다면
                delegate_AddedTouchPos(pos); // 위치 전달
        }
        #endregion

        /// 
        /// 터치 완료 위치 반환 이벤트
        #region Event Remove
        // 하나의 터치 완료 시 해당 터치 위치 반환 이벤트 델리게이트
        private DelegateVector2 delegate_RemovedTouchPos = null;
        public DelegateVector2 DelegateRemovedTouchPos { set { this.delegate_RemovedTouchPos = value; } }

        /// <summary>
        /// 터치 종료 이벤트 함수
        /// </summary>
        /// <param name="fingerID">종료된 터치 ID</param>
        private void RemovedTouchPos(int fingerID)
        {
            if (delegate_RemovedTouchPos == null) // 해당 델리게이트가 없다면 리턴
                return;

            Vector2 pos = FingerIDToTouchPosition(fingerID); // 전달받은 fingerID의 위치 확인
            if (!(pos.x < 0 || pos.y < 0f)) // 해당 위치에 음수가 포함되어있지 않다면
                delegate_RemovedTouchPos(pos); // 위치 전달
        }
        #endregion

        ///
        /// 오브젝트 위치 변화 발생 시 delta position 반환 이벤트
        #region Event Delta Position
        // 해당 오브젝트 이동에 따른 delta position이 발생한 경우 해당 delta position을 반환하는 이벤트 델리게이트
        private DelegateVector2 delegate_OccurDeltaPos = null;
        public DelegateVector2 DelegateOccurDeltaPos { set { this.delegate_OccurDeltaPos = value; } }

        /// <summary>
        /// delta position 발생 이벤트 함수
        /// </summary>
        /// <param name="delta">delta position</param>
        private void OccurDeltaPos(Vector2 delta)
        {
            if (delegate_OccurDeltaPos != null) // 해당 델리게이트가 존재하는 경우
                delegate_OccurDeltaPos(delta); // delta position 전달
        }
        #endregion

        ///
        /// 외부 접근을 통한 위치 이동(애니메이션) 관리
        /// 추후 부모 클래스에 필요한 경우 이동하면 됨
        #region AnimatePosition (iTween)
        // iTween 실행을 위한 gameObjectCache
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

        private bool isAnimating = false;

        /// <summary>
        /// 위치이동 애니메이션 종료 함수
        /// </summary>
        private void StopAnimatePosition()
        {
            if (isAnimating)
            {
                iTween.Stop(gameObjectCache);
                isAnimating = false;
            }
        }
        /// <summary>
        /// 위치이동 애니메이션 실행 함수
        /// </summary>
        /// <param name="pos">타겟위치</param>
        /// <param name="animateTime">애니메이션 시간</param>
        /// <param name="easeType">애니메이션 타입</param>
        public void AnimatePosition(Vector3 pos, float animateTime, iTween.EaseType easeType)
        {
            this.ClearTouch(); // 모든 터치 삭제

            StopAnimatePosition(); // 실행중인 애니메이션 종료

            pos.z = localPosition.z; // 깊이값 기존과 동일하게 설정

            if (animateTime <= 0f || !(gameObjectCache.activeInHierarchy))
                // 애니메이션 시간이 0보다 작거나 오브젝트가 꺼져있는 경우 즉시 이동
            {
                SetAnimatePosition(pos);
                CompleteAnimatePosition();
            }
            else
            // 그렇지 않은 경우 애니메이션
            {
                isAnimating = true;
                Singleton_Settings.iTweenControl(gameObjectCache, localPosition, pos
                    , animateTime, easeType, "SetAnimatePosition", "CompleteAnimatePosition");
            }
        }
        /// <summary>
        /// 위치이동 애니메이션을 통한 위치 이동 함수
        /// </summary>
        /// <param name="pos">위치</param>
        private void SetAnimatePosition(Vector3 pos)
        {
            localPosition = pos;
        }
        /// <summary>
        /// 위치이동 애니메이션 종료 시 이벤트 함수
        /// </summary>
        private void CompleteAnimatePosition()
        {
            isAnimating = false;
        }
        #endregion

        #region Static
        /// <summary>
        /// fingerID를 받아 해당 터치의 위치를 반환하는 함수
        /// </summary>
        /// <param name="fingerID">fingerID</param>
        /// <returns>위치</returns>
        private static Vector2 FingerIDToTouchPosition(int fingerID)
        {
            if (fingerID.Equals(TouchCenter.MouseIndexLeft)) // fingerID가 마우스 좌클릭인 경우
            {
                return Input.mousePosition; // 마우스 위치 반환
            }
            else // 아닌 경우(터치)
            {
                for (int i = 0; i < Input.touches.Length; i++)
                {
                    if (Input.touches[i].fingerId.Equals(fingerID)) // 해당 인덱스의 fingerID와 전달받은 fingerID가 동일한 경우
                        return Input.touches[i].position; // 해당 인덱스의 터치 위치 반환
                }
            }

            return new Vector2(-1f, -1f); // 찾지 못한 경우 음수반환
        }
        #endregion
    }
}