using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIControl
{
    public class RadioRoot : MonoBehaviour
    {
        /// <summary>
        /// 소속된 라디오 버튼들에 상태변화가 생겼다면 알리는 델리게이트
        /// </summary>
        private DelegateVoid delegate_Changed;
        public DelegateVoid Delegate_Changed
        { set { delegate_Changed = value; } }

        [SerializeField] private RadioItem[] radios; // 라디오 버튼 배열
        /// <summary>
        /// 소속된 라디오 버튼 수
        /// </summary>
        public int ItemCount { get { return radios.Length; } }
        /// <summary>
        /// 선택된 라디오 버튼 인덱스, 선택된 것이 없다면 '-1'을 반환
        /// </summary>
        public int CheckedIndex
        {
            get
            {
                for (int i = 0; i < radios.Length; i++)
                {
                    if (radios[i].IsChecked)
                        return i;
                }
                return -1;
            }
        }

        // 미선택을 사용할지 말지를 결정할 변수
        [SerializeField] private bool IsInvalidNoneCheck = false;

        [SerializeField] private bool OnAwakeInit = false;

        private void Awake()
        {
            if (OnAwakeInit)
                InitializeThis();
        }

        /// <summary>
        /// 초기 설정 함수
        /// </summary>
        public void InitializeThis()
        {
            for (int i = 0; i < radios.Length; i++)
            {
                radios[i].InitializeThis(ChangedChildState);
            }
        }

        /// <summary>
        /// 소속된 라디오 버튼에 변화가 생긴 경우 알림을 받는 함수
        /// </summary>
        /// <param name="item">변경된 라디오</param>
        private void ChangedChildState(RadioItem item)
        {
            if (item.IsChecked) // 변경된 라디오가 체크상태라면
            {
                // 해당 라디오를 제외한 모든 라디오의 체크를 해제해준다
                for (int i = 0; i < radios.Length; i++)
                {
                    if (!(radios[i].Equals(item)))
                        radios[i].ExternalCheckState(false);
                }
            }
            else if (IsInvalidNoneCheck)
            {
                item.ExternalCheckState(true);
            }

            if (delegate_Changed != null)
                delegate_Changed();
        }

        /// <summary>
        /// 라디오 버튼의 체크 상태를 변경하는 함수
        /// </summary>
        /// <param name="idx">선택된 라디오 인덱스</param>
        public void ExternalChildState(int idx)
        {
            for (int i = 0; i < radios.Length; i++)
            {
                radios[i].ExternalCheckState(i.Equals(idx));
            }
        }
    }
}