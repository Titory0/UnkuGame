using UnityEngine;

public class FollowObjectWithLag : MonoBehaviour
{

    /// <summary>
    /// / This script detach the following object from any parent and then makes it follow the gameobject 
    /// </summary>
    public Transform followingObject;

    public float followSpeed = 0.5f;
    public Vector3 followOffset;

    private void Start()
    {
        followingObject.SetParent(null);
    }
    void Update()
    {
        if (followingObject == null)
        {
            Debug.LogError("Following object is not assigned in FollowObjectWithLag script on : " + gameObject.name);
            Destroy(this);
            return;
        }

        followingObject.position = Vector3.Lerp
                                                    (
                                                        followingObject.position - followOffset
                                                        , transform.position,
                                                        followSpeed * Time.deltaTime
                                                    )
                                                    + followOffset;

    }
}
