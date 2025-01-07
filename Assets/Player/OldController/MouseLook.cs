using UnityEngine;

public class MouseLook : MonoBehaviour
{

    [SerializeField] float sensitivityX = 8.0f;
    [SerializeField] float sensitivityY = 0.5f;

    float mouseX, mouseY;

    [SerializeField] Transform camera;
    [SerializeField] float xClamp = 85f;
    float xRotation = 0f;

    private void Update()
    {
        transform.Rotate(Vector3.up,  mouseX * Time.deltaTime);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.x = xRotation;
        camera.eulerAngles = targetRotation;
    }

    public void ReceiveInput(Vector2 mouseInput)
    {
        mouseX = mouseInput.x * sensitivityX;
        mouseY = mouseInput.y * sensitivityY;
    }
}
