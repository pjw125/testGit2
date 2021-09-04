using System.Collections;
using UnityEngine;

namespace SongDuTouchSpace
{
    public class TouchMultipleWheelScale : TouchMultiple
    {
        #region Override
        //public override void InitializeThis() { base.InitializeThis(); }
        public override void DestroyObject()
        {
            // 진행중이던 휠 코루틴 종료
            StopWheelCoroutine();

            base.DestroyObject();
        }
        protected override void StartTouch()
        {
            base.StartTouch();

            CheckWheelAction();
        }
        protected override void EndTouch()
        {
            base.EndTouch();

            CheckWheelAction();
        }

        //protected override void SetTouchInfomation() { base.SetTouchInfomation(); }

        //protected override void MultiTouch() { base.MultiTouch(); }

        protected override void SetScale(float scale)
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

                if (delegate_ChangeScale != null)
                    // 크기 변경 이벤트 델리게이트 실행
                    delegate_ChangeScale();
            }
        }
        #endregion

        // 크기에 변경이 있는 경우 실행되는 이벤트 델리게이트
        private DelegateVoid delegate_ChangeScale = null;
        public DelegateVoid DelegateChangeScale { set { this.delegate_ChangeScale = value; } }
        /// <summary>
        /// 외부에서 크기를 변경하도록 하는 속성
        /// </summary>
        public float ObjectScale
        {
            set
            {
                // 크기 변경 함수 실행
                this.SetScale(value);
            }
        }

        #region Wheel Action
        [Header("Wheel")]
        [SerializeField] private float wheelSens = 0f; // 휠 민감도
        private IEnumerator IE_Wheel = null; // 마우스 휠 변경에 따른 크기조절 코루틴 변수
        /// <summary>
        /// 마우스 휠 변경에 따른 크기조절 코루틴 함수
        /// </summary>
        private IEnumerator Coroutine_Wheel()
        {
            while (true)
            {
                yield return null;

                if (bInitMultiTouch) // 터치 입력에 따른 초기 설정이 완료된 경우
                {
                    // 휠 민감도가 반영된 마우스 휠 스크롤 델타값
                    float wheelDelta = Input.GetAxis("Mouse ScrollWheel") * wheelSens;
                    // 변경 크기 계산
                    float scale = scaleTarget.localScale.x + wheelDelta;
                    // 크기 변경
                    SetScale(scale);
                }
            }
        }
        /// <summary>
        /// 마우스 휠 변경에 따른 크기조절 코루틴 실행 함수
        /// </summary>
        private void StartWheelCoroutine()
        {
            if (!bScale) // 크기 조절이 꺼져있는 경우 리턴
                return;

            StopWheelCoroutine();

            IE_Wheel = Coroutine_Wheel();
            StartCoroutine(IE_Wheel);
        }
        /// <summary>
        /// 마우스 휠 변경에 따른 크기조절 코루틴 종료 함수
        /// </summary>
        private void StopWheelCoroutine()
        {
            if (IE_Wheel != null)
            {
                StopCoroutine(IE_Wheel);
                IE_Wheel = null;
            }
        }
        /// <summary>
        /// 터치 상태를 체크하여 휠 코루틴을 실행/종료하는 함수
        /// </summary>
        private void CheckWheelAction()
        {
            if (this.touchCount.Equals(1)) // 터치가 1개 있는 경우
                StartWheelCoroutine(); // 휠 코루틴 실행
            else // 그렇지 않은 경우
                StopWheelCoroutine(); // 종료
        }
        #endregion
    }
}