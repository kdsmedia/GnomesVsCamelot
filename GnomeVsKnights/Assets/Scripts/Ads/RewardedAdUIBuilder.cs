using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Automatically builds the rewarded-ad ⚡ buttons at runtime so that no Unity
/// Editor scene editing is required. A <see cref="RuntimeInitializeOnLoadMethod"/>
/// hook attaches this builder to a temporary GameObject right after every scene
/// loads, then the builder creates the appropriate buttons and self-destructs.
///
/// Placement (all use the ⚡ icon/label, not 📺):
///  - MainMenuScene : a ⚡ button next to New Game → MainMenu.WatchAdForBonusStartEnergy
///  - GameScene     : a ⚡ button near the energy counter → GameManager.WatchAdForEnergy
///  - GameScene     : a ⚡ button inside the Game Over panel → GameManager.WatchAdToRevive
/// </summary>
public class RewardedAdUIBuilder : MonoBehaviour
{
    private static bool _mainMenuBuilt;
    private static bool _gameSceneBuilt;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoStart()
    {
        // Reset the per-scene build flags so buttons are recreated on every load
        // (e.g. when returning to the Main Menu or restarting the GameScene).
        string scene = SceneManager.GetActiveScene().name;
        if (scene == "MainMenuScene") _mainMenuBuilt = false;
        if (scene == "GameScene") _gameSceneBuilt = false;

        // Create a temporary host GameObject for the builder and let it run Start.
        var go = new GameObject("RewardedAdUIBuilder");
        go.AddComponent<RewardedAdUIBuilder>();
    }

    private void Awake()
    {
        // Make sure the AdMob manager exists and is initialized early.
        _ = RewardedAdManager.Instance;
        RewardedAdManager.Instance.Initialize();
    }

    private void Start()
    {
        string scene = SceneManager.GetActiveScene().name;

        if (scene == "MainMenuScene" && !_mainMenuBuilt)
        {
            BuildMainMenuButton();
            _mainMenuBuilt = true;
        }
        else if (scene == "GameScene" && !_gameSceneBuilt)
        {
            BuildInGameEnergyButton();
            BuildGameOverReviveButton();
            _gameSceneBuilt = true;
        }

        // This builder has done its job; remove it so it doesn't linger.
        Destroy(gameObject);
    }

    // ---------------------------------------------------------------------
    //  Shared helpers
    // ---------------------------------------------------------------------

