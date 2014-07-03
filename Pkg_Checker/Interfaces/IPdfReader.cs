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
        void TraverseWholeFile();        

        void CheckCommonFields();

        void CheckReviewStatus();
        void CheckWorkProducts();
        void CheckCheckList();

        void CheckWholeFileWide();
                
        List<String> GetDefects();

        void Close();
    }
}
