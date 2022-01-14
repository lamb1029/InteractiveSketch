using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZXing;

public class QRcodeReader : MonoBehaviour
{
    public CameraManager CM;

    public void OnGUI()
    {
        //OnGUI를 통한 화면에 가시화
        //카메라 화면 크기, 카메라에 쓰일 텍스쳐 값(웹 캠 텍스쳐), 화면에 맟게 그리기
        //GUI.DrawTexture(screenRect, CM.tex, ScaleMode.ScaleToFit);

        try
        {
            //Decode를 통한 QRcode 읽어 들이기. 
            IBarcodeReader barcodeReader = new BarcodeReader();
            var result = barcodeReader.Decode(CM.tex.GetPixels32(), CM.tex.width, CM.tex.height);
            //만약 결과 값이 널이 아니면
            if (result != null)
            {
                //인식한 QRcode의 텍스트 값을 로그.
                Debug.Log(result.Text);
                //strBarcodeRead = result.Text;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.Message);
        }
    }
}
