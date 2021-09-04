using UnityEngine;
using TMPro;

public class AmendOptometryData : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    [SerializeField] private bool ValidMinus; // 음수값 유효
    [SerializeField] private bool ValidPlus; // 양수값 유효

    [SerializeField] private bool isMinusDefault; // 기본값 음수
    [SerializeField] private bool isAmend025; // 도수값 보정(0.25)
    [SerializeField] private bool isMustSignMark; // 부호값 필수 표시 여부 설정

    public void AmendOptometryValue()
    {
        if (inputField.text.Equals(string.Empty))
        {
            Debug.LogWarning("값 없음");
            return;
        }

        bool ExistSignFlag = inputField.text[0].Equals('+') || inputField.text[0].Equals('-');

        float fVal = float.MinValue;
        float.TryParse(inputField.text, out fVal);

        if (fVal.Equals(float.MinValue))
        {
            Debug.LogError("숫자형식이 아님");
            inputField.text = string.Empty;
            return;
        }

        if (!ExistSignFlag && isMinusDefault) // 부호값이 없는데(기본 양수) 기본값이 음수로 설정되어 있다면 음수로 변경
            fVal *= -1f;

        if (isAmend025) // 값 보정이 들어가는 경우
        {
            if (fVal > 15f || fVal < -15f) // 양/음수 상 15를 넘어가는 경우
                fVal *= 0.01f; // 0.01을 곱하여 도수데이터로 맞춘다

            float fAmendVal = AmendData.RoundFloat(fVal, 0.25f); // 0.25를 기준으로 반올림
            if (!(fVal.Equals(fAmendVal)))
            // 현재 값이 보정된 값과 동일하지 않다면 (0.25단위에 맞지 않다면) 에러
            {
                Debug.LogError("0.25단위에 맞지 않습니다");
                fVal = float.MinValue;
            }
        }

        if (fVal.Equals(float.MinValue)) // 값이 유효하지 않는 경우 텍스트 입력 취소
        {
            inputField.text = string.Empty;
        }
        else // 값이 유효한 경우
        {
            if ((fVal > 0f && !ValidPlus) || (fVal < 0f && !ValidMinus))
            {
                Debug.LogError("허용되지 않은 부호값");
                inputField.text = string.Empty;
            }
            else
            {
                if (fVal > 0f && isMustSignMark)
                    // 값이 양수이며 부호값 필수 표시 설정상태라면
                    // '+' 부호 추가
                    inputField.text = string.Format("+{0}", fVal.ToString("F2"));
                else
                    // 아니라면 바로 값 입력
                    inputField.text = fVal.ToString("F2");
            }
        }
    }
}