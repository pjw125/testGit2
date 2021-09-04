using System.IO;
using System.Collections;
using UnityEngine;

namespace AssetBundleCtrl
{
    public class AssetBundleLoaderPrefab : AssetBundleLoaderParent
    {
        #region Override
        private readonly string AssetBundleRowRankFolderPath // 컨텐츠 에셋번들 하위 폴더 경로
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
        /// 파일명, 프리팹명을 받아 프리팹을 로드하는 함수
        /// </summary>
        /// <param name="fileName">파일명</param>
        /// <param name="prefabName">프리팹명</param>
        private void LoadPrefabInAssetBundle(string fileName, string prefabName)
        {
            IEnumerator ie = Coroutine_LoadPrefab(fileName, prefabName); // 로드 코루틴 생성
            StartLoadCoroutine(ie); // 코루틴 실행
        }

        /// <summary>
        /// 파일명, 프리팹명을 받아 프리팹을 로드하는 코루틴 함수
        /// </summary>
        /// <param name="fileName">파일명</param>
        /// <param name="prefabName">프리팹명</param>
        /// <returns></returns>
        private IEnumerator Coroutine_LoadPrefab(string fileName, string prefabName)
        {
            // 파일명에 따른 파일경로 가져오기
            string filePath = GetAssetBundleFilePath(fileName);
            
            if (File.Exists(filePath)) // 에셋번들 파일이 존재하지 않다면
            {
                // 에셋번들 로드 진행
                yield return Coroutine_Load(filePath);

                if (loadedAssetBundle != null) // 로드된 에셋번들이 존재한다면
                {
                    AssetBundleRequest assetbundleReq = loadedAssetBundle.LoadAssetAsync(prefabName, typeof(GameObject));
                    yield return assetbundleReq;

                    // 전달받은 오브젝트를 게임오브젝트화
                    GameObject gobj = Instantiate(assetbundleReq.asset, this.transform) as GameObject;
                    loadedObj = gobj;
                }
            }
            else
                FailLoadAssetBundle(); // 실패함수를 실행하고

            StopLoadCoroutine(); // 해당 코루틴을 종료한다
        }
    }
}