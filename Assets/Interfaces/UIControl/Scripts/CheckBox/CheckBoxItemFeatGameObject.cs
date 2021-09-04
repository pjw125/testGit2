using UnityEngine;

namespace UIControl
{
    public class CheckBoxItemFeatGameObject : CheckBoxItem
    {
        [SerializeField] private GameObject[] selector;

        #region Override
        protected override void SetCheckState(bool isCheck)
        {
            base.SetCheckState(isCheck);

            RefreshUI();
        }

        public override void InitializeThis()
        {
        }

        public override void ExternalCheckState(bool isCheck)
        {
            base.ExternalCheckState(isCheck);

            RefreshUI();
        }
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