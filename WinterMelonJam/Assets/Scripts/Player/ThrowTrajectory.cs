using UnityEngine;

public class ThrowTrajectory : MonoBehaviour
{
    [Header ("Line Renderer")]
    [SerializeField] private int lineRendResolution = 15;
    private LineRenderer lineRend;
    private Vector2 curVel;
    private float grav = -9.81f;
    private Vector3 curPos = Vector3.zero;

    [Header ("Collision")]
    [SerializeField] private int lineCastResolution = 15;
    [SerializeField] private float trajectoryMinYPos = -8f;
    [SerializeField] private LayerMask solidLayers;

    private void Awake()
    {
        lineRend = GetComponent<LineRenderer>();
        grav = Mathf.Abs(Physics2D.gravity.y);
    }

    public void UpdateThrowTrajectory(Vector2 throwVel, Vector3 throwPos)
    {
        curPos = throwPos;
        curVel = throwVel;

        // Calculate each point to renderer a line on
        lineRend.positionCount = lineRendResolution + 1;
        lineRend.SetPositions(CalculateLineArray());
    }

    public void StartThrowTrajectory()
    {
        lineRend.enabled = true;
    }

    public void StopThrowTrajectory()
    {
        lineRend.enabled = false;
    }


    // Calculate an array of points to draw a line on that connects them all
    private Vector3[] CalculateLineArray()
    {
        Vector3[] lineArr = new Vector3[lineRendResolution + 1];

        var timeStep = MaxTimeX() / lineRendResolution;   // The amount of time between each point in line renderer

        for(int i = 0; i < lineArr.Length; i++)
        {
            var t = timeStep * i;
            lineArr[i] = CalculateLinePoint(t);
        }

        return lineArr;
    }

    // Return the position that the line collides with a solid object
    private Vector2 HitPosition()
    {
        var timeStep = MaxTimeY() / lineCastResolution; // The amount of time between each point in line renderer

        for (int i = 0; i < lineCastResolution + 1; i++)
        {
            var curTime = timeStep * i;
            var nextTime = timeStep * (i + 1);

            // Do a linecast from position of current timestep to next timestep
            var hit = Physics2D.Linecast(CalculateLinePoint(curTime), CalculateLinePoint(nextTime), solidLayers);

            if (hit)
                return hit.point;
        }

        return CalculateLinePoint(MaxTimeY());
    }

    // Returns the point that the box will be at time t with current position and velocity
    private Vector3 CalculateLinePoint(float t)
    {
        float x = curVel.x * t;
        float y = (curVel.y * t) - (grav * Mathf.Pow(t, 2) / 2);
        return new Vector3(x + curPos.x, y + curPos.y);
    }

    // Returns the time at which the box will reach the trajectoryMinYPos given its current position, velocity, and gravity
    private float MaxTimeY()
    {
        var velSqr = curVel.y * curVel.y;
        return (curVel.y + Mathf.Sqrt(velSqr + 2 * grav * (curPos.y - trajectoryMinYPos))) / grav;
    }

    // Return the time at which the box will collide with a solid object given its current position and velocity.
    private float MaxTimeX()
    {
        var xVel = curVel.x;
        if(Mathf.Abs(xVel) < 0.0001f)   // Prevent division by 0
            return MaxTimeY();

        var t = (HitPosition().x - curPos.x) / xVel;
        return t;
    }

}
