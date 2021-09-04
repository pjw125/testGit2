using UnityEngine;

namespace UIControl
{
    /// <summary>
    /// 팝업창의 부모 클래스
    /// </summary>
    public abstract class PopupParent : MonoBehaviour
    {
        #region ObjectCache
        private GameObject _gameObjectCache;
        protected GameObject gameObjectCache
        {
            get
            {
                if (this._gameObjectCache == null)
                    this._gameObjectCache = this.gameObject;

                return this._gameObjectCache;
            }
        }
        #endregion

        /// <summary>
        /// 팝업 활성화 함수
        /// </summary>
        public virtual void OpenPopup()
        {
            gameObjectCache.SetActive(true);
        }
        /// <summary>
        /// 팝업 비활성화 함수
        /// </summary>
        public virtual void ClosePopup()
        {
            gameObjectCache.SetActive(false);
        }
    }
}