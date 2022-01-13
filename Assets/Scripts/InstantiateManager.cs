using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class InstantiateManager : MonoBehaviour
{
    #region variable
    public string _path; //���� ���

    public Transform spown; //������Ʈ ���� ��ġ

    backgroundTransparentManager BTM;

    //Ʋ ������ŭ �߰�
    public List<GameObject> objects = new List<GameObject>();
    string objectname;
    string texturename;
    string lastname;
    int Order;
    #endregion variable

    private void Start()
    {
        BTM = FindObjectOfType<backgroundTransparentManager>();
        spown = GameObject.Find("Spown").transform;  //������Ʈ ������ġ ����
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
            Debug.Log("SpriteRenderer�� ����!");
            go.AddComponent<SpriteRenderer>();
            sr = go.GetComponent<SpriteRenderer>();
        }
        Debug.Log($"{objectname}�� {texturename}�� ������ �����߽��ϴ�.");

        sr.sprite = sp;
        //sr.sortingLayerName = "Character";
        sr.sortingOrder = Order;
        Order += 5;
        go.transform.localScale = new Vector3(0.1f, 0.1f, 0); //������Ʈ ������ ����
        go.transform.rotation = Quaternion.Euler(0, 0, 90); //ĳ���� ȸ��
        go.name = name;
    }

    #region Texture2D To Sprite
    //Ư�� �������� �̹��� �̸� �޾ƿ� �����ϱ�
    public List<string> textures = new List<string>();
    public Sprite SaveImage(string path)
    {
        DirectoryInfo currentDirectory = new DirectoryInfo(path);

        Debug.Log($"��� : {currentDirectory.FullName}");
        if (currentDirectory.Exists == false) //��ΰ� �������� ������ ���� ���� -> ��ο� ���� �������� ����
        {
            Debug.Log($"{path}�� �������� ����");
            return null;
        }

        foreach (FileInfo file in currentDirectory.GetFiles()) //������ �����ϴ� ��� ���� �̸� ����
        {
            textures.Add(file.Name);
        }

        textures = textures.Distinct().ToList();  //�ߺ� ����

        byte[] byteTexture = File.ReadAllBytes(path + $"/{textures.Last()}"); //textures�� �������� ����� ����(png)�� sprite�� ���� 
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
        //���� ��� ����
        Sprite sp = Load<Sprite>($"Textures/{name}");
        if (sp == null)
        {
            Debug.Log($"Failed to load texture : {name}");
            return null;
        }

        return sp;
    }
    #endregion

    #region  �ʱ� �����Լ�
    public void Instantiate2D(string objectname, string texturename)
    {
        GameObject go = Instantiate($"{objectname}");
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.Log("SpriteRenderer�� ����!");
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
            Debug.Log("texturename�� ������ϴ�!");
            return;
        }

        if (texturename == lastname)
        {
            Debug.Log("���� ������ �̹����� �����ϴ�!");
            return;
        }

        GameObject go = Instantiate($"{objectname}");
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.Log("SpriteRenderer�� ����!");
            go.AddComponent<SpriteRenderer>();
            sr = go.GetComponent<SpriteRenderer>();
        }
        Debug.Log($"{objectname}�� {texturename}�� ������ �����߽��ϴ�.");

        sr.sprite = sp;
        lastname = texturename;
        texturename = null;
    }

    //�̹��� �̸� �޾ƿ� ������ ����
    void Gettexturename()
    {
        if (textures.Count == 0)
        {
            Debug.Log("����� �̹����� �����ϴ�!");
            return;
        }

        texturename = textures.Last();
    }
    #endregion
}
