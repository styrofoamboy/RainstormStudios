using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RainstormStudios;

using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace WebTestApp
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
                this.calTest.LoadUserEvents(new Guid("DAA7812C-0954-44EF-8C5F-23F82C316D48"));
        }
    }
}
