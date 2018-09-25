using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HoloToolkit.Unity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.WSA.WebCam;


namespace Neno.Scripts
{
    public class HoloCameraManager : Singleton<HoloCameraManager>
    {
        private PhotoCapture photoCaptureObject;
        private CameraParameters cameraParameters;
        public Resolution Resolution { get; private set; }
        private bool isCapturingPhoto = false;

        /// <summary>
        /// 画像が撮られたら、その生のbyte列を渡すaction
        /// </summary>
        public Action<List<byte>> TakeImageAction { get; set; }

        // Use this for initialization
        void Start()
        {
            this.Resolution = PhotoCapture.SupportedResolutions.First(); //1280*720

            this.cameraParameters = new CameraParameters(WebCamMode.PhotoMode)
            {
                cameraResolutionHeight = Resolution.height,
                cameraResolutionWidth = Resolution.width,
                hologramOpacity = 0f,
                pixelFormat = CapturePixelFormat.BGRA32
            };

            PhotoCapture.CreateAsync(false, CreateCaptureObj);
        }

        void CreateCaptureObj(PhotoCapture captureObject)
        {
            this.photoCaptureObject = captureObject;
            this.photoCaptureObject.StartPhotoModeAsync(cameraParameters, _ => { });
        }

        public void TakePhotoAsync()
        {
            if (this.isCapturingPhoto)
            {
                return;
            }

            isCapturingPhoto = true;
            this.photoCaptureObject.TakePhotoAsync(this.OnPhotoCaptured);
        }

        void OnPhotoCaptured(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
        {
            //Matrix4x4 cameraToWorldMatrix;

            //photoCaptureFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix);
            //Matrix4x4 worldToCameraMatrix = cameraToWorldMatrix.inverse;

            //Matrix4x4 projectionMatrix;
            //photoCaptureFrame.TryGetProjectionMatrix(out projectionMatrix);

            //var texture = new Texture2D(this.cameraParameters.cameraResolutionWidth, this.cameraParameters.cameraResolutionHeight, TextureFormat.ARGB32, false);
            //photoCaptureFrame.UploadImageDataToTexture(texture);
            List<byte> byteses = new List<byte>();
            photoCaptureFrame.CopyRawImageDataIntoBuffer(byteses);
            TakeImageAction?.Invoke(byteses);
            //texture.wrapMode = TextureWrapMode.Clamp;
            photoCaptureFrame.Dispose();
            //texture.Compress(true);//ここでの圧縮はDXTフォーマットに圧縮するということ。
            Resources.UnloadUnusedAssets();
            isCapturingPhoto = false;

        }
    }
}

