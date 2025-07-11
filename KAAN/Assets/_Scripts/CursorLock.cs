using UnityEngine;

public class CursorLock : MonoBehaviour
{
    void Awake()
    {
        LockCursor();
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;  // artýk mouse serbestçe hareket eder
        Cursor.visible = false;                      // ama ekranda gözükmez
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;      // tamamen serbest (menüler için)
        Cursor.visible = true;                       // tekrar görünür yap
    }
}
