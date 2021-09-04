using System.IO;
using System.Collections;
using UnityEngine;

namespace AssetBundleCtrl
{
    /// <summary>
    /// ���¹����� �ε��ϴ� �θ� Ŭ����
    /// </summary>
    public abstract class AssetBundleLoaderParent : MonoBehaviour
    {
        /// <summary>
        /// �ε��� ���¹��� ������ �����ϴ� ���� ���
        /// </summary>
        protected abstract string AssetBundleFolderPath { get; }

        /// <summary>
        /// ���ϸ��� ���޹޾� ���ϰ�θ� ��ȯ�ϴ� �Լ�
        /// </summary>
        /// <param name="fileName">���ϸ�</param>
        /// <returns>���ϰ��</returns>
        protected string GetAssetBundleFilePath(string fileName)
        { return string.Format("{1}{0}{2}", DataSpliterGroup.folderPathSpliter, AssetBundleFolderPath, fileName); }

        /// <summary>
        /// ���ϸ��� ���޹޾� �ش� ������ ���� ���θ� Ȯ���ϴ� �Լ�
        /// </summary>
        /// <param name="fileName">���ϸ�</param>
        /// <returns>���翩��</returns>
        protected bool ExistAssetBundleFile(string fileName)
        {
            // ���ϸ��� ���� ���ϰ�� ��������
            string filePath = GetAssetBundleFilePath(fileName);
            // ���� ���� ���� ��ȯ
            return File.Exists(filePath);
        }

        /// <summary>
        /// ������ �ε� ���� �� ������ �Լ�
        /// </summary>
        protected abstract void FailLoadAssetBundle();

        /// <summary>
        /// �ε�� ���¹����� �����ϴ� �Լ�
        /// ��� �ÿ��� �ε�� ������Ʈ ���� �������ش�
        /// </summary>
        public virtual void DeleteLoadedAssetBundle()
        {
            if (loadedAssetBundle != null) // �ε�� ���¹����� �����ϴ� ���
            {
                loadedAssetBundle.Unload(true); // Unload ����(���� ������ ������Ʈ���� ����)
                loadedAssetBundle = null; // nulló��
            }
        }

        #region LoadCoroutine
        protected AssetBundle loadedAssetBundle = null; // �ε�� ���¹���
        private bool isBusy = false; // ���¹��� �ε� ����
        public bool IsBusy { get { return isBusy || (IE_Load != null); } } // ���¹��� �ε� ����

        // �ε带 �����ϴ� ���� IEnumerator
        private IEnumerator IE_Load = null;
        
        /// <summary>
        /// ���¹��� �ε� �Ͽ� 'loadedAssetBundle'�� ����ϴ� �Լ�
        /// </summary>
        /// <param name="filePath">���¹��� ���ϰ��</param>
        protected IEnumerator Coroutine_Load(string filePath)
        {
            // �ش� ��� ���¹��� �ε�
            AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(filePath);
            yield return req;

            // �ε�� ���¹��� ������ ����
            loadedAssetBundle = req.assetBundle;
        }
        /// <summary>
        /// �ε� �ڷ�ƾ �����Լ�
        /// </summary>
        /// <param name="filePath">���¹��� ���ϰ��</param>
        protected void StartLoadCoroutine(IEnumerator ie_load)
        {
            StopLoadCoroutine();

            this.isBusy = true;
            IE_Load = ie_load;
            StartCoroutine(IE_Load);
        }
        /// <summary>
        /// �ε� �ڷ�ƾ �����Լ�
        /// </summary>
        protected void StopLoadCoroutine()
        {
            if (IE_Load != null)
            {
                StopCoroutine(IE_Load);
                IE_Load = null;
            }

            this.isBusy = false;
        }
        #endregion
    }
}