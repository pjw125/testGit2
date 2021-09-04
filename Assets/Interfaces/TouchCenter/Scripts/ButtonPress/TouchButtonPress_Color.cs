using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SongDuTouchSpace
{
	/// <summary>
	/// WebGL의 커서잠금 및 전체화면모드 수행은 버튼의 수행 첫 액션에서는 수행되지 않기 때문에
	/// Press액션으로 동작(동작예약)시키고 Up액션에서 실제 동작이 수행되도록 하기 위해
	/// Press액션으로 버튼을 연결해야함
	/// </summary>

	public class TouchButtonPress_Color : TouchButton
	{
		private GameObject _gameObjectCache;
		public GameObject gameObjectCache
		{
			get
			{
				if (this._gameObjectCache == null)
					this._gameObjectCache = this.gameObject;

				return this._gameObjectCache;
			}
		}

		[SerializeField] private SpriteRenderer[] spriteRenderer;
		[SerializeField] private TextMesh[] textMesh;
		[SerializeField] private TextMeshPro[] textMeshPro;

		private bool bOn = false;

		[SerializeField] private Color[] normalColor;
		[SerializeField] private Color[] pressColor;

		[SerializeField] private float SensChangedTime = 0f;
		private float colorRatio = 0f;

		private Color[] differColor;

		private void Awake()
		{
			this.InitEventHandler();

			this.AddEvent_Press(Clicked); // 동작 수행부를 Press이벤트에 연결
			this.AddEvent_Press(Button_Pressed);
			this.AddEvent_Up(Button_Up);

			differColor = new Color[normalColor.Length];
			for (int i = 0; i < differColor.Length; i++)
			{
				differColor[i] = pressColor[i] - normalColor[i];
			}
		}

		private void OnDestroy()
		{
			DestroyTouchEvent();
			Destroy(this);
		}

		private void ChangeColor_immediately(bool bPressed)
		{
			this.colorRatio = bPressed ? 1f : 0f;

			Color[] applyColor = bPressed ? pressColor : normalColor;

			for (int i = 0; i < spriteRenderer.Length; i++)
			{
				spriteRenderer[i].color = applyColor[i];
			}

			for (int i = 0; i < textMesh.Length; i++)
			{
				int textIdx = i + spriteRenderer.Length;
				textMesh[i].color = applyColor[textIdx];
			}

			for (int i = 0; i < textMeshPro.Length; i++)
			{
				int tmproIdx = i + spriteRenderer.Length + textMesh.Length;
				textMeshPro[i].color = applyColor[tmproIdx];
			}
		}

		private void Button_Pressed()
		{
			this.bPressed = true;

			if (bOn)
				return;

			if (SensChangedTime > 0f && (gameObjectCache.activeSelf && gameObjectCache.activeInHierarchy))
			{
				StartChangeCoroutine();
			}
			else
			{
				ChangeColor_immediately(true);
			}
		}

		private void Button_Up()
		{
			this.bPressed = false;

			if (bOn)
				return;

			if (SensChangedTime > 0f && (gameObjectCache.activeSelf && gameObjectCache.activeInHierarchy))
			{
				StartChangeCoroutine();
			}
			else
			{
				ChangeColor_immediately(false);
			}
		}

		public void ToggleActive(bool _bOn)
		{
			this.bOn = _bOn;

			if (this.bOn)
			{
				StopChangeCoroutine();

				ChangeColor_immediately(true);
			}
			else
			{
				if (!bPressed)
					Button_Up();
			}
		}

		private bool bPressed = false;
		private IEnumerator ie_ChangeColor;
		private IEnumerator Coroutine_ChangeColor()
		{
			while (true)
			{
				yield return null;

				if (bPressed)
					colorRatio += SensChangedTime * Time.deltaTime;
				else
					colorRatio -= SensChangedTime * Time.deltaTime;

				if (colorRatio < 0f)
				{
					colorRatio = 0f;
					break;
				}
				else if (colorRatio > 1f)
				{
					colorRatio = 1f;
					break;
				}

				for (int i = 0; i < spriteRenderer.Length; i++)
				{
					spriteRenderer[i].color = normalColor[i] + (differColor[i] * colorRatio);
				}
				for (int i = 0; i < textMesh.Length; i++)
				{
					int textIdx = i + spriteRenderer.Length;
					textMesh[i].color = normalColor[textIdx] + (differColor[textIdx] * colorRatio);
				}
				for (int i = 0; i < textMeshPro.Length; i++)
				{
					int tmproIdx = i + spriteRenderer.Length + textMesh.Length;
					textMeshPro[i].color = normalColor[tmproIdx] + (differColor[tmproIdx] * colorRatio);
				}
			}

			for (int i = 0; i < spriteRenderer.Length; i++)
			{
				spriteRenderer[i].color = normalColor[i] + (differColor[i] * colorRatio);
			}
			for (int i = 0; i < textMesh.Length; i++)
			{
				int textIdx = i + spriteRenderer.Length;
				textMesh[i].color = normalColor[textIdx] + (differColor[textIdx] * colorRatio);
			}
			for (int i = 0; i < textMeshPro.Length; i++)
			{
				int tmproIdx = i + spriteRenderer.Length + textMesh.Length;
				textMeshPro[i].color = normalColor[tmproIdx] + (differColor[tmproIdx] * colorRatio);
			}
		}

		private void StartChangeCoroutine()
		{
			StopChangeCoroutine();

			ie_ChangeColor = Coroutine_ChangeColor();
			StartCoroutine(ie_ChangeColor);
		}

		private void StopChangeCoroutine()
		{
			if (ie_ChangeColor != null)
			{
				StopCoroutine(ie_ChangeColor);

				ie_ChangeColor = null;
			}
		}

		public void SetTextMeshProText(int idx, string txt)
		{
			if (idx < 0 || idx >= textMeshPro.Length)
				return;

			textMeshPro[idx].text = txt;
		}

		public void SetColors(Color[] nColor, Color[] pColor)
		{
			this.normalColor = nColor;
			this.pressColor = pColor;

			differColor = new Color[normalColor.Length];
			for (int i = 0; i < differColor.Length; i++)
			{
				differColor[i] = pressColor[i] - normalColor[i];
			}

			for (int i = 0; i < spriteRenderer.Length; i++)
			{
				spriteRenderer[i].color = normalColor[i] + (differColor[i] * colorRatio);
			}
			for (int i = 0; i < textMesh.Length; i++)
			{
				int textIdx = i + spriteRenderer.Length;
				textMesh[i].color = normalColor[textIdx] + (differColor[textIdx] * colorRatio);
			}
			for (int i = 0; i < textMeshPro.Length; i++)
			{
				int tmproIdx = i + spriteRenderer.Length + textMesh.Length;
				textMeshPro[i].color = normalColor[tmproIdx] + (differColor[tmproIdx] * colorRatio);
			}
		}

		public void SetColors(Color[] colors, bool bPressed)
		{
			if (bPressed)
				this.pressColor = colors;
			else
				this.normalColor = colors;

			differColor = new Color[normalColor.Length];
			for (int i = 0; i < differColor.Length; i++)
			{
				differColor[i] = pressColor[i] - normalColor[i];
			}

			for (int i = 0; i < spriteRenderer.Length; i++)
			{
				spriteRenderer[i].color = normalColor[i] + (differColor[i] * colorRatio);
			}
			for (int i = 0; i < textMesh.Length; i++)
			{
				int textIdx = i + spriteRenderer.Length;
				textMesh[i].color = normalColor[textIdx] + (differColor[textIdx] * colorRatio);
			}
			for (int i = 0; i < textMeshPro.Length; i++)
			{
				int tmproIdx = i + spriteRenderer.Length + textMesh.Length;
				textMeshPro[i].color = normalColor[tmproIdx] + (differColor[tmproIdx] * colorRatio);
			}
		}
	}
}