using UnityEngine;

namespace SongDuKeyboardSpace
{
    public class KeyboardShotcut : MonoBehaviour
    {
        /// <summary>
        /// ���� Shift�� ���� ��� Uppercast ����
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
        /// ���ӿ�����Ʈ �� ��ũ��Ʈ ����
        /// </summary>
        public void DestroyThisObject()
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}