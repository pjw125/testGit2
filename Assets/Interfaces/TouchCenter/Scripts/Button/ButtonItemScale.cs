using UnityEngine;

namespace SongDuTouchSpace
{
    /// <summary>
    /// Press 상태에 따라 크기를 변경하는 버튼 클래스
    /// </summary>
    public class ButtonItemScale : ButtonItem
    {
        #region Override

        #region Override - Unity Functions
        //protected override void Awake() { base.Awake(); }
        protected override void OnDestroy()
        {
            StopTouchCoroutine();
            Destroy(this);
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            SetButtonScale(false);
        }
        #endregion

        #region Override - Initialize / Setting / Clear
        //protected override void InitEventHandler() { base.InitEventHandler(); }
        #endregion

        #region Override - Event
        //protected override void EventClick() { }
        protected override void EventPress()
        {
            SetButtonScale(true);
        }
        protected override void EventUp()
        {
            SetButtonScale(false);
        }
        //protected override void EventDoubleClick() { }
        #endregion

        protected override void InitButton()
        {
            scale_normal = scaleTarget.localScale.x;
        }

        #endregion

        [Header("ButtonItemScale")]
        [SerializeField] private Transform scaleTarget = null;
        private float scale_normal;
        [SerializeField] private float scale_press;
        private Vector3 scaleStorage = new Vector3();

        private void SetButtonScale(bool isPressed)
        {
            float selectedScale = isPressed ? scale_press : scale_normal;
            scaleStorage.Set(selectedScale, selectedScale, selectedScale);
            scaleTarget.localScale = scaleStorage;
        }
    }
}