    private static Canvas FindMainCanvas()
    {
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas c in canvases)
        {
            if (c.isRootCanvas && c.renderMode != RenderMode.WorldSpace)
            {
                return c;
            }
        }
        return canvases.Length > 0 ? canvases[0] : null;
    }

    /// <summary>
    /// Creates a Button with a ⚡ label as a child of the given parent.
    /// Returns the Button component so callers can wire OnClick.
    /// </summary>
    private static Button CreateIconButton(
        string objectName,
        string label,
        Vector2 anchoredPosition,
        Vector2 sizeDelta,
        Transform parent,
        int fontSize = 36)
    {
        GameObject btnObj = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        btnObj.transform.SetParent(parent, false);
        btnObj.layer = LayerMask.NameToLayer("UI");

        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPosition;
        rt.sizeDelta = sizeDelta;

        // Button background image – semi-transparent dark panel.
        Image bg = btnObj.GetComponent<Image>();
        bg.color = new Color(0.12f, 0.46f, 0.12f, 0.9f);
        bg.raycastTarget = true;

        // Button component colors (green highlight theme matching existing buttons).
        Button btn = btnObj.GetComponent<Button>();
        btn.targetGraphic = bg;
        var colors = btn.colors;
        colors.normalColor = new Color(1f, 1f, 1f, 1f);
        colors.highlightedColor = new Color(0.4f, 1f, 0.4f, 1f);
        colors.pressedColor = new Color(0.2f, 0.6f, 0.2f, 1f);
        colors.selectedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        colors.fadeDuration = 0.1f;
        btn.colors = colors;

        // ⚡ label text (TextMeshPro).
        GameObject txtObj = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer));
        txtObj.transform.SetParent(btnObj.transform, false);
        txtObj.layer = LayerMask.NameToLayer("UI");

        RectTransform txtRt = txtObj.GetComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.offsetMin = Vector2.zero;
        txtRt.offsetMax = Vector2.zero;

        // Try to reuse the default TMP font asset from an existing TMP text in the scene.
        TMP_FontAsset font = FindDefaultTmpFont();

        TMP_Text txt = txtObj.AddComponent<TextMeshProUGUI>();
        txt.text = label;
        txt.fontSize = fontSize;
        txt.alignment = TextAlignmentOptions.Center;
        txt.enableWordWrapping = false;
        if (font != null) txt.font = font;
        txt.color = Color.white;

        return btn;
    }

    private static TMP_FontAsset FindDefaultTmpFont()
    {
        TMP_Text[] existing = FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);
        foreach (TMP_Text t in existing)
        {
            if (t.font != null) return t.font;
        }
        return null;
    }

    // ---------------------------------------------------------------------
    //  Main Menu – ⚡ bonus start energy button
    // ---------------------------------------------------------------------

    private void BuildMainMenuButton()
    {
        Canvas canvas = FindMainCanvas();
        if (canvas == null)
        {
            Debug.LogWarning("[AdUI] No Canvas found in MainMenuScene; button not created.");
            return;
        }

        // Position the button near the bottom-center, below the usual New Game button.
        Button btn = CreateIconButton(
            "AdBonusEnergyButton",
            "\u26A1",            // ⚡
            new Vector2(0f, -120f),
            new Vector2(160f, 80f),
            canvas.transform,
            fontSize: 42);

        // Add a subtitle below the icon explaining the reward.
        AddSubtitle(btn.transform, "Watch Ad: +50 Energy", new Vector2(0f, -55f));

        btn.onClick.AddListener(() =>
        {
            // Find the MainMenu instance in the scene.
            MainMenu menu = FindFirstObjectByType<MainMenu>();
            if (menu != null)
            {
                menu.WatchAdForBonusStartEnergy();
            }
            else
            {
                // Fallback: call directly through the ad manager.
                RewardedAdManager.Instance.ShowRewardedAd((earned) =>
                {
                    if (earned) GameManager.GrantBonusStartEnergy(50);
                });
            }
        });

        Debug.Log("[AdUI] Main Menu ⚡ bonus-energy button created.");
    }

    // ---------------------------------------------------------------------
    //  In-game – ⚡ +50 energy button (near energy counter)
    // ---------------------------------------------------------------------

    private void BuildInGameEnergyButton()
    {
        Canvas canvas = FindMainCanvas();
        if (canvas == null)
        {
            Debug.LogWarning("[AdUI] No Canvas found in GameScene; energy button not created.");
            return;
        }

        // Try to position next to the energy counter. We search for the energy
        // text/Image by name; if found we anchor relative to it, otherwise we
        // fall back to a fixed top-left position.
        Vector2 pos = new Vector2(140f, 460f);
        Transform energyAnchor = FindEnergyCounterAnchor(canvas.transform);
        if (energyAnchor != null)
        {
            // Place the button to the right of the energy display.
            pos = new Vector2(120f, 0f);
            Button btn = CreateIconButton(
                "AdEnergyButton",
                "\u26A1",
                pos,
                new Vector2(70f, 70f),
                energyAnchor,
                fontSize: 40);
            WireEnergyButton(btn);
            return;
        }

        Button fallbackBtn = CreateIconButton(
            "AdEnergyButton",
            "\u26A1",
            pos,
            new Vector2(80f, 80f),
            canvas.transform,
            fontSize: 42);
        WireEnergyButton(fallbackBtn);
        Debug.Log("[AdUI] In-game ⚡ energy button created (fallback position).");
    }

    private void WireEnergyButton(Button btn)
    {
        btn.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.WatchAdForEnergy();
            }
        });
        Debug.Log("[AdUI] In-game ⚡ energy button created.");
    }

    private static Transform FindEnergyCounterAnchor(Transform root)
    {
        // Search recursively for a known energy UI element.
        string[] names = { "EnergyCounter#", "EnergyImage", "EnergyCounter" };
        foreach (string n in names)
        {
            Transform t = FindDeepChild(root, n);
            if (t != null) return t.parent != null ? t.parent : t;
        }
        return null;
    }

    // ---------------------------------------------------------------------
    //  Game Over – ⚡ revive button (inside Game Over panel)
    // ---------------------------------------------------------------------

    private void BuildGameOverReviveButton()
    {
        Canvas canvas = FindMainCanvas();
        if (canvas == null)
        {
            Debug.LogWarning("[AdUI] No Canvas found in GameScene; revive button not created.");
            return;
        }

        // Find the GameOverPanel GameObject.
        Transform panel = FindDeepChild(canvas.transform, "GameOverPanel");
        if (panel == null)
        {
            Debug.LogWarning("[AdUI] GameOverPanel not found; revive button not created.");
            return;
        }

        // Place the revive button at the top of the panel.
        Button btn = CreateIconButton(
            "AdReviveButton",
            "\u26A1 Revive",
            new Vector2(0f, 40f),
            new Vector2(200f, 70f),
            panel,
            fontSize: 30);

        AddSubtitle(btn.transform, "Watch Ad to Continue", new Vector2(0f, -50f));

        btn.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.WatchAdToRevive();
            }
        });

        Debug.Log("[AdUI] Game Over ⚡ revive button created.");
    }

    // ---------------------------------------------------------------------
    //  Small helpers
    // ---------------------------------------------------------------------

    private static void AddSubtitle(Transform parent, string text, Vector2 offset)
    {
        GameObject txtObj = new GameObject("Subtitle", typeof(RectTransform), typeof(CanvasRenderer));
        txtObj.transform.SetParent(parent, false);
        txtObj.layer = LayerMask.NameToLayer("UI");

        RectTransform rt = txtObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = offset;
        rt.sizeDelta = new Vector2(260f, 30f);

        TMP_FontAsset font = FindDefaultTmpFont();
        TMP_Text txt = txtObj.AddComponent<TextMeshProUGUI>();
        txt.text = text;
        txt.fontSize = 16;
        txt.alignment = TextAlignmentOptions.Center;
        txt.enableWordWrapping = false;
        if (font != null) txt.font = font;
        txt.color = new Color(1f, 1f, 0.8f, 1f);
    }

    /// <summary>Recursively search for a child Transform by name (breadth-first).</summary>
    private static Transform FindDeepChild(Transform parent, string name)
    {
        // Direct children first.
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == name || child.name == name + " ")
            {
                return child;
            }
        }
        // Then recurse.
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform found = FindDeepChild(parent.GetChild(i), name);
            if (found != null) return found;
        }
        return null;
    }
}
