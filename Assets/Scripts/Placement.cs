using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Placement : MonoBehaviour
{
    [SerializeField]
    private ARRaycastManager arRaycastManager;
    [SerializeField]
    private GameObject GrillePrefab;

    private bool grillePlacee = false;
    private GameObject grilleInstance;

    private PlayerInputActions controles;

    private void Awake()
    {
        controles = new PlayerInputActions();
        controles.Player.Tap.performed += Tap_performed;
    }

    private void OnEnable()
    {
        controles.Enable();
    }

    private void OnDisable()
    {
        controles.Disable();
        controles.Player.Tap.performed -= Tap_performed;
    }
    private void Tap_performed(InputAction.CallbackContext context)
    {
        // Obtenir la position du touch/clic
        Vector2 touchPosition = controles.Player.PointPosition.ReadValue<Vector2>();

        if (!grillePlacee)
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                grilleInstance = Instantiate(GrillePrefab, hitPose.position, Quaternion.identity);
                grillePlacee = true;
                Debug.Log("Grille placée!");
            }
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            CarreCollision carre = hit.collider.GetComponent<CarreCollision>();
            if (carre != null)
            {
                carre.PlacerPiece();
            }
        }
    }

    /// <summary>
    /// Replacer la grille (recommence le jeu automatiquement)
    /// </summary>
    public void ReplacerGrille()
    {

        RecommencerJeu();

        if (grilleInstance != null)
        {
            Destroy(grilleInstance);
            grilleInstance = null;
        }

        grillePlacee = false;
    }

    /// <summary>
    /// Recommencer le jeu sans toucher la grille
    /// </summary>
    public void RecommencerJeu()
    {
        //Effacer tous les X et O 
        CarreCollision[] carres = grilleInstance.GetComponentsInChildren<CarreCollision>();
        foreach (var carre in carres)
        {
            carre.ViderCase();
        }

        GameController.Instance.Recommencer();
    }
}
