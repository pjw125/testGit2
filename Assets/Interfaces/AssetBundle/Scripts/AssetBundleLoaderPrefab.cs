using System.IO;
using System.Collections;
using UnityEngine;

namespace AssetBundleCtrl
{
    public class AssetBundleLoaderPrefab : AssetBundleLoaderParent
    {
        #region Override
        private readonly string AssetBundleRowRankFolderPath // ������ ���¹��� ���� ���� ���
            = string.Format("Contents{0}VC", DataSpliterGroup.folderPathSpliter);
        protected override string AssetBundleFolderPath
        {
            get
            {
                return string.Format("{1}{0}{2}", DataSpliterGroup.folderPathSpliter
                    , Singleton_Settings.ExternalResourcesPath, AssetBundleRowRankFolderPath);
            }
        }
        protected override void FailLoadAssetBundle()
        {
            Debug.LogErrorFormat("ERROR (AssetBundleLoaderPrefab.cs) Fail Load AssetBundle");
        }

        public override void DeleteLoadedAssetBundle()
        {
            if (loadedObj != null)
                GameObject.Destroy(loadedObj);

            base.DeleteLoadedAssetBundle();
        }
        #endregion

        [SerializeField] private GameObject loadedObj = null;

        private void Awake()
        {
            LoadPrefabInAssetBundle("test", "VisionCheck_CDN");
        }

        /// <summary>
        /// ���ϸ�, �����ո��� �޾� �������� �ε��ϴ� �Լ�
        /// </summary>
        /// <param name="fileName">���ϸ�</param>
        /// <param name="prefabName">�����ո�</param>
        private void LoadPrefabInAssetBundle(string fileName, string prefabName)
        {
            IEnumerator ie = Coroutine_LoadPrefab(fileName, prefabName); // �ε� �ڷ�ƾ ����
            StartLoadCoroutine(ie); // �ڷ�ƾ ����
        }

        /// <summary>
        /// ���ϸ�, �����ո��� �޾� �������� �ε��ϴ� �ڷ�ƾ �Լ�
        /// </summary>
        /// <param name="fileName">���ϸ�</param>
        /// <param name="prefabName">�����ո�</param>
        /// <returns></returns>
        private IEnumerator Coroutine_LoadPrefab(string fileName, string prefabName)
        {
            // ���ϸ� ���� ���ϰ�� ��������
            string filePath = GetAssetBundleFilePath(fileName);
            
            if (File.Exists(filePath)) // ���¹��� ������ �������� �ʴٸ�
            {
                // ���¹��� �ε� ����
                yield return Coroutine_Load(filePath);

                if (loadedAssetBundle != null) // �ε�� ���¹����� �����Ѵٸ�
                {
                    AssetBundleRequest assetbundleReq = loadedAssetBundle.LoadAssetAsync(prefabName, typeof(GameObject));
                    yield return assetbundleReq;

                    // ���޹��� ������Ʈ�� ���ӿ�����Ʈȭ
                    GameObject gobj = Instantiate(assetbundleReq.asset, this.transform) as GameObject;
                    loadedObj = gobj;
                }
            }
            else
                FailLoadAssetBundle(); // �����Լ��� �����ϰ�

            StopLoadCoroutine(); // �ش� �ڷ�ƾ�� �����Ѵ�
        }
    }
}