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

        //드롭다운 초기화
        dropdown.ClearOptions();
        options = WebCamTexture.devices.Length;

        //시작시 path가 비어있거나 없으면 path 지정
        if (string.IsNullOrEmpty(path))
        {
            path = "C:/Users/USER/Documents/InteractiveSketch/";
            pathText.text = path;
        }
        //path가 없으면 폴더 생성
        if (Directory.Exists(path) == false)
        {
            Debug.Log("경로가 없습니다. 생성합니다");

            Directory.CreateDirectory(path);
            CreateFile();
        }

        //WebCam만큼 드롭다운 옵션 생성
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

    //path 변경 버튼
    public void OpenFolderExplorer()
    {
        path = EditorUtility.OpenFolderPanel("저장 경로", path, "") + "/";
        if (path == "/") //취소키 눌렀을시 경로
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
            toggleText.text = "Mask를 생성하여 사용합니다.";
        }
        else
        {
            toggleText.text = "미리 준비한 Mask를 사용합니다.";
        }
    }

    void Getpath()  //경로 변경시 다른곳들의 경로도 변경
    {
        next_path = path;
        if (current_path != next_path)
        {
            BTM.mask_path = path + "Mask/";
            BTM.fin_path = path + "Image/";
            SM.capture_path = path + "Capture/";
            Debug.Log("경로변경");

            current_path = next_path;
        }
    }

    void CreateFile() //하위 파일 생성
    {
        Directory.CreateDirectory(path + "QR/"); //QR코드 저장위치 필요한가?
        Directory.CreateDirectory(path + "Capture/"); //잘라낸 문서 이미지 저장위치
        Directory.CreateDirectory(path + "Mask/"); //Mask 이미지 저장위치
        Directory.CreateDirectory(path + "Image/"); //최종 이미지 저장위치
    }
}
