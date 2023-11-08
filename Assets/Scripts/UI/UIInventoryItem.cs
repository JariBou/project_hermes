using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;
using System;
using UnityEngine.EventSystems;

namespace Inventory.UI
{
    public class UIInventoryItem : MonoBehaviour, IPointerClickHandler,
        IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler
    {
        [SerializeField]
        private Image itemImage;
        [SerializeField] private TextMeshProUGUI quantityTxt;
        [SerializeField] private Image borderImage;
        [SerializeField] private GameObject CoutIcon;

        public event Action<UIInventoryItem> OnItemDroppedOn, OnItemClicked,
            OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick;
        private bool empty = true;

        private void Awake()
        {

            ResetData();
            Deselect();
        }

        public void ResetData()
        {
            itemImage.gameObject.SetActive(false);
            empty = true;
            CoutIcon.SetActive(false);
        }
        public void Deselect()
        {
            borderImage.enabled = false;
        }

        public void Select()
        {
            borderImage.enabled = true;
        }


        public void SetData(Sprite sprite, int quantity)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = sprite;
            quantityTxt.text = quantity + "";
            empty = false;
            CoutIcon.SetActive(true);
        }


        // Update is called once per frame
        public void Update()
        {

        }

        public void OnPointerClick(PointerEventData pointerData)
        {
            /*        if (empty) return;*/
            if (pointerData.button == PointerEventData.InputButton.Right)
            {
                OnRightMouseBtnClick?.Invoke(this);
            }
            else
            {
                OnItemClicked?.Invoke(this);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (empty) return;
            OnItemBeginDrag?.Invoke(this);
        }

        public void OnDrop(PointerEventData eventData)
        {
            OnItemDroppedOn?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            
            OnItemEndDrag?.Invoke(this);
        }



        public void OnDrag(PointerEventData eventData)
        {

        }
    }
}