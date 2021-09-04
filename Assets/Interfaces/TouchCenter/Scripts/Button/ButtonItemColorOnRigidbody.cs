using UnityEngine;

namespace SongDuTouchSpace
{
    /// <summary>
    /// rigidbody ������Ʈ ���� ��ư Ŭ���� (��������)
    /// - rigidbody ������Ʈ ���� ��ư�� ��� ��ġ�� hit�� transform Ȯ�� �� ��ư�� �ƴ� rigidbody ������Ʈ�� transform�� Ȯ�ε�
    /// - ������ hit�� collider Ȯ�� �� ��ư�� collider�� Ȯ�ε�
    /// - �Ͽ� CheckTouchObject�� override �Ͽ� transform Ȯ���� �ƴ� collider Ȯ���� ����ǵ��� �Ͽ� �����ذ��� Ŭ������
    /// </summary>
    public class ButtonItemColorOnRigidbody : ButtonItemColor
    {
        private Collider coll = null; // ��ġ üũ �ݸ���

        protected override void InitEventHandler()
        {
            // �ش� ��ư ������Ʈ�� �ݸ��� ����
            this.coll = this.GetComponent<Collider>();

            base.InitEventHandler();
        }

        protected override bool CheckTouchObject(Vector2 touchPosition, Camera TargetCamera)
        {
            Ray ray = TargetCamera.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Debug.DrawLine(ray.origin, hit.point);
                return hit.collider.Equals(this.coll);
            }
            else
                return false;
        }
    }
}