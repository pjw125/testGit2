using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardShotcutRoot : MonoBehaviour
{
    [SerializeField] private bool isUppercast = false;
    public bool IsUppercast
    {
        get { return this.isUppercast; }
    }
    [SerializeField] private bool isCtrl = false;
    public bool IsCtrl
    {
        get { return isCtrl; }
    }

    private void Awake()
    {
        //Singleton_Settings.getInstance.Shotcut = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            isUppercast = true;
        }
        if (Input.GetKeyUp(KeyCode.RightShift))
        {
            isUppercast = false;
        }
        
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isUppercast = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isUppercast = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCtrl = true;
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            ExecuteRefresh();
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCtrl = false;
        }

#if false
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (currentTabItem != null)
            {
                if (isUppercast)
                    currentTabItem.PrevTab();
                else
                    currentTabItem.NextTab();
            }
        }

        if (IsCtrl)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (currentTabItem != null)
                    currentTabItem.ClipboardCopy();
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                TextEditor te = new TextEditor();
                te.Paste();

                if (currentTabItem != null)
                    currentTabItem.ClipboardPaste(te.text);
            }
        }
#endif
    }
    #region Tab Action
    //public TabActItemParent currentTabItem = null;
    #endregion

    #region refresh
    public event DelegateVoid Event_Refresh;
    private void ExecuteRefresh()
    {
        if (Event_Refresh != null)
            Event_Refresh();
    }
    #endregion
}