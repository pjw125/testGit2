using UnityEngine;
using TMPro;

namespace SongDuTouchSpace
{
    /// <summary>
    /// Press 상태에 따라 색상(SpriteRenderer, TextMeshPro)을 변경하는 버튼 클래스
    /// </summary>
    public class ButtonItemColor : ButtonItem
    {
        #region Override

        #region Override - Unity Functions
        //protected override void Awake() { base.Awake(); }
        protected override void OnDestroy()
        {
            StopTouchCoroutine();
            Destroy(this);
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            SetButtonColor(false);
        }
        #endregion

        #region Override - Initialize / Setting / Clear
        //protected override void InitEventHandler() { base.InitEventHandler(); }
        #endregion

        #region Override - Event
        //protected override void EventClick() { }
        protected override void EventPress()
        {
            SetButtonColor(true);
        }
        protected override void EventUp()
        {
            SetButtonColor(false);
        }
        //protected override void EventDoubleClick() { }
        #endregion

        protected override void InitButton()
        {
            int count = spriteRenderers.Length + textMeshes.Length; // 설정된 SpriteRenderer, TextMeshPro 전체 수 계산
            if (!(colors_press.Length.Equals(count))) // 위 계산된 전체 수와 colors_press에 설정된 수가 다르다면
            {
                // 에러메세지 표시 후 리턴
                Debug.LogError("Not Correct Item Count");
                return;
            }

            colors_normal = new Color[count]; // normal 색상 배열 생성

            // 설정된 SpriteRenderer, TextMeshPro의 현재 색상을 normal 색상으로 설정
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                colors_normal[i] = spriteRenderers[i].color;
            }
            for (int i = 0; i < textMeshes.Length; i++)
            {
                int colorIdx = i + spriteRenderers.Length;
                colors_normal[colorIdx] = textMeshes[i].color;
            }
        }

        #endregion

        [Header("ButtonItemColor")]
        [SerializeField] protected SpriteRenderer[] spriteRenderers; // 색상 변경 SpriteRenderer 배열
        [SerializeField] protected TextMeshPro[] textMeshes; // 색상 변경 TextMeshPro 배열
        protected Color[] colors_normal; // 일반상태 색상배열
        [SerializeField] protected Color[] colors_press; // 눌렸을 상태의 색상 배열

        /// <summary>
        /// 버튼 Press에 따라 색상을 적용하는 함수
        /// </summary>
        /// <param name="isPressed">Press 상태</param>
        protected virtual void SetButtonColor(bool isPressed)
        {
            // Press상태에 따라 적용 색상 선택
            Color[] selectedColors = isPressed ? colors_press : colors_normal;

            // 선택된 색상으로 적용 진행
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].color = selectedColors[i];
            }
            for (int i = 0; i < textMeshes.Length; i++)
            {
                int colorIdx = i + spriteRenderers.Length;
                textMeshes[i].color = selectedColors[colorIdx];
            }
        }
    }
}