///Autheur: Breno Gomes da Silva
///Travail pratique - TicTacToe immersif
///Utilisation des notes de cours: https://envimmersif-cegepvicto.github.io/
///Ainsi que projet demonstratif: https://github.com/EnvImmersif-CegepVicto/AR_Demos.git
///Pour la réalisation du projet (Autheur des références: Frédérick Taleb
///Aide de l'intelligence artificielle pour certains concepts du projet 
///(plus de détails dans le ReadMe)

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Placement : MonoBehaviour
{
    [Header("Référence")]
    [SerializeField]
    private ARRaycastManager arRaycastManager;
    [SerializeField]
    private GameObject GrillePrefab;

    [Header("Paramètres")]
    private bool grillePlacee = false;
    private GameObject grilleInstance;
    private PlayerInputActions controles;

    //Initialiser le InputAction
    private void Awake()
    {
        controles = new PlayerInputActions();
        controles.Player.Tap.performed += Tap_performed;
    }

    //Activer les actions
    private void OnEnable()
    {
        controles.Enable();
    }

    //Désactiver les action
    private void OnDisable()
    {
        controles.Disable();
        controles.Player.Tap.performed -= Tap_performed;
    }

    /// <summary>
    /// Méthode appelée par l'événement 
    /// Placer la grille si elle n'est pas placée 
    /// Placer les pièces dans les cases
    /// </summary>
    /// <param name="context">Événement appelé par la touche</param>
    private void Tap_performed(InputAction.CallbackContext context)
    {
        //Obtenir la position du touch/clic
        Vector2 touchPosition = controles.Player.PointPosition.ReadValue<Vector2>();

        //Si la grille n'est placée => Placer la grille (horizontal seulement)
        if (!grillePlacee)
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            if (arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                ARRaycastHit touche = hits[0];
                ARPlane plane = hits[0].trackable as ARPlane;

                if (plane.alignment == PlaneAlignment.HorizontalUp)
                {
                    Pose hitPose = touche.pose;
                    grilleInstance = Instantiate(GrillePrefab, hitPose.position, Quaternion.identity);

                    //Vérifier l'existance d'une Anchor, si non, créer une
                    ARAnchor anchor = hits[0].trackable.GetComponent<ARAnchor>();

                    if (anchor == null)
                    {
                        anchor = hits[0].trackable.gameObject.AddComponent<ARAnchor>();
                    }

                    grilleInstance.transform.parent = anchor.transform;
                    grillePlacee = true;

                    GameController.Instance.GrillePlacee();

                    Debug.Log("Grille placée!");
                }
                else 
                {
                    GameController.Instance.GrillePlaceeMauvaisePlace();
                }
                
            }
            return;
        }

        //Vérifier si le jeu est terminé => Éviter appuyer sur les cases quand le joue est terminé
        if (GameController.Instance.JeuFini())
        {
            return;
        }


        //Quand la grille sera placée => activer détection touches cases
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //Prendre la case sélectionnée avec raycast
            CarreCollision carre = hit.collider.GetComponent<CarreCollision>();

            //Si la case est existante, placer la pièce et modifier couleur selon tour
            if (carre != null)
            {
                carre.PlacerPiece();

                Renderer rend = hit.collider.GetComponent<Renderer>();
                if (rend != null)
                {
                    if (GameController.Instance.EstTourX())
                    {
                        rend.material.color = Color.red;
                    }

                    else
                    {
                        rend.material.color = Color.blue;
                    }
                }
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
        GameController.Instance.GrilleRetiree();
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
            carre.ReinitialiserApparence();
        }

        GameController.Instance.Recommencer();
    }
}
