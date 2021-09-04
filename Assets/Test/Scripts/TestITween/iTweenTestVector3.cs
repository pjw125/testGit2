using UnityEngine;

/// <summary>
/// Vector3형 iTween 애니메이션을 통해 target 오브젝트의 위치를 이동하는 클래스
/// </summary>
public class iTweenTestVector3 : MonoBehaviour
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

	[SerializeField] private Transform target; // 애니메이션 타겟
	[SerializeField] private float animateTime; // 애니메이션 시간
	[SerializeField] private iTween.EaseType easeType; // 애니메이션 타입

	private string strX = string.Empty; // 문자열 x좌표
	private string strY = string.Empty; // 문자열 y좌표
	private string strZ = string.Empty; // 문자열 z좌표

	/// <summary>
	/// 애니메이션 실행 함수
	/// </summary>
	private void iTweenAnimate()
    {
		float x = float.MinValue;
		float y = float.MinValue;
		float z = float.MinValue;

        // 입력된 x, y, z float으로 형변환
        if (!(string.IsNullOrEmpty(strX))) // strX에 값이 있는 경우 형변환 진행
			float.TryParse(strX, out x);
		if (!(string.IsNullOrEmpty(strY))) // strY에 값이 있는 경우 형변환 진행
			float.TryParse(strY, out y);
		if (!(string.IsNullOrEmpty(strZ))) // strZ에 값이 있는 경우 형변환 진행
			float.TryParse(strZ, out z);

        if (x.Equals(float.MinValue) || y.Equals(float.MinValue) || z.Equals(float.MinValue))
			// 형변환 되지 않은 좌표값이 하나라도 있다면
        {
			Debug.LogErrorFormat("좌표 형변환 실패 [ {0} : {1} : {2} ]"
				, (x.Equals(float.MinValue) ? "Err" : x.ToString())
				, (y.Equals(float.MinValue) ? "Err" : y.ToString())
				, (z.Equals(float.MinValue) ? "Err" : z.ToString()));
        }
		else // 모두 형변환이 되었다면
        {
			Vector3 pos = new Vector3(x, y, z);
			Singleton_Settings.iTweenControl(gameObjectCache, target.localPosition, pos
				, animateTime, easeType, "SetPositioning", "CompletePositioning");
        }
    }
	/// <summary>
	/// 애니메이션으로 인한 위치 변경 함수
	/// </summary>
	/// <param name="pos">위치</param>
	private void SetPositioning(Vector3 pos)
    {
		target.localPosition = pos;
    }
	/// <summary>
	/// 애니메이션 종료 시 이벤트 함수
	/// </summary>
	private void CompletePositioning()
    {
		Debug.Log("애니메이션 완료");
    }

	public void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 0f, 1080f, 30f), "아래 TextField에 좌측 순서대로 x, y, z값을 넣고 여길 누르면 오브젝트가 해당 위치로 이동"))
        {
			iTweenAnimate();
		}
		strX = GUI.TextField(new Rect(0f, 30f, 60f, 30f), strX);
		strY = GUI.TextField(new Rect(60f, 30f, 60f, 30f), strY);
		strZ = GUI.TextField(new Rect(120f, 30f, 60f, 30f), strZ);
	}
}