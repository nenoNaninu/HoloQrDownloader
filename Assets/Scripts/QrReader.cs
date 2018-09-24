using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neno.Scripts
{
    public class QrReader : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        public static string Decode(byte[] src, int width, int height)
        {

#if ENABLE_WINMD_SUPPORT
        ZXing.IBarcodeReader reader = new ZXing.BarcodeReader();
        var res = reader.Decode(src, width, height, ZXing.BitmapFormat.BGRA32);
        if(res == null)
        {
        return null;
        }
        return res.Text;
#else
            return null;
#endif
        }
    }
}
