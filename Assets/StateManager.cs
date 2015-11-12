using UnityEngine;
using System.Collections;

public enum STATE //the overall states
{
    MENU_MAIN,
    MENU_PAUSE,
    MENU_PAUSE_SIMULATE,
    GAME_SIMULATION,
    GAME_TACTICSSCREEN,
    GAME_CAMPAIGNSCREEN
}

public enum INPUTLAYER //the state currently processing input, the 'current' state while others may simulate in the background
{
    MENU_MAIN, //
    MENU_PAUSE, //pause menu overlaid the game
    GAME_SIMULATION, //main game simulation
    GAME_TACTICSCREEN, //loadout select, tacmap
    GAME_CAMPAIGNSCREEN
}


public class StateManager : MonoBehaviour {

    public static StateManager Instance { get; private set; }

    INPUTLAYER m_currentInputLayer;
    STATE m_currentState;

    BaseState m_mainMenuState;
    BaseState m_pauseMenuState;
    BaseState m_inGameState;
    BaseState m_tacticScreenState;
    BaseState m_campaignScreenState;


    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this); //Not sure if this will work
        }
    }


	// Use this for initialization
	void Start () {
	    m_currentState = STATE.GAME_SIMULATION;
        m_currentInputLayer = INPUTLAYER.GAME_SIMULATION;


	}
	
	// Update is called once per frame
	void Update () {

        switch (m_currentState)
        {
            case STATE.MENU_MAIN:
                {
                    EnableInput(INPUTLAYER.MENU_MAIN);
                    m_mainMenuState.Run();
                    break;
                }
            case STATE.MENU_PAUSE:
                {
                    EnableInput(INPUTLAYER.MENU_PAUSE);
                    m_pauseMenuState.Run();
                    break;
                }
            case STATE.MENU_PAUSE_SIMULATE:
                {
                    EnableInput(INPUTLAYER.MENU_PAUSE);
                    m_inGameState.Run();
                    m_pauseMenuState.Run();
                    break;
                }
            case STATE.GAME_SIMULATION:
                {
                    EnableInput(INPUTLAYER.GAME_SIMULATION);
                    m_inGameState.Run();
                    break;
                }
            case STATE.GAME_TACTICSSCREEN:
                {
                    EnableInput(INPUTLAYER.GAME_TACTICSCREEN);
                    m_inGameState.Run();
                    m_tacticScreenState.Run();
                    break;
                }
            case STATE.GAME_CAMPAIGNSCREEN:
                {
                    EnableInput(INPUTLAYER.GAME_CAMPAIGNSCREEN);
                    m_campaignScreenState.Run();
                    break;
                }
        }

	}

    void EnableInput(INPUTLAYER a_inputLayer)
    {
        switch (a_inputLayer)
        {
            case INPUTLAYER.MENU_MAIN:
                {
                    m_mainMenuState.EnableInput(true);
                    m_pauseMenuState.EnableInput(false);
                    m_inGameState.EnableInput(false);
                    m_tacticScreenState.EnableInput(false);
                    m_campaignScreenState.EnableInput(false);
                    break;
                }
            case INPUTLAYER.MENU_PAUSE:
                {
                    m_mainMenuState.EnableInput(false);
                    m_pauseMenuState.EnableInput(true);
                    m_inGameState.EnableInput(false);
                    m_tacticScreenState.EnableInput(false);
                    m_campaignScreenState.EnableInput(false);
                    break;
                }
            case INPUTLAYER.GAME_SIMULATION:
                {
                    m_mainMenuState.EnableInput(false);
                    m_pauseMenuState.EnableInput(false);
                    m_inGameState.EnableInput(true);
                    m_tacticScreenState.EnableInput(false);
                    m_campaignScreenState.EnableInput(false);
                    break;
                }
            case INPUTLAYER.GAME_TACTICSCREEN:
                {
                    m_mainMenuState.EnableInput(false);
                    m_pauseMenuState.EnableInput(false);
                    m_inGameState.EnableInput(false);
                    m_tacticScreenState.EnableInput(true);
                    m_campaignScreenState.EnableInput(false);
                    break;
                }
            case INPUTLAYER.GAME_CAMPAIGNSCREEN:
                {
                    m_mainMenuState.EnableInput(false);
                    m_pauseMenuState.EnableInput(false);
                    m_inGameState.EnableInput(false);
                    m_tacticScreenState.EnableInput(false);
                    m_campaignScreenState.EnableInput(true);
                    break;
                }
        }

    }
}
