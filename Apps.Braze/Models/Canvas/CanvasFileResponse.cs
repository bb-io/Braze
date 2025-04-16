using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Models.Canvas
{
    public class CanvasFileResponse
    {
        [Display("Canvas message (HTML)")]
        public FileReference HtmlFile { get; set; }

        [Display("Canvas message (JSON)")]
        public FileReference JsonFile { get; set; }
    }
}
