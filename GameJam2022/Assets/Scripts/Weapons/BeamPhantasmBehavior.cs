using System.Diagnostics;
using UnityEngine;

public class BeamPhantasmBehavior : MonoBehaviour
{
    public float dieTime = 510f;
    public float Radius { get; set; } = 10f;
    private GameObject start, end, line;
    private LineRenderer lineRenderer;
    private Stopwatch time = new Stopwatch();
    // Start is called before the first frame update
    void Start()
    {
        time.Start();
        start = transform.Find("Start").gameObject;
        end = transform.Find("End").gameObject;
        line = transform.Find("Line").gameObject;
        lineRenderer = line.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, start.transform.localPosition);
        lineRenderer.SetPosition(1, end.transform.localPosition);
        var mul = 1f - (((float)time.ElapsedMilliseconds) / (dieTime * 0.2f));
        lineRenderer.widthMultiplier = mul * 0.2f;
        if (time.ElapsedMilliseconds > dieTime)
        {
            Destroy(this.transform.gameObject);
        }
    }

    void OnDrawGizmos()
    {
        if (start == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(start.transform.position, Radius);
    }
}
