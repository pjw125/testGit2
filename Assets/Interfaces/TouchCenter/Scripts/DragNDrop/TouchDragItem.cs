using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongDuTouchSpace
{
    public class TouchDragItem : TouchParent
    {
        // 드레그 시작 시 표시 컨텐츠
        [SerializeField] private GameObject contents;
        /// <summary>
        /// 드레그 시작 시 표시 컨텐츠 활성화 여부 설정 함수
        /// </summary>
        /// <param name="active"></param>
        public void SetActiveContents(bool active)
        {
            contents.SetActive(active);
        }

        #region event
        public event DelegateVoid event_StartDrag; // 드래그 시작 시 이벤트
        public event DelegateVoid event_StopDrag; // 드래그 종료 시 이벤트
        #endregion

        #region Destroy
        private void OnDestroy()
        {
            this.event_StartDrag = null;
            this.event_StopDrag = null;

            this.RemoveAllTouch();

            Object.Destroy(this);
        }
        #endregion

        [SerializeField] private bool isDefaultHide = false;
        [SerializeField] private bool isResetPos = false;
        private Vector3 defaultPos = Vector3.zero;

        private void Awake()
        {
            this.InitTouchParent(StartTouch, EndTouch);
            this.MaxTouchCount = 1;
        }

        private void StartTouch()
        {
            if (touchCount == 1)
            {
                if (isResetPos)
                    defaultPos = transformCache.position;

                if (isDefaultHide)
                    this.SetActiveContents(true);

                // 위 SetTouchInformation에서 GetPosition을 돌면서 터치가 바로 사라진 경우 touchCount가 0으로 변경될 수 있음
                // 그렇기 때문에 현 위치에서 touchCount가 0이라면 아래 내용을 수행하지 않음
                if (touchCount > 0)
                    StartTouchCoroutine();

                if (event_StartDrag != null)
                    event_StartDrag();
            }
        }

        private void EndTouch()
        {
            if (touchCount <= 0)
            {
                StopTouchCoroutine();

                if (isDefaultHide)
                    this.SetActiveContents(false);

                if (isResetPos)
                    transformCache.position = defaultPos;

                if (event_StopDrag != null)
                    event_StopDrag();

                DropAction();
            }
        }

        #region Coroutine
        private IEnumerator touchRoutine = null;
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

        private void MoveObject()
        {
            Vector2 position = GetTouchPosition(0);

            if (position.x < 0)
                return;

            Vector2 uPosition = TouchPositionToUnityPosition(position);

            Vector3 result = transformCache.position;
            result.x = uPosition.x;
            result.y = uPosition.y;

            transformCache.position = result;

            CheckOverObject(position);
        }

        #region DropTarget
        [Header("Drop Action")]
        [SerializeField] private int resultIndex = 0;
        [SerializeField] private TouchDropTarget target = null;
        private void CheckOverObject(Vector2 pos)
        {
            Ray ray = TouchCenter.MainCam.ScreenPointToRay(pos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                TouchDropTarget current = hit.collider.GetComponent<TouchDropTarget>();
                if (current == null)
                {
                    ClearTarget();
                }
                else
                {
                    if (target == null)
                    {
                        target = current;
                        target.SetFocus(true);
                    }
                    else
                    {
                        if (!target.Equals(current))
                        {
                            ClearTarget();

                            target = current;
                            target.SetFocus(true);
                        }
                    }
                }
            }
            else
            {
                ClearTarget();
            }
        }

        private void DropAction()
        {
            if (target != null)
            {
                target.ActionDropIndex(resultIndex);

                target.SetFocus(false);
                target = null;
            }
        }

        private void ClearTarget()
        {
            if (target != null)
            {
                target.SetFocus(false);
                target = null;
            }
        }
        #endregion
    }
}