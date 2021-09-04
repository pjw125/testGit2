using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIControl
{
    /// <summary>
    /// 좌우 Scroll Step Bar UI
    /// </summary>
    public class TouchUI_ScrollBar : MonoBehaviour
    {
        [SerializeField] private TouchUI_RatioBar bar; // 사용될 Bar
        [SerializeField] private float[] vals = null; // 각 Step에 사용될 값

        [SerializeField] private bool OnAwakeInit = false; // Awake에서 초기화를 할지 판단하는 변수
        [SerializeField] private bool isAutoSetValues = false; // 자동으로 'vals'를 설정할지 판단하는 변수

        private int selectedIdx = 0; // 현재 선택 인덱스
        private float ratioStep = 0f; // 선택 인덱스에 따른 비율값을 계산하기 위한 값

        private void Awake()
        {
            if (OnAwakeInit)
                InitControl();
        }

        /// <summary>
        /// 자동값 설정 오브젝트가 존재한다면 설정하는 함수
        /// </summary>
        private void AutoSettingValues()
        {
            // 초기 설정 오브젝트 추출
            Initializer_ScrollBar initializer = this.GetComponent<Initializer_ScrollBar>();

            if (initializer != null) // 초기 설정 오브젝트가 존재한다면
            {
                this.vals = initializer.GetValues(); // 값 설정
                Object.Destroy(initializer); // 초기 설정 오브젝트 삭제
            }
        }

        /// <summary>
        /// 초기 설정 함수
        /// </summary>
        public void InitControl()
        {
            if (isAutoSetValues) // 자동값설정 상태인 경우 자동 값 설정 함수 실행
                AutoSettingValues();

            ratioStep = 1f / (float)(vals.Length - 1); // 선택 인덱스에 따른 비율값을 계산하기 위한 값 설정

            bar.InitControl(); // 하위 Bar 초기 설정
            bar.SetEventDelegates(this.ChangeRatio, this.CompleteChangingRatio); // 델리게이트 연결
        }

        /// <summary>
        /// Bar의 비율값이 변경된 경우 비율값을 받는 함수
        /// </summary>
        /// <param name="ratio">비율값</param>
        private void ChangeRatio(float ratio)
        {
            int currentIdx = Mathf.RoundToInt(ratio / ratioStep); // 비율에 따른 선택 인덱스 계산

            if (!(selectedIdx.Equals(currentIdx))) // 비율에 따른 인덱스가 현재 선택 인덱스와 다른 경우
            {
                selectedIdx = currentIdx; // 비율에 다른 인덱스로 현재 인덱스 변경
                ApplyIndex(); // 인덱스 적용 함수 수행
            }
        }
        /// <summary>
        /// Bar 터치 완료 시 수행되는 함수
        /// </summary>
        private void CompleteChangingRatio()
        {
            // 선택 인덱스에 따라 Bar 위치 재조정
            bar.SetRatio(ratioStep * (float)selectedIdx);
        }

        #region Change Step
        // 선택값 전달 델리게이트
        private DelegateFloat delegate_ChangeValue;
        public DelegateFloat ChangeValueDelegate
        { set { delegate_ChangeValue = value; } }
        /// <summary>
        /// 이전 인덱스 선택 함수
        /// </summary>
        private void PrevIndex()
        {
            SetIndex(selectedIdx - 1);
        }
        /// <summary>
        /// 다음 인덱스 선택 함수
        /// </summary>
        private void NextIndex()
        {
            SetIndex(selectedIdx + 1);
        }
        /// <summary>
        /// 인덱스 설정 함수
        /// </summary>
        /// <param name="idx">변경 인덱스</param>
        private void SetIndex(int idx)
        {
            // 인덱스 값 보정
            if (idx < 0)
                idx = 0;
            else if (idx >= vals.Length)
                idx = vals.Length - 1;

            if (!(selectedIdx.Equals(idx))) // 인덱스가 다른 경우
            {
                selectedIdx = idx; // 인덱스 값 설정
                CompleteChangingRatio(); // 터치 완료 함수 수행
                ApplyIndex(); // 인덱스 적용 함수 수행
            }
        }
        /// <summary>
        /// 값을 이용해 인덱스를 변경 후 결과를 수행하는 함수
        /// </summary>
        /// <param name="val">값</param>
        /// <returns>변경되었는지 여부</returns>
        public bool SetIndexByValue(float val)
        {
            for (int i = 0; i < vals.Length; i++)
            {
                if (vals[i].Equals(val)) // 현재 인덱스의 값이 전달받은 값과 동일한 경우
                {
                    selectedIdx = i; // 해당 인덱스를 선택 인덱스로 변경하고
                    CompleteChangingRatio(); // 완료처리
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 인덱스 적용 함수
        /// </summary>
        private void ApplyIndex()
        {
            // 선택값 전달 델리게이트가 존재하는 경우 수행
            if (delegate_ChangeValue != null)
                delegate_ChangeValue(vals[selectedIdx]);
        }
        #endregion
    }
}