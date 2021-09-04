using UnityEngine;
using TMPro;

namespace UIControl
{
    public class CheckBoxItemFeatColor : CheckBoxItem
    {
        #region Override
        protected override void SetCheckState(bool isCheck)
        {
            base.SetCheckState(isCheck);

            RefreshUI();
        }

        public override void InitializeThis()
        {
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

        #region Initialize
        [SerializeField] private bool OnAwakeInit = false;
        private void Awake()
        {
            if (OnAwakeInit)
                InitializeThis();
        }

        #endregion

        [Header("ForColor")]
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