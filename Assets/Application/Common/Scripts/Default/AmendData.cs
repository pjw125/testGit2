using System;

public static class AmendData {

	public static bool CheckStrayFromRatio (ref float _v)
	{
		if (_v < 0f) {
			_v = 0f;
			return true;
		} else if (_v > 1f) {
			_v = 1f;
			return true;
		}

		return false;
	}

	public static void CheckStrayFromRatio (ref double _v)
	{
		if (_v < 0) {
			_v = 0;
		} else if (_v > 1) {
			_v = 1;
		}
	}

	public static void CheckStray (ref float _v, float min, float max)
	{
		if (_v < min) {
			_v = min;
		} else if (_v > max) {
			_v = max;
		}
	}

	public static float AmendAngle_Over180ToMinus (float _v)
	{
		if (_v > 180f)
			_v = _v - 360f;

		return _v;
	}
	public static float AmendAngle_MinusToOver180 (float _v)
	{
		if (_v < 0f)
			_v = 360f + _v;

		return _v;
	}
	public static void AmendAngle_Over180ToMinus (ref float _v)
	{
		if (_v > 180f)
			_v = _v - 360f;
	}
	public static void AmendAngle_MinusToOver180 (ref float _v)
	{
		if (_v < 0f)
			_v = 360f + _v;
	}
	public static void AmendAngle_In360 (ref float _v)
	{
		if (_v < 0f)
			_v = 360f + _v;
		else if (_v > 360f)
			_v = _v - 360f;
	}
	public static float GetNearAngle (float current, float target)
	{
		AmendAngle_In360 (ref current);
		AmendAngle_In360 (ref target);

		float data_1 = target - current;
		float data_2;
		if (data_1 < 0f) {
			data_2 = 360f + data_1;
		} else {
			data_2 = data_1 - 360f;
		}

		float compare_1 = (data_1 < 0f) ? (data_1 * -1f) : data_1;
		float compare_2 = (data_2 < 0f) ? (data_2 * -1f) : data_2;

		return (compare_1 < compare_2) ? data_1 : data_2;
	}

    public static void AmendNearAngle(ref float current, ref float target)
	{
		AmendAngle_In360(ref current);
        float target_0 = target;
        AmendAngle_In360(ref target_0);
        float differ_0 = target_0 - current;
        if (differ_0 < 0)
            differ_0 *= -1;

        float target_1 = target_0 - 360f;
        float differ_1 = target_1 - current;
        if (differ_1 < 0)
            differ_1 *= -1;

        if (differ_0 < differ_1)
            target = target_0;
        else
            target = target_1;
	}

	public static void AmendTransformXrotate (ref float x, float y, float z)
	// if youre Transform Rotation (x > 90[x > -270] and x < 270[x < -90]) :: OverTurn ::, 
	// 'transform'.localEulerAngles Data -> (x : ?, y : 180, z : 180)
	// this function is amending 'x' Value
	{
		if (y == 180f && z == 180f) {
			if (x > 270f) {
				x = x - 360f;
			}

			x = 180f - x;
		}
	}

    /// <summary>
    /// 값을 특정 수치에 맞춰 반올림 하는 함수
    /// </summary>
    /// <param name="fVal">값</param>
    /// <param name="fStepVal">반올림수치</param>
    /// <returns>반올림값</returns>
    public static float RoundFloat(float fVal, float fStepVal)
	{
		long lStep, lValue;
		float fReturn, fHalf;

		if (fStepVal == 0.01f) return fVal;

		fHalf = fStepVal * 0.5f;
		lStep = (long)(fStepVal * 10000f);

		if (fVal < -0.01f)
			fVal = fVal - fHalf;
		else if (fVal > 0.01f)
			fVal = fVal + fHalf;

		lValue = (long)(fVal * 10000f);
		lValue -= lValue % lStep;
		lValue -= lValue % 100;
		fReturn = lValue;
		fReturn /= 10000.0f;

		return fReturn;
	}

    // Spherical/Cylinder
    /// <summary>
    /// 값을 받아 검안데이터 문자열로 반환하는 함수
    /// </summary>
    /// <param name="val">값</param>
    /// <returns>검안데이터 문자열</returns>
    public static string ValueToOptometryStr(float val)
    {
        // 수소점 2자리, 양수일 경우에도 부호 추가
        return (val > 0) ? string.Format("+{0}", val.ToString("F2")) : val.ToString("F2");
    }
    public static string AmendSphericalData(string str)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;

