using System;

namespace FastWin32.Control
{
    /// <summary>
    /// 列表视图控件扩展样式
    /// </summary>
    [Flags]
    public enum ExtendedListViewStyles : uint
    {
        /// <summary>
        /// Displays gridlines around items and subitems. This style is available only in conjunction with the LVS_REPORT style.
        /// </summary>
        LVS_EX_GRIDLINES = 0x00000001,

        /// <summary>
        /// Allows images to be displayed for subitems. This style is available only in conjunction with the LVS_REPORT style.
        /// </summary>
        LVS_EX_SUBITEMIMAGES = 0x00000002,

        /// <summary>
        /// Version 4.70. Enables check boxes for items in a list-view control. When set to this style, the control creates and sets a state image list with two images using DrawFrameControl. State image 1 is the unchecked box, and state image 2 is the checked box. Setting the state image to zero removes the check box.
        /// Version 6.00 and later Check boxes are visible and functional with all list view modes except the tile view mode introduced in ComCtl32.dll version 6. Clicking a checkbox in tile view mode only selects the item; the state does not change.
        /// You can obtain the state of the check box for a given item with ListView_GetCheckState. To set the check state, use ListView_SetCheckState. If this style is set, the list-view control automatically toggles the check state when the user clicks the check box or presses the space bar.
        /// </summary>
        LVS_EX_CHECKBOXES = 0x00000004,

        /// <summary>
        /// Enables hot-track selection in a list-view control. Hot track selection means that an item is automatically selected when the cursor remains over the item for a certain period of time. The delay can be changed from the default system setting with a LVM_SETHOVERTIME message. This style applies to all styles of list-view control. You can check whether hot-track selection is enabled by calling SystemParametersInfo.
        /// </summary>
        LVS_EX_TRACKSELECT = 0x00000008,

        /// <summary>
        /// Enables drag-and-drop reordering of columns in a list-view control. This style is only available to list-view controls that use the LVS_REPORT style.
        /// </summary>
        LVS_EX_HEADERDRAGDROP = 0x00000010,

        /// <summary>
        /// When an item is selected, the item and all its subitems are highlighted. This style is available only in conjunction with the LVS_REPORT style.
        /// </summary>
        LVS_EX_FULLROWSELECT = 0x00000020,

        /// <summary>
        /// The list-view control sends an LVN_ITEMACTIVATE notification code to the parent window when the user clicks an item. This style also enables hot tracking in the list-view control. Hot tracking means that when the cursor moves over an item, it is highlighted but not selected. See the Extended List-View Styles Remarks section for a discussion of item activation.
        /// </summary>
        LVS_EX_ONECLICKACTIVATE = 0x00000040,

        /// <summary>
        /// The list-view control sends an LVN_ITEMACTIVATE notification code to the parent window when the user double-clicks an item. This style also enables hot tracking in the list-view control. Hot tracking means that when the cursor moves over an item, it is highlighted but not selected. See the Extended List-View Styles Remarks section for a discussion of item activation.
        /// </summary>
        LVS_EX_TWOCLICKACTIVATE = 0x00000080,

        /// <summary>
        /// Enables flat scroll bars in the list view. If you need more control over the appearance of the list view's scroll bars, you should manipulate the list view's scroll bars directly using the Flat Scroll Bar APIs. If the system metrics change, you are responsible for adjusting the scroll bar metrics with FlatSB_SetScrollProp. See Flat Scroll Bars for further details.
        /// </summary>
        LVS_EX_FLATSB = 0x00000100,

        /// <summary>
        /// Version 4.71 through Version 5.80 only. Not supported on Windows Vista and later. Sets the list view window region to include only the item icons and text using SetWindowRgn. Any area that is not part of an item is excluded from the window region. This style is only available to list-view controls that use the LVS_ICON style.
        /// </summary>
        LVS_EX_REGIONAL = 0x00000200,

        /// <summary>
        /// When a list-view control uses the LVS_EX_INFOTIP style, the LVN_GETINFOTIP notification code is sent to the parent window before displaying an item's tooltip.
        /// </summary>
        LVS_EX_INFOTIP = 0x00000400,

        /// <summary>
        /// Causes those hot items that may be activated to be displayed with underlined text. This style requires that LVS_EX_ONECLICKACTIVATE or LVS_EX_TWOCLICKACTIVATE also be set. See the Extended List-View Styles Remarks section for a discussion of item activation.
        /// </summary>
        LVS_EX_UNDERLINEHOT = 0x00000800,

