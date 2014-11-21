using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pkg_Checker.Entities
{
    /// <summary>
    /// // 定义：一个 annotation 组指一个 sticky note 及其 reply 和相关联的 attributes.        
    /// </summary>
    public class AnnotGroup
    {
        public PdfObject AnnotP { get; set; }  // PdfName.P value of an annotation
        public List<int> Numbers { get; set; } // the number set
    }
}
