using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Basic class with string fields required for an Alias
/// Serializable for use in the Unity Inspector
/// </summary>
[System.Serializable]
public class DeveloperConsoleAlias
{
    /// <summary>
    /// THe name of the alias
    /// </summary>
    public              string                                  aliasName = "";

    /// <summary>
    /// The text that the name of the alias represents
    /// </summary>
    public              string                                  aliasValue = "";
}

/// <summary>
/// Developer Console class
/// </summary>
public class DeveloperConsole : MonoBehaviour
{
    /// <summary>
    /// The GUI Skin the console will utilise for rendering
    /// </summary>
    public              GUISkin                                 guiSkin = null;

    /// <summary>
    /// Height of the console in percentage of the screen size (0.0 to 1.0)
    /// </summary>
    public              float                                   ySizePercent = 0.5f;

    /// <summary>
    /// Width of the console in percentage of the screen size (0.0 to 1.0)
    /// </summary>
    public              float                                   xSizePercent = 0.9f;

    /// <summary>
    /// Height in pixels of the console�s textbox
    /// </summary>
    public              float                                   textFieldHeight = 24f;

    /// <summary>
    /// Height of each text line in the console�s output window
    /// </summary>
    public              float                                   lineHeight = 22f;

    /// <summary>
    /// Time (in seconds) that the console takes to enter and leave the frame
    /// </summary>
    public              float                                   transitionTime = 1f;

    public char[] disallowedChars = new char[1] { '`' };

    public Vector2 autocompleteBoxMaxSize = new Vector2(200f, 250f);

    public Color autocompleteSelectedColor = Color.green;

    /// <summary>
    /// The key used to toggle the display of the console
    /// </summary>
    public              KeyCode                                 toggleKeyCode = KeyCode.BackQuote;

    /// <summary>
    /// The key used to trigger text autocompletion during typing
    /// </summary>
    public              KeyCode                                 autocompleteKeyCode = KeyCode.Tab;

    /// <summary>
    /// Whether or not to print standard log (Debug.Log) output to the Developer Console
    /// </summary>
    public              bool                                    copyLogOutput = true;

    public bool showCursorWhenActive = true;

    /// <summary>
    /// Whether or not to only accept aliases and not the standard variable and function call syntax
    /// </summary>
    public              bool                                    onlyAcceptAliases = false;

    /// <summary>
    /// List of all aliases the console will utilise at runtime
    /// </summary>
    public              DeveloperConsoleAlias[]                 aliasList;

    /// <summary>
    /// Whether or not the console is showing or in the process of showing (ie. sliding in)
    /// </summary>
    private             bool                                    showing = false;

    /// <summary>
    /// THe current scroll position of the console log
    /// </summary>
    private             Vector2                                 scrollPosition = Vector2.zero;

    private Vector2[] autocompleteScrollPos = new Vector2[2] { Vector2.zero, Vector2.zero };

    /// <summary>
    /// The current console input text
    /// </summary>
    private             string                                  consoleText = "";

    /// <summary>
    /// The previous console input text
    /// Used for comparison to see if the user has typed more
    /// </summary>
    private             string                                  previousConsoleText = "";

    /// <summary>
    /// Lists the previous text entries allowing the user to scroll through old entries
    /// </summary>
    private             List<string>                            previousTextEntries = new List<string>();

    /// <summary>
    /// The current previous entry index we're displaying
    /// </summary>
    private             int                                     previousEntriesIndex = 0;

    /// <summary>
    /// List of all output text lines displayed in the console window
    /// </summary>
    private             List<string>                            consoleOutput = new List<string>();

    /// <summary>
    /// Progress of the console window's slide in animation
    /// </summary>
    private             float                                   displayAmount = 0f;

    /// <summary>
    /// The strings displaying in the autocomplete list
    /// </summary>
    private             string[]                                autocomplete;

    /// <summary>
    /// List of all gameobject names in the scene
    /// </summary>
    private             List<string>                            gameObjectNames = new List<string>();

    /// <summary>
    /// Frequency at which to find all gameobjects in the scene
    /// </summary>
    private             float                                   findObjectsFrequency = 5f;

    /// <summary>
    /// List of all gameobject names that match the current text input
    /// </summary>
    private             List<string>                            foundGameObjectNames = new List<string>();

    /// <summary>
    /// List of all component types in the current gameobject that match the current text input
    /// </summary>
    private             List<string>                            foundComponentNames = new List<string>();

    /// <summary>
    /// List of all method names in the current component that match the current text input
    /// </summary>
    private             List<string>                            foundMethodNames = new List<string>();

    /// <summary>
    /// List of all field names in the current component that match the current text input
    /// </summary>
    private List<string> foundFieldNames = new List<string>();
    
    /// <summary>
    /// List of all property names in the current component that match the current text input
    /// </summary>
    private List<string> foundPropertyNames = new List<string>();

    /// <summary>
    /// The currently selected autocomplete option
    /// </summary>
    private             int                                     autocompleteIndex = 0;

    private int autocompleteWindowIndex = 0;

    /// <summary>
    /// Whether or not the autcomplete is to be shown
    /// </summary>
    private             bool                                    closeAutocomplete = false;

    /// <summary>
    /// Whether or not there are any matches found for autocomplete to display
    /// </summary>
    private             bool                                    showingAutocomplete = false;

    /// <summary>
    /// The previous event from OnGUI
    /// </summary>
    private             EventType                               previousEvent = EventType.Layout;

    private float autocompleteButtonHeight = 20f;
    private float autocompleteTitleHeight = 26f;
    private float autocompleteBoxBottomPadding = 4f;
    private float autocompleteScrollBarBuffer = 16f;
    private float autocompleteWindowPadding = 10f;

    private float timeBetweenAutocompleteSelectionChange = 0.1f;

