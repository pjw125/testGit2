using UnityEngine;

/// <summary>
/// Color�� iTween �ִϸ��̼��� ���� target ������Ʈ�� ������ �����ϴ� Ŭ����
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

	[SerializeField] private SpriteRenderer target; // �ִϸ��̼� Ÿ��
	[SerializeField] private float animateTime; // �ִϸ��̼� �ð�
	[SerializeField] private iTween.EaseType easeType; // �ִϸ��̼� Ÿ��

	// �Է¹޴� ���� 0 ~ 255�ε� ��ũ��Ʈ�� ���� ����Ǵ� ���� ��ġ�� 0 ~ 1�̱� ������ �Է¹��� ���� �����ϱ� ���� ����
	private const float colorForMult = 1f / 255f;

	private string strR = string.Empty; // ���ڿ� r
	private string strG = string.Empty; // ���ڿ� g
	private string strB = string.Empty; // ���ڿ� b
	private string strA = string.Empty; // ���ڿ� a

	/// <summary>
	/// �ִϸ��̼� ���� �Լ�
	/// </summary>
	private void iTweenAnimate()
	{
		float r = float.MinValue;
		float g = float.MinValue;
		float b = float.MinValue;
		float a = float.MinValue;

		// �Էµ� r, g, b, a float���� ����ȯ
		if (!(string.IsNullOrEmpty(strR))) // strR�� ���� �ִ� ��� ����ȯ ����
			float.TryParse(strR, out r);
		if (!(string.IsNullOrEmpty(strG))) // strG�� ���� �ִ� ��� ����ȯ ����
			float.TryParse(strG, out g);
		if (!(string.IsNullOrEmpty(strB))) // strB�� ���� �ִ� ��� ����ȯ ����
			float.TryParse(strB, out b);
		if (!(string.IsNullOrEmpty(strA))) // strA�� ���� �ִ� ��� ����ȯ ����
			float.TryParse(strA, out a);

		if (r.Equals(float.MinValue) || g.Equals(float.MinValue) || b.Equals(float.MinValue) || a.Equals(float.MinValue))
		// ����ȯ ���� ���� ��ǥ���� �ϳ��� �ִٸ�
		{
			Debug.LogErrorFormat("���� ����ȯ ���� [ {0} : {1} : {2} : {3} ]"
				, (r.Equals(float.MinValue) ? "Err" : r.ToString())
				, (g.Equals(float.MinValue) ? "Err" : g.ToString())
				, (b.Equals(float.MinValue) ? "Err" : b.ToString())
				, (a.Equals(float.MinValue) ? "Err" : a.ToString()));
		}
		else // ��� ����ȯ�� �Ǿ��ٸ�
		{
			Color col = new Color(r * colorForMult, g * colorForMult, b * colorForMult, a * colorForMult);
			Singleton_Settings.iTweenControl(gameObjectCache, target.color, col
				, animateTime, easeType, "SetColor", "CompleteColor");
		}
	}
	/// <summary>
	/// �ִϸ��̼����� ���� ���� ���� �Լ�
	/// </summary>
	/// <param name="col">����</param>
	private void SetColor(Color col)
	{
		target.color = col;
	}
	/// <summary>
	/// �ִϸ��̼� ���� �� �̺�Ʈ �Լ�
	/// </summary>
	private void CompleteColor()
	{
		Debug.Log("�ִϸ��̼� �Ϸ�");
	}

	public void OnGUI()
	{
		if (GUI.Button(new Rect(0f, 0f, 1080f, 30f), "�Ʒ� TextField�� ���� ������� r, g, b, a��(0 ~ 255)�� �ְ� ���� ������ ������Ʈ ���� ����"))
		{
			iTweenAnimate();
		}
		strR = GUI.TextField(new Rect(0f, 30f, 60f, 30f), strR);
		strG = GUI.TextField(new Rect(60f, 30f, 60f, 30f), strG);
		strB = GUI.TextField(new Rect(120f, 30f, 60f, 30f), strB);
		strA = GUI.TextField(new Rect(180f, 30f, 60f, 30f), strA);
	}
}
