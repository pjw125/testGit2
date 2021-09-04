using UnityEngine;

namespace SongDuTouchSpace
{
    /// <summary>
    /// 기본 버튼 클래스
    /// </summary>
    public class ButtonItem : ButtonEventHandler
    {
        #region Override

        #region Override - Unity Functions
        protected override void Awake()
        {
            base.Awake();
            InitButton();
        }
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
        protected override void EventClick()
        {
            if (eventTarget != null)
            {
                if (idx.Equals(float.MinValue))
                    eventTarget.SendMessage(functionName);
                else
                    eventTarget.SendMessage(functionName, idx);
            }
        }
        //protected override void EventPress() { }
        //protected override void EventUp() { }
        //protected override void EventDoubleClick() { }
        #endregion

        #endregion

        [Header("ButtonItem")]
        [SerializeField] protected GameObject eventTarget;
        [SerializeField] private string functionName;
        [SerializeField] private int idx = int.MinValue;

        /// <summary>
        /// 클릭(터치) 이벤트 타겟 변경 함수
        /// </summary>
        /// <param name="et"></param>
        public void SetEventTarget(GameObject et)
        {
            this.eventTarget = et;
        }
        /// <summary>
        /// 클릭(터치) 이벤트 함수명 변경 함수
        /// </summary>
        /// <param name="func"></param>
        public void SetFunctionName(string func)
        {
            this.functionName = func;
        }
        /// <summary>
        /// 클릭(터치) 이벤트 파라미터 변경 함수
        /// </summary>
        /// <param name="param"></param>
        public void SetParameterIndex(int param)
        {
            this.idx = param;
        }

        /// <summary>
        /// 버튼 관련 초기 설정 함수
        /// </summary>
        protected virtual void InitButton() { }
    }
}