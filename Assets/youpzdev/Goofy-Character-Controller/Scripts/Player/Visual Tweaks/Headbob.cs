using UnityEngine;

[RequireComponent(typeof(Camera))]
public class HeadBob : MonoBehaviour
{
    [Header("Headbob Settings")]
    [SerializeField] private float bobFrequency = 1.5f;  // базовая частота
    [SerializeField] private float bobAmplitude = 0.05f;  // амплитуда
    [SerializeField] private float sprintMultiplier = 1.5f; // ускорение при беге

    private float defaultY;
    private float timer = 0f;

    private Transform camTransform;

    private void Start()
    {
        camTransform = transform;
        defaultY = camTransform.localPosition.y;
    }

    // speed - значение от 0 до 1
    public void DoHeadBob(float speed)
    {
        if (UIManager.Instance.AreModalWindowsOpened()) return;
        
        if (speed > 0.01f)
        {
            timer += Time.deltaTime * bobFrequency * (speed > 0.99f ? sprintMultiplier : 1f);
            float yOffset = Mathf.Sin(timer) * bobAmplitude * speed;
            camTransform.localPosition = new Vector3(
                camTransform.localPosition.x,
                defaultY + yOffset,
                camTransform.localPosition.z
            );
        }
        else
        {
            // стоим на месте, возвращаем камеру
            timer = 0f;
            camTransform.localPosition = new Vector3(
                camTransform.localPosition.x,
                Mathf.Lerp(camTransform.localPosition.y, defaultY, Time.deltaTime * 10f),
                camTransform.localPosition.z
            );
        }
    }
}