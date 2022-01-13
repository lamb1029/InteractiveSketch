using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class GetImageManager
{
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
}
