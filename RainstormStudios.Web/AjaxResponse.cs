using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RainstormStudios.Web
{
    /// <summary>
    /// A simple class for transporting an encapsulated AJAX response between the server and client.
    /// </summary>
    [Serializable]
    public class AjaxResponse
    {
        public bool
            Success { get; set; }
        public string
            UserMessage { get; set; }
        public object[]
            Data { get; set; }

        public AjaxResponse(bool success)
        {
            this.Success = success;
        }
        public AjaxResponse(bool success, params object[] data)
            : this(success)
        {
            this.Data = data;
        }
        public AjaxResponse(bool success, string message)
            : this(success)
        {
            if (!success)
                this.UserMessage = message;
            else
                // If they passed a string, but said operation was successful, then the "message" value is the data result.
                this.Data = new object[] { message };
        }
    }
}
