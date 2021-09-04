using UnityEngine;

public delegate void DelegateBool(bool b);
public delegate void DelegateVoid();
public delegate void DelegateInt(int i);
public delegate void DelegateFloat(float f);
public delegate void DelegateStr(string str);
public delegate void DelegateStrArr(string[] strArr);
public delegate void DelegateVector2(Vector2 v2);
public delegate void DelegateVector3(Vector3 v3);
public delegate string DelegateVoidToStr();
public delegate float DelegateVoidToFloat();
public delegate int DelegateVoidToInt();
public delegate int DelegateStrToInt(string str);
public delegate void DelegateCollider(Collider col);

public class DataSpliterGroup
{
    private const char split_title = '▲';
    public static char SPLIT_TITLE
    {
        get
        {
            return split_title;
        }
    }

    private const char split_sub = '△';
    public static char SPLIT_SUB
    {
        get
        {
            return split_sub;
        }
    }

    private const char split_char = '→';
    public static char SPLIT_CHAR
    {
        get
        {
            return split_char;
        }
    }

    private const char split_key = '↑';
    public static char SPLIT_KEY
    {
        get
        {
            return split_key;
        }
    }

    private const char split_item = '↓';
    public static char SPLIT_ITEM
    {
        get
        {
            return split_item;
        }
    }

    private const char split_inc = '←';
    public static char SPLIT_INC
    {
        get
        {
            return split_inc;
        }
    }


    private const char split_normal = '/';
    public static char SPLIT_NORMAL
    {
        get
        {
            return split_normal;
        }
    }
    public static char SPLIT_END
    {
        get
        {
            return split_normal;
        }
    }

    private const char split_vertical = '|';
    public static char SPLIT_VERT
    {
        get
        {
            return split_vertical;
        }
    }

    private const char split_fileNameInfo = '_';
    public static char SPLIT_FILE_NAME_INFO
    {
        get
        {
            return split_fileNameInfo;
        }
    }

    private const char split_fileExtension = '.';
    public static char SPLIT_FILE_EXTENSION
    {
        get
        {
            return split_fileExtension;
        }
    }

    private const string dpSplit_item = ", ";
    public static string DPSPLIT_ITEM
    {
        get
        {
            return dpSplit_item;
        }
    }

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || PLATFORM_STANDALONE_OSX
    private const char pathSpliter = '/';
#else
	private const char pathSpliter = '\\';
#endif
    public static char folderPathSpliter
    {
        get { return pathSpliter; }
    }
}