    private float timeUntilAutocompleteSelectionChangeAllowed = 0f;

    /// <summary>
    /// Whether or not to re-focus on the textfield in the next update
    /// Currently Unity does not support caret positioning in textboxes, so the best option found after autcompleting is to refocus which highlights all. This is not ideal :/
    /// </summary>
    private             bool                                    focusOnTextField = false;

    private bool cursorShowingBefore = true;
    private bool cursorLockedBefore = true;

    /// <summary>
    /// Aliases in dictionary form
    /// </summary>
    private             Dictionary<string, string>              dictionaryOfAliases = new Dictionary<string, string>();

    IEnumerator Start ()
    {
        // Routinely finds the name of all gameobjects
        StartCoroutine( FindAllGameObjects() );

        // Puts all the aliases into a handy dictionary
        for ( int aliasI = 0; aliasI < aliasList.Length; aliasI++ )
        {
            dictionaryOfAliases[aliasList[aliasI].aliasName] = aliasList[aliasI].aliasValue;
        }

        int framesPassed = 0;
        while (framesPassed < 10)
        {
            // Register a log callback
            if (copyLogOutput)
            {
                Application.RegisterLogCallback(CopyLogOutout);
            }

            framesPassed++;
            yield return 0;
        }
    }

    void OnDisable()
    {
        Application.RegisterLogCallback(null);
    }

    void Update ()
    {
        if ( transitionTime < 0f ) transitionTime = 0f;

        // Update display pos
        if ( showing )
        {
            if ( displayAmount < 1f )
            {
                displayAmount = Mathf.Clamp01( displayAmount + Time.deltaTime / transitionTime );
            }
        }
        else
        {
            if ( displayAmount > 0f )
            {
                displayAmount = Mathf.Clamp01( displayAmount - Time.deltaTime / transitionTime );
            }
        }
        if (!showing && displayAmount == 0f && Input.GetKeyUp(toggleKeyCode))
        {
            cursorShowingBefore = Cursor.visible;
            cursorLockedBefore = Screen.lockCursor;
            showing = !showing;
            if (showing) focusOnTextField = true;

            if (showCursorWhenActive)
            {
                Cursor.visible = true;
                Screen.lockCursor = false;
            }
        }

        if (showing)
        {
            if (showCursorWhenActive)
            {
                Cursor.visible = true;
                Screen.lockCursor = false;
            }
        }

        if (timeUntilAutocompleteSelectionChangeAllowed > 0f)
        {
            timeUntilAutocompleteSelectionChangeAllowed -= 0.01f;
        }

        // Autocomplete
        if ( previousConsoleText != consoleText && !closeAutocomplete )
        {
            showingAutocomplete = false;

            foundGameObjectNames.Clear();
            foundComponentNames.Clear();
            foundMethodNames.Clear();
            foundFieldNames.Clear();
            foundPropertyNames.Clear();

            string workWithText = consoleText;

            if ( workWithText.ToLower().StartsWith( "print " ) )
            {
                workWithText = workWithText.Substring( 6, workWithText.Length - 6 );
            }

            string[] splitDots = workWithText.Split( '.' );

            //if ( workWithText.StartsWith( "call " ) )
            //{
            //    callingFunction = true;

            //    splitDots = workWithText.Substring( 5, workWithText.Length - 5 ).Split( '.' );
            //}

            GameObject go = null;

            if ( splitDots.Length == 1 )
            {
                if (!onlyAcceptAliases)
                {
                    foundGameObjectNames.AddRange(gameObjectNames.FindAll(
                        delegate(string goName)
                        {
                            return goName.ToLower().StartsWith(splitDots[0].ToLower());
                        }
                        ));
                }
                foreach ( string key in dictionaryOfAliases.Keys )
                {
                    if ( key.ToLower().StartsWith( splitDots[0].ToLower() ) )
                        foundGameObjectNames.Add( key );
                }
            }
            if ( splitDots.Length == 2 )
            {
                if (!onlyAcceptAliases)
                {
                    go = GameObject.Find(splitDots[0]);
                    if (go != null)
                    {
                        Component[] goComps = go.GetComponents<Component>();
                        for (int cI = 0; cI < goComps.Length; cI++)
                        {
                            string cTypeName = goComps[cI].GetType().ToString();
                            string[] splitCTypeName = cTypeName.Split('.');
                            cTypeName = splitCTypeName[splitCTypeName.Length - 1];
                            if (cTypeName.ToLower().StartsWith(splitDots[1].ToLower()))
                            {
                                foundComponentNames.Add(cTypeName);
                            }
                        }
                    }
                }
            }
            if ( splitDots.Length == 3 )
            {
                if (!onlyAcceptAliases)
                {
                    go = GameObject.Find(splitDots[0]);
                    if (go != null)
                    {
                        Component component = go.GetComponent(splitDots[1]) as Component;
                        if (component != null)
                        {
                            MethodInfo[] mInfos = (component.GetType()).GetMethods();
                            foreach (MethodInfo mInfo in mInfos)
                            {
                                if (mInfo.Name.ToLower().StartsWith(splitDots[2].ToLower()))
                                {
                                    if (!foundMethodNames.Contains(mInfo.Name))
                                        foundMethodNames.Add(mInfo.Name);
                                }
                            }
                            FieldInfo[] fInfos = (component.GetType()).GetFields();
                            foreach (FieldInfo fInfo in fInfos)
                            {
                                if (fInfo.Name.ToLower().StartsWith(splitDots[2].ToLower()))
                                {
                                    if (!foundFieldNames.Contains(fInfo.Name))
                                        foundFieldNames.Add(fInfo.Name);
                                }
                            }
                            PropertyInfo[] pInfos = (component.GetType()).GetProperties();
                            foreach (PropertyInfo pInfo in pInfos)
                            {
                                if (pInfo.Name.ToLower().StartsWith(splitDots[2].ToLower()))
                                {
                                    if (!foundPropertyNames.Contains(pInfo.Name))
                                        foundPropertyNames.Add(pInfo.Name);
                                }
                            }
                        }
                    }
                }
            }

            if ( foundGameObjectNames.Count > 0 || foundComponentNames.Count > 0 || foundMethodNames.Count > 0 )
            {
                showingAutocomplete = true;
            }
        }

        previousConsoleText = consoleText;
    }

