using UnityEngine;

namespace SongDuTouchSpace
{
    /// <summary>
    /// ��Ƽ��ġ�� �̿��� ��ġ/ũ�⺯�� Ŭ������ ��ӹ޾� �������� �����ϴ� Ŭ����
    /// </summary>
    public class TouchMultipleRotate : TouchMultiple
    {
        #region Override
        //public override void InitializeThis() { base.InitializeThis(); }
        //protected override void StartTouch() { base.StartTouch(); }
        //protected override void EndTouch() { base.EndTouch(); }
        protected override void SetTouchInfomation()
        {
            // ��ġ�� ���� ��� ����
            if (touchCount == 0)
                return;

            // ������ TouchCount ���
            touchCountForMult = 1f / (float)touchCount;

            Vector2[] positions = new Vector2[touchCount];

            if (!GetTouchPositions(positions, ref touchCenterPos))
            // ��ġ ��ġ�� ���������� �������� ���ߴٸ�
            {
                bInitMultiTouch = false; // ��ġ ����
                return;
            }

            touchCenterPosPrev = touchCenterPos; // ���� �߾� ��ġ ����
            this.fInitialDistance = this.CalculateDistanceAvg(touchCenterPos, positions); // ���� ��ġ �Ÿ� ����
            this.fInitialScale = this.scaleTarget.localScale.x; // ���� Ÿ�� ũ�� ����

            CalculateAngles(touchCenterPos, positions, angleListPrev); // �� ��ġ�� ���� ���� ������ ����

            this.scaleStorage = this.scaleTarget.localScale;

            // ��ġ ���� �Ϸ� ó��
            bInitMultiTouch = true;
        }
        protected override void MultiTouch()
        {
            Vector2[] positions = new Vector2[touchCount];
            if (!GetTouchPositions(positions, ref this.touchCenterPos)) // ��ġ ��ġ�� �������� �� ������ �߻��Ͽ��ٸ� ����
                return;

            // Scale
            if (touchCount > 1)
                ChangeScale(touchCenterPos, positions);

            // Angle
            if (touchCount > 1)
                ChangeRotate(touchCenterPos, positions);

            // Position
            if (touchCount > 0)
            {
                // ��ġ�� ���� ��Ÿ�� ���, ����Ƽ��ġ��(* ratio)
                Vector3 deltaPos = (touchCenterPos - touchCenterPosPrev) * ratio;
                // ��Ÿ�� �߰� �� ��ġ����
                this.moveTarget.Translate(deltaPos.x, deltaPos.y, 0f);
                // ���� ��ġ�� ������ġ�� �̰�
                touchCenterPosPrev = touchCenterPos;
            }
        }
        #endregion

        // �� ��ġ�� �߾� ���� ���� ������
        private float[] angleListPrev = new float[10] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };

        /// <summary>
        /// �� ��ġ�� ���� ���� ���� �������� ����Ͽ� �迭�� �����ϴ� �Լ�
        /// </summary>
        /// <param name="center">���� ��ġ��</param>
        /// <param name="positions">�� ��ġ ��ġ</param>
        /// <param name="angleList">���� ���� �迭</param>
        private void CalculateAngles(Vector2 center, Vector2[] positions, float[] angleList)
        {
            for (int i = 0; i < positions.Length; i++)
            {
                angleList[i] = (Mathf.Atan2(positions[i].x - center.x, positions[i].y - center.y) * 180) * PI_FORMULT;
            }
        }

        #region Angle
        /// <summary>
        /// ��ġ�� ���� ���� ���� �Լ�
        /// </summary>
        /// <param name="centerPos">���� ��ġ</param>
        /// <param name="positions">��ġ ��ġ �迭</param>
        protected void ChangeRotate(Vector2 centerPos, Vector2[] positions)
        {
            // �� ��ġ �� ���� ���� ������ ���
            float[] angles = new float[touchCount];
            CalculateAngles(centerPos, positions, angles);

            float deltaRotate = 0f; // ���� ������ ���� ���� ���̰�
            for (int i = 0; i < angles.Length; i++)
            {
                // ���� ���� ���� ���̰� ���� ���
                deltaRotate += angles[i] - angleListPrev[i];
                // ���� �������� ���� ���������� �̰�
                angleListPrev[i] = angles[i];
            }

            // ���� ���̰� ���� (���밪 ���� 180�� ���� �ʵ���)
            if (deltaRotate > 180) // 180�� �ʰ��� ��� -�� ������ ����
                deltaRotate = deltaRotate - 360f;
            else if (deltaRotate < -180) // -180 �̸��� ��� +�� ������ ����
                deltaRotate = 360f + deltaRotate;
            else if (deltaRotate.Equals(0)) // ������ ���̰� ���� ��� ����
                return;

            // ��� �������̰� ���
            deltaRotate = -deltaRotate * touchCountForMult;
            // ���� ����
            scaleTarget.Rotate(0f, 0f, deltaRotate);
        }
        #endregion
    }
}