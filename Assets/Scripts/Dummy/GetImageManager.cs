using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class GetImageManager
{
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
}
