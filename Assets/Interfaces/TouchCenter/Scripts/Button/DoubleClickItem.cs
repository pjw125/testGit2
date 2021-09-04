using UnityEngine;

namespace SongDuTouchSpace
{
    public class DoubleClickItem : ButtonEventHandler
    {
        #region Override

        #region Override - Unity Functions
        //protected override void Awake() { base.Awake(); }
        protected override void OnDestroy()
        {
            StopTouchCoroutine();
            Destroy(this);
        }
        //protected override void OnDisable() { base.OnDisable(); }
        #endregion

        #region Override Initialize / Setting / Clear
        //protected override void InitEventHandler() { base.InitEventHandler(); }
        #endregion

        #region Override - Event
        //protected override void EventClick() { }
        //protected override void EventPress() { }
        //protected override void EventUp() { }
        protected override void EventDoubleClick()
        {
            if (eventTarget != null)
            {
                if (idx.Equals(float.MinValue))
                    eventTarget.SendMessage(functionName);
                else
                    eventTarget.SendMessage(functionName, idx);
            }
        }
        #endregion

        #endregion

        [Header("Event")]
        [SerializeField] protected GameObject eventTarget;
        [SerializeField] private string functionName;
        [SerializeField] private int idx = int.MinValue;
    }
}