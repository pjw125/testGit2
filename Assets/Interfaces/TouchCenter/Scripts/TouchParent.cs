using System.Collections.Generic;
using UnityEngine;

namespace SongDuTouchSpace
{
	public class TouchParent : MonoBehaviour
	{
		// 하위에서 계산에 사용될 PI 나눗셈값
		protected const float PI_FORMULT = 0.31830989f; // 1 / 3.14159265( <- PI)

		private Transform _transformCache;
		protected Transform transformCache
		{
			get
			{
				if (_transformCache == null)
					_transformCache = this.transform;

				return _transformCache;
			}
		}

		protected float ratio; // 1픽셀 당 유니티 크기/위치값

		[SerializeField] protected int MaxTouchCount = 1; // 최대 터치 수

		
		public DelegateVoid TouchStart; // 하나의 터치 입력 시 수행되는 델리게이트
		public DelegateVoid TouchEnd; // 하나의 터치 종료 시 수행되는 델리게이트

		protected List<int> touchIDs = new List<int>();

		protected int touchCount = 0;
		public int tCount { get { return touchCount; } }

		protected void InitTouchParent()
		// call child scripts 'Awake ()'
		{
			ratio = Singleton_Settings.getInstance.WorldPosPerOnePixel;
		}

		protected void InitTouchParent(DelegateVoid startDel, DelegateVoid endDel)
		// call child scripts 'Awake ()'
		{
			ratio = Singleton_Settings.getInstance.WorldPosPerOnePixel;

			TouchStart = startDel;
			TouchEnd = endDel;
		}

		/// <summary>
		/// Touch 정보를 받아 메인카메라 기준 터치위치의 오브젝트가 해당 오브젝트인지 여부를 반환하는 함수
		/// </summary>
		/// <param name="touchInfo">Touch 정보</param>
		/// <returns>해당 오브젝트 터치 여부</returns>
		protected bool CheckTouchObject(Touch touchInfo)
		{
			return this.CheckTouchObject(touchInfo.position);
		}

		/// <summary>
		/// 메인카메라 기준 터치위치의 오브젝트가 해당 오브젝트인지 여부를 반환하는 함수
		/// </summary>
		/// <param name="touchPosition">터치 위치</param>
		/// <returns>해당 오브젝트 터치 여부</returns>
		protected bool CheckTouchObject(Vector2 touchPosition)
		{
			return CheckTouchObject(touchPosition, TouchCenter.MainCam);
		}

		/// <summary>
		/// 타겟카메라 기준 터치위치의 오브젝트가 해당 오브젝트인지 여부를 반환하는 함수
		/// </summary>
		/// <param name="touchPosition">터치 위치</param>
		/// <param name="TargetCamera">타겟카메라</param>
		/// <returns>해당 오브젝트 터치 여부</returns>
		protected virtual bool CheckTouchObject(Vector2 touchPosition, Camera TargetCamera)
		{
			Ray ray = TargetCamera.ScreenPointToRay(touchPosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit))
			{
				//Debug.DrawLine(ray.origin, hit.point);
				if (hit.transform == transformCache)
					return true;
			}

			return false;
		}

#if false
		// 사용되지 않는 함수
		protected bool CheckTouchObject(Vector2 touchPosition, Camera TargetCamera, string targetName)
		{
			Ray ray = TargetCamera.ScreenPointToRay(touchPosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit))
			{
				//          Debug.DrawLine (ray.origin, hit.point);

				if (hit.transform.name == targetName)
					return true;
			}

			return false;
		}