    public void CopyLogOutout(String condition, String stackTrace, LogType type)
    {
        PrintToConsole( "Log Output: " + condition );
    }

    public void PrintToConsole ( string text )
    {
        consoleOutput.Add( text );

        if ( scrollPosition.y >= ( ( ( consoleOutput.Count ) * lineHeight ) - Camera.main.pixelHeight * ( ySizePercent ) - textFieldHeight ) )
        {
            scrollPosition.y = consoleOutput.Count * lineHeight;
        }
    }
        
    IEnumerator FindAllGameObjects ()
    {
        while ( Application.isPlaying )
        {
            gameObjectNames.Clear();

            GameObject[] gos = FindObjectsOfType( typeof( GameObject ) ) as GameObject[];
            foreach ( GameObject go in gos )
            {
                gameObjectNames.Add( go.name );
            }

            yield return new WaitForSeconds( findObjectsFrequency );
        }

        yield return null;
    }

    void OnGUI ()
    {
        if ( guiSkin != null ) GUI.skin = guiSkin;

        bool doKey = false;
        EventType rawEventType = Event.current.type;
        if (showingAutocomplete && previousEvent != Event.current.type && (Event.current.type == EventType.KeyDown || Event.current.type == EventType.KeyUp) && (Event.current.keyCode == KeyCode.DownArrow || Event.current.keyCode == KeyCode.UpArrow))
        {
            Event.current.Use();
            doKey = true;
        }

        previousEvent = Event.current.type;

        if (displayAmount == 0f && !showing) return;

        Camera mainCamera = Camera.main;

        float consoleHeight = mainCamera.pixelHeight * ySizePercent;
        float consoleWidth = mainCamera.pixelWidth * xSizePercent;
        float leftMargin = ( mainCamera.pixelWidth - consoleWidth ) * 0.5f;

        Rect consoleRect = new Rect( leftMargin, Mathf.Lerp( -consoleHeight, 0f, displayAmount ), consoleWidth, consoleHeight );
        Rect consoleLogRect = new Rect( 0f, 0f, consoleWidth, consoleHeight - textFieldHeight );
        Rect consoleLogViewRect = new Rect( 0f, 0f, consoleWidth - 16f, Mathf.Max( consoleOutput.Count * lineHeight, consoleHeight - textFieldHeight ) );
        Rect consoleTextFieldRect = new Rect( 5f, consoleRect.height - textFieldHeight, consoleWidth - 10f, textFieldHeight );

        GUI.BeginGroup( consoleRect );
        
        // Background
        GUI.SetNextControlName( "ConsoleBackground" );
        GUI.Box( new Rect( 0f, 0f, consoleRect.width, consoleRect.height ), "" );

        // Log area
        scrollPosition = GUI.BeginScrollView( consoleLogRect, scrollPosition, consoleLogViewRect, false, true );
        for ( int i = 0; i < consoleOutput.Count; i++ )
        {
            string line = consoleOutput[i];
            GUI.Label( new Rect( 8f, i * lineHeight, consoleLogRect.width, lineHeight ), line );
        }
        GUI.EndScrollView();

        // Text Field
        string previousText = consoleText;
        GUI.SetNextControlName( "ConsoleTextField" );
        consoleText = GUI.TextField( consoleTextFieldRect, consoleText );
        consoleText = consoleText.Trim(disallowedChars);

        if ( previousText != consoleText )
        {
            closeAutocomplete = false;
            if ( previousTextEntries.Count > 0 ) previousEntriesIndex = previousTextEntries.Count;
            else previousEntriesIndex = 0;
        }

        if ( focusOnTextField )
        {
            GUI.FocusControl( "ConsoleBackground" );
            GUI.FocusControl( "ConsoleTextField" );

            TextEditor te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl); if (te != null) { te.MoveCursorToPosition(new Vector2(5555, 5555)); }
        }
        focusOnTextField = false;

        GUI.EndGroup();

