using UnityEngine;

public class Util {
    public static void SetLayerRecursively(GameObject _obj,LayerMask newLayer)
    {
        if (_obj == null)
            return;
        _obj.layer = newLayer;
        foreach(Transform _child in _obj.transform)
        {
            if (_child != null)
            {
                SetLayerRecursively(_child.gameObject, newLayer);
            }
        }
    }

    public static uint StringToUint(string numberString)
    {
        uint number = 0;
        for (int i = numberString.Length - 1; i >= 0; i--)
        {
            number = number * 10 + ((uint)numberString[i] - '0');
        }
        return number;
    }
}