#endif

		/// <summary>
		/// 터치 ID를 받아 해당 오브젝트 터치(touchIDs)로 등록하는 함수
		/// </summary>
		/// <param name="fingerID">터치 ID</param>
		public virtual void AddTouch(int fingerID)
		{
			if (touchIDs.Count < MaxTouchCount)
			{
				this.touchIDs.Add(fingerID);

				touchCount++;

				if (TouchStart != null)
					TouchStart();
			}
		}

		/// <summary>
		/// 특정 터치 ID를 삭제하는 함수
		/// </summary>
		/// <param name="fingerID">특정 터치 ID</param>
		/// <returns>삭제 성공 여부</returns>
		public virtual bool RemoveTouch(int fingerID)
		{
			int removeIdx = -1;

			for (int i = 0; i < touchCount; i++)
			{
				if (touchIDs[i] == fingerID)
				{
					removeIdx = i;
					break;
				}
			}
			touchIDs.Remove(fingerID);

			if (removeIdx == -1)
				return false;

			touchCount--;

			if (TouchEnd != null)
				TouchEnd();

			return true;
		}

		/// <summary>
		/// 모든 터치를 삭제하는 함수, 삭제 후 Feedback(TouchEnd) 수행
		/// </summary>
		public void RemoveAllTouch()
		{
			// RemoveAllTouch 시 삭제될만한 터치가 있는지 확인
			bool isRemoved = (touchIDs.Count > 0);

			for (int i = 0; i < touchIDs.Count; i++)
			{
				touchIDs.Remove(touchIDs[i]);
			}

			touchCount = 0;

			if (isRemoved)
			// 삭제된 터치가 있는 경우에만
			{
				// End 수행
				if (TouchEnd != null)
					TouchEnd();
			}
		}
		/// <summary>
		/// 모든 터치를 삭제하는 함수, 삭제 후 Feedback(TouchEnd) 수행하지 않음
		/// </summary>
		public void RemoveAllTouchNoneFeedback()
        {
			for (int i = 0; i < touchIDs.Count; i++)
			{
				touchIDs.Remove(touchIDs[i]);
			}

			touchCount = 0;
		}

		public void ClearTouch()
		{
			touchIDs.Clear();

			touchCount = 0;
		}

		public bool ContainTouchID(int touchID)
		{
			return touchIDs.Contains(touchID);
		}

		/// <summary>
		/// 해당 인덱스 터치의 위치값을 반환하는 함수
		/// </summary>
		/// <param name="idx">인덱스</param>
		/// <returns>위치값</returns>
		protected Vector2 GetTouchPosition(int idx)
		{
			Vector2 position = new Vector2(-1, -1);

			bool bCorrectTouch = false; // 유효 터치 확인

			for (int i = 0; i < Input.touchCount; i++)
			{
				if (this.touchIDs[idx].Equals(Input.touches[i].fingerId)) // 해당 인덱스의 터치와 동일한 터치인 경우
				{
					if (Input.touches[i].phase == TouchPhase.Ended || Input.touches[i].phase == TouchPhase.Canceled)
						// 해당 터치가 종료/취소 단계인 경우 해당 터치 삭제
						this.RemoveTouch(touchIDs[idx]);
					else
						// 아니라면 해당 터치 인덱스 위치 설정
						position = Input.touches[i].position;

					// 유효 터치
					bCorrectTouch = true;
					break;
				}
			}

			// 터치가 유효하지 않으면서 해당 인덱스가 마우스 좌클릭인 경우
			if (!bCorrectTouch && (this.touchIDs[idx].Equals(TouchCenter.MouseIndexLeft)))
			{
				if (Input.GetMouseButtonUp(0) || !Input.GetMouseButton(0))
					// 마우스 좌클릭이 떨어진 상태거나 마우스 좌클릭이 눌려있지 않다면 해당 터치 삭제
					this.RemoveTouch(touchIDs[idx]);
				else
					// 마우스 위치 설정
					position = Input.mousePosition;

				// 유효 터치
				bCorrectTouch = true;
			}

			if (!bCorrectTouch) // 해당 인덱스의 터치가 유효하지 않은 경우 터치 삭제
				this.RemoveTouch(touchIDs[idx]);

			// 위치 반환
			return position;
		}

		/// <summary>
		/// 해당 오브젝트에 터치되어있는 모든 위치값을 가져오는 함수, 정상여부 반환
		/// </summary>
		/// <param name="positions">터치 위치 배열</param>
		/// <returns>정상여부</returns>
		protected bool GetTouchPositions(ref Vector2[] positions)
		{
			if (!(positions.Length.Equals(touchCount))) // 배열수와 터치수가 다른 경우
				return false; // 비정상 반환

			// 삭제 터치 리스트 생성
			List<int> removeList = new List<int>();

			for (int a = 0; a < touchCount; a++)
			{
				bool bCorrectTouch = false;

				for (int i = 0; i < Input.touchCount; i++)
				{
					if (this.touchIDs[a].Equals(Input.touches[i].fingerId)) // 각 터치id가 동일한 경우
					{
						positions[a] = Input.touches[i].position; // 배열에 터치 위치값 설정

						if (Input.touches[i].phase == TouchPhase.Ended || Input.touches[i].phase == TouchPhase.Canceled)
							// 해당 터치가 종료/취소 상태인 경우 삭제 리스트에 추가
							removeList.Add(this.touchIDs[a]);

						// 유효 터치
						bCorrectTouch = true;
						break;
					}
				}

				// 터치가 유효하지 않으면서 해당 인덱스가 마우스 좌클릭인 경우
				if (!bCorrectTouch && (this.touchIDs[a] == TouchCenter.MouseIndexLeft))
				{
					positions[a] = Input.mousePosition; // 배열에 마우스 위치 설정

					if (Input.GetMouseButtonUp(0) || !Input.GetMouseButton(0))
						// 마우스 좌클릭이 떨어진 상태거나 마우스 좌클릭이 눌려있지 않다면 해당 터치 삭제 리스트에 추가
						removeList.Add(this.touchIDs[a]);

					// 유효 터치
					bCorrectTouch = true;
				}

				if (!bCorrectTouch) // 해당 터치가 유효하지 않은 경우 터치 삭제 리스트에 추가
					removeList.Add(this.touchIDs[a]);
			}

			if (removeList.Count > 0) // 터치 삭제 리스트에 값이 있는 경우
			{
				for (int i = 0; i < removeList.Count; i++)
				{
					// 삭제 리스트에 있는 모든 터치 삭제
					this.RemoveTouch(removeList[i]);
				}
				return false; // 비정상 반환
			}

			// 정상 반환
			return true;
		}

