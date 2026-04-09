using UnityEngine;
using System.Collections.Generic;

public class TrajectoryView : MonoBehaviour
{
    [SerializeField] private LineRenderer normalLine;
    [SerializeField] private LineRenderer perfectLine1;
    [SerializeField] private LineRenderer perfectLine2;

    public void ShowTrajectory(List<Vector2> points, bool isPerfect, List<Vector2> pointsPerfect1 = null, List<Vector2> pointsPerfect2 = null)
    {
        if (!isPerfect)
        {
            normalLine.gameObject.SetActive(true);
            perfectLine1.gameObject.SetActive(false);
            perfectLine2.gameObject.SetActive(false);
            DrawLine(normalLine, points);
        }
        else
        {
            normalLine.gameObject.SetActive(false);
            perfectLine1.gameObject.SetActive(true);
            perfectLine2.gameObject.SetActive(true);
            if (pointsPerfect1 != null) DrawLine(perfectLine1, pointsPerfect1);
            if (pointsPerfect2 != null) DrawLine(perfectLine2, pointsPerfect2);
        }
    }

    public void HideTrajectory()
    {
        normalLine.gameObject.SetActive(false);
        perfectLine1.gameObject.SetActive(false);
        perfectLine2.gameObject.SetActive(false);
    }

    private void DrawLine(LineRenderer line, List<Vector2> points)
    {
        line.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            line.SetPosition(i, new Vector3(points[i].x, points[i].y, 0));
        }
    }
}
