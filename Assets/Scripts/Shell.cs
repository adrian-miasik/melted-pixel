﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shell : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Start is called before the first frame update
    public GameObject Slot;

    private int slotsCount;
    public List<Slot> Slots;

    private RectTransform m_DraggingPlane;

    public Sprite[] shellImages;

    private void Start()
    {
        InitShell();
    }

    private void InitShell()
    {
        GetComponent<Image>().sprite = shellImages[Random.Range(0, 2)];

        foreach(Slot slot in Slots)
        {
            slot.InitSlot();
        }
    }

    private void ShrinkShell()
    {
        transform.localScale *= 0.5f;
    }

    private void ResetShell()
    {
        transform.localScale *= 2f;
    } 

    public void OnBeginDrag(PointerEventData eventData)
    {
        ShrinkShell();
        m_DraggingPlane = transform as RectTransform;

        SetDraggedPosition(eventData);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (transform != null)
            SetDraggedPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ResetShell();

        float distanceToTrashCan = Vector2.Distance(TrashCan.Instance.transform.position, transform.position);
        if (distanceToTrashCan < 200f)
        {
            Destroy(gameObject);
        }

        SendShellToCustomer();
    }

    private float threshold = 100f;
    private void SendShellToCustomer()
    {
        foreach(Customer c in Main.Instance.customerManager.GetAllCustomers())
        {
            float distance = Vector2.Distance(c.transform.position, transform.position);

            if(distance < threshold)
            {
                c.ReceiveShell(this);
                return;
            }
        }
    }


    private void SetDraggedPosition(PointerEventData data)
    {
        if (data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
            m_DraggingPlane = data.pointerEnter.transform as RectTransform;

        var rt = transform.GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.CreateNewShell();
        }
    }
}
