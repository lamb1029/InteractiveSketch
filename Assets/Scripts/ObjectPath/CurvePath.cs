using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CurvePath
{
    public List<Vector2> points;

    public CurvePath(Vector2 centre) //기본 위치 4곳 생성
    {
        points = new List<Vector2>
        {
            centre + Vector2.left, //첫 위치
            centre + (Vector2.left + Vector2.up) * 0.5f,
            centre + (Vector2.right + Vector2.down) * 0.5f,
            centre + Vector2.right //두번째 위치
        };
    }

    public Vector2 this[int i]
    {
        get
        {
            return points[i];
        }
    }

    public int NumPoints { get { return points.Count; } }

    public int Numsegment //경로에 있는 세그먼트 수
    {
        get
        {
            return (points.Count - 4) / 3 + 1;
        }
    }

    public void AddSegment(Vector2 anchorPos) //새로운 위치 추가
    {
        points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
        points.Add((points[points.Count - 1] + anchorPos) * 0.5f);
        points.Add(anchorPos);
    }

    public Vector2[] GetPointsInsegment(int i)
    {
        return new Vector2[] { points[i * 3], points[i * 3 + 1], points[i * 3 + 2], points[i * 3 + 3] };
    }

    public void MovePoint(int i, Vector2 pos)
    {
        Vector2 deltaMove = pos - points[i];
        points[i] = pos;

        if (i % 3 == 0)
        {
            if (i + 1 < points.Count)
            {
                points[i + 1] += deltaMove;
            }
            if (i - 1 >= 0)
            {
                points[i - 1] += deltaMove;
            }
        }
        else
        {
            bool nextPointIsAnchor = (i + 1) % 3 == 0;
            int correspondingControlIndex = (nextPointIsAnchor) ? i + 2 : i - 2;
            int anchorIndex = (nextPointIsAnchor) ? i + 1 : i - 1;

            if (correspondingControlIndex >= 0 && correspondingControlIndex < points.Count)
            {
                float dst = (points[anchorIndex] - points[correspondingControlIndex]).magnitude;
                Vector2 dir = (points[anchorIndex] - pos).normalized;
                points[correspondingControlIndex] = points[anchorIndex] + dir * dst;
            }
        }
    }
}
