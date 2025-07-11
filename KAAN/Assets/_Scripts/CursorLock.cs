using UnityEngine;

public class CursorLock : MonoBehaviour
{
    void Awake()
    {
        LockCursor();
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;  // art�k mouse serbest�e hareket eder
        Cursor.visible = false;                      // ama ekranda g�z�kmez
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;      // tamamen serbest (men�ler i�in)
        Cursor.visible = true;                       // tekrar g�r�n�r yap
    }
}
