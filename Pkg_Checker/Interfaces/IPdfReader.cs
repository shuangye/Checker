using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pkg_Checker.Interfaces
{
    public interface IPdfReader
    {
        // bool Init(string filePath);
        bool IsValidReviewPackage();
        
        void ReadWholeFile();
        void CheckCommonFields();
        void CheckReviewStatus();
        void CheckComments();
        void CheckWorkProducts();
        void CheckCheckList();
        void CheckWithCM21(String EID, String password, String cwd, int timeout);
        
        List<String> GetDefects();        
    }
}
