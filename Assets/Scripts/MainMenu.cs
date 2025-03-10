using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Slider slider;
    public Text sliderValue;

    public void SliderAdjusted()
    {
        sliderValue.text = slider.value.ToString();
    }

    public void StartGame()
    {
        int size = (int)slider.value;
        PlayerPrefs.SetInt("size", size);
        PlayerPrefs.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
