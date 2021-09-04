using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SongDuTouchSpace
{
	public class TouchSender : TouchParent
	{
		[SerializeField] private TouchParent touchParent;

		private void Awake()
		{
			InitEventHandler();
		}

		public void SetTouchParent(TouchParent parent)
		{
			touchParent = parent;
		}

		protected void InitEventHandler()
		{
			this.InitTouchParent(StartTouch, null);
		}

		private void StartTouch()
		{
			if (touchParent == null)
			{
				RemoveAllTouch();
				return;
			}

			if (touchCount == 1)
			{
				touchParent.AddTouch(touchIDs[0]);

				this.ClearTouch();
			}
		}
	}
}