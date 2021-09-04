using UnityEngine;

/// <summary>
/// Color형 iTween 애니메이션을 통해 target 오브젝트의 색상을 변경하는 클래스
/// </summary>
public class iTweenTestColor : MonoBehaviour
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
	#endregion

	[SerializeField] private SpriteRenderer target; // 애니메이션 타겟
	[SerializeField] private float animateTime; // 애니메이션 시간
	[SerializeField] private iTween.EaseType easeType; // 애니메이션 타입

	// 입력받는 값은 0 ~ 255인데 스크립트를 통해 적용되는 색상 수치는 0 ~ 1이기 때문에 입력받은 값에 적용하기 위한 변수
	private const float colorForMult = 1f / 255f;

	private string strR = string.Empty; // 문자열 r
	private string strG = string.Empty; // 문자열 g
	private string strB = string.Empty; // 문자열 b
	private string strA = string.Empty; // 문자열 a

	/// <summary>
	/// 애니메이션 실행 함수
	/// </summary>
	private void iTweenAnimate()
	{
		float r = float.MinValue;
		float g = float.MinValue;
		float b = float.MinValue;
		float a = float.MinValue;

		// 입력된 r, g, b, a float으로 형변환
		if (!(string.IsNullOrEmpty(strR))) // strR에 값이 있는 경우 형변환 진행
			float.TryParse(strR, out r);
		if (!(string.IsNullOrEmpty(strG))) // strG에 값이 있는 경우 형변환 진행
			float.TryParse(strG, out g);
		if (!(string.IsNullOrEmpty(strB))) // strB에 값이 있는 경우 형변환 진행
			float.TryParse(strB, out b);
		if (!(string.IsNullOrEmpty(strA))) // strA에 값이 있는 경우 형변환 진행
			float.TryParse(strA, out a);

		if (r.Equals(float.MinValue) || g.Equals(float.MinValue) || b.Equals(float.MinValue) || a.Equals(float.MinValue))
		// 형변환 되지 않은 좌표값이 하나라도 있다면
		{
			Debug.LogErrorFormat("색상 형변환 실패 [ {0} : {1} : {2} : {3} ]"
				, (r.Equals(float.MinValue) ? "Err" : r.ToString())
				, (g.Equals(float.MinValue) ? "Err" : g.ToString())
				, (b.Equals(float.MinValue) ? "Err" : b.ToString())
				, (a.Equals(float.MinValue) ? "Err" : a.ToString()));
		}
		else // 모두 형변환이 되었다면
		{
			Color col = new Color(r * colorForMult, g * colorForMult, b * colorForMult, a * colorForMult);
			Singleton_Settings.iTweenControl(gameObjectCache, target.color, col
				, animateTime, easeType, "SetColor", "CompleteColor");
		}
	}
	/// <summary>
	/// 애니메이션으로 인한 색상 변경 함수
	/// </summary>
	/// <param name="col">색상</param>
	private void SetColor(Color col)
	{
		target.color = col;
	}
	/// <summary>
	/// 애니메이션 종료 시 이벤트 함수
	/// </summary>
	private void CompleteColor()
	{
		Debug.Log("애니메이션 완료");
	}

	public void OnGUI()
	{
		if (GUI.Button(new Rect(0f, 0f, 1080f, 30f), "아래 TextField에 좌측 순서대로 r, g, b, a값(0 ~ 255)을 넣고 여길 누르면 오브젝트 색상 변경"))
		{
			iTweenAnimate();
		}
		strR = GUI.TextField(new Rect(0f, 30f, 60f, 30f), strR);
		strG = GUI.TextField(new Rect(60f, 30f, 60f, 30f), strG);
		strB = GUI.TextField(new Rect(120f, 30f, 60f, 30f), strB);
		strA = GUI.TextField(new Rect(180f, 30f, 60f, 30f), strA);
	}
}
