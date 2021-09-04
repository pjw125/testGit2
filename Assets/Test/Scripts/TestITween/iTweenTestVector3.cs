using UnityEngine;

/// <summary>
/// Vector3�� iTween �ִϸ��̼��� ���� target ������Ʈ�� ��ġ�� �̵��ϴ� Ŭ����
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

	[SerializeField] private Transform target; // �ִϸ��̼� Ÿ��
	[SerializeField] private float animateTime; // �ִϸ��̼� �ð�
	[SerializeField] private iTween.EaseType easeType; // �ִϸ��̼� Ÿ��

	private string strX = string.Empty; // ���ڿ� x��ǥ
	private string strY = string.Empty; // ���ڿ� y��ǥ
	private string strZ = string.Empty; // ���ڿ� z��ǥ

	/// <summary>
	/// �ִϸ��̼� ���� �Լ�
	/// </summary>
	private void iTweenAnimate()
    {
		float x = float.MinValue;
		float y = float.MinValue;
		float z = float.MinValue;

        // �Էµ� x, y, z float���� ����ȯ
        if (!(string.IsNullOrEmpty(strX))) // strX�� ���� �ִ� ��� ����ȯ ����
			float.TryParse(strX, out x);
		if (!(string.IsNullOrEmpty(strY))) // strY�� ���� �ִ� ��� ����ȯ ����
			float.TryParse(strY, out y);
		if (!(string.IsNullOrEmpty(strZ))) // strZ�� ���� �ִ� ��� ����ȯ ����
			float.TryParse(strZ, out z);

        if (x.Equals(float.MinValue) || y.Equals(float.MinValue) || z.Equals(float.MinValue))
			// ����ȯ ���� ���� ��ǥ���� �ϳ��� �ִٸ�
        {
			Debug.LogErrorFormat("��ǥ ����ȯ ���� [ {0} : {1} : {2} ]"
				, (x.Equals(float.MinValue) ? "Err" : x.ToString())
				, (y.Equals(float.MinValue) ? "Err" : y.ToString())
				, (z.Equals(float.MinValue) ? "Err" : z.ToString()));
        }
		else // ��� ����ȯ�� �Ǿ��ٸ�
        {
			Vector3 pos = new Vector3(x, y, z);
			Singleton_Settings.iTweenControl(gameObjectCache, target.localPosition, pos
				, animateTime, easeType, "SetPositioning", "CompletePositioning");
        }
    }
	/// <summary>
	/// �ִϸ��̼����� ���� ��ġ ���� �Լ�
	/// </summary>
	/// <param name="pos">��ġ</param>
	private void SetPositioning(Vector3 pos)
    {
		target.localPosition = pos;
    }
	/// <summary>
	/// �ִϸ��̼� ���� �� �̺�Ʈ �Լ�
	/// </summary>
	private void CompletePositioning()
    {
		Debug.Log("�ִϸ��̼� �Ϸ�");
    }

	public void OnGUI()
    {
        if (GUI.Button(new Rect(0f, 0f, 1080f, 30f), "�Ʒ� TextField�� ���� ������� x, y, z���� �ְ� ���� ������ ������Ʈ�� �ش� ��ġ�� �̵�"))
        {
			iTweenAnimate();
		}
		strX = GUI.TextField(new Rect(0f, 30f, 60f, 30f), strX);
		strY = GUI.TextField(new Rect(60f, 30f, 60f, 30f), strY);
		strZ = GUI.TextField(new Rect(120f, 30f, 60f, 30f), strZ);
	}
}