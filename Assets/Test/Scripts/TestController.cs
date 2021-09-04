using UnityEngine;
using UIControl;
using TMPro;

public class TestController : MonoBehaviour
{

    #region OnAwakeInit
    [SerializeField] private bool OnAwakeInit = false;
    private void Awake()
    {
        if (OnAwakeInit)
            InitializeThis();
    }
    #endregion

    /// <summary>
    /// 초기 설정 함수
    /// </summary>
    private void InitializeThis()
    {
        InitializeRadioRoot();

        InitializeCheckBoxes();

        InitializeRatioBar();

        InitializeScrollBar();
    }

    #region Button
    private void ClickButtonA()
    {
        Debug.Log("Click Button A");
    }

    private void ClickButtonB()
    {
        Debug.Log("Click Button B");
    }
    #endregion

    #region RadioButton
    [Header("RadioButton")]
    [SerializeField] private RadioRoot radioRoot;

    /// <summary>
    /// RadioRoot 초기 설정 함수
    /// </summary>
    private void InitializeRadioRoot()
    {
        this.radioRoot.InitializeThis(); // RadioRoot 초기 설정
        this.radioRoot.Delegate_Changed = RadioRoot_Changed; // RadioRoot 선택 변경 시 이벤트 델리게이트 연결
    }
    /// <summary>
    /// RadioRoot 선택 변경 시 이벤트 함수
    /// </summary>
    private void RadioRoot_Changed()
    {
        Debug.LogFormat("RadioRoot Selection : index {0}", this.radioRoot.CheckedIndex);
    }
    #endregion

    #region CheckBox
    [Header("CheckBoxes")]
    [SerializeField] private CheckBoxItem[] checkBoxes;

    /// <summary>
    /// CheckBox 초기 설정 함수
    /// </summary>
    private void InitializeCheckBoxes()
    {
        for (int i = 0; i < checkBoxes.Length; i++)
        {
            checkBoxes[i].InitializeThis(); // CheckBoxes 초기 설정
            checkBoxes[i].Delegate_Changed = CheckBoxes_Changed; // CheckBoxes 선택 변경 시 이벤트 델리게이트 연결
        }
    }
    /// <summary>
    /// CheckBoxes 선택 변경 시 이벤트 함수
    /// </summary>
    /// <param name="state"></param>
    private void CheckBoxes_Changed(bool state)
    {
        string rst = "CheckBoxes";

        for (int i = 0; i < checkBoxes.Length; i++)
        {
            rst = string.Format("{0} [{1} : {2}]", rst, i, checkBoxes[i].IsChecked);
        }

        Debug.Log(rst);
    }
    #endregion

    #region RatioBar
    [Header("RatioBar")]
    [SerializeField] private TouchUI_RatioBar ratioBar;
    [SerializeField] private TextMeshPro textMesh_ratioBar;

    /// <summary>
    /// RatioBar 초기 설정 함수
    /// </summary>
    private void InitializeRatioBar()
    {
        // RatioBar 초기 설정
        ratioBar.InitControl();
        // 이벤트 델리게이트 연결
        ratioBar.SetEventDelegates(RatioBar_SendRatio, RatioBar_EndControl);
    }
    /// <summary>
    /// RatioBar의 비율 변경 시 이벤트 함수
    /// </summary>
    /// <param name="ratio">비율</param>
    private void RatioBar_SendRatio(float ratio)
    {
        textMesh_ratioBar.text = ratio.ToString();
    }
    /// <summary>
    /// RatioBar 조절 완료 시 이벤트 함수
    /// </summary>
    private void RatioBar_EndControl()
    {
        Debug.Log("RatioBar 조절 완료");
    }
    #endregion

    #region ScrollBar
    [Header("ScrollBar")]
    [SerializeField] private TouchUI_ScrollBar scrollBar;
    [SerializeField] private TextMeshPro textMesh_scrollBar;

    /// <summary>
    /// RatioBar 초기 설정 함수
    /// </summary>
    private void InitializeScrollBar()
    {
        scrollBar.InitControl(); // ScrollBar 초기 설정
        scrollBar.ChangeValueDelegate = ScrollBar_ChangeValue; // 값 변경 시 이벤트 델리게이트 연결
    }

    /// <summary>
    /// 값 변경 시 이벤트 함수
    /// </summary>
    /// <param name="val">값</param>
    private void ScrollBar_ChangeValue(float val)
    {
        textMesh_scrollBar.text = val > 0 ? string.Format("+{0}", val.ToString("F2")) : val.ToString("F2");
    }
    #endregion
}