        if ( !closeAutocomplete )
        {
            bool hasMethodNames = foundMethodNames.Count > 0;
            bool hasFieldNames = foundFieldNames.Count > 0;
            bool hasPropertyNames = foundPropertyNames.Count > 0;

            if (hasMethodNames || hasFieldNames || hasPropertyNames)
            {
                int numWindows = 0;
                if (hasMethodNames) numWindows++;
                if (hasFieldNames || hasPropertyNames) numWindows++;

                autocompleteWindowIndex = Mathf.Clamp(autocompleteWindowIndex, 0, numWindows - 1);

                int selectedIndex = -1;
                float xPosition = Input.compositionCursorPos.x + autocompleteBoxMaxSize.x * 0.5f;
                if (hasMethodNames)
                {
                    if (autocompleteWindowIndex == 0 )
                    {
                        autocompleteIndex = Mathf.Clamp(autocompleteIndex, 0, foundMethodNames.Count - 1);
                        selectedIndex = autocompleteIndex;
                    }
                    DrawAutocompleteWindow("Methods", foundMethodNames.ToArray(), new Vector2(xPosition, consoleRect.yMax), selectedIndex, 0);
                    xPosition += autocompleteBoxMaxSize.x + 10f;
                }
                if (hasFieldNames || hasPropertyNames)
                {
                    selectedIndex = -1;
                    string[] fieldAndPropertyNames = new string[foundFieldNames.Count + foundPropertyNames.Count];
                    Array.Copy(foundFieldNames.ToArray(), 0, fieldAndPropertyNames, 0, foundFieldNames.Count);
                    Array.Copy(foundPropertyNames.ToArray(), 0, fieldAndPropertyNames, foundFieldNames.Count, foundPropertyNames.Count);
                    if (autocompleteWindowIndex == 0 + (Convert.ToInt32(hasMethodNames)))
                    {
                        autocompleteIndex = Mathf.Clamp(autocompleteIndex, 0, fieldAndPropertyNames.Length - 1);
                        selectedIndex = autocompleteIndex;
                    }

                    DrawAutocompleteWindow("Fields & Properties", fieldAndPropertyNames, new Vector2(xPosition + autocompleteWindowPadding, consoleRect.yMax), selectedIndex, Convert.ToInt32(hasMethodNames));
                }
            }
            else if ( foundComponentNames.Count > 0 )
            {
                autocompleteIndex = Mathf.Clamp(autocompleteIndex, 0, foundComponentNames.Count - 1);
                autocompleteWindowIndex = 0;

                DrawAutocompleteWindow("Components", foundComponentNames.ToArray(), new Vector2(Input.compositionCursorPos.x + autocompleteBoxMaxSize.x * 0.5f, consoleRect.yMax), autocompleteIndex);
            }
            else if ( foundGameObjectNames.Count > 0 )
            {
                autocompleteIndex = Mathf.Clamp(autocompleteIndex, 0, foundGameObjectNames.Count - 1);
                autocompleteWindowIndex = 0;

                DrawAutocompleteWindow("GameObjects", foundGameObjectNames.ToArray(), new Vector2(Input.compositionCursorPos.x + autocompleteBoxMaxSize.x * 0.5f, consoleRect.yMax), autocompleteIndex);
            }
        }

        if (Event.current.keyCode == KeyCode.LeftArrow && Event.current.type == EventType.Used)
        {
            if (showingAutocomplete && !closeAutocomplete && autocompleteWindowIndex > 0)
            {
                autocompleteWindowIndex--;
                Event.current.Use();
                ScrollToDisplaySelected();
            }
        }
        if (Event.current.keyCode == KeyCode.RightArrow && Event.current.type == EventType.Used)
        {
            bool hasMethodNames = foundMethodNames.Count > 0;
            bool hasFieldNames = foundFieldNames.Count > 0;
            bool hasPropertyNames = foundPropertyNames.Count > 0;
            int numWindows = Convert.ToInt32(hasMethodNames) + Convert.ToInt32(hasFieldNames || hasPropertyNames);

            if (showingAutocomplete && !closeAutocomplete && autocompleteWindowIndex < (numWindows - 1))
            {
                autocompleteWindowIndex++;
                Event.current.Use();
                ScrollToDisplaySelected();
            }
        }

