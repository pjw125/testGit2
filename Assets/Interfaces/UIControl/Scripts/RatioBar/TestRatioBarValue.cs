using UnityEngine;

namespace UIControl
{
    public class TestRatioBarValue : MonoBehaviour
    {
        [SerializeField] private TouchUI_RatioBar ratioBar;

        private void Awake()
        {
            ratioBar.SetEventDelegates(this.ReceiveRatio, this.CompleteRatioAction);
        }

        private void ReceiveRatio(float ratio)
        {
            Debug.LogFormat("Received Ratio : {0}", ratio.ToString("F2"));
        }

        private void CompleteRatioAction()
        {
            Debug.Log("CompleteRatioAction");
        }
    }
}