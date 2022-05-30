using UnityEngine;

public class WallDetectionLayer
{
    private float priority = 0;
    private readonly (bool, Vector3) FAIL = (false, Vector3.zero);
    public float Priority => priority;

    private struct WallDetections<T>
    {
        public T leftBack;
        public T left;
        public T leftFront;
        public T front;
        public T rightFront;
        public T right;
        public T rightBack;
    }

    private WallDetections<Ray> rays = new WallDetections<Ray>();

    private WallDetections<bool> status = new WallDetections<bool>();

    private WallDetections<Vector3> normals = new WallDetections<Vector3>();

    public WallDetectionLayer(float frontAngles, float backAngles, float height, float priority)
    {
        this.priority = priority;
        Vector3 origin = Vector3.zero + (Vector3.up * height);

        rays.front = new Ray(origin, Vector3.forward);
        Vector3 rightFrontD = Vector3.RotateTowards(Vector3.forward, Vector3.right, frontAngles * Mathf.Deg2Rad, 0);
        rays.rightFront = new Ray(origin, rightFrontD);
        Vector3 leftFrontD = Vector3.RotateTowards(Vector3.forward, Vector3.left, frontAngles * Mathf.Deg2Rad, 0);
        rays.leftFront = new Ray(origin, leftFrontD);

        rays.right = new Ray(origin, Vector3.right);
        rays.left = new Ray(origin, Vector3.left);

        Vector3 rightBackD = Vector3.RotateTowards(Vector3.right, Vector3.back, backAngles * Mathf.Deg2Rad, 0);
        rays.rightBack = new Ray(origin, rightBackD);
        Vector3 leftBackD = Vector3.RotateTowards(Vector3.left, Vector3.back, backAngles * Mathf.Deg2Rad, 0);
        rays.leftBack = new Ray(origin, leftBackD);
    }

    public bool LowerPriority(WallDetectionLayer otherLayer)
    {
        return this.Priority < otherLayer.Priority;
    }

    public void CheckWalls(Transform player, RigidbodyLinker rl, CollisionManagement cm, float distance, LayerMask layers)
    {
        (bool, Vector3) temp;
        temp = CheckDirection(rays.leftBack, player, rl, cm, distance, layers);
        status.leftBack = temp.Item1;
        normals.leftBack = temp.Item2;

        temp = CheckDirection(rays.left, player, rl, cm, distance, layers);
        status.left = temp.Item1;
        normals.left = temp.Item2;

        temp = CheckDirection(rays.leftFront, player, rl, cm, distance, layers);
        status.leftFront = temp.Item1;
        normals.leftFront = temp.Item2;

        temp = CheckDirection(rays.front, player, rl, cm, distance, layers);
        status.front = temp.Item1;
        normals.front = temp.Item2;

        temp = CheckDirection(rays.rightFront, player, rl, cm, distance, layers);
        status.rightFront = temp.Item1;
        normals.rightFront = temp.Item2;

        temp = CheckDirection(rays.right, player, rl, cm, distance, layers);
        status.right = temp.Item1;
        normals.right = temp.Item2;

        temp = CheckDirection(rays.rightBack, player, rl, cm, distance, layers);
        status.rightBack = temp.Item1;
        normals.rightBack = temp.Item2;
    }

    private (bool, Vector3) CheckDirection(Ray ray, Transform player, RigidbodyLinker rigidbodyLinker, CollisionManagement collisionManager, float distance, LayerMask layers)
    {
        Ray relativeRay = new Ray(player.TransformPoint(ray.origin), player.TransformDirection(ray.direction));
        Debug.DrawRay(relativeRay.origin, relativeRay.direction * distance, Color.yellow);
        if (Physics.Raycast(relativeRay, out RaycastHit hit, distance, layers))
        {
            if (!hit.transform.CompareTag("landable"))
            {
                return FAIL;
            }
            if (hit.rigidbody != null)
            {
                rigidbodyLinker.UpdateLink(hit.rigidbody);
            }

            float collisionAngle = Vector3.Angle(hit.normal, collisionManager.ValidUpAxis);
            if (collisionAngle > collisionManager.MaxSlope)
            {
                return (true, hit.normal);
            }
            else
            {
                return FAIL;
            }
        }
        return FAIL;
    }

    public (bool, Vector3) FrontDetected()
    {
        (bool, Vector3) success = (true, normals.front);
        if (!status.left && status.leftFront && status.front)
        {
            return success;
        }
        if (!status.right && status.rightFront && status.front)
        {
            return success;
        }
        if (status.front && status.leftFront && status.rightFront)
        {
            return success;
        }
        if (status.front && !status.leftFront && !status.rightFront)
        {
            return success;
        }

        return FAIL;
    }

    public (bool, Vector3) RightDetected()
    {
        if (!status.front && status.rightFront)
        {
            return (true, normals.rightFront);
        }
        if (status.right && !status.rightFront && !status.rightBack)
        {
            return (true, normals.right);
        }
        if (status.right && status.rightFront)
        {
            return (true, normals.right);
        }
        if (status.right && status.rightBack)
        {
            return (true, normals.right);
        }
        if (!status.right && status.rightBack)
        {
            return (true, normals.rightBack);
        }

        return FAIL;
    }

    public (bool, Vector3) LeftDetected()
    {
        if (!status.front && status.leftFront)
        {
            return (true, normals.leftFront);
        }
        if (status.left && !status.leftFront && !status.leftBack)
        {
            return (true, normals.left);
        }
        if (status.left && status.leftFront)
        {
            return (true, normals.left);
        }
        if (status.left && status.leftBack)
        {
            return (true, normals.left);
        }
        if (!status.left && status.leftBack)
        {
            return (true, normals.leftBack);
        }

        return FAIL;
    }
}