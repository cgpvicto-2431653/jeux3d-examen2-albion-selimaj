using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Objet représentant une boule contrôlée par le joueur.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Boule : MonoBehaviour
{
    [SerializeField, Tooltip("La cible pour le suvi de la caméra")]
    private Transform cibleCamera;

    [SerializeField, Tooltip("Référence au gestionnaire de charge pour gérer les charges d'accélération.")]
    private GestionnaireCharge gestionnaireCharge;

    [SerializeField, Tooltip("Force de déplacement de la boule.")]
    private float forceDeplacement;

    // Force appliquée à la boule pour le déplacement à chaque frame.
    private Vector3 forceAppliquee;

    // Référence au Rigidbody de la boule pour appliquer la physique.
    private Rigidbody rigidbody;

    // Indique si la boule est actuellement en accélération.
    private bool enAcceleration;

    [SerializeField, Tooltip("Durée de l'accélération en secondes.")]
    float dureeAcceleration = 1.0f;

    [SerializeField, Tooltip("Multiplicateur de force appliqué lors de l'accélération.")]
    float multiplicateurForceAcceleration = 15.0f;

    /// <summary>
    /// Obtient la vélocité actuelle de la boule.
    /// </summary>
    public Vector3 Velocite => rigidbody.linearVelocity;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        enAcceleration = false;

        PlayerInput controles = ControleurJeu.Instance.Controles;
        controles.actions.FindAction("Commencer").performed += CommencerJeu;
    }

    private void OnDestroy()
    {
        if (ControleurJeu.Instance == null)
            return;

        PlayerInput controles = ControleurJeu.Instance.Controles;

        if (controles == null) 
            return;

        controles.actions.FindAction("Diriger").performed -= CommencerDirection;
        controles.actions.FindAction("Diriger").canceled -= ArreterDirection;
        controles.actions.FindAction("Accelerer").performed -= UtiliserChargeAcceleration;
    }

    /// <summary>
    /// Commence le jeu lorsque l'action "Commencer" est déclenchée. 
    /// Cela active la gravité sur la boule et configure les contrôles pour le déplacement et l'accélération.
    /// </summary>
    /// <param name="contexte"></param>
    private void CommencerJeu(InputAction.CallbackContext contexte)
    {
        PlayerInput controles = ControleurJeu.Instance.Controles;

        controles.actions.FindAction("Diriger").performed += CommencerDirection;
        controles.actions.FindAction("Diriger").canceled += ArreterDirection;
        controles.actions.FindAction("Accelerer").performed += UtiliserChargeAcceleration;
        controles.actions.FindAction("Commencer").performed -= CommencerJeu;

        rigidbody.useGravity = true;
    }

    private void Update()
    {
        if (cibleCamera != null)
        {
            cibleCamera.position = rigidbody.position;
        }
    }

    private void FixedUpdate()
    {
        Diriger();
    }

    private void CommencerDirection(InputAction.CallbackContext contexte)
    {
        forceAppliquee += contexte.ReadValue<float>() * forceDeplacement * transform.right;
    }

    private void ArreterDirection(InputAction.CallbackContext contexte)
    {
        forceAppliquee = Vector3.zero;
    }

    private void Diriger()
    {
        if(!Mathf.Approximately(forceAppliquee.sqrMagnitude, 0.0f))
        {
            rigidbody.AddForce(forceAppliquee, ForceMode.Force);
        }
    }

    public void AjouterChargeAcceleration()
    {
        gestionnaireCharge.AjouterCharge();
    }

    private void UtiliserChargeAcceleration(InputAction.CallbackContext contexte)
    {
        if (!enAcceleration && gestionnaireCharge.UtiliserCharge())
        {
            StartCoroutine(AppliquerAcceleration());
        }
    }

    private IEnumerator AppliquerAcceleration()
    {
        enAcceleration = true;
       
        float tempsEcoule = 0.0f;
        while (tempsEcoule < dureeAcceleration)
        {
            rigidbody.AddForce(Velocite.normalized * multiplicateurForceAcceleration, ForceMode.Force); // Applique une force doublée pour l'accélération
            tempsEcoule += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        enAcceleration = false;
    }
}
