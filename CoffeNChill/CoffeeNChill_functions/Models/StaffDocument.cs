using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// <summary>
/// An operational document (recipe sheet, cleaning manual, health &amp;
/// safety policy, etc.) stored in the "staff-docs" Azure File Share.
/// This is the storage model — see StaffDocumentResponse for the
/// shape returned over HTTP.
/// </summary>


namespace CoffeeNChill.Functions.Models
{
    public class StaffDocument
    {
        /// <summary> /// Name of the file. /// </summary>/// 
        public string FileName { get; set; } = string.Empty;

        /// <summary> /// File extension, such as "pdf" or "docx". /// </summary>/// 
        public string Extension { get; set; } = string.Empty;

        /// <summary>MIME content type of the stored file (e.g. "application/pdf").</summary>/// 
        public string Type { get; set; } = string.Empty;
        /// <summary> /// MIME type of the file. /// </summary>
        public long FileSize { get; set; }
        /// <summary> /// Date and time the file was uploaded. /// </summary>
        public DateTime UploadDate { get; set; }
        /// <summary> /// Name of the Azure File Share. /// </summary>
        public string ContainerName { get; set; } = "staff-docs";

    }
}
