using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition;

public class VoiceManager : MonoBehaviour
{
    private GCSpeechRecognition gcSpeech;

    public delegate void OnResultEvent(string result);
    public static event OnResultEvent OnResult;

    private static VoiceManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static VoiceManager Instance { 
        get 
        {
            if (Instance == null)
                return null;
        
            return instance; 
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        gcSpeech = GCSpeechRecognition.Instance;
        gcSpeech.apiKey = "AIzaSyDLyMR1xs2exZDujLAJDcU7n_Rqwu7ulVM";
        gcSpeech.RecognizeSuccessEvent += RecognizeSuccessEventHandler;
        gcSpeech.FinishedRecordEvent += FinishedRecordEventHandler;
    }

    private void OnDestroy()
    {
        gcSpeech.RecognizeSuccessEvent -= RecognizeSuccessEventHandler;
    }


    public void StartVoiceManager()
    {
        gcSpeech.StartRecord(false);
    }

    public void StopVoiceManager()
    {
        gcSpeech.StopRecord();
    }
    
    private void FinishedRecordEventHandler(AudioClip clip, float[] raw)
    {
        if (clip == null)
            return;

        RecognitionConfig config = RecognitionConfig.GetDefault();
        config.languageCode = (Enumerators.LanguageCode.en_US).Parse();

        config.speechContexts = new SpeechContext[]
        {
            new SpeechContext()
            {

            }
        };
        config.audioChannelCount = clip.channels;

        GeneralRecognitionRequest recogRequest = new GeneralRecognitionRequest()
        {
            audio = new RecognitionAudioContent()
            {
                content = raw.ToBase64()
            },
            config = config
        };
        gcSpeech.Recognize(recogRequest);
    }

    void RecognizeSuccessEventHandler(RecognitionResponse response)
    {
        InsertRecognitionResponseInfo(response);
    }

    void InsertRecognitionResponseInfo(RecognitionResponse response)
    {
        if (response == null || response.results.Length == 0)
        {
            OnResult("failed");
            return;
        }

        string sSpeechResult = response.results[0].alternatives[0].transcript;

        OnResult(sSpeechResult);

        var words = response.results[0].alternatives[0].words;

        if (words != null)
        {
            string times = string.Empty;
            foreach(var item in response.results[0].alternatives[0].words)
            {
                times += "<color=green>" + item.word + "</color> - start : " + item.startTime + " : end: " + item.endTime + "\n";
            }
        }

        string other = string.Empty;

        foreach(var result in response.results)
        {
            foreach(var alternative in result.alternatives)
            {
                if (response.results[0].alternatives[0] != alternative)
                {
                    other += alternative.transcript + ", ";
                }
            }
        }    
    }
}
