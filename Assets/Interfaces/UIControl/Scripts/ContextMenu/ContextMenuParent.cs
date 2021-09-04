using UnityEngine;
using SongDuTouchSpace;

namespace UIControl
{
    /// <summary>
    /// 우클릭 시 표시되는 UI
    /// ContextMenu가 부분에서 해당 클래스를 상속받아 사용하도록 한다
    /// </summary>
    public abstract class ContextMenuParent : MonoBehaviour
    {
        #region Object Caches
        private Transform _transformCache;
        private Transform transformCache
        {
            get
            {
                if (_transformCache == null)
                    _transformCache = this.transform;
                return _transformCache;
            }
        }

        private GameObject _gameObjectCache;
        private GameObject gameObjectCache
        {
            get
            {
                if (_gameObjectCache == null)
                    _gameObjectCache = this.gameObject;

                return _gameObjectCache;
            }
        }
        public void SetActive(bool active)
        {
            gameObjectCache.SetActive(active);
        }
        #endregion

        /// Initialize
        /// 초기 설정 관련
        #region Initialize
        /// <summary>
        /// ContextMenu 초기 설정 함수
        /// </summary>
        public virtual void InitializeContextMenu()
        {
            // 터치영역 델리게이트 연결
            touchArea.delegate_TouchInArea = TouchInArea;
            touchArea.InitializeThis(); // 터치영역 클래스 초기 설정

            InitializePositioningOption(); // Positioning Option 초기 설정
        }
        #endregion

        /// TouchArea
        /// ContextMenu 활성화 시 유효 영역을 표시 관리
        /// 예를 들어 유효 영역이 아닌 곳을 클릭/터치 하는 경우 해당 ContextMenu가 비활성화되는 처리
        #region TouchArea
        [SerializeField] private TouchBeganColliderArea touchArea;
        protected virtual void TouchInArea(bool bIn)
        {
            if (!bIn)
                Deactivate();
        }
        #endregion

        /// Positioning Option
        /// ContextMenu 위치 관련
        #region Positioning Option
        private Vector2 ContextMenuMinimumPos; // 메뉴 위치 minimum값
        private Vector2 ContextMenuMaximumPos; // 메뉴 위치 maximum값

        /// <summary>
        /// 해당 오브젝트의 위치 및 터치 오브젝트의 위치, 그리고 터치영역을 고려하여
        /// ContextMenu의 Minimum 위치와 Maximum위치를 계산하는 함수
        /// </summary>
        private void InitializePositioningOption()
        {
            // 해당obj(transformCache)와 터치obj(touchArea.TargetTransform)는 다르고 위치도 다르기 때문에 둘다 고려해서 계산

            // 해당obj 대비 터치obj의 위치값
            Vector2 differPos = Singleton_Settings.getInstance.GetLocalPosition(touchArea.TargetTransform, transformCache);
            // 터치영역의 반값 계산 (터치 오브젝트 위치 기준 터치영역 상하/좌우로 각각 나뉘기 때문에 미리 half값 계산)
            Vector2 halfBoxColliderArea = touchArea.BoxColliderArea * 0.5f;

            // x : 스크린 좌측 끝위치 - 해당obj 대비 터치obj x위치값 + half 터치 영역의 x값
            // y : 스크린 하단 끝위치 - 해당obj 대비 터치obj y위치값 + half 터치 영역의 y값
            ContextMenuMinimumPos =
                new Vector2(-Singleton_Settings.getInstance.screenRatio - differPos.x + halfBoxColliderArea.x
                , -1f - differPos.y + halfBoxColliderArea.y);

            // x : 스크린 우측 끝위치 - 해당obj 대비 터치 obj x위치값 - half 터치 영역의 x값
            // y : 스크린 상단 끝위치 - 해당obj 대비 터치 obj x위치값 - half 터치 영역의 y값
            ContextMenuMaximumPos = new Vector2(Singleton_Settings.getInstance.screenRatio - differPos.x - halfBoxColliderArea.x
                , 1f - differPos.y - halfBoxColliderArea.y);
        }

        /// <summary>
        /// Minimum / Maximum을 고려한 값 보정 함수
        /// </summary>
        /// <param name="pos">보정될 값</param>
        private void AmendMenuPosition(ref Vector3 pos)
        {
            // Minimum/Maximum에 맞춰 값 보정
            if (pos.x < ContextMenuMinimumPos.x)
                pos.x = ContextMenuMinimumPos.x;
            else if (pos.x > ContextMenuMaximumPos.x)
                pos.x = ContextMenuMaximumPos.x;

            if (pos.y < ContextMenuMinimumPos.y)
                pos.y = ContextMenuMinimumPos.y;
            else if (pos.y > ContextMenuMaximumPos.y)
                pos.y = ContextMenuMaximumPos.y;
        }
        #endregion

        /// Active / Deactive
        /// 메뉴 활성화/비활성화 관련
        #region Active / Deactive
        /// <summary>
        /// ContextMenu 활성화 함수
        /// </summary>
        public virtual void Activate()
        {
            // 마우스 포지션에 맞는 유니티 포지션 추출
            Vector3 pos = TouchCenter.TouchPositionToUnityPosition(Input.mousePosition);
            pos.z = transformCache.position.z; // z값은 기존 위치와 동일하게 맞춤

            AmendMenuPosition(ref pos); // minimum/maximum을 고려해 위치 보정

            transformCache.position = pos; // 위치값 설정

            SetActive(true); // 게임오브젝트 활성화
        }

        /// <summary>
        /// ContextMenu 비활성화 함수
        /// </summary>
        public virtual void Deactivate()
        {
            ClearContextMenu();
            SetActive(false); // 게임오브젝트 비활성화
        }

        /// <summary>
        /// ContextMenu 초기화 함수
        /// </summary>
        protected abstract void ClearContextMenu();
        #endregion
    }
}