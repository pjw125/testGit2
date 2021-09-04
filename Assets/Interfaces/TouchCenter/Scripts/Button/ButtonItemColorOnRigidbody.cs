using UnityEngine;

namespace SongDuTouchSpace
{
    /// <summary>
    /// rigidbody 오브젝트 내부 버튼 클래스 (색상조절)
    /// - rigidbody 오브젝트 내부 버튼의 경우 터치된 hit의 transform 확인 시 버튼이 아닌 rigidbody 오브젝트의 transform이 확인됨
    /// - 하지만 hit의 collider 확인 시 버튼의 collider가 확인됨
    /// - 하여 CheckTouchObject를 override 하여 transform 확인이 아닌 collider 확인이 진행되도록 하여 문제해결한 클래스임
    /// </summary>
    public class ButtonItemColorOnRigidbody : ButtonItemColor
    {
        private Collider coll = null; // 터치 체크 콜리더

        protected override void InitEventHandler()
        {
            // 해당 버튼 오브젝트의 콜리더 설정
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