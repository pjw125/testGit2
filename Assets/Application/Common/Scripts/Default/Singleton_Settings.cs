using UnityEngine;
using System;
using System.IO;
using UIControl;
using SongDuTouchSpace;

public class Singleton_Settings
{

	#region Singleton
	private volatile static Singleton_Settings instance;

	public static Singleton_Settings getInstance
	{
		get
		{
			if (instance == null)
			{
				instance = new Singleton_Settings();
			}

			return instance;
		}
	}

	private Singleton_Settings()
	{
        this.SettingScreenInformation();
    }
    #endregion Singleton

    private const float defaultScreenRatio = 1.777777777f;

    private float worldPos_perOnePixel; // 1픽셀당 유니티 오브젝트 크기/위치값
    public float WorldPosPerOnePixel { get { return this.worldPos_perOnePixel; } }

	public Vector2 screenSize; // 해상도
    public float screenRatio = 0f; // 해상도 비율(가로크기 / 세로크기)
    public bool isCompleteScreenInformation // 스크린해상도 설정이 제대로 됐는지 확인
    { get { return (screenRatio > 0f); } }
    public float screenRatioForMultiple
    {
        get { return this.screenRatio / defaultScreenRatio; }
    }
    private void SettingScreenInformation()
    {
        this.SettingScreenInformation(Screen.width, Screen.height);
    }
    public void SettingScreenInformation(int width, int height)
    {
        if (width <= 0 || height <= 0)
            return;

        worldPos_perOnePixel = 1f / (height * 0.5f);
        screenSize = new Vector2((float)width, (float)height);
        this.screenRatio = screenSize.x / screenSize.y;
    }

    #region Price Format
    /// <summary>
    /// 정수형 숫자를 금액포멧(3자리당 , 표시) 문자열로 반환하는 함수
    /// </summary>
    /// <param name="price">정수형 숫자</param>
    /// <returns>금액포멧 문자열</returns>
    public string GetPriceFormat(int price)
    {
        return string.Format("{0:#,###}", price);
    }
    /// <summary>
    /// 소수형 숫자를 금액포멧(3자리당 , 표시) 문자열로 반환하는 함수
    /// </summary>
    /// <param name="price">소수형 숫자</param>
    /// <returns>금액포멧 문자열</returns>
    public string GetPriceFormat(float price)
    {
        return string.Format("{0:#,###.###}", price);
    }
    #endregion

    #region iTween
    // float
    public static void iTweenControl(GameObject _gameObject, float _from, float _to, float _time, iTween.EaseType easeType, string onupdate)
    {
        iTween.ValueTo(_gameObject,
            iTween.Hash("from", _from, "to", _to, "time", _time, "easetype", easeType,
                "onupdate", onupdate));
    }
    public static void iTweenControl(GameObject _gameObject, float _from, float _to, float _time, iTween.EaseType easeType, string onupdate, string oncomplete)
    {
        iTween.ValueTo(_gameObject,
            iTween.Hash("from", _from, "to", _to, "time", _time, "easetype", easeType,
                "onupdate", onupdate, "oncomplete", oncomplete));
    }
    // Vector3
    public static void iTweenControl(GameObject _gameObject, Vector3 _from, Vector3 _to, float _time, iTween.EaseType easeType, string onupdate)
    {
        iTween.ValueTo(_gameObject,
            iTween.Hash("from", _from, "to", _to, "time", _time, "easetype", easeType,
                "onupdate", onupdate));
    }
    public static void iTweenControl(GameObject _gameObject, Vector3 _from, Vector3 _to, float _time, iTween.EaseType easeType, string onupdate, string oncomplete)
    {
        iTween.ValueTo(_gameObject,
            iTween.Hash("from", _from, "to", _to, "time", _time, "easetype", easeType,
                "onupdate", onupdate, "oncomplete", oncomplete));
    }
    // Vector2
    public static void iTweenControl(GameObject _gameObject, Vector2 _from, Vector2 _to, float _time, iTween.EaseType easeType, string onupdate)
    {
        iTween.ValueTo(_gameObject,
            iTween.Hash("from", _from, "to", _to, "time", _time, "easetype", easeType,
                "onupdate", onupdate));
    }
    public static void iTweenControl(GameObject _gameObject, Vector2 _from, Vector2 _to, float _time, iTween.EaseType easeType, string onupdate, string oncomplete)
    {
        iTween.ValueTo(_gameObject,
            iTween.Hash("from", _from, "to", _to, "time", _time, "easetype", easeType,
                "onupdate", onupdate, "oncomplete", oncomplete));
    }
    // Color
    public static void iTweenControl(GameObject _gameObject, Color _from, Color _to, float _time, iTween.EaseType easeType, string onupdate)
    {
        iTween.ValueTo(_gameObject,
            iTween.Hash("from", _from, "to", _to, "time", _time, "easetype", easeType,
                "onupdate", onupdate));
    }
    public static void iTweenControl(GameObject _gameObject, Color _from, Color _to, float _time, iTween.EaseType easeType, string onupdate, string oncomplete)
    {
        iTween.ValueTo(_gameObject,
            iTween.Hash("from", _from, "to", _to, "time", _time, "easetype", easeType,
                "onupdate", onupdate, "oncomplete", oncomplete));
    }
    #endregion

