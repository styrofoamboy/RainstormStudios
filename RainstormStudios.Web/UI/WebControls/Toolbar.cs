using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RainstormStudios.Web.UI.WebControls
{
    [Author("Unfried, Michael")]
    [DefaultProperty("Items"), ParseChildren(ChildrenAsProperties = true), ToolboxData("<{0}:ToolBar runat=\"server\"></{0}:ToolBar>")]
    public class ToolBar : System.Web.UI.WebControls.CompositeControl, INamingContainer
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        ToolBarItemCollection
            _items;
        ITemplate
            _floatRightTemplate;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ToolBarItemCollection Items
        { get { return this._items; } }
        public string ActiveHoverCssClassName
        {
            get
            {
                string vsVal = (string)this.ViewState["acthvrcssclsnm"];
                if (string.IsNullOrEmpty(vsVal))
                    return "active";
                else
                    return vsVal;
            }
            set { this.ViewState["acthvrcssclsnm"] = value; }
        }
        public override string CssClass
        {
            get
            {
                return !string.IsNullOrEmpty(base.CssClass)
                        ? base.CssClass
                        : "toolbar";
            }
            set { base.CssClass = value; }
        }
        public ToolBarDisplayMode DisplayMode
        {
            get { return this.ViewState.GetValue<ToolBarDisplayMode>("DisplayMode", ToolBarDisplayMode.ImageOnly, false, Enum.TryParse); }
            set { this.ViewState["DisplayMode"] = value; }
        }
        [TemplateContainer(typeof(ToolBar))
        , PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate ExtendedControlTemplate
        {
            get { return this._floatRightTemplate; }
            set { this._floatRightTemplate = value; }
        }
        //***************************************************************************
        // Private Properties
        // 
        protected override HtmlTextWriterTag TagKey
        { get { return HtmlTextWriterTag.Div; } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ToolBar()
            : base()
        {
            this._items = new ToolBarItemCollection(this);
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        protected override void OnPreRender(EventArgs e)
        {
            Control lnkCheck = this.Page.Header.FindControl("ToolBarCSS");
            if (lnkCheck == null)
            {
                System.Web.UI.HtmlControls.HtmlLink link = new System.Web.UI.HtmlControls.HtmlLink();
                link.ID = "ToolBarCSS";
                link.Attributes.Add("href", this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.SlideShow), "RainstormStudios.Web.UI.WebControls.style.toolbar.css"));
                link.Attributes.Add("type", "text/css");
                link.Attributes.Add("rel", "stylesheet");
                this.Page.Header.Controls.Add(link);
            }

            string scriptPathRef = this.Page.ClientScript.GetWebResourceUrl(typeof(RainstormStudios.Web.UI.WebControls.ToolBar), "RainstormStudios.Web.UI.WebControls.scripts.toolbar.js");
            this.Page.ClientScript.RegisterClientScriptInclude(this.Page.GetType(), "ToolBarScript", scriptPathRef);

            System.Text.StringBuilder sbInit = new StringBuilder("$(document).ready(function () { initToolbar(");
            sbInit.AppendFormat("'{0}', '{1}'", this.ClientID, this.ActiveHoverCssClassName);
            sbInit.Append("); });");
            this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), this.ClientID + "_ToolbarInit", sbInit.ToString(), true);

            base.OnPreRender(e);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            writer.BeginRender();
            try
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClass);
                base.Render(writer);
            }
            finally
            { writer.EndRender(); }
        }
        protected override void CreateChildControls()
        {
            if (this._floatRightTemplate != null)
            {
                Panel panExtCtrl = new Panel();
                this.Controls.Add(panExtCtrl);
                panExtCtrl.ID = "panExtCtrl";
                panExtCtrl.Style.Add("float", "right");
                this._floatRightTemplate.InstantiateIn(panExtCtrl);
            }

            for (int i = 0; i < this._items.Count; i++)
            {
                ToolBarItem item = this._items[i];

                Label span = new Label();
                span.CssClass = "tbItemCont";

                if (!string.IsNullOrEmpty(item.CommandName))
                {
                    ImageButton btnImg = new ImageButton();
                    btnImg.ID = "imgBtn_" + i.ToString();
                    btnImg.ImageUrl = item.ImageUrl;
                    btnImg.AlternateText = item.Text;
                    btnImg.CommandName = item.CommandName;
                    if (!string.IsNullOrEmpty(item.CommandArgument))
                        btnImg.CommandArgument = item.CommandArgument;
                    if (!string.IsNullOrEmpty(item.OnClientClick))
                        btnImg.OnClientClick = item.OnClientClick;
                    span.Controls.Add(btnImg);
                }
                else
                {
                    System.Web.UI.HtmlControls.HtmlImage img = new System.Web.UI.HtmlControls.HtmlImage();
                    if (!string.IsNullOrEmpty(item.OnClientClick))
                        span.Attributes.Add("onclick", item.OnClientClick);
                    img.Src = this.Page.ResolveUrl(item.ImageUrl);
                    img.Alt = item.Text;
                    span.Controls.Add(img);
                }
                this.Controls.Add(span);
            }

            base.CreateChildControls();
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    [Serializable]
    public class ToolBarItem
    {
        #region Properties
        //***********************************************************************
        // Public Properties
        // 
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public string OnClientClick { get; set; }
        public string CommandName { get; set; }
        public string CommandArgument { get; set; }
        #endregion

        #region Class Constructors
        //***********************************************************************
        // Class Constructors
        // 
        public ToolBarItem()
        { }
        #endregion
    }
    [Author("Unfried, Michael")]
    [Serializable]
    public class ToolBarItemCollection : Collections.ObjectCollectionBase<ToolBarItem>
    {
        #region Declarations
        //***********************************************************************
        // Private Fields
        // 
        private ToolBar
            _parent;
        #endregion

        #region Properties
        //***********************************************************************
        // Public Properties
        // 
        public ToolBar Parent
        { get { return this._parent; } }
        #endregion

        #region Class Constructors
        //***********************************************************************
        // Class Constructors
        // 
        internal ToolBarItemCollection(ToolBar owner)
            : base()
        {
            this._parent = owner;
        }
        #endregion

        #region Public Methods
        //***********************************************************************
        // Public Methods
        // 
        public void Add(ToolBarItem item)
        {
            base.Add(item, string.Empty);
        }
        #endregion
    }
    [Author("Unfried, Michael")]
    public enum ToolBarDisplayMode : uint
    {
        ImageOnly = 0,
        TextAndImage = 1,
        ImageBeforeText = 2,
        TextOverImage = 3
    }
}
