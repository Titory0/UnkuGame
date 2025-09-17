using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public AnimationCurve baseCurve;
    public static CameraShake Instance;
    public bool isActive;
    public float shakeCount;
    List<Shake> activeShakes = new List<Shake>();

    Vector3 baseLocalPos;

    private void Start()
    {

        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        baseLocalPos = transform.localPosition;
    }

    void Update()
    {
        if (!isActive)
        {
            return;
        }
        UpdateCameraShake();
    }

    #region AddShaking
    public Shake AddShake(float shakeDistance = 1f, float shakesPerSec = 10f)
    {
        return SetShakeToList(false, 0f, shakeDistance, shakesPerSec);
    }
    public Shake AddShake(float neededTime = 1f, float shakeDistance = 1f, float shakesPerSec = 10f, AnimationCurve shakeCurve = null)
    {
        return SetShakeToList(true, neededTime, shakeDistance, shakesPerSec, shakeCurve);
    }
    public Shake SetShakeToList(bool useTime = true, float neededTime = 1f, float shakeDistance = 1f, float shakesPerSec = 10f, AnimationCurve shakeCurve = null)
    {
        Shake shake = new Shake(useTime, neededTime, shakeDistance, shakesPerSec, shakeCurve);
        activeShakes.Add(shake);

        return shake;
    }

    #endregion


    #region UpdateShaking

    void UpdateCameraShake()
    {
        Vector3 offset = Vector3.zero;

        for (int i = 0; i < activeShakes.Count; i++)
        {
            Shake shake = activeShakes[i];
            if (shake == null)
            {
                return;
            }
            if ((shake.currentDuration >= shake.maxDuration) && (shake.hasTime == true) && (shake.shouldStop == false))
            {
                StopShake(shake);
                continue;
            }
            offset += shake.UpdateShake();
        }
        if (offset.magnitude > 0.01f)
        {
            transform.localPosition = Quaternion.Inverse(transform.rotation) * offset;
        }
        else
        {
            transform.localPosition = Vector3.zero;
        }
    }

    #endregion


    #region RemoveShake
    public void StopShake(Shake shake)
    {
        if (shake != null)
        {
            StartCoroutine(StopShakeCoroutine(shake));
        }
    }

    public IEnumerator StopShakeCoroutine(Shake shake)
    {
        //Security so it doesn't remove in the loop
        yield return null;

        shake.shouldStop = true;
        while (shake.hasStopped == false)
        {
            yield return null;
        }
        activeShakes.Remove(shake);

    }

    #endregion
}

public class Shake
{
    //Script info
    public float baseShakeTime;
    float currentShakeTime;

    public float currentMultiplier = 1f;
    Vector3 lastPos = Vector3.zero;
    Vector3 nextPos = Vector3.zero;
    Vector3 currentPos;

    public bool shouldStop;
    public bool hasStopped;

    //Parameters
    public bool hasTime;
    public float maxDuration;
    public float currentDuration;
    public float shakeDistance;
    public float shakePerSec;
    public AnimationCurve shakeCurve;

    public Shake(bool useTime = true, float neededTime = 1f, float shakeDistance = 1f, float shakesPerSec = 10f, AnimationCurve shakeCurve = null)
    {
        this.hasTime = useTime;
        this.maxDuration = neededTime;
        this.shakeDistance = shakeDistance;
        this.shakePerSec = shakesPerSec;
        this.shakeCurve = shakeCurve;

        baseShakeTime = 1f / shakesPerSec;
        SetShakePos();
    }

    public Vector3 UpdateShake()
    {
        if (shakeDistance == 0)
        {
            return new Vector3(0, 0, 0);
        }

        currentDuration += Time.deltaTime;
        currentShakeTime += Time.deltaTime;

        currentMultiplier = (shakeCurve == null || hasTime == false) ? currentMultiplier : shakeCurve.Evaluate(currentDuration / maxDuration);

        if (currentShakeTime >= baseShakeTime)
        {
            currentPos = nextPos;
            currentShakeTime = 0f;
            SetShakePos();
        }
        else
        {
            currentPos = Vector3.Lerp(lastPos, nextPos, EasingFunctions.InOutSine(currentShakeTime / baseShakeTime));
        }

        return currentPos * currentMultiplier;
    }
    void SetShakePos()
    {
        lastPos = nextPos;

        float durationAfterShake = currentDuration + baseShakeTime;
        if (hasTime == true && durationAfterShake >= maxDuration)
        {
            nextPos = Vector3.zero;
            if (lastPos == Vector3.zero)
            {
                hasStopped = true;
            }
            return;
        }

        //Verifie si le shake doit d'arreter ou non en engageant le prochain shake, si l'ancien shake était déja dirrigé vers l'arret préviens qu'il s'arrette
        if (shouldStop)
        {
            nextPos = Vector3.zero;
            return;
        }


        if (lastPos == Vector3.zero)
        {
            nextPos = Random.insideUnitCircle.normalized * shakeDistance;
            return;
        }

        //Shake distance est la moitié de la distance max car c'est le rayon
        int count = 0;
        int maxRandomTry = 10;  

        while ((nextPos - lastPos).magnitude < shakeDistance * 1.5f)
        {
            if (count++ > maxRandomTry)
            {
                break;
            }
            nextPos = Random.insideUnitCircle.normalized * shakeDistance;
        }
    }
}
