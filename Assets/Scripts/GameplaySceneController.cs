using UnityEngine;
using UnityEngine.SceneManagement;

//GameplaySceneController class
//High level gameplay controller
public class GameplaySceneController : MonoBehaviour
{

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.LoadScene("CombatAdditive", LoadSceneMode.Additive);
        SceneManager.LoadScene("GearAdditive", LoadSceneMode.Additive);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
