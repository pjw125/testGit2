using UnityEngine;

namespace SongDuTouchSpace
{
    /// <summary>
    /// 멀티터치를 이용한 위치/크기변경 클래스를 상속받아 각도까지 변경하는 클래스
    /// </summary>
    public class TouchMultipleRotate : TouchMultiple
    {
        #region Override
        //public override void InitializeThis() { base.InitializeThis(); }
        //protected override void StartTouch() { base.StartTouch(); }
        //protected override void EndTouch() { base.EndTouch(); }
        protected override void SetTouchInfomation()
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

            CalculateAngles(touchCenterPos, positions, angleListPrev); // 각 터치에 대한 이전 각도값 설정

            this.scaleStorage = this.scaleTarget.localScale;

            // 터치 설정 완료 처리
            bInitMultiTouch = true;
        }
        protected override void MultiTouch()
        {
            Vector2[] positions = new Vector2[touchCount];
            if (!GetTouchPositions(positions, ref this.touchCenterPos)) // 터치 위치를 가져오는 중 문제가 발생하였다면 리턴
                return;

            // Scale
            if (touchCount > 1)
                ChangeScale(touchCenterPos, positions);

            // Angle
            if (touchCount > 1)
                ChangeRotate(touchCenterPos, positions);

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

        // 각 터치의 중앙 기준 이전 각도값
        private float[] angleListPrev = new float[10] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };

        /// <summary>
        /// 각 터치에 대한 센터 기준 각도값을 계산하여 배열로 저장하는 함수
        /// </summary>
        /// <param name="center">센터 위치값</param>
        /// <param name="positions">각 터치 위치</param>
        /// <param name="angleList">계산된 각도 배열</param>
        private void CalculateAngles(Vector2 center, Vector2[] positions, float[] angleList)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                angleList[i] = (Mathf.Atan2(positions[i].x - center.x, positions[i].y - center.y) * 180) * PI_FORMULT;
            }
        }

        #region Angle
        /// <summary>
        /// 터치에 따른 각도 조절 함수
        /// </summary>
        /// <param name="centerPos">센터 위치</param>
        /// <param name="positions">터치 위치 배열</param>
        protected void ChangeRotate(Vector2 centerPos, Vector2[] positions)
        {
            // 각 터치 당 센터 기준 각도값 계산
            float[] angles = new float[touchCount];
            CalculateAngles(centerPos, positions, angles);

            float deltaRotate = 0f; // 이전 프레임 기준 각도 차이값
            for (int i = 0; i < angles.Length; i++)
            {
                // 이전 각도 기준 차이값 누적 계산
                deltaRotate += angles[i] - angleListPrev[i];
                // 현제 각도값을 이전 각도값으로 이관
                angleListPrev[i] = angles[i];
            }

            // 각도 차이값 보정 (절대값 기준 180을 넘지 않도록)
            if (deltaRotate > 180) // 180을 초과한 경우 -의 값으로 보정
                deltaRotate = deltaRotate - 360f;
            else if (deltaRotate < -180) // -180 미만인 경우 +의 값으로 보정
                deltaRotate = 360f + deltaRotate;
            else if (deltaRotate.Equals(0)) // 각도값 차이가 없는 경우 리턴
                return;

            // 평균 각도차이값 계산
            deltaRotate = -deltaRotate * touchCountForMult;
            // 각도 적용
            scaleTarget.Rotate(0f, 0f, deltaRotate);
        }
        #endregion
    }
}