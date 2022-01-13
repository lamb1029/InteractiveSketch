using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    SettingManager setting;

    public PathFollow[] go;

    public int pool;

    // Start is called before the first frame update
    void Start()
    {
        setting = FindObjectOfType<SettingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        go = GetComponentsInChildren<PathFollow>();

        //if(setting.isDestroyLoop == false)
        //{
        //    if (go.Length > pool)
        //    {
        //        Destroy(go[0].gameObject);
        //    }
        //}

        if (go.Length > pool)
        {
            for(int i = 0; i < go.Length - pool; i++)
            {
                go[i].isDestroyloop = true;
            }
        }
    }
}
