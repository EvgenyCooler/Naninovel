using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Naninovel;
using Debug = UnityEngine.Debug;

public class MemoryGame : MonoBehaviour
{
    public GameObject cardPrefab; // Префаб карточки
    public Transform board; // Родительский объект для карточек
    public Sprite[] cardImages; // Массив изображений для карточек

    private List<Card> cards = new List<Card>();
    private Card firstSelected, secondSelected;
    private bool canSelect = true;
    private int moveCount = 0;
    private Stopwatch stopwatch = new Stopwatch();


    private Func<GameResult, bool> _func;
    
    public void StartGame(Func<GameResult, bool> func)
    {
        _func = func;
        stopwatch.Start();
        GenerateCards();
    }

    void GenerateCards()
    {
        List<Sprite> images = new List<Sprite>(cardImages);
        images.AddRange(cardImages); // Дублируем, чтобы создать пары
        images = Shuffle(images); // Перемешиваем карточки

        for (int i = 0; i < images.Count; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, board);
            Card card = cardObj.GetComponent<Card>();
            card.SetCard(images[i], this);
            cards.Add(card);
        }
    }

    public void OnCardSelected(Card card)
    {
        if (!canSelect || card == firstSelected || card == secondSelected)
            return;

        card.Flip();
        moveCount++;
        
        if (firstSelected == null)
        {
            firstSelected = card;
        }
        else
        {
            secondSelected = card;
            CheckMatch().Forget();
        }
    }

    private async UniTaskVoid CheckMatch()
    {
        canSelect = false;

        if (firstSelected.GetImage() == secondSelected.GetImage())
        {
            firstSelected.SetState(CardState.Matched);
            secondSelected.SetState(CardState.Matched);
        }
        else
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            
            firstSelected.FlipBack();
            secondSelected.FlipBack();
        }

        firstSelected = null;
        secondSelected = null;
        canSelect = true;

        CheckWin();
    }

    void CheckWin()
    {
        foreach (var card in cards)
        {
            if (card.GetState() != CardState.Matched)
                return;
        }
        
        stopwatch.Stop();
        GameResult result = new GameResult(moveCount, stopwatch.Elapsed);
        Debug.Log($"Победа! Все карточки найдены! Ходы: {result.Moves}, Время: {result.TimeSpent}");

        _func?.Invoke(result);
    }

    List<T> Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int rand = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[rand];
            list[rand] = temp;
        }
        return list;
    }

}

public struct GameResult
{
    public int Moves { get; }
    public TimeSpan TimeSpent { get; }

    public GameResult(int moves, TimeSpan timeSpent)
    {
        Moves = moves;
        TimeSpent = timeSpent;
    }
}