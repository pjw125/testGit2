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
    /// �ʱ� ���� �Լ�
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
    /// RadioRoot �ʱ� ���� �Լ�
    /// </summary>
    private void InitializeRadioRoot()
    {
        this.radioRoot.InitializeThis(); // RadioRoot �ʱ� ����
        this.radioRoot.Delegate_Changed = RadioRoot_Changed; // RadioRoot ���� ���� �� �̺�Ʈ ��������Ʈ ����
    }
    /// <summary>
    /// RadioRoot ���� ���� �� �̺�Ʈ �Լ�
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
    /// CheckBox �ʱ� ���� �Լ�
    /// </summary>
    private void InitializeCheckBoxes()
    {
        for (int i = 0; i < checkBoxes.Length; i++)
        {
            checkBoxes[i].InitializeThis(); // CheckBoxes �ʱ� ����
            checkBoxes[i].Delegate_Changed = CheckBoxes_Changed; // CheckBoxes ���� ���� �� �̺�Ʈ ��������Ʈ ����
        }
    }
    /// <summary>
    /// CheckBoxes ���� ���� �� �̺�Ʈ �Լ�
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
    /// RatioBar �ʱ� ���� �Լ�
    /// </summary>
    private void InitializeRatioBar()
    {
        // RatioBar �ʱ� ����
        ratioBar.InitControl();
        // �̺�Ʈ ��������Ʈ ����
        ratioBar.SetEventDelegates(RatioBar_SendRatio, RatioBar_EndControl);
    }
    /// <summary>
    /// RatioBar�� ���� ���� �� �̺�Ʈ �Լ�
    /// </summary>
    /// <param name="ratio">����</param>
    private void RatioBar_SendRatio(float ratio)
    {
        textMesh_ratioBar.text = ratio.ToString();
    }
    /// <summary>
    /// RatioBar ���� �Ϸ� �� �̺�Ʈ �Լ�
    /// </summary>
    private void RatioBar_EndControl()
    {
        Debug.Log("RatioBar ���� �Ϸ�");
    }
    #endregion

    #region ScrollBar
    [Header("ScrollBar")]
    [SerializeField] private TouchUI_ScrollBar scrollBar;
    [SerializeField] private TextMeshPro textMesh_scrollBar;

    /// <summary>
    /// RatioBar �ʱ� ���� �Լ�
    /// </summary>
    private void InitializeScrollBar()
    {
        scrollBar.InitControl(); // ScrollBar �ʱ� ����
        scrollBar.ChangeValueDelegate = ScrollBar_ChangeValue; // �� ���� �� �̺�Ʈ ��������Ʈ ����
    }

    /// <summary>
    /// �� ���� �� �̺�Ʈ �Լ�
    /// </summary>
    /// <param name="val">��</param>
    private void ScrollBar_ChangeValue(float val)
    {
        textMesh_scrollBar.text = val > 0 ? string.Format("+{0}", val.ToString("F2")) : val.ToString("F2");
    }
    #endregion
}
