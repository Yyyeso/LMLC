using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class UIPopUpButton : UIBase
{
    [SerializeField] private TMP_Text txtTitle;
    [SerializeField] private TMP_Text txtMessage;
    [SerializeField] private Button btnConfirm;
    [SerializeField] private Button btnCancel;
    [SerializeField] private RefreshUI refresh;
    [SerializeField] private TMP_Text txtConfirm;
    [SerializeField] private TMP_Text txtCancel;

    Action _confirmAction;
    Action _cancelAction;


    protected override void AddListener()
    {
        base.AddListener();
        btnConfirm.onClick.AddListener(ConfirmAction);
        btnCancel.onClick.AddListener(CancelAction);
    }

    void ConfirmAction()
    {
        CloseUI();
        _confirmAction?.Invoke();
    }

    void CancelAction()
    {
        CloseUI();
        _cancelAction?.Invoke();
    }

    void ResetPopUp()
    {
        _confirmAction = null;
        _cancelAction = null;
        btnCancel.gameObject.SetActive(false);
    }

    void SetTitle(string title)
    {
        bool nullTitle = string.IsNullOrEmpty(title);

        if (!nullTitle) { txtTitle.text = title; }
        txtTitle.gameObject.SetActive(!nullTitle);
    }

    public UIPopUpButton SetButtonName(string confirm, string cancel)
    {
        txtConfirm.text = confirm;
        txtCancel.text  = cancel;
        return this;
    }

    public UIPopUpButton SetMessage(string message, string title = null)
    {
        ResetPopUp();
        refresh.Refresh();
        SetTitle(title);
        txtMessage.text = message;
        return this;
    }

    public UIPopUpButton AddConfirmAction(Action action)
    {
        _confirmAction = action;
        return this;
    }

    public UIPopUpButton AddCancelAction(Action action = null)
    {
        btnCancel.gameObject.SetActive(true);
        refresh.Refresh();
        _cancelAction = action;
        return this;
    }
}