    #region Check Data
    /// <summary>
    /// 해당 enum(Type t)에 특정 문자열에 해당하는 아이템이 있는지 확인하는 함수
    /// </summary>
    /// <param name="t">enum 타입</param>
    /// <param name="str">아이템 문자열</param>
    /// <returns>결과</returns>
    public bool CheckEnumParse(Type t, string str)
    {
        Array arr = Enum.GetValues(t);

        bool result = false;
        for (int i = 0; i < arr.Length; i++)
        {
            if (str.Equals(arr.GetValue(i).ToString()))
            {
                result = true;
                break;
            }
        }

        return result;
    }
    #endregion

    #region DateTime
    /// <summary>
    /// DateTime 문자열을 특정포멧에 맞춰 DateTime으로 변환 후 반환하는 함수
    /// 변환에 실패한 경우 DateTime.MinValue를 반환한다
    /// </summary>
    /// <param name="strDateTime">DateTime 문자열</param>
    /// <param name="format">DateTime 문자열 포멧</param>
    /// <returns>결과 DateTime</returns>
    public DateTime StringToDateTime(string strDateTime, string format)
    {
        DateTime dt = DateTime.MinValue;

        DateTime.TryParseExact(strDateTime, format,
            System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt);

        return dt;
    }
    /// <summary>
    /// 연월일이 동일한지 체크하는 함수
    /// </summary>
    /// <param name="dt1">비교 DateTime 1</param>
    /// <param name="dt2">비교 DateTime 2</param>
    /// <returns>결과</returns>
    public bool EqualDate(DateTime dt1, DateTime dt2)
    { return (dt1.Year.Equals(dt2.Year) && dt1.Month.Equals(dt2.Month) && dt1.Day.Equals(dt2.Day)); }

    /// <summary>
    /// 연월일 / 시분초가 동일한지 체크하는 함수
    /// </summary>
    /// <param name="dt1">비교 DateTime 1</param>
    /// <param name="dt2">비교 DateTime 2</param>
    /// <returns>결과</returns>
    public bool EqualDateTime(DateTime dt1, DateTime dt2)
    { return (instance.EqualDate(dt1, dt2) && dt1.Hour.Equals(dt2.Hour) && dt1.Minute.Equals(dt2.Minute) && dt1.Second.Equals(dt2.Second)); }
    /// <summary>
    /// 연월이 동일한지 체크하는 함수
    /// </summary>
    /// <param name="dt1">비교 DateTime 1</param>
    /// <param name="dt2">비교 DateTime 2</param>
    /// <returns>결과</returns>
    public bool EqualMonth(DateTime dt1, DateTime dt2)
    { return dt1.Year.Equals(dt2.Year) && dt1.Month.Equals(dt2.Month); }
    #endregion

    #region Touch Center
    private TouchCenter _touchCenter;
    public TouchCenter touchCenter
    {
        get { return _touchCenter; }

        set { _touchCenter = value; }
    }
    #endregion

    #region Network
    public static string GetIpAddress()
	{
		System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

		string strIP = string.Empty;

		for (int i = 0; i < host.AddressList.Length; i++)
		{
			if (host.AddressList[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
			{
                strIP = host.AddressList[i].ToString();
                break;
            }
		}

		return strIP;
	}
    #endregion

    /// <summary>
    /// 외부 리소스 경로
    /// </summary>
    #region ExternalResources
#if UNITY_EDITOR
    private const string ExternalResourcesFolderName = "ExtRsc";
#endif

    public static string ExternalResourcesPath
    {
        get
        {
#if UNITY_EDITOR
            return string.Format("{1}{0}{2}{0}StreamingAssets"
                , DataSpliterGroup.folderPathSpliter, Directory.GetCurrentDirectory(), ExternalResourcesFolderName);
#else
            return Application.streamingAssetsPath;
#endif
        }
    }
    #endregion

    #region MouseOver
    private MouseOverRoot mouseOverRoot;
    public MouseOverRoot MouseOver
    {
        get { return this.mouseOverRoot; }
        set
        {
            if (mouseOverRoot == null) this.mouseOverRoot = value;
            else value.DestroyThisObject();
        }
    }
    #endregion

    #region Target Local Position By Local
    /// <summary>
    /// 특정 로컬 오브젝트 기준 타겟 오브젝트의 위치값
    /// </summary>
    /// <param name="target">타겟 오브젝트</param>
    /// <param name="local">로컬 오브젝트</param>
    /// <returns>위치값</returns>
    public Vector3 GetLocalPosition(Transform target, Transform local)
    {
        return target.position - local.position;
    }
    #endregion
}