        if (Event.current.isKey || doKey)
        {
            if ( Event.current.type == EventType.KeyUp && Event.current.keyCode == autocompleteKeyCode )
            {
                if ( !closeAutocomplete )
                {
                    ApplyAutocompleteText();
                }

                closeAutocomplete = false;
            }
            if ( Event.current.keyCode == KeyCode.DownArrow )
            {
                if (showingAutocomplete && !closeAutocomplete)
                {
                    if (timeUntilAutocompleteSelectionChangeAllowed <= 0f)
                    {
                        autocompleteIndex++;
                        timeUntilAutocompleteSelectionChangeAllowed = timeBetweenAutocompleteSelectionChange;
                        ScrollToDisplaySelected();
                    }
                }
                else if (rawEventType == EventType.KeyUp)
                {
                    if ( previousEntriesIndex < previousTextEntries.Count ) previousEntriesIndex++;
                    if ( previousTextEntries.Count > previousEntriesIndex ) consoleText = previousTextEntries[previousEntriesIndex];
                    else consoleText = "";
                }
            }
            if ( Event.current.keyCode == KeyCode.UpArrow )
            {
                if (showingAutocomplete && !closeAutocomplete)
                {
                    if (timeUntilAutocompleteSelectionChangeAllowed <= 0f)
                    {
                        autocompleteIndex--;
                        timeUntilAutocompleteSelectionChangeAllowed = timeBetweenAutocompleteSelectionChange;
                        ScrollToDisplaySelected();
                    }
                }
                else if (rawEventType == EventType.KeyUp)
                {
                    if ( previousEntriesIndex > 0 ) previousEntriesIndex--;
                    if (previousTextEntries.Count > 0 && previousTextEntries.Count >= (previousEntriesIndex - 1)) consoleText = previousTextEntries[previousEntriesIndex];
                }
            }
            if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Escape)
            {
                closeAutocomplete = true;
            }
            if (Event.current.type == EventType.KeyUp && Event.current.keyCode == toggleKeyCode)
            {
                if (showing)
                {
                    if (showCursorWhenActive)
                    {
                        Cursor.visible = cursorShowingBefore;
                        Screen.lockCursor = cursorLockedBefore;
                    }
                }
                showing = !showing;
                if (showing) focusOnTextField = true;
            }
            if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Return)
            {
                closeAutocomplete = true;

                bool printResult = false;

                string workWithText = consoleText;

                // Replace aliases
                string[] keyArray = new string[dictionaryOfAliases.Count];
                dictionaryOfAliases.Keys.CopyTo(keyArray, 0);
                string[] valArray = new string[dictionaryOfAliases.Count];
                dictionaryOfAliases.Values.CopyTo(valArray, 0);

                for (int aI = 0; aI < dictionaryOfAliases.Count; aI++)
                {
                    if (workWithText.StartsWith(keyArray[aI]))
                    {
                        workWithText = workWithText.Replace(keyArray[aI], valArray[aI]);
                    }
                }

                if (workWithText.ToLower().StartsWith("print "))
                {
                    workWithText = workWithText.Substring(6, workWithText.Length - 6);
                    printResult = true;
                }

                string[] splitSpaces = workWithText.Split(' ');

                if (splitSpaces.Length > 0)
                {
                    string[] splitDots = workWithText.Split('.');

                    if (splitDots.Length >= 3)
                    {
                        GameObject go = GameObject.Find(splitDots[0]);
                        if (go != null)
                        {
                            int endIndexOfObjectName = splitDots[0].Length + 1;
                            //if (callingFunction) endIndexOfObjectName += 5;
                            string[] splitSpacesAgain = workWithText.Substring(endIndexOfObjectName, workWithText.Length - endIndexOfObjectName).Split(' ');

                            splitDots = splitSpacesAgain[0].Split('.');

                            if (splitDots.Length >= 2)
                            {
                                string argsString = "";
                                if (splitSpacesAgain[0].Length > 0)
                                {
                                    if (workWithText.Length - (endIndexOfObjectName + splitSpacesAgain[0].Length + 1) >= 0)
                                    {
                                        argsString = workWithText.Substring(endIndexOfObjectName + splitSpacesAgain[0].Length + 1,
                                         workWithText.Length - (endIndexOfObjectName + splitSpacesAgain[0].Length + 1));
                                    }
                                }

                                List<string> foundArgs = new List<string>();
                                bool inQuote = false;
                                string currentString = "";
                                for (int sI = 0; sI < argsString.Length; sI++)
                                {
                                    if (argsString[sI] == ' ')
                                    {
                                        if (!inQuote)
                                        {
                                            if (currentString != "")
                                            {
                                                foundArgs.Add(currentString);
                                                currentString = "";
                                            }
                                            continue;
                                        }
                                    }
                                    else if (argsString[sI] == '"')
                                    {
                                        if (inQuote)
                                        {
                                            inQuote = false;
                                            if (currentString != "")
                                            {
                                                foundArgs.Add(currentString);
                                                currentString = "";
                                            }
                                        }
                                        else
                                        {
                                            inQuote = true;

                                        }
                                        continue;
                                    }

                                    currentString += argsString[sI];

                                    if (sI == argsString.Length - 1)
                                    {
                                        foundArgs.Add(currentString);
                                    }
                                }

                                string[] arguments = foundArgs.ToArray();

                                Component component = go.GetComponent(splitDots[0]) as Component;

                                if (component != null)
                                {
                                    bool foundMatch = false;
                                    MethodInfo[] mInfos = ( component.GetType() ).GetMethods();
                                    foreach (MethodInfo mInfo in mInfos)
                                    {
                                        if (mInfo.Name.ToLower() == splitDots[1].ToLower())
                                        {
                                            RunMethod(go, component, splitDots[1], arguments, printResult);
                                            foundMatch = true;
                                            break;
                                        }
                                    }
                                    if (!foundMatch)
                                    {
                                        FieldInfo[] fInfos = (component.GetType()).GetFields();
                                        foreach (FieldInfo fInfo in fInfos)
                                        {
                                            if (fInfo.Name.ToLower() == splitDots[1].ToLower())
                                            {
                                                if (arguments.Length == 0)
                                                {
                                                    GetField(go, component, splitDots[1]);
                                                }
                                                else
                                                {
                                                    SetField(go, component, splitDots[1], arguments[0]);
                                                }
                                                foundMatch = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (!foundMatch)
                                    {
                                        PropertyInfo[] pInfos = (component.GetType()).GetProperties();
                                        foreach (PropertyInfo pInfo in pInfos)
                                        {
                                            if (pInfo.Name.ToLower() == splitDots[1].ToLower())
                                            {
                                                if (arguments.Length == 0)
                                                {
                                                    GetProperty(go, component, splitDots[1]);
                                                }
                                                else
                                                {
                                                    SetProperty(go, component, splitDots[1], arguments[0]);
                                                }
                                                foundMatch = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                previousTextEntries.Add(consoleText);
                consoleText = "";
                previousEntriesIndex = previousTextEntries.Count;
            }
        }

        GUI.skin = null;
    }

    void RunMethod ( GameObject gameObject, Component component, string methodName, string[] arguments, bool printResult )
    {
        Type componentType = component.GetType();
        MethodInfo[] mInfos = componentType.GetMethods();

        object[] pMatchedArguments = new object[0];

        int mostMatchingParams = -1;
        int bestMatchNumParams = 0;
        int bestMatch = -1;
        bool willReturn = false;
        object result = null;
        for ( int i = 0; i < mInfos.Length; i++ )
        {
            MethodInfo mInfo = mInfos[i];

            int numMatchingParams = 0;

            if ( mInfo != null )
            {
                if ( mInfo.Name != methodName )
                {
                    continue;
                }

                bool paramSuccess = true;

                // Find arg types
                ParameterInfo[] pInfo = mInfo.GetParameters();

                object[] pArguments = new object[pInfo.Length];

                for ( int pI = 0; pI < pInfo.Length; pI++ )
                {
                    if ( arguments.Length > pI )
                    {
                        if ( pInfo[pI].ParameterType == typeof( int ) )
                        {
                            int intResult = 0;
                            paramSuccess = int.TryParse( arguments[pI], out intResult );
                            if ( paramSuccess )
                            {
                                pArguments[pI] = intResult;
                                numMatchingParams++;
                            }
                        }
                        else if ( pInfo[pI].ParameterType == typeof( float ) )
                        {
                            float floatResult = 0;
                            paramSuccess = float.TryParse( arguments[pI], out floatResult );
                            if ( paramSuccess )
                            {
                                pArguments[pI] = floatResult;
                                numMatchingParams++;
                            }
                        }
                        else if ( pInfo[pI].ParameterType == typeof( double ) )
                        {
                            double doubleResult = 0;
                            paramSuccess = double.TryParse( arguments[pI], out doubleResult );
                            if ( paramSuccess )
                            {
                                pArguments[pI] = doubleResult;
                                numMatchingParams++;
                            }
                        }
                        else if ( pInfo[pI].ParameterType == typeof( bool ) )
                        {
                            if ( arguments[pI].ToUpper() == "FALSE" )
                            {
                                pArguments[pI] = false;
                                numMatchingParams++;
                            }
                            else if ( arguments[pI].ToUpper() == "TRUE" )
                            {
                                pArguments[pI] = true;
                                numMatchingParams++;
                            }
                            else
                            {
                                int intResult = 0;
                                paramSuccess = int.TryParse( arguments[pI], out intResult );
                                if ( paramSuccess )
                                {
                                    pArguments[pI] = ( intResult == 0 ) ? false : true;
                                    numMatchingParams++;
                                }
                            }
                        }
                        else if ( pInfo[pI].ParameterType == typeof( string ) )
                        {
                            pArguments[pI] = arguments[pI];
                            numMatchingParams++;
                        }
                        else if ( pInfo[pI].ParameterType == typeof( Vector2 ) )
                        {
                            string[] splitArg = arguments[pI].Split( ',' );
                            if ( splitArg.Length == 2 )
                            {
                                float xResult = 0f;
                                bool xSuccess = false;
                                xSuccess = float.TryParse( splitArg[0], out xResult );
                                
                                float yResult = 0f;
                                bool ySuccess = false;
                                ySuccess = float.TryParse( splitArg[2], out yResult );

                                if ( xSuccess && ySuccess )
                                {
                                    pArguments[pI] = new Vector2( xResult, yResult );
                                    numMatchingParams++;
                                }
                                else
                                {
                                    paramSuccess = false;
                                }
                            }
                        }
                        else if ( pInfo[pI].ParameterType == typeof( Vector3 ) )
                        {
                            string[] splitArg = arguments[pI].Split( ',' );
                            if ( splitArg.Length == 3 )
                            {
                                float xResult = 0f;
                                bool xSuccess = false;
                                xSuccess = float.TryParse( splitArg[0], out xResult );

                                float yResult = 0f;
                                bool ySuccess = false;
                                ySuccess = float.TryParse( splitArg[1], out yResult );

                                float zResult = 0f;
                                bool zSuccess = false;
                                zSuccess = float.TryParse( splitArg[2], out zResult );

                                if ( xSuccess && ySuccess && zSuccess )
                                {
                                    pArguments[pI] = new Vector3( xResult, yResult, zResult );
                                    numMatchingParams++;
                                }
                                else
                                {
                                    paramSuccess = false;
                                }
                            }
                        }
                        else if ( pInfo[pI].ParameterType == typeof( Quaternion ) )
                        {
                            string[] splitArg = arguments[pI].Split( ',' );
                            if ( splitArg.Length == 4 )
                            {
                                float xResult = 0f;
                                bool xSuccess = false;
                                xSuccess = float.TryParse( splitArg[0], out xResult );

                                float yResult = 0f;
                                bool ySuccess = false;
                                ySuccess = float.TryParse( splitArg[1], out yResult );

                                float zResult = 0f;
                                bool zSuccess = false;
                                zSuccess = float.TryParse( splitArg[2], out zResult );

                                float wResult = 0f;
                                bool wSuccess = false;
                                wSuccess = float.TryParse( splitArg[2], out wResult );

                                if ( xSuccess && ySuccess && zSuccess && wSuccess )
                                {
                                    pArguments[pI] = new Quaternion( xResult, yResult, zResult, wResult );
                                    numMatchingParams++;
                                }
                                else
                                {
                                    paramSuccess = false;
                                }
                            }
                        }
                        else if ( pInfo[pI].ParameterType == typeof( Color ) )
                        {
                            string[] splitArg = arguments[pI].Split( ',' );
                            if ( splitArg.Length == 4 )
                            {
                                float xResult = 0f;
                                bool xSuccess = false;
                                xSuccess = float.TryParse( splitArg[0], out xResult );

                                float yResult = 0f;
                                bool ySuccess = false;
                                ySuccess = float.TryParse( splitArg[1], out yResult );

                                float zResult = 0f;
                                bool zSuccess = false;
                                zSuccess = float.TryParse( splitArg[2], out zResult );

                                float wResult = 0f;
                                bool wSuccess = false;
                                wSuccess = float.TryParse( splitArg[2], out wResult );

                                if ( xSuccess && ySuccess && zSuccess && wSuccess )
                                {
                                    pArguments[pI] = new Color( xResult, yResult, zResult, wResult );
                                    numMatchingParams++;
                                }
                                else
                                {
                                    paramSuccess = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if ( pInfo[pI].DefaultValue != null )
                        {
                            paramSuccess = false;
                        }
                        else
                        {
                            pArguments[pI] = pInfo[pI].DefaultValue;
                        }
                    }

                    if ( !paramSuccess ) break;
                }

                if ( numMatchingParams > mostMatchingParams || ( ( numMatchingParams == mostMatchingParams ) && ( bestMatchNumParams > pInfo.Length ) ) )
                {
                    bestMatchNumParams = pInfo.Length;
                    mostMatchingParams = numMatchingParams;
                    bestMatch = i;
                    pMatchedArguments = pArguments;
                    if ( mInfo.ReturnType != typeof( void ) )
                    {
                        willReturn = true;
                    }
                    else
                    {
                        willReturn = false;
                    }
                }
            }
        }

        if ( bestMatch >= 0 )
        {
            if ( willReturn )
            {
                result = mInfos[bestMatch].Invoke( component, pMatchedArguments );
                if ( printResult ) PrintToConsole( result.ToString() );
            }
            else
            {
                mInfos[bestMatch].Invoke( component, pMatchedArguments );
            }
        }
    }

    void GetField ( GameObject gameObject, Component component, string fieldName )
    {
        Type componentType = component.GetType();
        FieldInfo fInfo = componentType.GetField( fieldName );

        if ( fInfo != null )
        {
            PrintToConsole( fInfo.GetValue( component ).ToString() );
        }
    }

    void SetField ( GameObject gameObject, Component component, string fieldName, string argument )
    {
        Type componentType = component.GetType();
        FieldInfo fInfo = componentType.GetField( fieldName );

        if ( fInfo != null )
        {
            bool paramSuccess = true;
            object pArgument = null;

            paramSuccess = ParseStringToObject(fInfo.FieldType, argument, ref pArgument);

            if (paramSuccess)
            {
                fInfo.SetValue(component, pArgument);
            }
            else
            {
                PrintToConsole( "Failed to set variable of type " + fInfo.FieldType.ToString() );
            }
        }
    }

    void GetProperty(GameObject gameObject, Component component, string propertyName)
    {
        Type componentType = component.GetType();
        PropertyInfo pInfo = componentType.GetProperty(propertyName);

        if (pInfo != null)
        {
            PrintToConsole(pInfo.GetValue(component, null).ToString());
        }
    }

    void SetProperty(GameObject gameObject, Component component, string propertyName, string argument)
    {
        Type componentType = component.GetType();
        PropertyInfo pInfo = componentType.GetProperty(propertyName);

        if (pInfo != null)
        {
            bool paramSuccess = true;
            object pArgument = null;

            paramSuccess = ParseStringToObject(pInfo.PropertyType, argument, ref pArgument);

            if (paramSuccess)
            {
                pInfo.SetValue(component, pArgument, null);
            }
            else
            {
                PrintToConsole( "Failed to set variable of type " + pInfo.PropertyType.ToString() );
            }
        }
    }

    bool ParseStringToObject(Type objectType, string argument, ref object returnObject)
    {
        bool paramSuccess = false;

        if (objectType == typeof(int))
        {
            int intResult = 0;
            paramSuccess = int.TryParse(argument, out intResult);
            if (paramSuccess)
            {
                returnObject = intResult;
                return true;
            }
            return false;
        }
        else if (objectType == typeof(float))
        {
            float floatResult = 0;
            paramSuccess = float.TryParse(argument, out floatResult);
            if (paramSuccess)
            {
                returnObject = floatResult;
                return true;
            }
            return false;
        }
        else if (objectType == typeof(double))
        {
            double doubleResult = 0;
            paramSuccess = double.TryParse(argument, out doubleResult);
            if (paramSuccess)
            {
                returnObject = doubleResult;
                return true;
            }
            return false;
        }
        else if (objectType == typeof(bool))
        {
            if (argument.ToUpper() == "FALSE")
            {
                returnObject = false;
                return true;
            }
            else if (argument.ToUpper() == "TRUE")
            {
                returnObject = true;
                return true;
            }
            else
            {
                int intResult = 0;
                paramSuccess = int.TryParse(argument, out intResult);
                if (paramSuccess)
                {
                    returnObject = (intResult == 0) ? false : true;
                    return true;
                }
                return false;
            }
        }
        else if (objectType == typeof(string))
        {
            returnObject = argument;
            return true;
        }
        else if (objectType == typeof(Vector2))
        {
            string[] splitArg = argument.Split(',');
            if (splitArg.Length == 2)
            {
                float xResult = 0f;
                bool xSuccess = false;
                xSuccess = float.TryParse(splitArg[0], out xResult);

                float yResult = 0f;
                bool ySuccess = false;
                ySuccess = float.TryParse(splitArg[2], out yResult);

                if (xSuccess && ySuccess)
                {
                    returnObject = new Vector2(xResult, yResult);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        else if (objectType == typeof(Vector3))
        {
            string[] splitArg = argument.Split(',');
            if (splitArg.Length == 3)
            {
                float xResult = 0f;
                bool xSuccess = false;
                xSuccess = float.TryParse(splitArg[0], out xResult);

                float yResult = 0f;
                bool ySuccess = false;
                ySuccess = float.TryParse(splitArg[1], out yResult);

                float zResult = 0f;
                bool zSuccess = false;
                zSuccess = float.TryParse(splitArg[2], out zResult);

                if (xSuccess && ySuccess && zSuccess)
                {
                    returnObject = new Vector3(xResult, yResult, zResult);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        else if (objectType == typeof(Quaternion))
        {
            string[] splitArg = argument.Split(',');
            if (splitArg.Length == 4)
            {
                float xResult = 0f;
                bool xSuccess = false;
                xSuccess = float.TryParse(splitArg[0], out xResult);

                float yResult = 0f;
                bool ySuccess = false;
                ySuccess = float.TryParse(splitArg[1], out yResult);

                float zResult = 0f;
                bool zSuccess = false;
                zSuccess = float.TryParse(splitArg[2], out zResult);

                float wResult = 0f;
                bool wSuccess = false;
                wSuccess = float.TryParse(splitArg[3], out wResult);

                if (xSuccess && ySuccess && zSuccess && wSuccess)
                {
                    returnObject = new Quaternion(xResult, yResult, zResult, wResult);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        else if (objectType == typeof(Rect))
        {
            string[] splitArg = argument.Split(',');
            if (splitArg.Length == 4)
            {
                float xResult = 0f;
                bool xSuccess = false;
                xSuccess = float.TryParse(splitArg[0], out xResult);

                float yResult = 0f;
                bool ySuccess = false;
                ySuccess = float.TryParse(splitArg[1], out yResult);

                float wResult = 0f;
                bool wSuccess = false;
                wSuccess = float.TryParse(splitArg[2], out wResult);

                float hResult = 0f;
                bool hSuccess = false;
                hSuccess = float.TryParse(splitArg[3], out hResult);

                if (xSuccess && ySuccess && wSuccess && hSuccess)
                {
                    returnObject = new Rect(xResult, yResult, wResult, hResult);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        else if (objectType == typeof(Color))
        {
            string[] splitArg = argument.Split(',');
            if (splitArg.Length == 4)
            {
                float xResult = 0f;
                bool xSuccess = false;
                xSuccess = float.TryParse(splitArg[0], out xResult);

                float yResult = 0f;
                bool ySuccess = false;
                ySuccess = float.TryParse(splitArg[1], out yResult);

                float zResult = 0f;
                bool zSuccess = false;
                zSuccess = float.TryParse(splitArg[2], out zResult);

                float wResult = 0f;
                bool wSuccess = false;
                wSuccess = float.TryParse(splitArg[2], out wResult);

                if (xSuccess && ySuccess && zSuccess && wSuccess)
                {
                    returnObject = new Color(xResult, yResult, zResult, wResult);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        return false;
    }

    void DrawAutocompleteWindow(string title, string[] names, Vector2 offset, int selectedIndex)
    {
        DrawAutocompleteWindow(title, names, offset, selectedIndex, 0);
    }
    void DrawAutocompleteWindow(string title, string[] names, Vector2 offset, int selectedIndex, int windowIndex)
    {
        autocompleteScrollPos[windowIndex] = GUI.BeginScrollView(new Rect(offset.x + autocompleteScrollBarBuffer, offset.y, autocompleteBoxMaxSize.x + autocompleteScrollBarBuffer, autocompleteBoxMaxSize.y), autocompleteScrollPos[windowIndex], new Rect(0f, 0f, autocompleteBoxMaxSize.x + autocompleteScrollBarBuffer, names.Length * autocompleteButtonHeight), false, false);
        GUI.Box(new Rect(0f, 0f, autocompleteBoxMaxSize.x, autocompleteTitleHeight + autocompleteBoxBottomPadding + names.Length * autocompleteButtonHeight), "");
        GUIStyle titleStyle = new GUIStyle(GUI.skin.GetStyle("Label"));
        titleStyle.alignment = TextAnchor.UpperCenter;
        GUI.Label(new Rect(0f, 0f, autocompleteBoxMaxSize.x, autocompleteTitleHeight), title, titleStyle);

        for (int i = 0; i < names.Length; i++)
        {
            Color prevColor = GUI.color;
            if (selectedIndex == i)
            {
                GUI.color = autocompleteSelectedColor;
            }

            if (GUI.Button(new Rect(0f, autocompleteTitleHeight + i * autocompleteButtonHeight, autocompleteBoxMaxSize.x, autocompleteButtonHeight), names[i]))
            {
                autocompleteWindowIndex = windowIndex;
                autocompleteIndex = i;
                ApplyAutocompleteText();
            }

            if (selectedIndex == i)
            {
                GUI.color = prevColor;
            }
        }
        GUI.EndScrollView();
    }

    void ScrollToDisplaySelected()
    {
        float boxHeight = autocompleteBoxMaxSize.y - autocompleteTitleHeight;
        float selectedItemYPos = autocompleteTitleHeight + autocompleteButtonHeight * autocompleteIndex;
        if (autocompleteScrollPos[autocompleteWindowIndex].y > selectedItemYPos)
        {
            autocompleteScrollPos[autocompleteWindowIndex].y = selectedItemYPos;
        }
        else if (autocompleteScrollPos[autocompleteWindowIndex].y < selectedItemYPos + autocompleteButtonHeight - boxHeight)
        {
            autocompleteScrollPos[autocompleteWindowIndex].y = selectedItemYPos - boxHeight + autocompleteButtonHeight;
        }
    }

    void ApplyAutocompleteText()
    {
        int startLength = 0;

        if (consoleText.ToLower().StartsWith("print "))
        {
            startLength += 6;
        }

        string[] dotSplits = consoleText.Split('.');

        bool hasMethodNames = foundMethodNames.Count > 0;
        bool hasFieldNames = foundFieldNames.Count > 0;
        bool hasPropertyNames = foundPropertyNames.Count > 0;

        if (hasMethodNames || hasFieldNames || hasPropertyNames)
        {
            int numWindows = 0;
            if (hasMethodNames) numWindows++;
            if (hasFieldNames || hasPropertyNames) numWindows++;

            string noCall = consoleText.Substring(startLength, consoleText.Length - startLength);
            consoleText = consoleText.Substring(0, startLength) + noCall.Substring(0, noCall.LastIndexOf('.')) + ".";

            if (autocompleteWindowIndex == 0 && hasMethodNames)
            {
                consoleText += foundMethodNames[autocompleteIndex];
            }
            else
            {
                if (autocompleteIndex < foundFieldNames.Count)
                {
                    consoleText += foundFieldNames[autocompleteIndex];
                }
                else
                {
                    consoleText += foundPropertyNames[autocompleteIndex - foundFieldNames.Count];
                }
            }

            focusOnTextField = true;
        }
        else if (foundComponentNames.Count > 0)
        {
            string noCall = consoleText.Substring(startLength, consoleText.Length - startLength);
            consoleText = consoleText.Substring(0, startLength) + noCall.Substring(0, noCall.LastIndexOf('.')) + ".";
            consoleText += foundComponentNames[autocompleteIndex] + ".";

            focusOnTextField = true;
        }
        else if (foundGameObjectNames.Count > 0)
        {
            string noCall = consoleText.Substring(startLength, consoleText.Length - startLength);
            consoleText = consoleText.Substring(0, startLength) + noCall.Substring(0, consoleText.LastIndexOf(dotSplits[dotSplits.Length - 1]));
            consoleText += foundGameObjectNames[autocompleteIndex] + ".";
            focusOnTextField = true;
        }
    }
}