using System.Collections;
using UnityEngine;

namespace UIControl
{
    public class MouseOverFollowItem : MonoBehaviour
    {
		#region ObjectCaches
		private GameObject _gameObjectCache;
		private GameObject gameObjectCache
		{
			get
			{
				if (this._gameObjectCache == null)
					this._gameObjectCache = this.gameObject;

				return this._gameObjectCache;
			}
		}
		public void SetActive(bool active)
        {
			gameObjectCache.SetActive(active);
        }

		private Transform _transformCache;
		private Transform transformCache
        {
            get
            {
                if (this._transformCache == null)
					this._transformCache = this.transform;

				return this._transformCache;
            }
        }
		#endregion


		private Vector2 minimumPos;
		private Vector2 maximumPos;

		[SerializeField] private Vector2 offset;
		[SerializeField] private Vector2 scale;

		/// <summary>
		/// 초기 설정 함수
		/// </summary>
		public void InitializeThis()
        {
			maximumPos = new Vector2((float)Screen.width / (float)Screen.height, 1f);
			minimumPos = new Vector2(-maximumPos.x, -maximumPos.y);
			
			Vector2 halfScale = scale * 0.5f;
			maximumPos.x -= halfScale.x;
			maximumPos.y -= halfScale.y;

			minimumPos.x += halfScale.x;
			minimumPos.y += halfScale.y;

			maximumPos.x -= offset.x;
			maximumPos.y -= offset.y;

			minimumPos.x -= offset.x;
			minimumPos.y -= offset.y;
        }

		private Vector3 posStorage = new Vector3();
		public void SetItemPosition(Vector2 pos)
        {
			// 제한값 보정
            if (pos.x < minimumPos.x)
				pos.x = minimumPos.x;
            else if (pos.x > maximumPos.x)
				pos.x = maximumPos.x;
            if (pos.y < minimumPos.y)
				pos.y = minimumPos.y;
            else if (pos.y > maximumPos.y)
				pos.y = maximumPos.y;

			posStorage.Set(pos.x, pos.y, transformCache.position.z);

			transformCache.position = posStorage;
        }
	}
}