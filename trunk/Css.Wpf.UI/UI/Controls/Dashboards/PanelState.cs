//-----------------------------------------------------------------------
// <copyright file="PanelState.cs" company="Microsoft Corporation copyright 2008.">
// (c) 2008 Microsoft Corporation. All rights reserved.
// This source is subject to the Microsoft Public License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// </copyright>
// <date>06-Mar-2009</date>
// <author>Martin Grayson</author>
// <summary>Enum for panel states.</summary>
//-----------------------------------------------------------------------
namespace Css.Wpf.UI.Controls.Dashboards
{
    using System;

    /// <summary>
    /// Enum for panel states.
    /// </summary>
    /// 
    [Serializable]
    public enum PanelState
    {
        /// <summary>
        /// The normal state for a panel.
        /// </summary>
        Restored=0,

        /// <summary>
        /// The maxmized state for a panel.
        /// </summary>
        Maximized=1,

        /// <summary>
        /// The minimized state for a panel.
        /// </summary>
        Minimized=2
    }
}
