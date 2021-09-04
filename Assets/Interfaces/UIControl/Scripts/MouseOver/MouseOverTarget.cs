using UnityEngine;

namespace UIControl
{
    public class MouseOverTarget : MonoBehaviour
    {
        #region GameObject Cache
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

        private void OnEnable()
        {
            Singleton_Settings.getInstance.MouseOver.AddTarget(this);
        }

        private void OnDisable()
        {
            Singleton_Settings.getInstance.MouseOver.RemoveTarget(this);
        }

        [SerializeField] protected Collider col;
        public virtual bool CheckCollider(Collider _col)
        {
            return this.col.Equals(_col);
        }

        /// <summary>
        /// 마우스오버 시 실행될 델리게이트
        /// </summary>
        private DelegateVoid delegate_Focus;
        public DelegateVoid Focused { set { this.delegate_Focus = value; } }

        /// <summary>
        /// 마우스오버 해제 시 실행될 델리게이트
        /// </summary>
        private DelegateVoid delegate_Unfocus;
        public DelegateVoid Unfocused { set { this.delegate_Unfocus = value; } }

        public void FocusTarget()
        {
            if (delegate_Focus != null)
                delegate_Focus();
        }
        public void UnfocusTarget()
        {
            if (delegate_Unfocus != null)
                delegate_Unfocus();
        }
    }
}