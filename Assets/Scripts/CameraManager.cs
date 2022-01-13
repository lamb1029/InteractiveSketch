using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class CameraManager : MonoBehaviour
{
    public List<Texture2D> Captures = new List<Texture2D>();
    public int currentCamIndex;
    int camIndex;

    InstantiateManager IM;
    backgroundTransparentManager BTM;
    ScannerManager SM;

    [HideInInspector]
    public WebCamTexture tex;

    public SettingManager setting;
    public RawImage webcam;
    public GameObject settingUI;
    public GameObject WarningUI;

    //public Text startstoptext;
    public RawImage target;

    public string QRtext;

    private void Start()
    {
        IM = FindObjectOfType<InstantiateManager>();
        BTM = FindObjectOfType<backgroundTransparentManager>();
        SM = FindObjectOfType<ScannerManager>();

        SetUI();
        StartStopCam_Clicked();
    }

    private void Update()
    {
        if (WebCamTexture.devices.Length > 0)
        {
            GetCamIndex();
            if (currentCamIndex == camIndex)
                return;
            currentCamIndex = camIndex;
            SwapCam();
        }
        else
        {
            Debug.Log("WebCam�� �����ϴ�.");
            SetUI(WarningUI);

            return; 
        }
    }

    private void SwapCam()
    {
        stopWebcam();
        StartStopCam_Clicked();
    }

    public void SwapCam_Clicked()
    {
        if (WebCamTexture.devices.Length > 0)
        {
            currentCamIndex += 1;
            currentCamIndex %= WebCamTexture.devices.Length;
        }
        if (tex != null)
        {
            stopWebcam();
            StartStopCam_Clicked();
        }
    }

    private void stopWebcam()
    {
        webcam.texture = null;
        tex.Stop();
        tex = null;
    }

    public void StartStopCam_Clicked()
    {
        if (tex != null)
        {
            stopWebcam();
            //startstoptext.text = "Start Camera";
        }
        else
        {
            WebCamDevice device = WebCamTexture.devices[currentCamIndex];
            tex = new WebCamTexture(device.name);
            webcam.texture = tex;

            tex.Play();
            //startstoptext.text = "Stop Camera";
        }
    }

    //Capture��ư Ŭ���� ����
    public void CaptureCam_Clicked()
    {
        Texture2D tx = GetTexture2DFromWebcamTexture(tex);
        Captures.Add(tx);
        for(int i = 0; i < Captures.Count; i++)
        {
            string number = (i+1).ToString("0000");
            Captures[i].name = $"Capture {number}";
        }
        target.texture = tx;

        //ĸ��ȭ�鿡�� ������ �߶󳻱�
        //SM.SetTexture2D(tx);
        SM.SetTexture2D(tx);

        //�߶� �������� QR�ڵ� �ϱ�
        //ReaderQR(SM.baseTexture);
        ReaderQR(SM.baseTexture);

        //QR�ڵ�� mask���� ���� �� �״�� ��� ����ȭ(����)
        BTM.backgroundTransparent();

        //�������ȭ�� �ؽ��� ���� ������Ʈ ����
        IM.InstantiateObject("Test", tx.name);
    }

    public Texture2D GetTexture2DFromWebcamTexture(WebCamTexture webCamTexture)
    {
        // Create new texture2d
        Texture2D tx2d = new Texture2D(webCamTexture.width, webCamTexture.height);
        // Gets all color data from web cam texture and then Sets that color data in texture2d
        tx2d.SetPixels(webCamTexture.GetPixels());
        // Applying new changes to texture2d
        tx2d.Apply();

        Resources.UnloadUnusedAssets();

        return tx2d;
    }

    void GetCamIndex()
    {
        string op = setting.dropdown.options[setting.dropdown.value].text;
        int index = op.LastIndexOf(" ");
        if (index >= 0)
            op = op.Substring(index + 1);
        camIndex = int.Parse(op) - 1;
    }

    public void SettingOn_Click()
    {
        settingUI.SetActive(true);
    }

    public void SettingOff_Click()
    {
        settingUI.SetActive(false);
    }

    void SetUI(GameObject name = null)
    {
        WarningUI.SetActive(false);
        settingUI.SetActive(false);
        if (name == null)
            return;

        name.SetActive(true);
    }

    void ReaderQR(Texture2D texture2D)  //�̹������� qr�ڵ� �б�
    {
        IBarcodeReader barcodeReader = new BarcodeReader();
        var result = barcodeReader.Decode(texture2D.GetPixels32(), texture2D.width, texture2D.height);
        //���� ��� ���� ���� �ƴϸ�
        if (result != null)
        {
            //�ν��� QRcode�� �ؽ�Ʈ ���� �α�.
            Debug.Log(result.Text);
            Debug.Log("QR�ڵ� ����!");
            QRtext = result.Text;
        }
        else
            Debug.Log("QR�ڵ� �� ����!");
    }
}
