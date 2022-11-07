using System.Collections.Generic;
using UnityEngine;

public class ShowButtons : MonoBehaviour
{
    [SerializeField] private List<GameObject> menuButtons;
    [SerializeField] private Animator menuAnimator;

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            menuAnimator.Play("Loop");
        }
    }

    public void ShowMenuButtons()
    {
        menuButtons.ForEach(x => x.SetActive(true));
    }
}
