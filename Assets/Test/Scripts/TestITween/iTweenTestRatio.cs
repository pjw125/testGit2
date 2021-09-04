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

	[SerializeField] private Transform scaleTarget; // ũ�� Ÿ��
	[SerializeField] private SpriteRenderer colorTarget; // ���� Ÿ��

	private float defaultScale = 0f; // �Ϲ� ������ ������Ʈ ũ��
	[SerializeField] private float activeScale = 0f; // Ȱ��ȭ ������ ������Ʈ ũ��
	private float differScale = 0f; // Ȱ��ȭ/�Ϲ� ���� �� ũ�� ����

	private Color defaultColor; // �Ϲ� ������ ������Ʈ ����
	[SerializeField] private Color activeColor; // Ȱ��ȭ ������ ������Ʈ ����
	private Color differColor; // Ȱ��ȭ/�Ϲ� ���� �� ���� ����

	[SerializeField] private float animateTime; // �ִϸ��̼� �ð�
	[SerializeField] private iTween.EaseType easeType; // �ִϸ��̼� Ÿ��
	private float animateRatio = 0f; // �ִϸ��̼� ����

	private Vector3 scaleStorage; // ũ�� ����� ����

    private void Awake()
    {
		InitializeThis();
    }

    /// <summary>
    /// �ʱ� ���� �Լ�
    /// </summary>
    private void InitializeThis()
    {
		defaultScale = scaleTarget.localScale.x; // ũ�� Ÿ���� �ʱ� ũ�⸦ �Ϲ� ũ��� ����
		differScale = activeScale - defaultScale; // Ȱ��ȭ/�Ϲ� ���� �� ũ�� ���� ����
		scaleStorage = new Vector3(defaultScale, defaultScale, defaultScale); // ũ�� ����� �⺻ �� ����

		defaultColor = colorTarget.color; // ���� Ÿ���� �ʱ� ������ �Ϲ� �������� ����
		differColor = activeColor - defaultColor; // Ȱ��ȭ/�Ϲ� ���� �� ���� ���� ����
	}

	private string strRatio = string.Empty; // ���ڿ� ������

	/// <summary>
	/// �ִϸ��̼� ���� �Լ�
	/// </summary>
	private void iTweenAnimate()
    {
		float ratio = float.MinValue;

		// �Էµ� ������ float���� ����ȯ
		if (!(string.IsNullOrEmpty(strRatio))) // strR�� ���� �ִ� ��� ����ȯ ����
			float.TryParse(strRatio, out ratio);

        if (ratio.Equals(float.MinValue))
		// ����ȯ ���� �ʾҴٸ�
		{
			Debug.LogError("���� ����ȯ ����");
        }
		else
		// ����ȯ �Ǿ��ٸ�
        {
			Singleton_Settings.iTweenControl(gameObjectCache, animateRatio, ratio
				, animateTime, easeType, "SetRatio", "CompleteRatio");
        }
	}
	/// <summary>
	/// �ִϸ��̼����� ���� ���� ���� �Լ�
	/// </summary>
	/// <param name="ratio">����</param>
	private void SetRatio(float ratio)
	{
		animateRatio = ratio; // �ִϸ��̼� ���� ����

		// ũ�� ����
		float scale = defaultScale + (differScale * animateRatio);
		scaleStorage.Set(scale, scale, scale);
		scaleTarget.localScale = scaleStorage;

		// ���� ����
		colorTarget.color = defaultColor + (differColor * animateRatio);
	}
	/// <summary>
	/// �ִϸ��̼� ���� �� �̺�Ʈ �Լ�
	/// </summary>
	private void CompleteRatio()
    {
		Debug.Log("�ִϸ��̼� �Ϸ�");
	}

	public void OnGUI()
	{
		if (GUI.Button(new Rect(0f, 0f, 1080f, 30f), "�Ʒ� TextField�� ������(0 ~ 1)�� �ְ� ���� ������ ������Ʈ ũ�� �� ���� ����"))
		{
			iTweenAnimate();
		}
		strRatio = GUI.TextField(new Rect(0f, 30f, 60f, 30f), strRatio);
	}
}