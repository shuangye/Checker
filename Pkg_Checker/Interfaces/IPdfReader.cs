using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pkg_Checker.Interfaces
{
    public interface IPdfReader
    {
        void Read(String pdfPath);

        bool IsValidReviewPackage();

        void CheckCommonFields();
        void CheckWorkProductType();
        void CheckCheckList();

        void TraverseWholeFile();

        void WorkWithAnnot();

        List<String> GetDefects();

        void Close();
    }
}
