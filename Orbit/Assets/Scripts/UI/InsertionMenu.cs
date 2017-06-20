﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InsertionMenu : MonoBehaviour
{
    [SerializeField]
    private GameCell[] _prefabCells;

    [SerializeField]
    private GameObject _itemPrefab;

    [SerializeField]
    private Button _quitButton;

    public uint X;
    public uint Y;

    // Use this for initialization
    void Start ()
    {
        for ( int i = 0; i < _prefabCells.Length; ++i )
        {
            GameObject item = Instantiate( _itemPrefab, transform, false );
            Image image = item.GetComponent<Image>();
            image.sprite = _prefabCells[i].GetComponent<Sprite>();
            int y = i;
            Button button = item.GetComponent<Button>();
            button.onClick.AddListener( () => { AddCell( y ); } );
        }

        _quitButton.onClick.AddListener(Quit);

    }

    void AddCell( int index )
    {
        GameGrid.Instance.AddCase(_prefabCells[index], X, Y);
        Quit();
    }

    void Quit()
    {
        Destroy(this);
    }
}