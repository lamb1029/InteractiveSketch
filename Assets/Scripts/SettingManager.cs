using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    ScannerManager SM;
    backgroundTransparentManager BTM;

    public Dropdown dropdown;
    int options;

    public Text pathText;
    string path;
    string current_path;
    string next_path;

    public bool saveMask;
    public Toggle toggle;
    public Text toggleText;

    public int width;
    public int height;

    //public bool isDestroyLoop;
    public int destroyLoopTime;

    private void Start()
    {
        SM = FindObjectOfType<ScannerManager>();
        BTM = FindObjectOfType<backgroundTransparentManager>();

        //��Ӵٿ� �ʱ�ȭ
        dropdown.ClearOptions();
        options = WebCamTexture.devices.Length;

        //���۽� path�� ����ְų� ������ path ����
        if (string.IsNullOrEmpty(path))
        {
            path = "C:/Users/USER/Documents/InteractiveSketch/";
            pathText.text = path;
        }
        //path�� ������ ���� ����
        if (Directory.Exists(path) == false)
        {
            Debug.Log("��ΰ� �����ϴ�. �����մϴ�");

            Directory.CreateDirectory(path);
            CreateFile();
        }

        //WebCam��ŭ ��Ӵٿ� �ɼ� ����
        for (int i = 0; i < options; i++)
        {
            Dropdown.OptionData newData = new Dropdown.OptionData();
            newData.text = $"Camera {i + 1}";
            dropdown.options.Add(newData);
        }
    }

    private void Update()
    {
        Getpath();
        ToggleText();
    }

    //path ���� ��ư
    public void OpenFolderExplorer()
    {
        path = EditorUtility.OpenFolderPanel("���� ���", path, "") + "/";
        if (path == "/") //���Ű �������� ���
        {
            path = current_path;
        }
        pathText.text = path;
        CreateFile();
    }

    public void SaveMask(bool _bool)
    {
        saveMask = _bool;
    }

    void ToggleText()
    {
        if (toggle.isOn == true)
        {
            toggleText.text = "Mask�� �����Ͽ� ����մϴ�.";
        }
        else
        {
            toggleText.text = "�̸� �غ��� Mask�� ����մϴ�.";
        }
    }

    void Getpath()  //��� ����� �ٸ������� ��ε� ����
    {
        next_path = path;
        if (current_path != next_path)
        {
            BTM.mask_path = path + "Mask/";
            BTM.fin_path = path + "Image/";
            SM.capture_path = path + "Capture/";
            Debug.Log("��κ���");

            current_path = next_path;
        }
    }

    void CreateFile() //���� ���� ����
    {
        Directory.CreateDirectory(path + "QR/"); //QR�ڵ� ������ġ �ʿ��Ѱ�?
        Directory.CreateDirectory(path + "Capture/"); //�߶� ���� �̹��� ������ġ
        Directory.CreateDirectory(path + "Mask/"); //Mask �̹��� ������ġ
        Directory.CreateDirectory(path + "Image/"); //���� �̹��� ������ġ
    }
}