        /// <summary>
        /// Causes those non-hot items that may be activated to be displayed with underlined text. This style requires that LVS_EX_TWOCLICKACTIVATE be set also. See the Extended List-View Styles Remarks section for a discussion of item activation.
        /// </summary>
        LVS_EX_UNDERLINECOLD = 0x00001000,

        /// <summary>
        /// If the list-view control has the LVS_AUTOARRANGE style, the control will not autoarrange its icons until one or more work areas are defined (see LVM_SETWORKAREAS). To be effective, this style must be set before any work areas are defined and any items have been added to the control.
        /// </summary>
        LVS_EX_MULTIWORKAREAS = 0x00002000,

        /// <summary>
        /// If a partially hidden label in any list view mode lacks tooltip text, the list-view control will unfold the label. If this style is not set, the list-view control will unfold partly hidden labels only for the large icon mode.
        /// </summary>
        LVS_EX_LABELTIP = 0x00004000,

        /// <summary>
        /// Version 4.71 and later. Changes border color when an item is selected, instead of highlighting the item.
        /// </summary>
        LVS_EX_BORDERSELECT = 0x00008000,

        /// <summary>
        /// Version 6.00 and later. Paints via double-buffering, which reduces flicker. This extended style also enables alpha-blended marquee selection on systems where it is supported.
        /// </summary>
        LVS_EX_DOUBLEBUFFER = 0x00010000,

        /// <summary>
        /// Version 6.00 and later. Hides the labels in icon and small icon view.
        /// </summary>
        LVS_EX_HIDELABELS = 0x00020000,

        /// <summary>
        /// Version 6.00 and later. Not used.
        /// </summary>
        LVS_EX_SINGLEROW = 0x00040000,

        /// <summary>
        /// Version 6.00 and later. In icon view, icons automatically snap into a grid.
        /// </summary>
        LVS_EX_SNAPTOGRID = 0x00080000,

        /// <summary>
        /// Version 6.00 and later. In icon view, moves the state image of the control to the top right of the large icon rendering. In views other than icon view there is no change. When the user changes the state by using the space bar, all selected items cycle over, not the item with the focus.
        /// </summary>
        LVS_EX_SIMPLESELECT = 0x00100000,

        /// <summary>
        /// Windows Vista and later. Icons are lined up in columns that use up the whole view.
        /// </summary>
        LVS_EX_JUSTIFYCOLUMNS = 0x00200000,

        /// <summary>
        /// Windows Vista and later. Background is painted by the parent via WM_PRINTCLIENT.
        /// </summary>
        LVS_EX_TRANSPARENTBKGND = 0x00400000,

        /// <summary>
        /// Windows Vista and later. Enable shadow text on transparent backgrounds only.
        /// </summary>
        LVS_EX_TRANSPARENTSHADOWTEXT = 0x00800000,

        /// <summary>
        /// Windows Vista and later. Automatically arrange icons if no icon positions have been set (Similar to LVS_AUTOARRANGE).
        /// </summary>
        LVS_EX_AUTOAUTOARRANGE = 0x01000000,

        /// <summary>
        /// Windows Vista and later. Show column headers in all view modes.
        /// </summary>
        LVS_EX_HEADERINALLVIEWS = 0x02000000,

        /// <summary>
        /// Windows Vista and later. Automatically select check boxes on single click.
        /// </summary>
        LVS_EX_AUTOCHECKSELECT = 0x08000000,

        /// <summary>
        /// Windows Vista and later. Automatically size listview columns.
        /// </summary>
        LVS_EX_AUTOSIZECOLUMNS = 0x10000000,

        /// <summary>
        /// Windows Vista and later. Snap to minimum column width when the user resizes a column.
        /// </summary>
        LVS_EX_COLUMNSNAPPOINTS = 0x40000000,

        /// <summary>
        /// Indicates that an overflow button should be displayed in icon/tile view if there is not enough client width to display the complete set of header items. The list-view control sends the LVN_COLUMNOVERFLOWCLICK notification when the overflow button is clicked. This flag is only valid when LVS_EX_HEADERINALLVIEWS is also specified.
        /// </summary>
        LVS_EX_COLUMNOVERFLOW = 0x80000000
    }
}
