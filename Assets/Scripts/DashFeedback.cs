using System.Collections;
using UnityEngine;

public class DashFeedback : MonoBehaviour
{
    [Header("Feedback")]
    TrailRenderer trailRenderer;

    public AnimationCurve trailOutCurve;
   
    public float trailOutTime = 0.5f;
    float baseTrailTime;

    bool wasDahsing = false;
    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        baseTrailTime = trailRenderer.time;
    }

    private void Update()
    {
        if(PlayerState.Instance.isDashing)
        {
            trailRenderer.emitting = true;
            wasDahsing = true;
        }
        else
        {
            if(wasDahsing)
            {
                wasDahsing = false;
                StartCoroutine(DisableTrailAfterTime());
            }
        }
    }

    public IEnumerator DisableTrailAfterTime()
    {
        while (trailRenderer.time != 0)
        {
            trailRenderer.time -= Time.deltaTime / trailOutTime;
            trailRenderer.time = Mathf.Max(trailRenderer.time, 0);
            yield return null;
        }

        trailRenderer.emitting = false;
        trailRenderer.time = baseTrailTime;
    }
}
