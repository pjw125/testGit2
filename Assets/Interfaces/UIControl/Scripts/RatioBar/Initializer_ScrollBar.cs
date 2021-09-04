using UnityEngine;

namespace UIControl
{
    public class Initializer_ScrollBar : MonoBehaviour
    {
        [SerializeField] private float minValue;
        [SerializeField] private float maxValue;
        [SerializeField] private float step;
        [SerializeField] private bool isReverse;

        public float[] GetValues()
        {
            int leng = Mathf.RoundToInt(((maxValue - minValue) / step)) + 1;

            float[] vals = new float[leng];

            for (int i = 0; i < vals.Length; i++)
            {
                vals[i] = AmendData.RoundFloat(minValue + (step * i), step);
            }

            if (isReverse)
                System.Array.Reverse(vals);

            return vals;
        }
    }
}