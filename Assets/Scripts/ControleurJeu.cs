using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Contrôleur de jeu qui gère l'état global du jeu et fournit un accès centralisé aux contrôles et aux autres systèmes.
/// </summary>
public class ControleurJeu : MonoBehaviour
{
    /// <summary>
    /// Instance singleton du contrôleur de jeu.
    /// </summary>
    public static ControleurJeu Instance { get; private set; }

    [field:SerializeField, Tooltip("Référence aux contrôles du joueur.")]
    public PlayerInput Controles { get; private set; }

    public bool EstEnPause { get; private set; }
    public event System.Action PauseActive;
    public event System.Action PauseDesactive;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DeclencherPause()
    {
        if (!EstEnPause)
            StartCoroutine(Pause());
    }

    private System.Collections.IEnumerator Pause()
    {
        EstEnPause = true;
        PauseActive?.Invoke();

        Time.timeScale = 0.0f;
        yield return new WaitForSecondsRealtime(3.0f);
        Time.timeScale = 1.0f;

        EstEnPause = false;
        PauseDesactive?.Invoke();
    }
}
