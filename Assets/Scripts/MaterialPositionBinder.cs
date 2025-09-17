using UnityEngine;
[ExecuteAlways]
public class MaterialPositionBinder : MonoBehaviour
{
    public Transform reference;
    public SpriteRenderer targetRenderer;
    Material mat;
    public Material displayMat;
    public string propertyName = "Position";
    public bool executeInEditMode = true;
    private void Start()
    {
        mat = targetRenderer.sharedMaterial;
    }
    void Update()
    {

        if (executeInEditMode && Application.isPlaying == false) 
        {
            displayMat.SetVector(propertyName, reference.position); 
            return;
        }
        else
        {
            if(Application.isPlaying && mat != null)
            {
                mat.SetVector(propertyName, reference.position);
            }
        }

    }
}
