using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    public SpriteRenderer playerRenderer;
    Material playerMat;

    public float shieldActivationTime;

    public float shieldOutlineDistance;
    public float shieldOutlineWitdh;

    public float currentShieldActivation;
    public float currentOutlineDistance = 0f;
    public float currentOutlineWidth = 0f;

    public AnimationCurve activationCurve;

    bool isShieldActive = false;

    public void Start()
    {
        playerMat = playerRenderer.material;

        GetComponent<PlayerInputManager>().onShield.AddListener(OnShieldInput);
    }
    public void OnShieldInput(bool isPressed)
    {
        isShieldActive = isPressed;
        print(isPressed);
    }

    private void Update()
    {
        currentShieldActivation += isShieldActive ? Time.deltaTime/shieldActivationTime : -Time.deltaTime / shieldActivationTime;
        currentShieldActivation = Mathf.Clamp(currentShieldActivation, 0f, 1);

        currentOutlineDistance = Mathf.Lerp(0,shieldOutlineDistance, activationCurve.Evaluate(currentShieldActivation));
        currentOutlineWidth = Mathf.Lerp(0,shieldOutlineWitdh, activationCurve.Evaluate(currentShieldActivation));

        playerMat.SetFloat("_OutlineDistance", currentOutlineDistance);
        playerMat.SetFloat("_OutlineThickness", currentOutlineWidth);
    }
}
