using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongDuTouchSpace
{
    /// <summary>
    /// �� ��ũ��Ʈ �߿�
    /// ��ġ�� ũ�� ����('ChangeScale' �Լ�) �� ������Ʈ ��ġ ��� ��ġ ��ġ�� �����
    /// ��ġ ��ġ�� �߽����� ũ�Ⱑ Ŀ������ �� Ŭ����
    /// ���� �ʿ�� ����ؾ� �ϱ� ������ ���ܵ�
    /// </summary>
    public class TouchMultipleNewExample : TouchParent
    {
        #region OnAwakeInit
        [SerializeField] private bool OnAwakeInit = false;
        private void Awake()
        {
            if (OnAwakeInit)
                InitializeThis();
        }
        #endregion

        /// <summary>
        /// ���� ��ġ ���� �� �߻� �̺�Ʈ ��������Ʈ
        /// </summary>
        private DelegateVoid delegate_InitiateTouch = null;
        public DelegateVoid DelegateInitiateTouch { set { this.delegate_InitiateTouch = value; } }

        /// <summary>
        /// ��� ��ġ ��Ʈ�� ���� �� �߻� �̺�Ʈ ��������Ʈ
        /// </summary>
        private DelegateVoid delegate_CompleteTouch = null;
        public DelegateVoid DelegateCompleteTouch { set { this.delegate_CompleteTouch = value; } }

        // ��ġ�̵� Ÿ�� ������Ʈ
        [SerializeField] protected Transform moveTarget;
        // ũ�⺯�� Ÿ�� ������Ʈ
        [SerializeField] protected Transform scaleTarget;
        // �ű� ��ġ �Է� �� �ʱ�ȭ �Ϸ� Ȯ�� ����
        protected bool bInitMultiTouch = false;

        /// <summary>
        /// �ʱ� ���� �Լ�
        /// </summary>
        public virtual void InitializeThis()
        {
            InitTouchParent(StartTouch, EndTouch);
        }

        /// <summary>
        /// ������Ʈ ���� �Լ�
        /// </summary>
        public virtual void DestroyObject()
        {
            // �������̴� ��ġ �ڷ�ƾ ����
            StopTouchCoroutine();

            Object.Destroy(this);
        }

        /// <summary>
        /// �Ѱ��� ��ġ �Է� �� �̺�Ʈ �Լ�
        /// </summary>
        protected virtual void StartTouch()
        {
            bInitMultiTouch = false; // ��ġ �Է� �ʱ� ���� �̿Ϸ� ���·� ����

            if (touchCount.Equals(1)) // ���� ��ġ�� ������ ���
            {
                StartTouchCoroutine(); // ��ġ �ڷ�ƾ ����

                if (delegate_InitiateTouch != null) // ���� ��ġ �̺�Ʈ ����
                    delegate_InitiateTouch();
            }
        }
        /// <summary>
        /// �Ѱ��� ��ġ ���� �� �̺�Ʈ �Լ�
        /// </summary>
        protected virtual void EndTouch()
        {
            if (touchCount <= 0) // ��� ��ġ�� ����� ���
            {
                StopTouchCoroutine(); // ��ġ �ڷ�ƾ ����

                if (delegate_CompleteTouch != null) // ��� ��ġ ���� �̺�Ʈ ����
                    delegate_CompleteTouch();
            }
        }

        #region Touch Action
        // ��ġ ���� �ڷ�ƾ ����
        private IEnumerator IE_Touch = null;
        /// <summary>
        /// ��ġ ���� �ڷ�ƾ �Լ�
        /// </summary>
        protected virtual IEnumerator Coroutine_Touch()
        {
            // ��ġ�� �ִ� ��� �ݺ�
            while (touchCount > 0)
            {
                yield return null;

                // �ý��ۻ� ��ġ�� ���� ���콺 ��Ŭ�� ���°� �ƴ� ���
                if (Input.touchCount.Equals(0) && !(Input.GetMouseButton(0)))
                {
                    // ��ġ ���� Ŭ���� �� ����
                    this.ClearTouch();
                    EndTouch();
                    continue;
                }

                if (bInitMultiTouch) // ��ġ �Է� �ʱ� ������ �� ���¶��
                    MultiTouch(); // ��ġ�� ���� ������Ʈ ���� �Լ� ����
                else // �ʱ⼳���� ���� ���� ���¶��
                    SetTouchInfomation(); // �ʱ� ���� ����
            }
        }
        /// <summary>
        /// ��ġ ���� �ڷ�ƾ ���� �Լ�
        /// </summary>
        private void StartTouchCoroutine()
        {
            StopTouchCoroutine();

            this.IE_Touch = Coroutine_Touch();
            StartCoroutine(this.IE_Touch);
        }
        /// <summary>
        /// ��ġ ���� �ڷ�ƾ ���� �Լ�
        /// </summary>
        private void StopTouchCoroutine()
        {
            if (this.IE_Touch != null)
            {
                StopCoroutine(this.IE_Touch);
                this.IE_Touch = null;
            }
        }
        #endregion

        #region Move
        protected float touchCountForMult = 0f; // ������ ��ġ �� (1 / TouchCount)
        protected Vector2 touchCenterPos = new Vector2(); // ���� ��ġ�� �߾� ��ġ
        protected Vector2 touchCenterPosPrev = new Vector2(); // ���� ��ġ�� �߾� ��ġ

        protected float fInitialDistance = 0f; // �ʱ� ��ġ�� ��� �Ÿ���
        protected float fInitialScale = 0f; // �ʱ� ������Ʈ ũ��

        /// <summary>
        /// ��ġ (�߰�) �Է� �� �⺻ ��ġ ������ �����ϴ� �Լ�
        /// </summary>
        protected virtual void SetTouchInfomation()
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

            this.scaleStorage = this.scaleTarget.localScale; // ũ�� ����� �ʱⰪ ����

            // ��ġ ���� �Ϸ� ó��
            bInitMultiTouch = true;
        }

        /// <summary>
        /// ��ġ�� ���� �̵� �Լ�
        /// </summary>
        protected virtual void MultiTouch()
        {
            Vector2[] positions = new Vector2[touchCount];
            if (!GetTouchPositions(positions, ref this.touchCenterPos)) // ��ġ ��ġ�� �������� �� ������ �߻��Ͽ��ٸ� ����
                return;

            // Scale
            if (touchCount > 1)
                ChangeScale(touchCenterPos, positions);

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

        #region Position
        /// <summary>
        /// ������Ʈ ��ġ
        /// </summary>
        public Vector3 localPosition
        {
            get { return this.moveTarget.localPosition; }
            set { this.moveTarget.localPosition = value; }
        }
        #endregion

        #region Scale
        [Header("Scale")]
        // �ּ� ũ�Ⱚ
        [SerializeField] protected float minimumScale = 0;
        public float MinimumScale { set { this.minimumScale = value; } }

        // �ִ� ũ�Ⱚ
        [SerializeField] protected float maximumScale = float.MaxValue;
        public float MaximumScale { set { this.maximumScale = value; } }

        // ũ�� �����
        protected Vector3 scaleStorage = Vector3.one;

        /// <summary>
        /// �ܺ� ������ ������Ʈ ũ�Ⱚ
        /// </summary>
        public float Scale { get { return this.scaleTarget.localScale.x; } }

        /// <summary>
        /// ��ġ�� ���� ũ�� ���� �Լ�
        /// </summary>
        /// <param name="centerPos">������ġ</param>
        /// <param name="positions">��ġ ��ġ �迭</param>
        protected void ChangeScale(Vector2 centerPos, Vector2[] positions)
        {
            float fChangedScale = CalculateDistanceAvg(centerPos, positions); // ��ġ ��� �Ÿ��� ����
            fChangedScale = (fChangedScale - fInitialDistance) * this.ratio; // ���� ��ġ�Ÿ� ���� ����� ��ġ �Ÿ� ����Ƽ��(* this.ratio)
            fChangedScale *= 2f; // �Ÿ��� ���� �����̴� 2��...�³�??...

            float fPrevScale = scaleStorage.x; // ���� ������ ũ�� ����

            this.SetScale(fInitialScale + fChangedScale); // ũ�� ����

            float fChangeRatio = scaleStorage.x / fPrevScale; // ���� ���� ũ�� ��� ���� ũ�� ����
            
            // ��ġ �߽� ��ġ ����Ƽ ������ȭ
            centerPos = TouchCenter.TouchPositionToUnityPosition(centerPos);

            // ������Ʈ ��ġ���� ��ġ ��ġ������ �Ÿ�
            centerPos.x = localPosition.x - centerPos.x;
            centerPos.y = localPosition.y - centerPos.y;

            // 
            float deltaX = (centerPos.x * fChangeRatio) - centerPos.x;
            float deltaY = (centerPos.y * fChangeRatio) - centerPos.y;

            //centerPos *= fChangeRatio;
            
            this.moveTarget.Translate(deltaX, deltaY, 0f);
        }
        /// <summary>
        /// Ÿ�� ũ�� ���� �Լ�
        /// </summary>
        /// <param name="scale">ũ�Ⱚ</param>
        protected virtual void SetScale(float scale)
        {
            // ũ�Ⱚ �ּ�/�ִ밪�� ���� ����
            if (scale < this.minimumScale)
                scale = this.minimumScale;
            else if (scale > this.maximumScale)
                scale = this.maximumScale;

            if (!(scale.Equals(scaleTarget.localScale.x)))
            // Ÿ���� ũ��� �������� �ϴ� ũ�Ⱑ �ٸ� ���
            {
                // ����ҿ� ũ�� ����
                scaleStorage.x = scale;
                scaleStorage.y = scale;
                // ũ�� ����
                scaleTarget.localScale = scaleStorage;
            }
        }
        /// <summary>
        /// �� ��ġ�� ���� ���� �Ÿ� ��� ��ȯ �Լ�
        /// </summary>
        /// <param name="center">���� ��ġ</param>
        /// <param name="positions">��ġ ��ġ</param>
        /// <returns>��ġ �Ÿ� ��հ�</returns>
        protected float CalculateDistanceAvg(Vector2 center, Vector2[] positions)
        {
            // ��� ����
            float result = 0f;

            for (int i = 0; i < positions.Length; i++)
            {
                // �� ��ġ�� ���Ͱ� �Ÿ� ���� ���
                result += Vector2.Distance(center, positions[i]);
            }

            // ��� ���
            result = result * touchCountForMult;

            return result;
        }
        #endregion

        #region GetTouchPositions
        /// <summary>
        /// �� ��ġ�� ��ġ�� �߾���ġ�� �����ϴ� �Լ�
        /// </summary>
        /// <param name="positions">��ġ �迭</param>
        /// <param name="centerPos">�߾� ��ġ��</param>
        /// <returns>���󿩺�</returns>
        protected bool GetTouchPositions(Vector2[] positions, ref Vector2 centerPos)
        {
            centerPos.Set(0f, 0f); // �߾� ��ġ �ʱ�ȭ

            // ��ġ ���� ����Ʈ
            List<int> removeList = new List<int>();

            for (int a = 0; a < touchCount; a++)
            {
                bool bCorrectTouch = false; // ��ġ ��ȿ��

                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (this.touchIDs[a].Equals(Input.touches[i].fingerId)) // ��ġid�� ������ ���
                    {
                        positions[a] = Input.touches[i].position; // �迭�� ��ġ ��ġ�� ����

                        if (Input.touches[i].phase == TouchPhase.Ended || Input.touches[i].phase == TouchPhase.Canceled)
                            // �ش� ��ġ�� ����/��� ������ ��� ���� ����Ʈ�� �߰�
                            removeList.Add(this.touchIDs[a]);

                        // ��ȿ ��ġ
                        bCorrectTouch = true;
                        break;
                    }
                }

                // ��ġ�� ��ȿ���� �����鼭 �ش� �ε����� ���콺 ��Ŭ���� ���
                if (!bCorrectTouch && (this.touchIDs[a] == TouchCenter.MouseIndexLeft))
                {
                    positions[a] = Input.mousePosition; // �迭�� ���콺 ��ġ ����

                    if (Input.GetMouseButtonUp(0) || !Input.GetMouseButton(0))
                        // ���콺 ��Ŭ���� ������ ���°ų� ���콺 ��Ŭ���� �������� �ʴٸ� �ش� ��ġ ���� ����Ʈ�� �߰�
                        removeList.Add(this.touchIDs[a]);

                    // ��ȿ ��ġ
                    bCorrectTouch = true;
                }

                if (bCorrectTouch) // ��ȿ��ġ�� ���
                {
                    // �ش� ��ġ ��ġ�� �߾� ��ġ ��ġ�� ���ϱ�
                    centerPos.x += positions[a].x;
                    centerPos.y += positions[a].y;
                }
                else // �ش� ��ġ�� ��ȿ���� ���� ��� ��ġ ���� ����Ʈ�� �߰�
                    removeList.Add(this.touchIDs[a]);
            }

            if (removeList.Count > 0) // ��ġ ���� ����Ʈ�� ���� �ִ� ���
            {
                bInitMultiTouch = false; // ��ġ �ʱ�ȭ�� ���� false�� ����
                for (int i = 0; i < removeList.Count; i++)
                {
                    // ���� ����Ʈ�� �ִ� ��� ��ġ ����
                    this.RemoveTouch(removeList[i]);
                }
                // ������ ��ȯ
                return false;
            }

            // �߾���ġ ����� ���� ��� ���
            centerPos *= touchCountForMult;
            // ���� ��ȯ
            return true;
        }
        #endregion

        /// <summary>
        /// ���� �ڵ�, ������� ���°� �˰ڴµ� ��� ������ ���� �𸣰ٴ�... Ȯ�� �ʿ�
        /// </summary>
#if false
        #region SimulationAngleAmend
        protected float deratioSimulAngle = 1f;
        protected float ratioSimulAngle = 0f;
        protected void InitAngleData()
        {
            float angle = targetObject.localEulerAngles.z;
            bool bOverAngle180 = false;
            if (angle > 180)
            {
                angle = angle - 360f;
                angle *= -1f;
                bOverAngle180 = true;
            }
            ratioSimulAngle = angle * 0.01111111f;
            deratioSimulAngle = 1f - ratioSimulAngle;
            if (ratioSimulAngle > 1f)
                ratioSimulAngle = 2f - ratioSimulAngle;
            if (bOverAngle180)
                ratioSimulAngle *= -1;
        }
        protected Vector2 CalculateDeltaPositionForAngle(Vector2 deltaPos)
        {
            Vector2 resultPos = new Vector2((deltaPos.x * deratioSimulAngle) + (deltaPos.y * ratioSimulAngle), (deltaPos.y * deratioSimulAngle) + (deltaPos.x * -ratioSimulAngle));

            return resultPos;
        }
        protected void CalculateDeltaPositionForAngle(ref Vector2 deltaPos)
        {
            deltaPos.x = (deltaPos.x * deratioSimulAngle) + (deltaPos.y * ratioSimulAngle);
            deltaPos.y = (deltaPos.y * deratioSimulAngle) + (deltaPos.x * -ratioSimulAngle);
        }
        #endregion
#endif
    }
}