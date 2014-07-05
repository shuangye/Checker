using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pkg_Checker.Interfaces
{
    public interface IPdfReader
    {
        void Init(string filePath);
        bool IsValidReviewPackage();
        
        void ReadWholeFile();
        void CheckCommonFields();
        void CheckReviewStatus();
        void CheckWorkProducts();
        void CheckCheckList();
        
        List<String> GetDefects();

        void Close();
    }
}
