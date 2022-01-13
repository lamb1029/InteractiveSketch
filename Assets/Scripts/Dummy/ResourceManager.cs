using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        if(typeof(T) == typeof(GameObject))
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
        if(orgonal == null)
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
        if(sp == null)
        {
            Debug.Log($"Failed to load texture : {name}");
            return null;
        }

        return sp;
    }
}
