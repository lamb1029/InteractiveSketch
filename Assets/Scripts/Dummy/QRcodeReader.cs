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
        //OnGUI�� ���� ȭ�鿡 ����ȭ
        //ī�޶� ȭ�� ũ��, ī�޶� ���� �ؽ��� ��(�� ķ �ؽ���), ȭ�鿡 ���� �׸���
        //GUI.DrawTexture(screenRect, CM.tex, ScaleMode.ScaleToFit);

        try
        {
            //Decode�� ���� QRcode �о� ���̱�. 
            IBarcodeReader barcodeReader = new BarcodeReader();
            var result = barcodeReader.Decode(CM.tex.GetPixels32(), CM.tex.width, CM.tex.height);
            //���� ��� ���� ���� �ƴϸ�
            if (result != null)
            {
                //�ν��� QRcode�� �ؽ�Ʈ ���� �α�.
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
