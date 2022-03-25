using System;
using TMPro;
using UnityEngine;

public class InteractText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private ChoreStation choreStation;
    [SerializeField] private string putAwayText;
    [SerializeField] private string startInteractText;
    [SerializeField] private bool primary;
    [SerializeField] private bool isWindow;
    private static InteractText selected;
    private PlayerInputs playerInput;

    public static void SetSelected(InteractText newSelected = null, ChoreObject choreObject = null)
    {
        if (selected != null)
            selected.text.gameObject.SetActive(false);
        selected = newSelected;
        if (selected != null)
        {
            selected.text.gameObject.SetActive(true);
            SetText(selected, choreObject);
        }
    }

    private static void SetText(InteractText interact, ChoreObject choreObject)
    {
        string message = "";
        ChoreStation cs = interact.choreStation;
        if (interact.choreStation != null)
        {
            if (cs.CanPutAway(choreObject) || (interact.isWindow && choreObject != null))
                message += String.Format(interact.putAwayText + '\n', 
                    interact.playerInput.GetInteractKey(interact.primary));
            if (cs.EnoughForInteraction)
                message += String.Format(interact.startInteractText + '\n',
            interact.playerInput.GetInteractKey(interact.primary));
            if (cs.TakesItems)
                message += "Amount: " + cs.AmountContained.ToString() + '\n';
        }

        interact.text.text = message;
    }

    private void Awake()
    {
        playerInput = GameObject.Find("Player").GetComponent<PlayerInputs>();
    }
}
