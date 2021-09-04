using UnityEngine;

namespace SongDuTouchSpace
{
	public class TouchButton : TouchEventHandler
	{

		[SerializeField] protected GameObject eventTarget;
		[SerializeField] private string functionName;

		[SerializeField] private int idx = -1;

		private void Awake()
		{
			this.InitEventHandler();

			Event_Click += Clicked;
		}

		private void OnDisable()
		{
			RemoveAllTouch();
		}

		private void OnDestroy()
		{
			DestroyTouchEvent();

			Destroy(this);
		}

		public void SetEventTarget(GameObject et)
		{
			eventTarget = et;
		}

		public void SetEventTarget(GameObject et, string _functionName)
		{
			SetEventTarget(et);

			this.functionName = _functionName;
		}

		public void SetEventTarget(GameObject et, string _functionName, int paramIdx)
		{
			SetEventTarget(et, _functionName);

			this.idx = paramIdx;
		}

		protected void Clicked()
		{
			if (eventTarget != null)
			{
				if (idx < 0)
					eventTarget.SendMessage(functionName);
				else
					eventTarget.SendMessage(functionName, idx);
			}
		}
	}
}