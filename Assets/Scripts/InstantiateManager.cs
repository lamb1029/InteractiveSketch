using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class InstantiateManager : MonoBehaviour
{
    #region variable
    public string _path; //폴더 경로

    public Transform spown; //오브젝트 생성 위치

    backgroundTransparentManager BTM;

    //틀 개수만큼 추가
    public List<GameObject> objects = new List<GameObject>();
    string objectname;
    string texturename;
    string lastname;
    int Order;
    #endregion variable

    private void Start()
    {
        BTM = FindObjectOfType<backgroundTransparentManager>();
        spown = GameObject.Find("Spown").transform;  //오브젝트 생성위치 지정
        Order = 0;
        //if(string.IsNullOrEmpty(_path))
        //{
        //    _path = "C:/Users/USER/Documents/InteractiveSketch/";
        //}
    }

    public void InstantiateObject(string objectname, string name)
    {
        //Texture2D tx = SM.target;
        Texture2D tx = (Texture2D)BTM.target;
        Rect rect = new Rect(0, 0, tx.width, tx.height);
        Sprite sp = Sprite.Create(tx, rect, new Vector2(0.5f, 0.5f));
        sp.name = name;


        GameObject go = Instantiate(objectname, spown);
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.Log("SpriteRenderer가 없음!");
            go.AddComponent<SpriteRenderer>();
            sr = go.GetComponent<SpriteRenderer>();
        }
        Debug.Log($"{objectname}에 {texturename}을 입혀서 생성했습니다.");

        sr.sprite = sp;
        //sr.sortingLayerName = "Character";
        sr.sortingOrder = Order;
        Order += 5;
        go.transform.localScale = new Vector3(0.1f, 0.1f, 0); //오브젝트 사이즈 조절
        go.transform.rotation = Quaternion.Euler(0, 0, 90); //캐릭터 회전
        go.name = name;
    }

    #region Texture2D To Sprite
    //특정 폴더에서 이미지 이름 받아와 저장하기
    public List<string> textures = new List<string>();
    public Sprite SaveImage(string path)
    {
        DirectoryInfo currentDirectory = new DirectoryInfo(path);

        Debug.Log($"경로 : {currentDirectory.FullName}");
        if (currentDirectory.Exists == false) //경로가 존재하지 않으면 실행 종료 -> 경로에 파일 생성으로 수정
        {
            Debug.Log($"{path}가 존재하지 않음");
            return null;
        }

        foreach (FileInfo file in currentDirectory.GetFiles()) //폴더에 존재하는 모든 파일 이름 저장
        {
            textures.Add(file.Name);
        }

        textures = textures.Distinct().ToList();  //중복 제거

        byte[] byteTexture = File.ReadAllBytes(path + $"/{textures.Last()}"); //textures에 마지막에 저장된 파일(png)을 sprite로 생성 
        Texture2D texture = new Texture2D(0, 0);
        texture.LoadImage(byteTexture);

        Rect rect = new Rect(0, 0, texture.width, texture.height);
        //Rect rect = new Rect(0, 0, 512, 512);
        return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
    }
#endregion
    #region Resources
    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf("/");
            if (index >= 0)
                name = name.Substring(index + 1);

        }

        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string name, Transform parent = null)
    {
        GameObject orgonal = Load<GameObject>($"Prefabs/{name}");
        if (orgonal == null)
        {
            Debug.Log($"Failed to load prefab : {name}");
            return null;
        }

        GameObject go = Object.Instantiate(orgonal, parent);

        go.name = orgonal.name;

        return go;
    }

    public Sprite GetTexture(string name)
    {
        //추후 경로 변경
        Sprite sp = Load<Sprite>($"Textures/{name}");
        if (sp == null)
        {
            Debug.Log($"Failed to load texture : {name}");
            return null;
        }

        return sp;
    }
    #endregion

    #region  초기 생성함수
    public void Instantiate2D(string objectname, string texturename)
    {
        GameObject go = Instantiate($"{objectname}");
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.Log("SpriteRenderer가 없음!");
            go.AddComponent<SpriteRenderer>();
            sr = go.GetComponent<SpriteRenderer>();
        }
        Sprite sp = GetTexture($"{texturename}");

        sr.sprite = sp;
    }

    public void Instantiate2D(string objectname)
    {
        Sprite sp = SaveImage(_path);
        Gettexturename();

        if (texturename == null)
        {
            Debug.Log("texturename이 비었습니다!");
            return;
        }

        if (texturename == lastname)
        {
            Debug.Log("새로 생성된 이미지가 없습니다!");
            return;
        }

        GameObject go = Instantiate($"{objectname}");
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.Log("SpriteRenderer가 없음!");
            go.AddComponent<SpriteRenderer>();
            sr = go.GetComponent<SpriteRenderer>();
        }
        Debug.Log($"{objectname}에 {texturename}을 입혀서 생성했습니다.");

        sr.sprite = sp;
        lastname = texturename;
        texturename = null;
    }

    //이미지 이름 받아와 변수에 적용
    void Gettexturename()
    {
        if (textures.Count == 0)
        {
            Debug.Log("저장된 이미지가 없습니다!");
            return;
        }

        texturename = textures.Last();
    }
    #endregion
}
