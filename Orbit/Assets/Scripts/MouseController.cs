﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    private static MouseController _instance;

    public static MouseController Instance
    {
        get
        {
            if ( _instance == null )
                _instance = FindObjectOfType<MouseController>();
            return _instance;
        }   
    }

    public enum MouseMode
    {
        Selection,
        None
    }

    private MouseMode _mode = MouseMode.Selection;

    public MouseMode Mode
    {
        get { return _mode; }
        private set
        {
            switch ( value )
            {
                case MouseMode.Selection:
                    SelectionModeCallback();
                    break;
                case MouseMode.None:
                    NoneModeCallback();
                    break;
                default:
                    throw new ArgumentOutOfRangeException( "value", value, null );
            }
            _mode = value;
        }
    }

    [SerializeField]
    private ParticleSystem _highlightParticleSystem;

    [SerializeField]
    private InsertionMenu _insertionMenu;

    private InsertionMenu _currentInsertionMenu;

    [SerializeField]
    private float _longClickLength = 0.5f;

    private float _clickLength = 0.0f;
	
	// Update is called once per frame
	void Update ()
    {
	    if ( _mode == MouseMode.Selection)
	    {
	        Vector2 mousePosition = Input.mousePosition;

	        Highlight(mousePosition);

	        if ( Input.GetMouseButton( 0 ) )
	            OnLeftClick( mousePosition, Time.deltaTime );
	        else
	            _clickLength = 0.0f;
            if (Input.GetMouseButton(1))
                OnRightClick(mousePosition);
        }
	}

    void Highlight(Vector2 mousePos)
    {
        GameGrid gameGrid = GameGrid.Instance;

        float cellSize = gameGrid.CellSize;
        float side = gameGrid.Side;

        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));

        int posX = (int)(pos.x / cellSize);
        int posY = (int)(pos.y / cellSize);

        if (posX > 0 && posX < side)
        {
            if (posY > 0 && posY < side)
            {
                if (_highlightParticleSystem)
                    _highlightParticleSystem.transform.position = new Vector3( (posX + 0.5f ) * cellSize, 
                        (posY + 0.5f) * cellSize,
                        gameGrid.FixedZ);
            }
        }
    }

    void OnLeftClick( Vector2 mousePos, float deltaTime )
    {
        _clickLength += deltaTime;

        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
        GameCell cell = GameGrid.Instance.GetCellFromWorldPoint( pos );
        if ( cell )
            //TODO: Activate only in game mode
     //       if (cell.Connected)
                cell.Selected = true;
        else if ( GameManager.Instance.CanBuild && _clickLength > _longClickLength && _currentInsertionMenu == null)
        {
            int x, y;
            if ( GameGrid.Instance.GetPositionFromWorldPoint( pos, out x, out y ) )
            {
                uint ux = ( uint )x;
                uint uy = ( uint )y;
                /*if ( !GameGrid.Instance.IsConnected(ux, uy) )
                    return;*/
                _currentInsertionMenu = Instantiate( _insertionMenu, FindObjectOfType<Canvas>().transform, false );
                _currentInsertionMenu.X = ux;
                _currentInsertionMenu.Y = uy;
                _currentInsertionMenu.DestroyCallback += () => { _currentInsertionMenu = null; };
            }
        }
            
    }

    private void OnRightClick(Vector2 mousePos)
    {
        GameCell cell = GameCell.SelectedCell;
        if ( cell && cell.Connected )
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
            cell.LaunchAction(pos);
        }
    }

    void SelectionModeCallback()
    {
        if (_highlightParticleSystem)
            _highlightParticleSystem.gameObject.SetActive( true );
    }

    void NoneModeCallback()
    {
        if (_highlightParticleSystem)
            _highlightParticleSystem.gameObject.SetActive(false);
    }

}
