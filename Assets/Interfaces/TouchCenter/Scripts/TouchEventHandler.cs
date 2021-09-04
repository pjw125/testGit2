using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Click Event Handler
namespace SongDuTouchSpace
{
	public class TouchEventHandler : TouchParent
	{

		public delegate void DelegateVoidToVoid();
		protected event DelegateVoidToVoid Event_Click;
		protected event DelegateVoidToVoid Event_Press;
		protected event DelegateVoidToVoid Event_Up;
		protected event DelegateVoidToVoid Event_DoubleClick;

		protected float fDoubleClickTime = 0.2f;

		#region add event
		public void AddEvent_Click(DelegateVoidToVoid e)
		{
			Event_Click += e;
		}
		public void AddEvent_Press(DelegateVoidToVoid e)
		{
			Event_Press += e;
		}
		public void AddEvent_Up(DelegateVoidToVoid e)
		{
			Event_Up += e;
		}
		public void AddEvent_DoubleClick(DelegateVoidToVoid e)
		{
			Event_DoubleClick += e;
		}
		#endregion

		Vector2 initPosition_Touch;

		[SerializeField] private TouchParent touchParent;

		protected IEnumerator touchRoutine;

		protected float DoubleTouchStartTime = -1f;

		private bool bClickState = false;

		protected void InitEventHandler()
		{
			this.InitTouchParent(StartTouch, EndTouch);
		}

		protected void SetTouchInfomation()
		{
			initPosition_Touch = GetTouchPosition(0);
		}

		public void SetTouchParent(TouchParent tp)
		{
			this.touchParent = tp;
		}

		private const float touchDist = 5f;
		private bool CheckDistance_SendTouchToParent(Vector2 position)
		{
			float dist = Mathf.Sqrt((position - initPosition_Touch).sqrMagnitude);

			if (dist > touchDist && touchCount > 0)
			{
				touchParent.AddTouch(touchIDs[0]);
				RemoveTouchUnusualy();

				return true;
			}

			return false;
		}

		private void SendAllTouchToParnet()
		{
			for (int i = 0; i < touchIDs.Count; i++)
			{
				touchParent.AddTouch(touchIDs[i]);
			}

			RemoveTouchUnusualy();
		}

		protected void DestroyTouchEvent()
		{
			if (touchRoutine != null)
				StopCoroutine(touchRoutine);

			touchRoutine = null;
		}

		private void StartTouch()
		{
			if (touchCount == 1)
			{
				bClickState = true;

				SetTouchInfomation();
				// 위 SetTouchInformation에서 GetPosition을 돌면서 터치가 바로 사라진 경우 touchCount가 0으로 변경될 수 있음
				// 그렇기 때문에 현 위치에서 touchCount가 0이라면 아래 내용을 수행하지 않음
				if (touchCount > 0)
				{
					touchRoutine = coroutine_touch();
					StartCoroutine(touchRoutine);

					if (Event_Press != null)
						Event_Press();
				}
			}
		}

		private void EndTouch()
		{
			if (touchCount <= 0)
			{
				if (touchRoutine != null)
				{
					StopCoroutine(touchRoutine);
				}

				// new
				if (Event_Up != null)
					Event_Up();

				if (bClickState)
				{
					if (Event_Click != null)
						Event_Click();

					float fTime = Time.time - this.DoubleTouchStartTime;
					if (this.DoubleTouchStartTime > 0f && fTime < fDoubleClickTime)
					{
						if (Event_DoubleClick != null)
							Event_DoubleClick();
					}
					this.DoubleTouchStartTime = Time.time;
				}

				// old
				//if (Event_Up != null)
				//    Event_Up();

				touchRoutine = null;
			}
		}

		public void RemoveTouchUnusualy()
		{
			bClickState = false;

			RemoveAllTouch();
		}

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
					this.CheckTouch();
				}
			}
		}

		private void CheckTouch()
		{
			Vector2 position = GetTouchPosition(0);

			if (position.x < 0)
				return;

			if (touchParent != null)
			{
				bool bSendParent = CheckDistance_SendTouchToParent(position);
				if (bSendParent)
					return;
			}

			bClickState = CheckTouchObject(position);
		}
	}
}