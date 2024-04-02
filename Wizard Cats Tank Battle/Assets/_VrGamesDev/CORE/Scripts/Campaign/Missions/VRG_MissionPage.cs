using System.Collections;

using UnityEngine;
using UnityEngine.UI;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;

namespace VrGamesDev
{
    /// <summary>
    /// Adjust the canvas and the viewport to size the buttons into the page
    /// this data is defined in the VRG_Campaign singleton
    /// </summary>

    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class VRG_MissionPage : VRG_Base
    {
        /// <summary>
        /// The pixels that gets padding from upper left corner
        /// </summary>
        [Tooltip("The pixels that gets padding from upper left corner")]
        [SerializeField]
        private int m_Padding = 25;

        [Header("From: Components")]

        /// <summary>
        /// The RectTransform where to render the page
        /// </summary>
        [Tooltip("The RectTransform where to render the page")]
        [SerializeField]
        private RectTransform m_RectTransform = null;

        /// <summary>
        /// A grid layout UI unity component
        /// </summary>
        [Tooltip("A grid layout UI unity component")]
        [SerializeField]
        private GridLayoutGroup m_GridLayoutGroup = null;

        [Header("FROM Debug:  - DO NOT EDIT unless you understand what is going on - ")]

        [Tooltip("The A grid layout parent")]
        //[SerializeField]
        /// #IGNORE
        private GridLayoutGroup m_Parent = null;

        [Tooltip("The width of the page in pixels")]
        //[SerializeField]
        /// #IGNORE
        private float m_Width = 0.0f;

        [Tooltip("The Height of the page in pixels")]
        //[SerializeField]
        /// #IGNORE
        private float m_Height = 0.0f;

        [Tooltip("The pixels to Space each button in X")]
        //[SerializeField]
        /// #IGNORE
        private int m_SpacingX = 48;

        [Tooltip("The pixels to Space each button in Y")]
        //[SerializeField]
        /// #IGNORE
        private int m_SpacingY = 60;



        /// #IGNORE
        private void Awake()
        {
            // find my rect transform
            this.m_RectTransform = this.FindMy(this.m_RectTransform);

            // find my Grid Layout Group
            this.m_GridLayoutGroup = this.FindMy(this.m_GridLayoutGroup);

            // get my Grid Layout Group parent
            this.m_Parent = this.transform.parent.transform.GetComponent<GridLayoutGroup>();

            // if not found 
            if (this.m_Parent == null)
            {
                // ... destroy the object and inform the game dev
                this.Logs("<color=red><b>DESTROYED: </b></color> GridLayoutGroup", ENUM_Verbose.ERROR);

                Destroy(this.gameObject);
            }

        }

        /// <summary>
        /// <strong><em>Do it's thing: </em></strong> Adjust the row and columns pixels to fit the screen content and display the page according
        /// </summary>
        protected override IEnumerator Do()
        {
            // Let's assume everything is configured properly
            yield return VRG_Campaign.IsValid();

            // is it?
            if (VRG_Campaign.Instance != null)
            {
                // the spacing is calculated as 10% of the total width
                this.m_SpacingX = int.Parse((this.m_Parent.cellSize.x / 10.0f).ToString());
                this.m_SpacingY = int.Parse((this.m_Parent.cellSize.y / 10.0f).ToString());

                // the widht and height is caculated as the total width - the spacing
                this.m_Width = this.m_Parent.cellSize.x - this.m_Parent.spacing.x;
                this.m_Height = this.m_Parent.cellSize.y - this.m_Parent.spacing.y;

                // and the rows and columns are calculated acoording of how many elements needs to be displayed
                int iRows       = (VRG_Campaign.missionRow - 1) < 1 ? 0 : (this.m_SpacingY / (VRG_Campaign.missionRow - 1));
                int iColumns    = (VRG_Campaign.missionColumn - 1) < 1 ? 0 : (this.m_SpacingX / (VRG_Campaign.missionColumn - 1));

                // set the spacing 
                this.m_GridLayoutGroup.spacing = new Vector2(iColumns, iRows);

                // and the padding
                this.m_GridLayoutGroup.padding.top = this.m_Padding;
                this.m_GridLayoutGroup.padding.left = this.m_Padding;

                // set the spacing, if just 1 element, then no spacing
                int iSpacingX = VRG_Campaign.missionColumn == 1 ? 0 : this.m_SpacingX;
                int iSpacingY = VRG_Campaign.missionRow == 1 ? 0 : this.m_SpacingY;

                // and set the cell size, taking into account width/height and spacing and padding
                this.m_GridLayoutGroup.cellSize = new Vector2
                (
                    (this.m_Width - this.m_Padding - iSpacingX) / VRG_Campaign.missionColumn,
                    (this.m_Height - this.m_Padding - iSpacingY) / VRG_Campaign.missionRow                
                );
            }

            // next frame
            yield return null;
        }


    }
}