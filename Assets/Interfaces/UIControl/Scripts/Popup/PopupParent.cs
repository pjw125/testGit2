using UnityEngine;

namespace UIControl
{
    /// <summary>
    /// �˾�â�� �θ� Ŭ����
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
        /// �˾� Ȱ��ȭ �Լ�
        /// </summary>
        public virtual void OpenPopup()
        {
            gameObjectCache.SetActive(true);
        }
        /// <summary>
        /// �˾� ��Ȱ��ȭ �Լ�
        /// </summary>
        public virtual void ClosePopup()
        {
            gameObjectCache.SetActive(false);
        }
    }
}