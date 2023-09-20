using System;
using System.Data.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Movement : MonoBehaviour
{
    [SerializeField] private GameObject[] moles;
    [SerializeField] private GameObject hammer;
    [SerializeField] private String[] Notes;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private AudioClip[] suoniNote;
    [SerializeField] private TextMeshProUGUI rightNote;
    [SerializeField] private GameObject fillableBar;
    private Action<String, int> MoleMessage;
    private AudioSource noteSound;
    private bool notaSuonata;
    public Text pointText;
    private float punteggioPartita;
    public int index = 0;
    int index1 = 0;



    private void Awake()
    {
        int sharedRandIndex = UnityEngine.Random.Range(0, Notes.Length - 1);
        RandomNote.SetRandomIndex(sharedRandIndex);
        rightNote.text = "whack the note " + Notes[sharedRandIndex];
    }

    // Start is called before the first frame update
    void Start()
    {

        punteggioPartita = 0;
        if(moles != null)
        {
            this.moles[index].transform.position = this.moles[index].GetComponent<MoleController>().Points[moles[index].GetComponent<MoleController>()._indexPoint].transform.position;
        }
        noteSound = GetComponent<AudioSource>();
        this.MoleMessage = FindObjectOfType<UI_Message>().SpawnMessage;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.moles[index].GetComponent<MoleController>().hasFinished == true)
        {
            index1 = UnityEngine.Random.Range(0, this.moles.Length);
            while (index1 == index)
            {
                index1 = UnityEngine.Random.Range(0, this.moles.Length);
            }
            index = index1;
        }
        this.GestionePunteggio();
        this.suonoNota();
        //this.GestioneUI();
        fillableBar.GetComponent<GameTimer>().GameTimerUpdate();
        this.moles[index].GetComponent<MoleController>().Move();
        this.hammer.GetComponent<HammerController>().Move();
    }

    private void GestionePunteggio()
    {
        if (this.moles[index].GetComponent<MoleController>().isHitted && this.moles[index].GetComponent<MoleController>().GetIndexNote() == RandomNote.GetRandomIndex())
        {
            GameTimer.UpdateScore(this.moles[index].GetComponent<MoleController>().GetPoint());
            this.moles[index].GetComponent<MoleController>().isHitted = false;
        }
        else if (this.moles[index].GetComponent<MoleController>().isHitted)
        {
            GameTimer.UpdateScore(-this.moles[index].GetComponent<MoleController>().GetPoint());
            this.moles[index].GetComponent<MoleController>().isHitted = false;
        }

    }

    private void suonoNota()
    {
        if (this.moles[index].GetComponent<MoleController>()._indexPoint == 1 && !notaSuonata)
        {
            string notaSuon = Notes[this.moles[index].GetComponent<MoleController>().GetIndexNote()];
            this.MoleMessage(notaSuon, 1);
            notaSuonata = true;
            noteSound.clip = suoniNote[this.moles[index].GetComponent<MoleController>().GetIndexNote()];
            noteSound.Play();
        }
        if (this.moles[index].GetComponent<MoleController>()._indexPoint == 2)
        {
            notaSuonata = false;
            noteSound.Stop();
        }
    }

    //private void GestioneUI()
    //{
    //    pointText.text = punteggioPartita.ToString();
    //}

}
