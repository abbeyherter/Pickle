using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tile : MonoBehaviour
{
    // Visual state of tiles
    [System.Serializable]
    public class State {
        public Color fillColor;
        public Color outlineColor;
    }
    
    // Current state of tile
    public State state { get; private set; }
    public char letter { get; private set; }

    // Reference to tile letter and color
    private TextMeshProUGUI text;
    private Image fill;
    private Outline outline;
    
    private void Awake() {
        // Initialize references to child components
        text = GetComponentInChildren<TextMeshProUGUI>();
        fill = GetComponent<Image>();
        outline = GetComponent<Outline>();
    }

    public void SetLetter(char letter) {
        // Set letter to display on tile
        this.letter = letter;
        text.text = letter.ToString();
    }

    public void SetState(State state) {
        // Set visul state of tile
        this.state = state;
        fill.color = state.fillColor;
        outline.effectColor = state.outlineColor;
    }
}
