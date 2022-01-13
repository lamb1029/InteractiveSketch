using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollow : MonoBehaviour
{
    SettingManager setting;

    [SerializeField] PathCreator[] paths;
    PathCreator[] Air_paths;
    PathCreator[] Car_paths;
    PathCreator[] Balloon_paths;
    Transform[] objects;

    int routeToGo;

    float tparam;

    Vector2 objectPosition;

    public float speed;

    bool coroutineAllowed;

    public bool isDestroyloop; //�Ʒ� ����� ������� ���� ����
    [Tooltip("������ŭ �ݺ��� ����")]
    int destroyLoopTime;
    private int loop; //�ݺ��ɶ����� 1�� ���� destoryTime�� �������� ������Ʈ �ı�

    int pathnumber = -1;
    int randomVehicle;

    // Start is called before the first frame update
    void Start()
    {
        setting = FindObjectOfType<SettingManager>();
        routeToGo = 0;
        tparam = 0f;
        coroutineAllowed = true;
        loop = 0;
        //paths = FindObjectsOfType<PathCreator>();  //��� ��� Ž��
        Air_paths = GameObject.Find("Air_Paths").GetComponentsInChildren<PathCreator>();
        Car_paths = GameObject.Find("Car_Paths").GetComponentsInChildren<PathCreator>();
        Balloon_paths = GameObject.Find("Balloon_Paths").GetComponentsInChildren<PathCreator>();

        objects = GetComponentsInChildren<Transform>();
        if (objects.Length >= 2)  //Ż���� ������ ���� Ż�� ����
        {
            randomVehicle = Random.Range(1, objects.Length);
            RandomVehicle(randomVehicle);
        }
        IsDestroyLoop();

        //���� �ӵ�
        speed = Random.Range(0.2f, 0.5f);

        GameObject vehicle = objects[randomVehicle].gameObject;
        //ĳ���� ��ġ����
        if (vehicle.CompareTag("Balloon"))
        {
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Balloon";
            vehicle.GetComponent<SpriteRenderer>().sortingLayerName = "Balloon";
            vehicle.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder - 1;
        }
        else if (vehicle.CompareTag("Air"))
        {
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Air";
            vehicle.GetComponent<SpriteRenderer>().sortingLayerName = "Air";
            if (vehicle.name == "���̺����")
                vehicle.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder - 1;
            else
                vehicle.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        }
        else if (vehicle.CompareTag("Car"))
        {
            gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Car";
            vehicle.GetComponent<SpriteRenderer>().sortingLayerName = "Car";
            vehicle.GetComponent<SpriteRenderer>().sortingOrder = gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pathnumber < 0)
        {
            //Ż���� �±� ������ �̿��� ��� ����
            if (objects[randomVehicle].gameObject.CompareTag("Balloon"))
            {
                paths = Balloon_paths;
                pathnumber = Random.Range(0, paths.Length);
            }
            else if (objects[randomVehicle].gameObject.CompareTag("Car"))
            {
                paths = Car_paths;
                pathnumber = Random.Range(0, paths.Length);
            }
            else if (objects[randomVehicle].gameObject.CompareTag("Air"))
            {
                paths = Air_paths;
                pathnumber = Random.Range(0, paths.Length);
            }
        }

        if (coroutineAllowed)
        {
            StartCoroutine(GoByTheRoute(pathnumber, routeToGo));
        }
    }

    IEnumerator GoByTheRoute(int _pathnumber, int _routeNumber)
    {
        coroutineAllowed = false;

        Vector2 p0 = paths[_pathnumber].path.points[_routeNumber];
        Vector2 p1 = paths[_pathnumber].path.points[_routeNumber + 1];
        Vector2 p2 = paths[_pathnumber].path.points[_routeNumber + 2];
        Vector2 p3 = paths[_pathnumber].path.points[_routeNumber + 3];

        while (tparam <= 1)
        {
            tparam += Time.deltaTime * speed;

            objectPosition = Mathf.Pow(1 - tparam, 3) * p0 + 3 * Mathf.Pow(1 - tparam, 2) * tparam * p1 + 3 * (1 - tparam) * Mathf.Pow(tparam, 2) * p2 + Mathf.Pow(tparam, 3) * p3;

            transform.position = objectPosition;
            yield return new WaitForEndOfFrame();
        }

        tparam = 0f;

        routeToGo += 3;

        if (routeToGo > paths[_pathnumber].path.points.Count - 2)
        {
            routeToGo = 0;
            
            if (isDestroyloop)  //isDestoryTim�� true�϶�
            {
                loop++; //�ݺ��� ������ 1����
                if (loop >= destroyLoopTime) //�������� ������Ʈ �ı�
                    Destroy(gameObject);
            }
        }

        coroutineAllowed = true;
    }

    void IsDestroyLoop()  //����â���� �ݺ�Ƚ�� �޾ƿ���
    {
        //isDestroyloop = setting.isDestroyLoop;
        destroyLoopTime = setting.destroyLoopTime;
    }

    void RandomVehicle(int _randomVehicle)
    {
        for (int i = 1; i < objects.Length; i++)
        {
            if (i == _randomVehicle)
                objects[i].gameObject.SetActive(true);
            else
                objects[i].gameObject.SetActive(false);
        }
    }
}
