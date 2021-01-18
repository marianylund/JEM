using UnityEngine;
// Credit to: https://medium.com/@mormo_music/game-settings-with-scriptable-objects-in-unity3d-6f753fe508fd
// Attach this to a gameobject that exists in the initial scene
public class Settings : MonoBehaviour
{
    [Tooltip("Choose which GameSettings asset to use")]
    public GameSettings _settings; // drag GameSettings asset here in inspector    [SerializeField]
    public static GameSettings s;
    public static Settings instance;

    void Awake()
    {
        if (Settings.instance == null)
        {
            Settings.instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Object.Destroy(gameObject);
        }
        if (Settings.s == null)
        {
            // Fy fy code
            _settings.NUMBER_OF_BEES = _settings.numberOfBeesToStartWith;
            _settings.UNLOCKED_LEVELS = new System.Collections.Generic.HashSet<Level>() { Level.Past };
            Settings.s = _settings;
        }
    }
}