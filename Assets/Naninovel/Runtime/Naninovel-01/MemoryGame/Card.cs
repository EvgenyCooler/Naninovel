using UnityEngine;
using UnityEngine.UI;

public enum CardState
{
    Closed,
    Opened,
    Matched
}

public class Card : MonoBehaviour
{
    private Sprite cardImage;
    private MemoryGame game;
    private CardState cardState = CardState.Closed;
    private Button button;
    private Image imageComponent;
    private Sprite backSprite;

    void Awake()
    {
        button = GetComponent<Button>();
        imageComponent = GetComponentInChildren<Image>();
        backSprite = imageComponent.sprite;
        button.onClick.AddListener(OnClick);
    }

    public void SetCard(Sprite image, MemoryGame memoryGame)
    {
        cardImage = image;
        game = memoryGame;
    }

    private void OnClick()
    {
        game.OnCardSelected(this);
    }

    public void Flip()
    {
        if (cardState != CardState.Closed) return;
        cardState = CardState.Opened;
        imageComponent.sprite = cardImage;
    }

    public void FlipBack()
    {
        if (cardState == CardState.Opened)
        {
            cardState = CardState.Closed;
            imageComponent.sprite = backSprite;
        }
    }

    public void SetState(CardState newState)
    {
        cardState = newState;
    }

    public CardState GetState()
    {
        return cardState;
    }

    public Sprite GetImage()
    {
        return cardImage;
    }
}