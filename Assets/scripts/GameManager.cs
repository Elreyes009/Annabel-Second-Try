using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager game_manager;

    Mov pc;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (game_manager == null)
        {
            game_manager = this;
        }

        if (game_manager != null && game_manager != this)
        {
            Destroy(gameObject);
        }

        pc = GameObject.Find("Player").GetComponent<Mov>();
    }

    //public void Reiniciar(Transform respawn, Transform entity)
    //{
    //    if (pc.Muerte == true)
    //    {
    //        Debug.Log("reinicio");
    //        entity.position = respawn.position;
    //    }
    //}
}
