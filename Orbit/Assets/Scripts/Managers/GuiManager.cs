﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiManager : MonoBehaviour
{
    private static GuiManager _instance;

    public static GuiManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GuiManager>();
            return _instance;
        }
    }

    [SerializeField]
    private GameObject _hudPrefab;
    private GameObject _hudObject;
    [SerializeField]
    private GameObject _buildUiPrefab;
    private GameObject _buildUiObject;
    [SerializeField]
    private GameObject _pauseUiPrefab;
    private GameObject _pauseUiObject;
    [SerializeField]
    private GameObject _gameOverUiPrefab;
    private GameObject _gameOverUiObject;

    // Use this for initialization
    void Start ()
    {

        GameManager.Instance.OnPlay.AddListener(ShowHud);
        GameManager.Instance.OnPause.AddListener(ShowPauseUi);
        GameManager.Instance.OnGameOver.AddListener(ShowGameOverUi);
        GameManager.Instance.OnBuildSetEnabled += ShowBuildUi;
    }

    void CleanCanvas()
    {
        if ( _hudObject )
        {
            Destroy( _hudObject );
            _hudObject = null;
        }
        if ( _pauseUiObject )
        {
            Destroy( _pauseUiObject );
            _pauseUiObject = null;
        }
        if ( _gameOverUiObject )
        {
            Destroy( _gameOverUiObject );
            _gameOverUiObject = null;
        }
    }

    void ShowHud()
    {
        CleanCanvas();
        if (_hudPrefab && _hudObject )
            _hudObject = Instantiate( _hudPrefab, transform, false );
    }

    void ShowPauseUi()
    {
        CleanCanvas();
        if (_pauseUiPrefab && _pauseUiObject )
            _pauseUiObject = Instantiate(_pauseUiPrefab, transform, false);
    }

    void ShowGameOverUi()
    {
        CleanCanvas();
        if (_gameOverUiPrefab && _gameOverUiObject)
            _gameOverUiObject = Instantiate(_gameOverUiPrefab, transform, false);
    }

    void ShowBuildUi( bool show )
    {
        if ( show )
        {
            if ( _buildUiPrefab && _buildUiObject == null )
                _buildUiObject = Instantiate(_buildUiPrefab, transform, false );
        }
        else
        {
            if ( _buildUiObject )
            {
                Destroy(_buildUiObject);
                _buildUiObject = null;
            }
        }
    }
}
