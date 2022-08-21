using UnityEngine;

public class SosDemo_DrawLine : MonoBehaviour
{
    [Range(0.001f, 30)]
    public float f = 1;

    [Range(-1, 4)]
    public float z = 0.5f;

    [Range(-3, 3)]
    public float r = 2;


    private void Update()
    {
        var startPos = new Vector3(-5, 0, 0);
        var sod = new SecondOrderDynamics(f, z, r, startPos);
        var target = startPos + Vector3.forward + Vector3.right * 5f;
        var lastPoint = Vector3.zero;
        for (int i = 0; i < 2000; i++)
        {
            var pos = sod.Update(0.001f, target);

            var x = 0.001f * i;
            var y = (pos - startPos).magnitude / (target - startPos).magnitude;
            var point = new Vector3(x, 0, y);
            Debug.DrawLine(lastPoint, point, Color.cyan);
            lastPoint = point;
        }
    }
}