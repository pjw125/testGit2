using System.IO;
using System.Collections;
using UnityEngine;

namespace AssetBundleCtrl
{
    /// <summary>
    /// 에셋번들을 로드하는 부모 클래스
    /// </summary>
    public abstract class AssetBundleLoaderParent : MonoBehaviour
    {
        /// <summary>
        /// 로드할 에셋번들 파일이 존재하는 폴더 경로
        /// </summary>
        protected abstract string AssetBundleFolderPath { get; }

        /// <summary>
        /// 파일명을 전달받아 파일경로를 반환하는 함수
        /// </summary>
        /// <param name="fileName">파일명</param>
        /// <returns>파일경로</returns>
        protected string GetAssetBundleFilePath(string fileName)
        { return string.Format("{1}{0}{2}", DataSpliterGroup.folderPathSpliter, AssetBundleFolderPath, fileName); }

        /// <summary>
        /// 파일명을 전달받아 해당 파일의 존재 여부를 확인하는 함수
        /// </summary>
        /// <param name="fileName">파일명</param>
        /// <returns>존재여부</returns>
        protected bool ExistAssetBundleFile(string fileName)
        {
            // 파일명을 통해 파일경로 가져오기
            string filePath = GetAssetBundleFilePath(fileName);
            // 파일 존재 여부 반환
            return File.Exists(filePath);
        }

        /// <summary>
        /// 프리팹 로드 실패 시 수행할 함수
        /// </summary>
        protected abstract void FailLoadAssetBundle();

        /// <summary>
        /// 로드된 에셋번들을 삭제하는 함수
        /// 상속 시에는 로드된 오브젝트 또한 삭제해준다
        /// </summary>
        public virtual void DeleteLoadedAssetBundle()
        {
            if (loadedAssetBundle != null) // 로드된 에셋번들이 존재하는 경우
            {
                loadedAssetBundle.Unload(true); // Unload 진행(관련 생성된 오브젝트까지 삭제)
                loadedAssetBundle = null; // null처리
            }
        }

        #region LoadCoroutine
        protected AssetBundle loadedAssetBundle = null; // 로드된 에셋번들
        private bool isBusy = false; // 에셋번들 로드 상태
        public bool IsBusy { get { return isBusy || (IE_Load != null); } } // 에셋번들 로드 상태

        // 로드를 수행하는 메인 IEnumerator
        private IEnumerator IE_Load = null;
        
        /// <summary>
        /// 에셋번들 로드 하여 'loadedAssetBundle'에 등록하는 함수
        /// </summary>
        /// <param name="filePath">에셋번들 파일경로</param>
        protected IEnumerator Coroutine_Load(string filePath)
        {
            // 해당 경로 에셋번들 로드
            AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(filePath);
            yield return req;

            // 로드된 에셋번들 변수에 적용
            loadedAssetBundle = req.assetBundle;
        }
        /// <summary>
        /// 로드 코루틴 실행함수
        /// </summary>
        /// <param name="filePath">에셋번들 파일경로</param>
        protected void StartLoadCoroutine(IEnumerator ie_load)
        {
            StopLoadCoroutine();

            this.isBusy = true;
            IE_Load = ie_load;
            StartCoroutine(IE_Load);
        }
        /// <summary>
        /// 로드 코루틴 종료함수
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