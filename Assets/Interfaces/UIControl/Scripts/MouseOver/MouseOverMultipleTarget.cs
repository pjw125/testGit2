using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIControl
{
    public sealed class MouseOverMultipleTarget : MouseOverTarget
    {
        [SerializeField] private Collider[] cols;

        public override bool CheckCollider(Collider _col)
        {
            if (base.CheckCollider(_col))
            {
                return true;
            }
            else
            {
                for (int i = 0; i < cols.Length; i++)
                {
                    if (cols[i].Equals(_col))
                        return true;
                }

                return false;
            }
        }
    }
}