using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.UI;

namespace Neno.Scripts
{
    public class QrReader
    {

        //private int imgWidth = -1;
        //private int imgHeight = -1;

        ////[SerializeField] private Text uiDebugText;

        //// Use this for initialization
        //void Start()
        //{
        //    HoloCameraManager.Instance.TakeImageAction += DecodeQr;

        //    //InputManager.Instance.AddGlobalListener(gameObject);
        //    //InputManager.Instance.RemoveGlobalListener();
        //}

        //private void DecodeQr(List<byte> bytes)
        //{
        //    imgWidth = HoloCameraManager.Instance.Resolution.width;
        //    imgHeight = HoloCameraManager.Instance.Resolution.height;
        //    this.uiDebugText.text += "\n" + Decode(bytes.ToArray(), imgWidth, imgHeight);
        //}

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

        //public void OnInputClicked(InputClickedEventData eventData)
        //{
        //    HoloCameraManager.Instance.TakePhotoAsync();
        //}
    }
}
