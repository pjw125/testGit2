using System.Collections;
using UnityEngine;

namespace SongDuTouchSpace
{
    public class TouchCenter : MonoBehaviour
    {

        protected bool TouchPause = false;
#if false // 임시 Hide
    public bool touchPause
    {
        set
        {
            this.TouchPause = value;
        }
    }

    private Transform _transformCache;
	public Transform transformCache {
		get {
			if (this._transformCache == null)
				this._transformCache = this.transform;

			return this._transformCache;
		}
	}

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
#endif

        public static Camera MainCam;

        private void Awake()
        {
            MainCam = GetComponent<Camera>();

            Application.targetFrameRate = 60;
#if UNITY_EDITOR
            Debug.LogFormat("Set Target FrameRate({0})", Application.targetFrameRate);
#endif

            StartCoroutine(coroutine_multiTouch());

            Singleton_Settings.getInstance.touchCenter = this;
        }

        private void Start()
        {
            if (!(Singleton_Settings.getInstance.isCompleteScreenInformation))
                // 화면 해상도가 제대로 설정되지 않았다면 다시 설정
                Singleton_Settings.getInstance.SettingScreenInformation(Screen.width, Screen.height);
        }

        public static Vector2 CheckTouchCoord(Vector2 touchPos)
        {
            Ray ray = TouchCenter.MainCam.ScreenPointToRay(touchPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                return hit.textureCoord;
            }

            return new Vector2(-1f, -1f);
        }

        private bool CheckTouchObject(Touch touchInfo)
        {

            Ray ray = MainCam.ScreenPointToRay(touchInfo.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //			Debug.DrawLine (ray.origin, hit.point);
                TouchParent touchTarget = hit.collider.GetComponent<TouchParent>();

                if (touchTarget)
                {
                    touchTarget.AddTouch(touchInfo.fingerId);
                    return true;
                }
            }

            return false;
        }

        private IEnumerator coroutine_multiTouch()
        {
            while (true)
            {
                yield return null;
                
                if (TouchPause)
                    continue;

                bool isBegan = false;
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (Input.touches[i].phase == TouchPhase.Began)
                    {
                        ActEventTouchBegan();
                        ActEventTouchBegan(Input.touches[i].position);
                        this.CheckTouchObject(Input.touches[i]);
                        isBegan = true;
                    }
                }

                if (!isBegan)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        ActEventTouchBegan();
                        ActEventTouchBegan(Input.mousePosition);
                        CheckMouseDownObject(MouseIndexLeft);
                    }

                    if (Input.GetMouseButtonDown(1))
                    {
                        ActEventTouchBegan();
                        ActEventTouchBegan(Input.mousePosition);
                        CheckMouseDownObject(MouseIndexRight);
                    }
                }
            }
        }

        public Vector2 GetInputTouchPosition(int idx)
        // replace 'Input.touches[i].position' -> 'GetInputTouchPosition(int idx)'
        {
            if (idx < 0 || idx >= Input.touchCount)
                return Vector2.zero;

            Vector2 pos = Input.touches[idx].position;
            if (transform.localEulerAngles.z != 0f)
            {
                pos.x = Screen.width - pos.x;
                pos.y = Screen.height - pos.y;
            }

            return pos;
        }

        #region Mouse Action
        // MOUSE CONTROL 검색
        public const int MouseIndexLeft = 100;
        private const int MouseIndexRight = 101;

        private bool CheckMouseDownObject(int mouseIndex)
        {
            Ray ray = MainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                bool isCorrectTouch = false;

                switch (mouseIndex)
                {
                    case MouseIndexLeft:
                        isCorrectTouch = ClickMouseLeft(hit.collider);
                        break;
                    case MouseIndexRight:
                        isCorrectTouch = ClickMouseRight(hit.collider);
                        break;
                }

                return isCorrectTouch;
            }

            return false;
        }

        private bool ClickMouseLeft(Collider col)
        {
            TouchParent target = col.GetComponent<TouchParent>();

            if (target)
            {
                target.AddTouch(MouseIndexLeft);
                return true;
            }

            return false;
        }

        private bool ClickMouseRight(Collider col)
        {
            MouseRightTarget target = col.GetComponent<MouseRightTarget>();

            if (target)
            {
                target.ActionMouseDown();
                return true;
            }

            return false;
        }
        #endregion

        public static Vector2 TouchPositionToUnityPosition(Vector2 touchPosition)
        {
            return new Vector2((touchPosition.x - (Singleton_Settings.getInstance.screenSize.x * 0.5f)) * Singleton_Settings.getInstance.WorldPosPerOnePixel
                , (touchPosition.y - (Singleton_Settings.getInstance.screenSize.y * 0.5f)) * Singleton_Settings.getInstance.WorldPosPerOnePixel);
        }
        public static void TouchPositionToUnityPosition(Vector2 touchPosition, out float x, out float y)
        {
            x = (touchPosition.x - (Singleton_Settings.getInstance.screenSize.x * 0.5f)) * Singleton_Settings.getInstance.WorldPosPerOnePixel;
            y = (touchPosition.y - (Singleton_Settings.getInstance.screenSize.y * 0.5f)) * Singleton_Settings.getInstance.WorldPosPerOnePixel;
        }
        public static Vector2 UnityPositionToTouchPosition(Vector2 unityPosition)
        {
            Vector2 halfScreenSize = Singleton_Settings.getInstance.screenSize * 0.5f;
            // UnityPosition 1당 TouchPosition(screenPosition)은 'halfScreenSize.y'와 동일하기 때문에
            // 받아온 unityPosition의 x, y값에 'halfScreenSize.y'를 곱해주어 계산
            return new Vector2((unityPosition.x * halfScreenSize.y) + halfScreenSize.x
                , (unityPosition.y * halfScreenSize.y) + halfScreenSize.y);
        }

        #region Touch Began
        /// <summary>
        /// 터치 입력 발생 시 실행 델리게이트/함수
        /// </summary>
        public event DelegateVoid Event_TouchBegan;
        private void ActEventTouchBegan()
        {
            if (Event_TouchBegan != null)
                Event_TouchBegan();
        }

        /// <summary>
        /// 터치 입력 발생 시 해당 터치의 위치 전달을 수행하는 델리게이트/함수
        /// </summary>
        public event DelegateVector2 Event_TouchBegan_Pos;
        private void ActEventTouchBegan(Vector2 v2)
        {
            if (Event_TouchBegan_Pos != null)
                Event_TouchBegan_Pos(v2);
        }
        #endregion
    }
}