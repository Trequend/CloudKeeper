using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResultCounter : MonoBehaviour
{
    private const string RecordProperty = "Record";

    [SerializeField] private float _delay = 0.5f;

    [SerializeField] private float _animationTime = 0.5f;

    [SerializeField] private Text _record;

    [SerializeField] private Text _score;

    [SerializeField] private AudioSource _counterSound;

    [SerializeField] private GameObject _newRecordLabel;

    [SerializeField] private SkipActionZone _skipActionZone;

    private int Record
    {
        get => PlayerPrefs.GetInt(RecordProperty, 0);
        set => PlayerPrefs.SetInt(RecordProperty, value);
    }

    private void Start()
    {
        UpdateRecordText();
        HideNewRecordLabel();
        SetScore(0);
        if (Score.Value < 5)
        {
            SetResult();
            return;
        }

        _skipActionZone.Activate(onSkip: () =>
        {
            _counterSound.Stop();
            StopAllCoroutines();
            SetResult();
        });

        StartCoroutine(CountScore());
    }

    private IEnumerator CountScore()
    {
        yield return new WaitForSecondsRealtime(_delay);
        int score = Score.Value;
        float time = 0.0f;
        _counterSound.Play();
        while (time < _animationTime)
        {
            SetScore(Mathf.FloorToInt(score * (time / _animationTime)));
            yield return null;
            time += Time.unscaledDeltaTime;
        }

        _skipActionZone.Deactivate();
        _counterSound.Stop();
        SetResult();
    }

    private void SetResult()
    {
        SetScore(Score.Value);
        int score = Score.Value;
        if (score > Record)
        {
            Record = score;
            UpdateRecordText();
            ShowNewRecordLabel();
        }
    }

    private void SetScore(int score)
    {
        _score.text = $"Score: {score}";
    }

    private void UpdateRecordText()
    {
        _record.text = $"Record: {Record}";
    }

    private void ShowNewRecordLabel()
    {
        _newRecordLabel.SetActive(true);
    }

    private void HideNewRecordLabel()
    {
        _newRecordLabel.SetActive(false);
    }
}
