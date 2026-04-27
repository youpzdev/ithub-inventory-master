using System;
using System.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private GameObject[] modalWindows;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        Cursor.lockState = AreModalWindowsOpened() ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = AreModalWindowsOpened();
    }

    public bool AreModalWindowsOpened()
    {
        foreach (GameObject panel in modalWindows) if (panel.activeSelf)  return true;
        return false;
    }
}
