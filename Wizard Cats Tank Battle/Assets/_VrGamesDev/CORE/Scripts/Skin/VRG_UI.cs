using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
using Sirenix.OdinInspector;
#endif



namespace VrGamesDev
{
    [RequireComponent(typeof(Canvas))]

    /// <summary>
    /// Set the VRG_SkinPool UI data into a canvas elements
    /// </summary>
    public class VRG_UI : VRG_Base
    {
#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Configuration")]
#endif
        /// <summary>
        /// The following objects will not be modified
        /// </summary>
        [Tooltip("The following objects will not be modified")]
        [SerializeField] private bool m_SetMainCamera = true;

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Configuration")]
#endif
        /// <summary>
        /// The following objects will not be modified
        /// </summary>
        [Tooltip("The following objects will not be modified")]
        [SerializeField] private bool m_IncludeCameraColor = true;










#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Data")]
#endif
        /// <summary>
        /// The following objects will not be modified
        /// </summary>
        [Tooltip("The following objects will not be modified")]
        [SerializeField] private List<Transform> m_Except = new List<Transform>();

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Data")]
#endif
        /// <summary>
        /// The following objects will not be modified
        /// </summary>
        [Tooltip("This component and all its child are excepted")]
        [SerializeField] private List<Transform> m_AllChildren = new List<Transform>();

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Data")]
#endif
        /// <summary>
        /// The following objects will be Background Colored
        /// </summary>
        [Tooltip("The following objects will be Background Colored")]
        [SerializeField] private List<Transform> m_BackgroundColor = new List<Transform>();

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Data")]
#endif
        /// <summary>
        /// The following objects will be Foreground Colored
        /// </summary>
        [Tooltip("The following objects will be Foreground Colored")]
        [SerializeField] private List<Transform> m_ForegroundColor = new List<Transform>();

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Data")]
#endif
        /// <summary>
        /// The following objects will be Background Colored
        /// </summary>
        [Tooltip("The following objects will be Camera Colored")]
        [SerializeField] private List<Transform> m_ThirdColor = new List<Transform>();

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Data")]
#endif
        /// <summary>
        /// The following objects will be marked as font with the secondary color
        /// </summary>
        [Tooltip("The following objects will be marked as font with the secondary color")]
        [SerializeField] private List<Transform> m_FontMarked = new List<Transform>();



        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]


#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Data")]
#endif
        [Tooltip("If the except list was generated")]
        //[SerializeField]
        private bool m_ExceptLoaded = false;

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Data")]
#endif
        [Tooltip("The texts found")]
        [SerializeField]
        private Text[] m_Text = null;

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3
        [ToggleGroup("Data")]
#endif
        [Tooltip("The buttons found")]
        //[SerializeField]
        private Button[] m_Button = null;


        public VRG_UI()
        {
            m_PlayOnEnable = true;
            m_SelfTurnOff = false;
            m_NextFrame = true;
        }

