using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIControl
{
    public class MouseOverAnimator : MonoBehaviour
    {
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

        [SerializeField] private MouseOverTarget mouseOverTarget;

        protected virtual void Awake()
        {
            mouseOverTarget.Focused = this.Focused;
            mouseOverTarget.Unfocused= this.Unfocused;

            InitAnimation();
        }

        protected virtual void OnDisable()
        {
            SetAnimationState(false, true);
        }

        protected virtual void Focused()
        {
            SetAnimationState(true);
        }

        protected virtual void Unfocused()
        {
            SetAnimationState(false);
        }

        #region Animation
        [Header("Animation")]
        [SerializeField] private float animateTime;
        private float animateTimeForMult;
        [SerializeField] private iTween.EaseType easeType;
        protected float animateRatio = 0f;
        protected void InitAnimation()
        {
            animateTimeForMult = 1f / animateTime;
        }

        private void SetAnimationState(bool state)
        {
            SetAnimationState(state, (animateTime <= 0 || !(gameObjectCache.activeInHierarchy)));
        }
        private void SetAnimationState(bool state, bool immediately)
        {
            float _to = state ? 1f : 0f;

            iTween.Stop(gameObjectCache);

            if (immediately)
            {
                SetAnimateRatio(_to);
                CompleteAnimation();
            }
            else
                Singleton_Settings.iTweenControl(gameObjectCache, animateRatio, _to, animateTime, easeType
                    , "SetAnimateRatio", "CompleteAnimation");
        }
        protected virtual void SetAnimateRatio(float val)
        {
            this.animateRatio = val;
        }

        protected virtual void CompleteAnimation()
        {

        }
        #endregion
    }
}