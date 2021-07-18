using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains a list of 3D points that make up a chain of quadratic bezier curves
/// </summary>
[System.Serializable]
public class BezierPath
{
    [SerializeField]
    private List<Vector3> points;
    //private BezierSegment[] segments;

    public void ReversePoints()
    {
        points.Reverse();
    }

    public BezierPath()
    {
        points = new List<Vector3>
        {
            Vector3.left,
            Vector3.forward * 0.5f,
            Vector3.back * 0.5f,
            Vector3.right
        };
    }

    public Vector3 this[int i]
    {
        get
        {
            return points[i];
        }
    }

    public int NumPoints
    {
        get
        {
            return points.Count;
        }
    }

    public int NumSegments
    {
        get
        {
            return (points.Count - 1) / 3;
        }
    }

    public int NumAnchors
    {
        get
        {
            return (points.Count + 2) / 3;
        }
    }

    public void MovePoint(int i, Vector3 pos)
    {
        points[i] = pos;
    }

    // Source: https://en.wikipedia.org/wiki/B%C3%A9zier_curve
    public static Vector3 CubicBezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        float oneMinusT = 1 - t;
        float tCubed = t * t * t;
        float tSqOmt3 = 3 * t * t * oneMinusT;
        float tOmtSq3 = 3 * t * oneMinusT * oneMinusT;
        float omtCubed = oneMinusT * oneMinusT * oneMinusT;
        return new Vector3(
            (a.x * omtCubed) + (b.x * tOmtSq3) + (c.x * tSqOmt3) + (d.x * tCubed),
            (a.y * omtCubed) + (b.y * tOmtSq3) + (c.y * tSqOmt3) + (d.y * tCubed),
            (a.z * omtCubed) + (b.z * tOmtSq3) + (c.z * tSqOmt3) + (d.z * tCubed)
        );
    }

    private Vector3 CubicBezier(BezierSegment p, float t)
    {
        return CubicBezier(p.a, p.b, p.c, p.d, t);
    }

    /*public void CreateSegments()
    {
        segments = new BezierSegment[this.NumSegments];
        for (int i = 0; i < segments.Length; i++)
        {
            segments[i] = new BezierSegment(
                points[i * 3],
                points[(i * 3) + 1],
                points[(i * 3) + 2],
                points[(i * 3) + 3]);
        }
    }*/

    /// <summary>
    /// Returns the position of a point on the path when given t, the progress along the path.
    /// </summary>
    /// <param name="t"> The progress along the path with floor(t) representing segment number, and t-floor(t) representing progress along the segment.</param>
    /// <returns></returns>
    public Vector3 GetPathPoint(float t)
    {
        if (t < 0 || this.NumSegments < t)
        {
            t = Mathf.Clamp(t, 0, this.NumSegments);
            Debug.LogWarning("For GetPathPoint(t), argument passed into t is outside of the proper index and has been clamped");
        }
        if (t == this.NumSegments)
        {
            return CubicBezier(GetSegment(this.NumSegments - 1), 1.0f);
        }
        else
        {
            int seg = (int)Mathf.Floor(t);
            return CubicBezier(GetSegment(seg), t - Mathf.Floor(t));
        }
    }

    public struct BezierSegment
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
        public Vector3 d;

        public BezierSegment(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            this.a = p1;
            this.b = p2;
            this.c = p3;
            this.d = p4;
        }
    }

    /// <summary>
    /// Returns the points which make up the requested segment
    /// </summary>
    /// <param name="seg"></param>
    /// <returns> A list of 4 3D positions, the 4 points which make up the quadratic bezier curve.</returns>
    public BezierSegment GetSegment(int seg)
    {
        if (0 <= seg && seg < this.NumSegments)
        {
            return new BezierSegment(
                points[seg * 3],
                points[(seg * 3) + 1],
                points[(seg * 3) + 2],
                points[(seg * 3) + 3]);
        }
        else
        {
            throw new System.IndexOutOfRangeException("Segment index out of range");
        }
    }

    /// <summary>
    /// Adds 3 points to create another segment within the path at a given segment index
    /// </summary>
    /// <param name="seg"></param>
    /// <param name="p1"> The point that will have the lowest index.</param>
    /// <param name="p2"> The point inbetween p1 and p3.</param>
    /// <param name="p3"> The point that will have the highest index.</param>
    public void AddSegment(int seg, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        int index;
        if (seg == 0)
        {
            index = 0;
        }
        else if (0 < seg && seg < this.NumSegments + 1)
        {
            index = (3 * seg) - 1;
        }
        else if (seg == this.NumSegments + 1)
        {
            index = (3 * seg) - 2;
        }
        else
        {
            throw new System.IndexOutOfRangeException("Segment index out of range");
        }
        points.Insert(index, p3);
        points.Insert(index, p2);
        points.Insert(index, p1);
    }

    /// <summary>
    /// Returns the position of an anchor, the points that connect each bezier curve to form the path
    /// </summary>
    /// <param name="anc"></param>
    /// <returns></returns>
    public Vector3 GetAnchor(int anc)
    {
        if (0 <= anc && anc <= this.NumAnchors - 1)
        {
            return points[3 * anc];
        }
        else
        {
            throw new System.IndexOutOfRangeException("Anchor index out of range");
        }
    }

    /// <summary>
    /// Removes the desired anchor point along with 2 other non-anchor points to remove a segment from the path.
    /// </summary>
    /// <param name="anc"></param>
    public void RemoveAnchor(int anc)
    {
        if (this.NumSegments <= 1)
        {
            throw new System.Exception("BezierPath must have at least 1 segment");
        }
        int index;
        if (anc == 0)
        {
            index = 0;
        }
        else if (0 < anc && anc < this.NumAnchors - 1)
        {
            index = (3 * anc) - 1;
        }
        else if (anc == this.NumAnchors - 1)
        {
            index = (3 * anc) - 2;
        }
        else
        {
            throw new System.IndexOutOfRangeException("Anchor index out of range");
        }
        points.RemoveAt(index);
        points.RemoveAt(index);
        points.RemoveAt(index);
    }
}