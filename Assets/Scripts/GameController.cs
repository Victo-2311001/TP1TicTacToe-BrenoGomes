///Autheur: Breno Gomes da Silva
///Travail pratique - TicTacToe immersif
///Utilisation des notes de cours: https://envimmersif-cegepvicto.github.io/
///Ainsi que projet demonstratif: https://github.com/EnvImmersif-CegepVicto/AR_Demos.git
///Pour la réalisation du projet (Autheur des références: Frédérick Taleb
///Aide de l'intelligence artificielle pour certains concepts du projet 
///(plus de détails dans le ReadMe)

using System;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Référence")]
    [SerializeField]
    private TextMeshProUGUI instructionsText;
    [SerializeField]
    private TextMeshProUGUI tourText;
    [SerializeField]
    private TextMeshProUGUI victoireText;

    [SerializeField]
    private GameObject btnReplacer;
    [SerializeField]
    private GameObject btnRecommencer;

    [Header("Paramètres")]
    public int tour = 0;
    public int nbCoupsMax = 9;
    private int[,] grille = new int[3, 3];
    private bool grillePlacee = false;
    private bool toucheMur = false;
    private bool partieTerminee = false;

    private void Awake()
    {
        //Singleton pour avoir accès dans l'autre script (Placement.cs)
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        InitialiserGrille();
        MettreAJourAffichage();
        
        btnRecommencer.SetActive(false);
        btnReplacer.SetActive(false);
    }

    //Initaliser les dimentions de la grille pour savoir les combinaisons gagnantes 
    private void InitialiserGrille()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                grille[i, j] = 0;
            }
        }
    }
    
    //Vérifier tour
    public bool EstTourX()
    {
        return tour % 2 == 0;
    }

    public void CoupJoue(int numeroCarre)
    {
        if (!grillePlacee)
        {
            return;
        }        

        //Convertir ligne case en numéro de la grille (3x3)
        int ligne = (numeroCarre - 1) / 3;
        int colonne = (numeroCarre - 1) % 3;

        //Savoir si partie est terminée ou case est occupée
        if (partieTerminee || grille[ligne, colonne] != 0)
        {
            return;
        }

        //Placer les chiffrens selon le tour
        //(c'est avec ces chiffres qu'il est possible valider la victoire)
        grille[ligne, colonne] = EstTourX() ? 1 : 2;

        tour++;

        //Activer le boutton recommecer
        if (tour >= 1)
        {
            btnRecommencer.SetActive(true);
        }

        if (VerifierVictoire())
        {
            partieTerminee = true;
            AfficherVictoire();
        }
        else if (tour >= nbCoupsMax)
        {
            partieTerminee = true;
            AfficherEgalite();
        }
        else
        {
            MettreAJourAffichage();
        }
    }

    private bool VerifierVictoire()
    {
        //Inversion car tour a changé
        int joueur = EstTourX() ? 2 : 1;

        //Vérifier les lignes
        for (int i = 0; i < 3; i++)
        {
            if (grille[i, 0] == joueur && grille[i, 1] == joueur && grille[i, 2] == joueur)
            {
                return true;
            }

        }

        //Vérifier les colonnes
        for (int j = 0; j < 3; j++)
        {
            if (grille[0, j] == joueur && grille[1, j] == joueur && grille[2, j] == joueur)
            {
                return true;
            }
        }

        //Vérifier diagonale principale (\)
        if (grille[0, 0] == joueur && grille[1, 1] == joueur && grille[2, 2] == joueur)
        {
            return true;
        }

        //Vérifier diagonale secondaire (/)
        if (grille[0, 2] == joueur && grille[1, 1] == joueur && grille[2, 0] == joueur)
        {
            return true;
        }

        return false;
    }
    //Code créé avec l'aide de l'intelligence artificielle => Rêquete: montre moi la logique d'un TicTacToe pour que je puisse incrémenter dans mon jeu
    public bool JeuFini()
    {
        return partieTerminee;
    }
    public void Recommencer()
    {
        tour = 0;
        MettreAJourAffichage();
        InitialiserGrille();
        partieTerminee = false;
    }

    private void AfficherVictoire()
    {
        string gagnant = EstTourX() ? "O" : "X";
        victoireText.text = gagnant + " a gagné!";
        instructionsText.text = "Recommencez ou quittez";
        tourText.text = "Terminé";
    }

    private void AfficherEgalite()
    {
        victoireText.text = "Égalité!";
        instructionsText.text = "Recommencez ou quittez";
        tourText.text = "Terminé";
    }

    public void GrillePlacee()
    {
        if (grillePlacee)
        {
            return;
        }

        grillePlacee = true;
        toucheMur = false;
        btnReplacer.SetActive(true);
        MettreAJourAffichage();
    }

    public void GrilleRetiree()
    {
        grillePlacee = false;
        btnReplacer.SetActive(false);
        btnRecommencer.SetActive(false);
        MettreAJourAffichage();
    }

    public void GrillePlaceeMauvaisePlace()
    {
        toucheMur = true;
        MettreAJourAffichage();
    }

    private void MettreAJourAffichage()
    {
        if (!grillePlacee)
        {
            instructionsText.text = "Placez la grille";
            if (toucheMur)
            {
                instructionsText.text = "Placez la grille sur une surface horizontale";
            }
            tourText.text = "";
            victoireText.text = "";
            return;
        }

        instructionsText.text = "Placez la pièce sur une case";
        victoireText.text = "";

        if (EstTourX())
        {
            tourText.text = "Tour du X";
        }
        else
        {
            tourText.text = "Tour du O";
        }
    }

    //Savoir si la case clické est libre
    public bool CaseLibre(int ligne, int colonne)
    {
        return grille[ligne, colonne] == 0 && !partieTerminee;
    }

    public bool CaseLibreParCarre(int numeroCarre)
    {
        int ligne = (numeroCarre - 1) / 3;
        int colonne = (numeroCarre - 1) % 3;
        return CaseLibre(ligne, colonne);
    }

   
}
