using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIControl
{
    public class RadioItemFeatGameObject : RadioItem
    {
        [SerializeField] private GameObject[] selector;

        #region Override
        protected override void SetCheckState(bool isCheck)
        {
            base.SetCheckState(isCheck);
            RefreshUI();
        }
        public override void ExternalCheckState(bool isCheck)
        {
            base.ExternalCheckState(isCheck);
            RefreshUI();
        }
        //public override void InitializeThis(DelegateRadioItem dele_Changed) { base.InitializeThis(dele_Changed); }
        #endregion

        private void RefreshUI()
        {
            for (int i = 0; i < selector.Length; i++)
            {
                selector[i].SetActive(IsChecked);
            }
        }
    }
}