/// * class MouseOverTargetByMousePos
/// Collider 체크가 아닌 마우스 위치값을 기반으로 MouseOver를 수행하는 클래스

using UnityEngine;

namespace UIControl
{
    public class MouseOverTargetByMousePos : MonoBehaviour
    {
        [Header("MouseOverTargetByMousePos")] 
        [SerializeField] private Vector2 minimum = new Vector2(float.MinValue, float.MinValue);
        [SerializeField] private Vector2 maximum = new Vector2(float.MaxValue, float.MaxValue);
        protected float MinimumX
        { set { minimum.x = value; } }
        protected float MinimumY
        { set { minimum.y = value; } }
        protected float MaximumX
        { set { maximum.x = value; } }
        protected float MaximumY
        { set { maximum.y = value; } }

        private bool isFocused = false;

        private void OnEnable()
        {
            Singleton_Settings.getInstance.MouseOver.AddTarget(this);
        }

        private void OnDisable()
        {
            Singleton_Settings.getInstance.MouseOver.RemoveTarget(this);
        }

        public void CheckPosition(Vector2 mousePos)
        {
            bool isFocus = mousePos.x > minimum.x && mousePos.x < maximum.x
                && mousePos.y > minimum.y && mousePos.y < maximum.y;

            if (!(isFocused.Equals(isFocus)))
            {
                isFocused = isFocus;

                if (isFocused)
                    FocusTarget();
                else
                    UnfocusTarget();
            }
        }

        protected virtual void FocusTarget()
        {
            //Debug.LogFormat("{0}'s Focused", name);
        }

        protected virtual void UnfocusTarget()
        {
            //Debug.LogFormat("{0}'s Unfocused", name);
        }
    }
}