        private void Awake()
        {
            VRG_UICustom[] vrg_UIExcept = this.GetComponentsInChildren<VRG_UICustom>(true);

            foreach (VRG_UICustom child in vrg_UIExcept)
            {
                if (child.enabled)
                {
                    switch (child.except)
                    {
                        case ENUM_VRG_UIExcept.SELF:
                            if (!this.m_Except.Contains(child.transform))
                            {
                                this.m_Except.Add(child.transform);
                            }
                            break;

                        case ENUM_VRG_UIExcept.ALL_CHILDREN:
                            if (!this.m_AllChildren.Contains(child.transform))
                            {
                                this.m_AllChildren.Add(child.transform);
                            }
                            break;
                    }

                    switch (child.color)
                    {
                        case ENUM_VRG_UIColor.BACKGROUND:
                            if (!this.m_BackgroundColor.Contains(child.transform))
                            {
                                this.m_BackgroundColor.Add(child.transform);
                            }
                            break;

                        case ENUM_VRG_UIColor.FOREGROUND:
                            if (!this.m_ForegroundColor.Contains(child.transform))
                            {
                                this.m_ForegroundColor.Add(child.transform);
                            }
                            break;

                        case ENUM_VRG_UIColor.THIRD:
                            if (!this.m_ThirdColor.Contains(child.transform))
                            {
                                this.m_ThirdColor.Add(child.transform);
                            }
                            break;
                    }

                    if (child.font)
                    {
                        if (!this.m_FontMarked.Contains(child.transform))
                        {
                            this.m_FontMarked.Add(child.transform);
                        }                        
                    }
                }
            }

            int iElement = 0;
            iElement++;

            foreach (Transform child in this.m_AllChildren)
            {
                if (child != null)
                {
                    Transform[] vrg_Transform = child.GetComponentsInChildren<Transform>(true);

                    foreach (Transform childTransform in vrg_Transform)
                    {
                        if (!this.m_Except.Contains(childTransform))
                        {
                            this.m_Except.Add(childTransform);
                        }
                    }
                }
                else
                {
                    this.Logs("<color=red>VRG_UI AllChildren (" + iElement + ")  is NULL</color>");
                }
            }

            // this was already excepted
            this.m_ExceptLoaded = true;
        }

        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Apply the "CSS" styles from VRG_SkinPool into this UI/Canvas elements
        /// </summary>
        protected override IEnumerator Do()
        {
            if (this.m_SetMainCamera)
            {
                Canvas myCanvas = this.GetComponent<Canvas>();

                if (myCanvas.renderMode == RenderMode.ScreenSpaceOverlay && myCanvas.worldCamera == null)
                {
                    myCanvas.worldCamera = Camera.main;
                    myCanvas.planeDistance = 1;
                }
            }

            // Let's assume everything is configured properly
            yield return VRG_SkinPool.IsValid(false);

            // is it?
            if (VRG_SkinPool.Instance != null && VRG_SkinPool.skin != null)
            {
                // wait until the first expecting happens
                while (!this.m_ExceptLoaded)
                {
                    yield return null;
                }

                // find all the texts
                this.m_Text = this.GetComponentsInChildren<Text>(true);

                // cycle through them
                foreach (Text child in this.m_Text)
                {
                    // modify if it is a text NOT belonging to a button
                    if (this.IsModify(child.transform))
                    {
                        // if a font is declared 
                        if (VRG_SkinPool.skin.font != null)
                        {
                            // change font
                            child.font = VRG_SkinPool.skin.font;
                        }

                        if (child.gameObject.transform.parent.GetComponent<Button>() == null)
                        {
                            // and the color
                            child.color = VRG_SkinPool.skin.fontColor;
                        }

                    }
                }



                // find all the buttons
                this.m_Button = this.GetComponentsInChildren<Button>(true);

                // cycle the buttons
                foreach (Button child in this.m_Button)
                {
                    // is this button into the exceptions?
                    if (!this.IsModify(child.transform))
                    {
                        // yes, let's try next button
                        continue;
                    }

                    // get the colorblock for this button
                    ColorBlock colors = child.colors;

                    // modify with the VRG_SkinPool settings
                    colors.normalColor = VRG_SkinPool.skin.buttonNormal;
                    colors.highlightedColor = VRG_SkinPool.skin.buttonHighlighted;
                    colors.pressedColor = VRG_SkinPool.skin.buttonPressed;
                    colors.disabledColor = VRG_SkinPool.skin.buttonDisabled;
                    #if UNITY_2019_1_OR_NEWER
                    colors.selectedColor = VRG_SkinPool.skin.buttonSelected;
                    #endif //UNITY_2019_1_OR_NEWER
                    child.colors = colors;

                    // find the text from this button
                    Text[] text = child.GetComponentsInChildren<Text>(true);

                    // cycle through them
                    foreach (Text childText in text)
                    {
                        // is not excepted?
                        if (this.IsModify(childText.transform))
                        {
                            // find if it is a button or an icon
                            if (child.GetComponent<Image>().enabled)
                            {
                                // a button
                                childText.color = VRG_SkinPool.skin.buttonText;
                            }
                            else
                            {
                                // an icon
                                childText.color = VRG_SkinPool.skin.iconText;
                            }

                            // if a font is defined
                            if (VRG_SkinPool.skin.font != null)
                            {
                                // change the text of button
                                childText.font = VRG_SkinPool.skin.font;
                            }
                        }
                    }

                    // cycle the childs of the button
                    foreach (Transform childTransform in child.transform)
                    {
                        // get an image
                        Image image = childTransform.GetComponent<Image>();

                        // Is there an image?
                        if (image != null)
                        {
                            // is it modficable?
                            if (this.IsModify(image.transform))
                            {
                                // assign the color
                                image.color = VRG_SkinPool.skin.iconBackground;

                                //cycle the childs of the image
                                foreach (Transform childIcon in childTransform)
                                {
                                    //does the image has an image, it means its an icon with background
                                    Image icon = childIcon.GetComponent<Image>();

                                    // let's see
                                    if (icon != null)
                                    {
                                        // is it modify?
                                        if (this.IsModify(icon.transform))
                                        {
                                            // change color
                                            icon.color = VRG_SkinPool.skin.iconColor;
                                        }

                                        // found let's move out
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }
                }






                int iElement = 0;

                // cycle the images/Renderers
                foreach (Transform child in this.m_BackgroundColor)
                {
                    if (child != null)
                    {
                        // find an image
                        Image myImage = child.gameObject.GetComponent<Image>();

                        // if found
                        if (myImage != null)
                        {
                            // and modifiable
                            if (this.IsModify(myImage.transform))
                            {
                                // set color
                                myImage.color = VRG_SkinPool.skin.backgroundColor;
                            }
                        }

                        // find a renderer
                        Renderer myRenderer = child.gameObject.GetComponent<Renderer>();

                        // if found
                        if (myRenderer != null)
                        {
                            // and modifiable
                            if (this.IsModify(myRenderer.transform))
                            {
                                // set color
                                myRenderer.material.color = VRG_SkinPool.skin.backgroundColor;
                            }
                        }


                        // find all the texts
                        Text[] Texts = child.GetComponentsInChildren<Text>(true);

                        // cycle through them
                        foreach (Text childTexts in Texts)
                        {
                            // modify if it is a text NOT belonging to a button
                            if (this.IsModify(childTexts.transform))
                            {
                                // if a font is declared 
                                if (VRG_SkinPool.skin.font != null)
                                {
                                    // change font
                                    childTexts.font = VRG_SkinPool.skin.font;
                                }

                                if (childTexts.gameObject.transform.parent.GetComponent<Button>() == null)
                                {
                                    if (this.m_FontMarked.Contains(childTexts.gameObject.transform))
                                    {
                                        // and the color
                                        childTexts.color = VRG_SkinPool.skin.fontColorTitle;
                                    }
                                }
                            }
                        }
                    }

                    else
                    {
                        this.Logs("VRG_UI BackgroundColor (" + iElement + ")  is NULL", ENUM_Verbose.WARNING);
                    }

                    iElement++;
                }



                iElement = 0;

                // cycle the images/Renderers from foregrounds
                foreach (Transform child in this.m_ForegroundColor)
                {
                    if (child != null)
                    {
                        // find an image
                        Image myImage = child.gameObject.GetComponent<Image>();

                        // if found
                        if (myImage != null)
                        {
                            // and modifiable
                            if (this.IsModify(myImage.transform))
                            {
                                // set color
                                myImage.color = VRG_SkinPool.skin.foregroundColor;
                            }
                        }

                        // find a renderer
                        Renderer myRenderer = child.gameObject.GetComponent<Renderer>();

                        // if found
                        if (myRenderer != null)
                        {
                            // and modifiable
                            if (this.IsModify(myRenderer.transform))
                            {
                                // set color
                                myRenderer.material.color = VRG_SkinPool.skin.foregroundColor;
                            }
                        }

                        // find all the texts
                        Text[] Texts = child.GetComponentsInChildren<Text>(true);

                        // cycle through them
                        foreach (Text childTexts in Texts)
                        {
                            // modify if it is a text NOT belonging to a button
                            if (this.IsModify(childTexts.transform))
                            {
                                // if a font is declared 
                                if (VRG_SkinPool.skin.font != null)
                                {
                                    // change font
                                    childTexts.font = VRG_SkinPool.skin.font;
                                }

                                if (childTexts.gameObject.transform.parent.GetComponent<Button>() == null)
                                {
                                    if (this.m_FontMarked.Contains(childTexts.gameObject.transform))
                                    {
                                        // and the color
                                        childTexts.color = VRG_SkinPool.skin.fontColorForegroundTitle;
                                    }
                                    else
                                    {
                                        // and the color
                                        childTexts.color = VRG_SkinPool.skin.fontColorForeground;
                                    }
                                }
                            }
                        }
                    }

                    else
                    {
                        this.Logs("VRG_UI Foreground Color (" + iElement + ")  is NULL", ENUM_Verbose.WARNING);
                    }

                    iElement++;
                }





                iElement = 0;

                // cycle the images/Renderers
                foreach (Transform child in this.m_ThirdColor)
                {
                    if (child != null)
                    {
                        // find an image
                        Image myImage = child.gameObject.GetComponent<Image>();

                        // if found
                        if (myImage != null)
                        {
                            // and modifiable
                            if (this.IsModify(myImage.transform))
                            {
                                // set color
                                myImage.color = VRG_SkinPool.skin.thirdColor;
                            }
                        }

                        // find a renderer
                        Renderer myRenderer = child.gameObject.GetComponent<Renderer>();

                        // if found
                        if (myRenderer != null)
                        {
                            // and modifiable
                            if (this.IsModify(myRenderer.transform))
                            {
                                // set color
                                myRenderer.material.color = VRG_SkinPool.skin.thirdColor;
                            }
                        }
                    }

                    else
                    {
                        this.Logs("VRG_UI thirdColor (" + iElement + ")  is NULL", ENUM_Verbose.WARNING);
                    }

                    iElement++;
                }

                if (this.m_IncludeCameraColor)
                {
                    VRG_CameraBackground vrg_CameraBackground = Object.FindObjectOfType<VRG_CameraBackground>();

                    if (vrg_CameraBackground != null)
                    {
                        vrg_CameraBackground.color = VRG_SkinPool.skin.thirdColor;
                    }
                }
            }
            else
            {
                if (VRG_SkinPool.Instance == null)
                {
                    this.Logs
                    (
                        "if you want to use skins, make sure you added a VRG_SkinPool Singleton",
                        "VRG_UI->Do()",
                        ENUM_Verbose.WARNING
                    );
                }
            }

            // return, it is like a void
            yield return null;
        }

        private bool IsModify(Transform valueLocal)
        {
            // by default modify it
            bool bContinue = true;

            int iElement = 0;

            if (bContinue)
            {
                // cycle the except
                foreach (Transform child in this.m_Except)
                {
                    if (child != null)
                    {
                        // is it ?
                        if (child.Equals(valueLocal))
                        {
                            // then stop
                            bContinue = false;
                            break;
                        }
                    }
                    else
                    {
                        this.Logs("<color=red>VRG_UI Except (" + iElement + ")  is NULL</color>", ENUM_Verbose.WARNING);
                    }

                    iElement++;
                }
            }

            return bContinue;
        }

    }
}
