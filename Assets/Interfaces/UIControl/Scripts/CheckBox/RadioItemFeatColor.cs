using UnityEngine;
using TMPro;

namespace UIControl
{
    /// <summary>
    /// 고객 검안도수 선택 시 사용되는 라디오버튼
    /// </summary>
    public class RadioItemFeatColor : RadioItem
    {
        #region Override

        #region Override - CheckBoxParent
        protected override void SetCheckState(bool isCheck)
        {
            base.SetCheckState(isCheck);
            RefreshUI();
        }
        #endregion

        #region Override - RadioItem
        public override void InitializeThis(DelegateRadioItem dele_Changed)
        {
            base.InitializeThis(dele_Changed);

            defaultColors = new Color[srs.Length + textMeshes.Length];

            for (int i = 0; i < srs.Length; i++)
            {
                defaultColors[i] = srs[i].color;
            }

            for (int i = 0; i < textMeshes.Length; i++)
            {
                int defaultColorIndex = srs.Length + i;
                defaultColors[defaultColorIndex] = textMeshes[i].color;
            }
        }

        public override void ExternalCheckState(bool isCheck)
        {
            base.ExternalCheckState(isCheck);
            RefreshUI();
        }
        #endregion

        #endregion

        [Header("ForOptometry")]

        [SerializeField] private SpriteRenderer[] srs;
        [SerializeField] private TextMeshPro[] textMeshes;
        [SerializeField] private Color[] checkedColors;
        private Color[] defaultColors;

        private void RefreshUI()
        {
            Color[] selectedColors = IsChecked ? checkedColors : defaultColors;
            for (int i = 0; i < srs.Length; i++)
            {
                srs[i].color = selectedColors[i];
            }

            for (int i = 0; i < textMeshes.Length; i++)
            {
                int textMeshIdx = srs.Length + i;
                textMeshes[i].color = selectedColors[textMeshIdx];
            }
        }
    }
}