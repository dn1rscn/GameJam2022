using System.Diagnostics;
using UnityEngine;

public class BeamPhantasmBehavior : MonoBehaviour
{
    public long dieTime = 2000;
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
        if (time.ElapsedMilliseconds > dieTime)
        {
            Destroy(this.transform.gameObject);
        }
    }
}
