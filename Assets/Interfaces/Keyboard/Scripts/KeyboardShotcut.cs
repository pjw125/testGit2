using UnityEngine;

namespace SongDuKeyboardSpace
{
    public class KeyboardShotcut : MonoBehaviour
    {
        /// <summary>
        /// 우측 Shift가 눌린 경우 Uppercast 상태
        /// </summary>
        public bool IsUppercast
        { get { return Input.GetKey(KeyCode.LeftShift); } }

        private void Awake()
        {
            //Singleton_Settings.getInstance.ShotCut = this;
        }

        private void OnDisable()
        {
            Object.Destroy(this);
        }

        private void Update()
        {
            //Debug.LogFormat("Is Uppercast : {0}", IsUppercast ? "Yes!" : "No...");

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            { Debug.Log("Enter Down"); }
            if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))
            { Debug.Log("Enter Up"); }
        }

        /// <summary>
        /// 게임오브젝트 및 스크립트 삭제
        /// </summary>
        public void DestroyThisObject()
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}