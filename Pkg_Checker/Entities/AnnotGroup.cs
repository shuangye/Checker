using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pkg_Checker.Entities
{
    /// <summary>
    /// 定义：一个 annotation 组指一个 sticky note 及其 reply 和相关联的 attributes.        
    /// 在逻辑上可认为一个 annotation 组是一个树形结构，sticky note 是其根，
    /// 其他的 attributes 都是对根或前驱的 in reply to.
    /// 
    /// 定位一个 annotation 组的方法是，debug 查看大多数 annotation 都包含哪些信息，统计这些信息，从而得出结论：
    /// P 值指的是文档中的一个位置；不同的 annotation 组可能有相同的 P 值，同一个 annotation 组的 P 值是相同的。
    /// Attributes 的 IRT 值位于本 annotation 组其他元素的 Number 值集合中。
    /// 结合以上两点可定位出一个 annotation 组。
    /// </summary>
    public class AnnotGroup
    {
        // P 值指的是文档中的一个位置；不同的 annotation 组可能有相同的 P 值，同一个 annotation 组的 P 值是相同的。
        public PdfIndirectReference AnnotP { get; set; }  // PdfName.P value of an annotation
        public PdfIndirectReference Root { get; set; }  // root of this annot group, i.e., the sticky note itself
        public List<int> Numbers { get; set; } // the number set
        public int Page { get; set; }  // the page number on which this annot group locates
        public bool IsDefectAccepted { get; set; }
        public bool IsDefectTypeFound { get; set; }
        public bool IsDefectSeverityFound { get; set; }
        public bool IsAuthorWorkCompleted { get; set; }
        public bool IsModeratorVerifyCompleted { get; set; }
    }
}