        return AmendOptometryData(str, 0.25f, true, true, "F2");
    }
    public static string AmendAdditionData(string str)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;
        
        return AmendOptometryData(str, 0.25f, false, true, "F2");
    }
    public static string AmendAxisData(string str)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;

        float fVal = float.MinValue;

        bool bTry = float.TryParse(str, out fVal);
        if (!bTry)
            return string.Empty;

        if (fVal < 0f)
            fVal = 0f;
        else if (fVal > 180f)
            fVal = 180f;
        else
            fVal = AmendData.RoundFloat(fVal, 1f);

        return fVal.ToString();
    }
    public static string AmendOptometryData(string str, float fStepVal, bool bDefaultMinus, bool bShowPositiveSign)
    {
        return AmendOptometryData(str, fStepVal, bDefaultMinus, bShowPositiveSign, string.Empty);
    }
    public static string AmendOptometryData(string str, float fStepVal, bool bDefaultMinus, bool bShowPositiveSign, string format)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;

        float fVal = float.MinValue;
        bool bTry = float.TryParse(str, out fVal);
        if (!bTry)
            return string.Empty;

        bool bNoneSign = false;
        bool bDataManufacture = false;
        CheckNoneSignAndDataManufacture(str, ref bNoneSign, ref bDataManufacture);

        if (bDataManufacture)
            // 100이상의 값을 직접적으로 취급하지 않고 1차 가공한(* 0.01)
            fVal = fVal * 0.01f;

        fVal = AmendData.RoundFloat(fVal, fStepVal);

        if (bNoneSign && bDefaultMinus)
            // 부호가 없는데 기본값 음수로 설정(bDefaultMinus)되어 있다면
            // 음수로 변경해준다
            fVal *= -1f;

        string strResult = fVal.ToString(format);
        if (fVal > 0 && bShowPositiveSign)
            // 결과 값이 양수인데 '+'부호 표시로 설정(bShowPositiveSign)이 되어 있다면
            // '+'를 붙여준다
            strResult = string.Format("+{0}", strResult);

        return strResult;
    }

    public static string AmendOptometryData(string str, ref float fVal, float fStepVal, bool bDefaultMinus, bool bShowPositiveSign)
    {
        return AmendOptometryData(str, ref fVal, fStepVal, bDefaultMinus, bShowPositiveSign, string.Empty);
    }
    public static string AmendOptometryData(string str, ref float fVal, float fStepVal, bool bDefaultMinus, bool bShowPositiveSign, string format)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;

        bool bTry = float.TryParse(str, out fVal);
        if (!bTry)
            return string.Empty;

        bool bNoneSign = false;
        bool bDataManufacture = false;
        CheckNoneSignAndDataManufacture(str, ref bNoneSign, ref bDataManufacture);

        if (bDataManufacture)
            // 100이상의 값을 직접적으로 취급하지 않고 1차 가공한(* 0.01)
            fVal = fVal * 0.01f;

        fVal = AmendData.RoundFloat(fVal, fStepVal);

        if (bNoneSign && bDefaultMinus)
            // 부호가 없는데 기본값 음수로 설정(bDefaultMinus)되어 있다면
            // 음수로 변경해준다
            fVal *= -1f;

        string strResult = fVal.ToString(format);
        if (fVal > 0 && bShowPositiveSign)
            // 결과 값이 양수인데 '+'부호 표시로 설정(bShowPositiveSign)이 되어 있다면
            // '+'를 붙여준다
            strResult = string.Format("+{0}", strResult);

        return strResult;
    }
    private static void CheckNoneSignAndDataManufacture(string str, ref bool bNoneSign, ref bool bDataManufacture)
        // 부호값이 있는지 없는지 체크(bNoneSign), 없을경우 true
        // data가 정수 기준 자릿수가 2자리를 넘는지 확인하여 가공되어야하는지를 체크(bDataManufacture) : 가공은 *0.01
    {
        switch(str[0])
        {
            case '+':
            case '-':
                bNoneSign = false;
                break;
            default:
                bNoneSign = true;
                break;
        }

        int startIndex = bNoneSign ? 0 : 1;
        int nCipher = 0;
        for (int i = startIndex; i < str.Length; i++)
        {
            if (str[i].Equals('.'))
                break;
            else
                nCipher++;
        }

        bDataManufacture = (nCipher > 2);
    }
}
