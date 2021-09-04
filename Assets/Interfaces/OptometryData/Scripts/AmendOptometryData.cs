using UnityEngine;
using TMPro;

public class AmendOptometryData : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    [SerializeField] private bool ValidMinus; // ������ ��ȿ
    [SerializeField] private bool ValidPlus; // ����� ��ȿ

    [SerializeField] private bool isMinusDefault; // �⺻�� ����
    [SerializeField] private bool isAmend025; // ������ ����(0.25)
    [SerializeField] private bool isMustSignMark; // ��ȣ�� �ʼ� ǥ�� ���� ����

    public void AmendOptometryValue()
    {
        if (inputField.text.Equals(string.Empty))
        {
            Debug.LogWarning("�� ����");
            return;
        }

        bool ExistSignFlag = inputField.text[0].Equals('+') || inputField.text[0].Equals('-');

        float fVal = float.MinValue;
        float.TryParse(inputField.text, out fVal);

        if (fVal.Equals(float.MinValue))
        {
            Debug.LogError("���������� �ƴ�");
            inputField.text = string.Empty;
            return;
        }

        if (!ExistSignFlag && isMinusDefault) // ��ȣ���� ���µ�(�⺻ ���) �⺻���� ������ �����Ǿ� �ִٸ� ������ ����
            fVal *= -1f;

        if (isAmend025) // �� ������ ���� ���
        {
            if (fVal > 15f || fVal < -15f) // ��/���� �� 15�� �Ѿ�� ���
                fVal *= 0.01f; // 0.01�� ���Ͽ� ���������ͷ� �����

            float fAmendVal = AmendData.RoundFloat(fVal, 0.25f); // 0.25�� �������� �ݿø�
            if (!(fVal.Equals(fAmendVal)))
            // ���� ���� ������ ���� �������� �ʴٸ� (0.25������ ���� �ʴٸ�) ����
            {
                Debug.LogError("0.25������ ���� �ʽ��ϴ�");
                fVal = float.MinValue;
            }
        }

        if (fVal.Equals(float.MinValue)) // ���� ��ȿ���� �ʴ� ��� �ؽ�Ʈ �Է� ���
        {
            inputField.text = string.Empty;
        }
        else // ���� ��ȿ�� ���
        {
            if ((fVal > 0f && !ValidPlus) || (fVal < 0f && !ValidMinus))
            {
                Debug.LogError("������ ���� ��ȣ��");
                inputField.text = string.Empty;
            }
            else
            {
                if (fVal > 0f && isMustSignMark)
                    // ���� ����̸� ��ȣ�� �ʼ� ǥ�� �������¶��
                    // '+' ��ȣ �߰�
                    inputField.text = string.Format("+{0}", fVal.ToString("F2"));
                else
                    // �ƴ϶�� �ٷ� �� �Է�
                    inputField.text = fVal.ToString("F2");
            }
        }
    }
}