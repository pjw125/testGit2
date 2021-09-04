using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SongDuTouchSpace;

namespace UIControl
{
    public class MouseOverRoot : MonoBehaviour
    {
        private void Awake()
        {
            // 로드와 동시에 Singleton_Settings에 MouseOver 등록
            Singleton_Settings.getInstance.MouseOver = this;
        }

        private void OnDestroy()
        {
            Object.Destroy(this);
        }

        /// <summary>
        /// 해당 오브젝트 삭제 함수
        /// </summary>
        public void DestroyThisObject()
        {
            GameObject.Destroy(this.gameObject);
        }

        // 마우스 오버 기능 실행중인 타겟 오브젝트 리스트
        private List<MouseOverTarget> targets = new List<MouseOverTarget>();

        /// <summary>
        /// 기능 실행 오브젝트 추가 함수
        /// </summary>
        /// <param name="target">기능 실행 오브젝트</param>
        public void AddTarget(MouseOverTarget target)
        {
            targets.Add(target);

            CheckCoroutineState();
        }

        /// <summary>
        /// 기능 실행 오브젝트 삭제 함수
        /// </summary>
        /// <param name="target">기능 실행 오브젝트</param>
        public void RemoveTarget(MouseOverTarget target)
        {
            targets.Remove(target);

            CheckCoroutineState();
        }

        #region MouseOverByPosition (마우스 포지션 기반 마우스오버 타겟)
        // 기본 targets는 Collider 기반이고 이곳의 애들은 마우스 포지션 기반
        // 마우스 포지션 기반의 마우스 오버는 포커스를 맞추는 행위 없음
        // 단순히 현재 마우스 포지션을 전달하여 범위를 체크하여 그에 따른 행동만 수행할 뿐임
        // ** 유니티 게임오브젝트 기준의 마우스 포지션 **
        private List<MouseOverTargetByMousePos> posTargets = new List<MouseOverTargetByMousePos>();
        /// <summary>
        /// 기능 실행 오브젝트 추가 함수 (마우스 포지션 기반)
        /// </summary>
        /// <param name="target">기능 실행 오브젝트</param>
        public void AddTarget(MouseOverTargetByMousePos target)
        {
            posTargets.Add(target);

            CheckCoroutineState();
        }
        /// <summary>
        /// 기능 실행 오브젝트 삭제 함수 (마우스 포지션 기반)
        /// </summary>
        /// <param name="target">기능 실행 오브젝트</param>
        public void RemoveTarget(MouseOverTargetByMousePos target)
        {
            posTargets.Remove(target);

            CheckCoroutineState();
        }
        #endregion

        // 포커스 중인 마우스오버 타겟
        private MouseOverTarget focused = null;
        private void CheckMouseOver()
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = TouchCenter.MainCam.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // 마우스 포지션에 콜리더(오브젝트)가 존재한다면
            {
                if (focused != null && focused.CheckCollider(hit.collider))
                // 포커스 오브젝트가 있으면서
                // 현재 포커스 오브젝트와 기존 포커스 오브젝트가 같은 경우
                // 아무것도 수행하지 않는다
                {

                }
                else // 그렇지 않은 경우
                {
                    if (focused != null) // 현재 포커스 오브젝트가 있다면
                    {
                        focused.UnfocusTarget(); // 포커스 해제
                        focused = null; // null 처리
                    }

                    for (int i = 0; i < targets.Count; i++)
                    {
                        if (targets[i].CheckCollider(hit.collider)) // 해당 타켓의 콜리더가 포커스 콜리더와 같은 경우
                        {
                            focused = targets[i]; // 포커스 아이템 등록
                            break; // 반복문 탈출
                        }
                    }

                    if (focused != null) // 포커스된 오브젝트가 있다면
                        focused.FocusTarget(); // 타겟 포커스 처리
                }
            }
            else // 마우스 포지션에 콜리더(오브젝트)가 존재하지 않는다면
            {
                if (focused != null) // 현재 포커스 오브젝트가 있는 경우
                {
                    focused.UnfocusTarget(); // 포커스 해제
                    focused = null; // null 처리
                }
            }

            if (posTargets.Count > 0) // 위치기반 포커스 오브젝트가 존재하는 경우
            {
                mousePos = TouchCenter.TouchPositionToUnityPosition(mousePos); // 마우스 포지션을 유니티 포지션으로 변경
                for (int i = 0; i < posTargets.Count; i++)
                {
                    // 유니티 포지션을 전달하여 포커스/포커스해제 처리
                    posTargets[i].CheckPosition(mousePos);
                }
            }
        }

        #region Coroutine
        private IEnumerator ie_check = null;

        private IEnumerator coroutine_check()
        {
            while (true)
            {
                yield return null;

                CheckMouseOver();
            }
        }

        private void CheckCoroutineState()
        {
            if (targets.Count > 0 || posTargets.Count > 0)
            {
                if (ie_check == null)
                    StartCheckCoroutine();
            }
            else
            {
                if (ie_check != null)
                    StopCheckCoroutine();
            }
        }
        private void StartCheckCoroutine()
        {
            StopCheckCoroutine();

            ie_check = coroutine_check();

            StartCoroutine(ie_check);
        }
        private void StopCheckCoroutine()
        {
            if (ie_check != null)
            {
                StopCoroutine(ie_check);
                ie_check = null;
            }
        }
        #endregion
    }
}