using UnityEngine;

public class iTweenTestRatio : MonoBehaviour
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

	[SerializeField] private Transform scaleTarget; // 크기 타겟
	[SerializeField] private SpriteRenderer colorTarget; // 색상 타겟

	private float defaultScale = 0f; // 일반 상태의 오브젝트 크기
	[SerializeField] private float activeScale = 0f; // 활성화 상태의 오브젝트 크기
	private float differScale = 0f; // 활성화/일반 상태 간 크기 차이

	private Color defaultColor; // 일반 상태의 오브젝트 색상
	[SerializeField] private Color activeColor; // 활성화 상태의 오브젝트 색상
	private Color differColor; // 활성화/일반 상태 간 색상 차이

	[SerializeField] private float animateTime; // 애니메이션 시간
	[SerializeField] private iTween.EaseType easeType; // 애니메이션 타입
	private float animateRatio = 0f; // 애니메이션 비율

	private Vector3 scaleStorage; // 크기 저장소 변수

    private void Awake()
    {
		InitializeThis();
    }

    /// <summary>
    /// 초기 설정 함수
    /// </summary>
    private void InitializeThis()
    {
		defaultScale = scaleTarget.localScale.x; // 크기 타겟의 초기 크기를 일반 크기로 설정
		differScale = activeScale - defaultScale; // 활성화/일반 상태 간 크기 차이 설정
		scaleStorage = new Vector3(defaultScale, defaultScale, defaultScale); // 크기 저장소 기본 값 설정

		defaultColor = colorTarget.color; // 색상 타겟의 초기 색상을 일반 색상으로 설정
		differColor = activeColor - defaultColor; // 활성화/일반 상태 간 색상 차이 설정
	}

	private string strRatio = string.Empty; // 문자열 비율값

	/// <summary>
	/// 애니메이션 실행 함수
	/// </summary>
	private void iTweenAnimate()
    {
		float ratio = float.MinValue;

		// 입력된 비율값 float으로 형변환
		if (!(string.IsNullOrEmpty(strRatio))) // strR에 값이 있는 경우 형변환 진행
			float.TryParse(strRatio, out ratio);

        if (ratio.Equals(float.MinValue))
		// 형변환 되지 않았다면
		{
			Debug.LogError("비율 형변환 실패");
        }
		else
		// 형변환 되었다면
        {
			Singleton_Settings.iTweenControl(gameObjectCache, animateRatio, ratio
				, animateTime, easeType, "SetRatio", "CompleteRatio");
        }
	}
	/// <summary>
	/// 애니메이션으로 인한 비율 변경 함수
	/// </summary>
	/// <param name="ratio">비율</param>
	private void SetRatio(float ratio)
	{
		animateRatio = ratio; // 애니메이션 비율 설정

		// 크기 설정
		float scale = defaultScale + (differScale * animateRatio);
		scaleStorage.Set(scale, scale, scale);
		scaleTarget.localScale = scaleStorage;

		// 색상 설정
		colorTarget.color = defaultColor + (differColor * animateRatio);
	}
	/// <summary>
	/// 애니메이션 종료 시 이벤트 함수
	/// </summary>
	private void CompleteRatio()
    {
		Debug.Log("애니메이션 완료");
	}

	public void OnGUI()
	{
		if (GUI.Button(new Rect(0f, 0f, 1080f, 30f), "아래 TextField에 비율값(0 ~ 1)을 넣고 여길 누르면 오브젝트 크기 및 색상 변경"))
		{
			iTweenAnimate();
		}
		strRatio = GUI.TextField(new Rect(0f, 30f, 60f, 30f), strRatio);
	}
}