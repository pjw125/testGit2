using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongDuTouchSpace
{
    public class TouchLensMover : TouchParent
    {
        private GameObject _gameObjectCache;
        private GameObject gameObjectCache
        {
            get
            {
                if (_gameObjectCache == null)
                    _gameObjectCache = this.gameObject;

                return _gameObjectCache;
            }
        }

        public event DelegateVoid event_StartTouch;

        [SerializeField] private Transform target;

        private void Awake()
        {
            this.InitTouchParent(StartTouch, EndTouch);

            this.InitTouchInformation();
            this.InitRigidbody();
        }

        private void StartTouch()
        {
            if (touchCount == 1)
            {
                SetTouchInfomation();

                // 위 SetTouchInformation에서 GetPosition을 돌면서 터치가 바로 사라진 경우 touchCount가 0으로 변경될 수 있음
                // 그렇기 때문에 현 위치에서 touchCount가 0이라면 아래 내용을 수행하지 않음
                if (touchCount > 0)
                    StartTouchCoroutine();

                if (event_StartTouch != null)
                    event_StartTouch();

                this.SetMass(true);
            }
        }

        private void EndTouch()
        {
            if (touchCount <= 0)
            {
                StopTouchCoroutine();

                CheckDoubleClick();

                this.SetMass(false);
            }
        }

        #region DoubleClick
        public DelegateVoid delegate_DoubleClick;
        private float DoubleTouchStartTime = -1f;
        [SerializeField] private float fDoubleClickTime = 0.5f;

        private void CheckDoubleClick()
        {
            float fTime = Time.time - this.DoubleTouchStartTime;
            if (this.DoubleTouchStartTime > 0f && fTime < fDoubleClickTime)
            {
                if (delegate_DoubleClick != null)
                    delegate_DoubleClick();

                this.DoubleTouchStartTime = -1;
            }
            else
                this.DoubleTouchStartTime = Time.time;
        }
        #endregion

        #region Coroutine
        private IEnumerator touchRoutine = null;
        private IEnumerator coroutine_touch()
        {

            while (touchCount > 0)
            {
                yield return null;

                if (Input.touchCount == 0 && !(Input.GetMouseButton(0)))
                {
                    this.ClearTouch();
                    EndTouch();
                    break;
                }

                if (touchCount == 1)
                {
                    this.MoveObject();
                }
            }
        }

        private void StartTouchCoroutine()
        {
            StopTouchCoroutine();

            touchRoutine = coroutine_touch();

            StartCoroutine(touchRoutine);
        }

        private void StopTouchCoroutine()
        {
            if (touchRoutine != null)
            {
                StopCoroutine(touchRoutine);
                touchRoutine = null;
            }
        }
        #endregion

        #region Control Move
        private Vector2 initPos_Touch;
        private Vector3 initPos_Object;
        private Vector3 posStorage;
        private void InitTouchInformation()
        {
            // 실시간 위치 저장 설정
            posStorage = target.localPosition;
        }
        private void SetTouchInfomation()
        {
            initPos_Touch = TouchPositionToUnityPosition(GetTouchPosition(0));
            initPos_Object = target.localPosition;
        }
        private void MoveObject()
        {
            // 터치 포지션 가져오고
            Vector2 position = GetTouchPosition(0);

            if (position.x < 0)
                return;

            // 터치포지션을 유니티 포지션으로 변경
            position = TouchPositionToUnityPosition(position);

            // 위 포지션값과 초기 터치 유니티포지션을 가지고 된 위치를 가져옴
            Vector2 movedPos = position - initPos_Touch;

            posStorage.x = initPos_Object.x + movedPos.x;
            posStorage.y = initPos_Object.y + movedPos.y;
            posStorage.z = target.localPosition.z;
            target.localPosition = posStorage;
        }
        #endregion

        #region Animate Move
        [Header("Animation")]
        [SerializeField] private float animateTime;
        [SerializeField] private iTween.EaseType easeType;
        private Vector3 differPos = new Vector3();
        public void SetAnimatePosition(float x, float y)
        {
            this.RemoveAllTouch();

            posStorage.Set(x, y, target.localPosition.z);
            differPos.Set(target.localPosition.x - posStorage.x, target.localPosition.y - posStorage.y, 0f);

            Singleton_Settings.iTweenControl(gameObjectCache, 1f, 0f, this.animateTime, this.easeType, "iTween_Animate", "iTween_Animate_Fin");
        }

        private void iTween_Animate(float v)
        {
            posStorage.z = target.localPosition.z;
            target.localPosition = posStorage + (differPos * v);
        }

        private void iTween_Animate_Fin()
        {
            iTween_Animate(0);
        }
        #endregion

        #region Rigidbody
        [SerializeField] private Rigidbody _rigidbody;
        private void InitRigidbody()
        {
            if (this._rigidbody == null)
                this._rigidbody = this.GetComponent<Rigidbody>();
        }
        private const float fMovedMass = 50f;
        private const float fUnmovedMass = 1f;
        private void SetMass(bool isMoved)
        {
            this.SetMass(isMoved ? fMovedMass : fUnmovedMass);
        }
        private void SetMass(float fMass)
        {
            if (_rigidbody.Equals(null))
                return;

            _rigidbody.mass = fMass;
        }
        #endregion

        #region SphereCollider
        [SerializeField] private SphereCollider sphereCol;
        public float SphereColRadius
        {
            get
            {
                if (this.sphereCol == null)
                    return 0f;

                return this.sphereCol.radius;
            }
            set
            {
                //if (!(this.sphereCol.Equals(null)))
                if (this.sphereCol != null)
                    this.sphereCol.radius = value;
            }
        }
        #endregion
    }
}