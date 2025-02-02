using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Sprite bgImage;

    //public Sprite[] puzzles;
    public Sprite[] notes;
    public Sprite[] duration;

    public List<Sprite> gamePuzzles = new List<Sprite>();

    public List<Button> btns = new List<Button>();

    private bool firstGuess, secondGuess;

    private int countCorrectGuesses, gameGuesses;

    private int firstGuessIndex, secondGuessIndex; // prima e seconda carta girata

    private string firstGuessPuzzle, secondGuessPuzzle;

    private bool firstAnim1, firstAnim2;

    private GameTimer gameTimer;

    [SerializeField] private GameObject[] monsters;
    [SerializeField] private Animator[] movements;
    [SerializeField] private Animator[] cardRotation;

    [SerializeField] private GameObject fillableBar;
    private int _scoreVincita; 
    private int addScore = 5;

    public int scoreVincita => _scoreVincita;

    private void Awake() // caricamento sprite note e durate 
    {
        notes = Resources.LoadAll<Sprite>("Sprite/Notes");
        duration = Resources.LoadAll<Sprite>("Sprite/Duration");


    }


    // Start is called before the first frame update
    void Start()
    {
        GetButtons();
        AddListeners();
        AddGamePuzzles();
        ShuffleList(gamePuzzles);
        gameGuesses = gamePuzzles.Count / 2; // quante serve indovinare 
        setAnimation();
        this.gameTimer = FindObjectOfType<GameTimer>();
    }

    private void Update()
    {
        this.gameTimer.GameTimerUpdate();
    }

    void setAnimation()
    {
        for (int i = 0; i < monsters.Length; i++)
        {
          
            movements[i] = monsters[i].GetComponent<Animator>();
        }
    }

    void GetButtons()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("PuzzleButton");

        for( int i = 0; i < objects.Length; i++ )
        {
            btns.Add(objects[i].GetComponent<Button>());
            btns[i].image.sprite = bgImage;
            cardRotation[i] = btns[i].GetComponent<Animator>();
        }
    }

   
    void AddGamePuzzles()
    {
        int looper = btns.Count; // num tot bottoni 
        ShuffleVect(notes);
        for ( int i = 0; i < looper/2; i++)
        {
            gamePuzzles.Add(notes[i]);
            string tempName= notes[i].name;

            for(int j = 0; j < duration.Length; j++)
            {
                if (tempName == duration[j].name)
                    gamePuzzles.Add(duration[j]);
            }
 
        }
    }


    void AddListeners()
    {
        foreach( Button btn in btns)
        {
            btn.onClick.AddListener(() => PickAPuzzle()); // funzione lambda definita sul momento
        }
    }




    public void PickAPuzzle()
    {
        string name = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;

        if ( !firstGuess ) // primo tasto premuto
        {
            firstGuess = true;
            firstGuessIndex = int.Parse(name);
            firstGuessPuzzle = gamePuzzles[firstGuessIndex].name;
            if (!firstAnim1)
            {
                cardRotation[firstGuessIndex].SetTrigger("MostraCarta");
                firstAnim1 = true;
            }

            btns[firstGuessIndex].image.sprite = gamePuzzles[firstGuessIndex];

        }

        else if(!secondGuess)
        {
            secondGuess = true;
            secondGuessIndex = int.Parse(name);

            secondGuessPuzzle = gamePuzzles[secondGuessIndex].name;
            if (!firstAnim2)
            {
                cardRotation[secondGuessIndex].SetTrigger("MostraCarta");
                firstAnim2 = true;
            }
            btns[secondGuessIndex].image.sprite = gamePuzzles[secondGuessIndex];

            StartCoroutine(CheckIfThePuzzlesMatch());
        }

    }


    IEnumerator CheckIfThePuzzlesMatch()
    {
        bool correct = true;
        firstAnim1 = false;
        firstAnim2 = false;

        yield return new WaitForSeconds(1f);

        cardRotation[firstGuessIndex].SetTrigger("ToIdle");
        cardRotation[firstGuessIndex].SetTrigger("ToIdle");

        if ( firstGuessPuzzle == secondGuessPuzzle)
        {
            yield return new WaitForSeconds(.5f);

            btns[firstGuessIndex].interactable = false; // non si possono più toccare
            btns[secondGuessIndex].interactable = false;

            btns[firstGuessIndex].image.color  = new Color(0 , 0 , 0, 0); // non si vedono più
            btns[secondGuessIndex].image.color = new Color(0, 0, 0, 0);

            for (int i = 0; i < monsters.Length; i++)
            {

                movements[i].SetTrigger("from idle to correct");
            }

            this.gameTimer.UpdateScore(addScore);

            CheckIfTheGameIsFinished();
        }
        else
        {
            correct = false; 
            yield return new WaitForSeconds(.5f); // aspetto .5f secondi che l'immagine sia visibile

            btns[firstGuessIndex].image.sprite = bgImage; // ritorna immagine del dietro
            btns[secondGuessIndex].image.sprite = bgImage;

            for (int i = 0; i < monsters.Length; i++)
            {

                movements[i].SetTrigger("from idle to wrong");

            }
        }

        yield return new WaitForSeconds(.5f);
        if ( correct)
        {
            for (int i = 0; i < monsters.Length; i++)
            {

                movements[i].SetTrigger("from correct to idle");

            }
        }
        else
        {
            for (int i = 0; i < monsters.Length; i++)
            {

                movements[i].SetTrigger("from wrong to idle");

            }
        }

        firstGuess = false; 
        secondGuess = false;
    }

    void CheckIfTheGameIsFinished()
    {
        countCorrectGuesses++;

        if(countCorrectGuesses == gameGuesses)
        {
            _scoreVincita = (int) fillableBar.GetComponent<GameTimer>().GetTimeLeft() ;
            _scoreVincita *= 10;
            ScoreForMiniGame.Instance.SetHighScore(_scoreVincita);
            SaveManager.Instance.bestFindTheNote = this._scoreVincita;
            SaveManager.Instance.Save();
            SceneManager.LoadScene(sceneName: "Victory");

        }
    }

    //mischia le carte 
    void ShuffleList(List<Sprite> list)
    {
        for ( int i = 0; i < list.Count; i++ )
        {
            Sprite temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp; 
        }
    }

    void ShuffleVect(Sprite[] vect)
    {
        for ( int i = 0; i < vect.Length; i++)
        {
            Sprite temp = vect[i];
            int randomIndex = Random.Range(i, vect.Length);
            vect[i] = vect[randomIndex];
            vect[randomIndex] = temp; 
        }
    }


}