#if false
		/// 이거 왜 만든건지 모르겠음.. 과거의 나... 왜 그랬니...
		protected bool GetTouchPositions(ref Vector2[] positions, ref float[] radius)
		{
			if (positions.Length != touchCount)
				return false;

			List<int> removeList = new List<int>();

			for (int a = 0; a < touchCount; a++)
			{
				bool bCorrectTouch = false;
				for (int i = 0; i < Input.touchCount; i++)
				{
					if (this.touchIDs[a] == Input.touches[i].fingerId)
					{
						positions[a] = Input.touches[i].position;
						radius[a] = Input.touches[i].radius;

						if (Input.touches[i].phase == TouchPhase.Ended || Input.touches[i].phase == TouchPhase.Canceled)
						{
							removeList.Add(this.touchIDs[a]);
						}

						bCorrectTouch = true;
						break;
					}
				}

				if (!bCorrectTouch)
					removeList.Add(this.touchIDs[a]);
			}

			if (removeList.Count > 0)
			{
				for (int i = 0; i < removeList.Count; i++)
				{
					this.RemoveTouch(removeList[i]);
				}
				return false;
			}

			return true;
		}
#endif

		/// <summary> TouchPositionToUnityPosition ()
		/// Setting Main Cmaera
		/// Projection : Orthographic
		/// Size : 1
		/// </summary>
		protected Vector2 TouchPositionToUnityPosition(Vector2 touchPosition)
		{
			return new Vector2((touchPosition.x - (Singleton_Settings.getInstance.screenSize.x * 0.5f)) * Singleton_Settings.getInstance.WorldPosPerOnePixel
				, (touchPosition.y - (Singleton_Settings.getInstance.screenSize.y * 0.5f)) * Singleton_Settings.getInstance.WorldPosPerOnePixel);
		}
		protected void TouchPositionToUnityPosition(ref Vector2 touchPosition)
		{
			// recycle variable
			touchPosition.x = (touchPosition.x - (Singleton_Settings.getInstance.screenSize.x * 0.5f)) * Singleton_Settings.getInstance.WorldPosPerOnePixel;
			touchPosition.y = (touchPosition.y - (Singleton_Settings.getInstance.screenSize.y * 0.5f)) * Singleton_Settings.getInstance.WorldPosPerOnePixel;
		}

		/// Legacy Code
#if false
		// Edit By Song.Du - 20181018 - Because Send Touch Info 'TouchButtonOnRT' to Screen Touch(TouchMoveObject)
		// 더 이상 렌더텍스처에서의 버튼터치가 사용되지 않음
		protected void InitTouchParent(defaultDelegate startDel, defaultDelegate startDel_compulsory, defaultDelegate endDel)
		// call child scripts 'Awake ()'
		{
			ratio = Singleton_Settings.worldPos_perOnePixel;

			TouchStart = startDel;
			TouchStart_Compulsory = startDel_compulsory;
			TouchEnd = endDel;
		}
		public defaultDelegate TouchStart_Compulsory;
		public void AddTouch_Compulsory(int fingerID)
		{
			if (touchIDs.Count < MaxTouchCount)
			{
				if (TouchStart_Compulsory != null)
				{
					this.touchIDs.Add(fingerID);

					touchCount++;

					TouchStart_Compulsory();
				}
			}
		}
#endif
	}
}