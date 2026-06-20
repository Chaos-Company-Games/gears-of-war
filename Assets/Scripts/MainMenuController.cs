using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    //awake is called before Start
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #region SceneManagement
    public void EnterGameplay()
    {
        SceneManager.LoadScene("GameplayScene");
    }
    public void Quit()
    {
        Application.Quit();
    }
    #endregion

}
